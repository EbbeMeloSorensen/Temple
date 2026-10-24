using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.Saves;
using Temple.Application.State;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD.Common;
using Temple.Domain.Entities.DD.Quests;
using Temple.ViewModel.DD.Wilderness;
using Xunit;

namespace Temple.ViewModel.UnitTest;

public class GameSaveViewModelTests
{
    [Fact]
    public async Task PendingLoadDisablesInteraction_AndFailureIsVisible()
    {
        var machine = new ApplicationStateMachine();
        var gate = new PendingStore();
        using var controller = CreateController(machine, gate);
        var viewModel = new GameSaveViewModel(controller, canSave: false);
        var load = viewModel.LoadAsync();
        Assert.False(viewModel.IsInteractionEnabled);
        Assert.False(viewModel.LoadCommand.CanExecute(null));
        Assert.False(viewModel.SaveCommand.CanExecute(null));
        gate.Completion.SetException(new FileNotFoundException("Slot is empty."));

        await load;

        Assert.True(viewModel.IsInteractionEnabled);
        Assert.True(viewModel.LoadCommand.CanExecute(null));
        Assert.Contains("Slot is empty", viewModel.StatusMessage);
        Assert.Equal(StateMachineState.MainMenu, controller.CurrentApplicationState.StateMachineState);
    }

    [Fact]
    public async Task LoadingRebindsDialogueServices_AndOpensRestoredWilderness()
    {
        var machine = new ApplicationStateMachine();
        var store = new PendingStore();
        using var controller = CreateController(machine, store);
        var factory = new GameSessionFactory(new Content(), new GameSaveValidator());
        using var source = factory.CreateNew();
        source.EventBus.Publish(new Temple.Domain.Entities.DD.Quests.Events.SiteUnlockedEvent("mine"));
        var snapshot = source.Capture(GameSaveLocation.Wilderness);
        var dialogues = new RecordingDialogueFactory();
        var shell = new MainWindowViewModel(null!, null!, null!, null!, dialogues, null!, controller);
        store.Completion.SetResult(snapshot);

        await Assert.IsType<HomeViewModel>(shell.CurrentViewModel).Saves.LoadAsync();

        var wilderness = Assert.IsType<WildernessViewModel>(shell.CurrentViewModel);
        Assert.Contains(wilderness.Sites, site => site.Text == "mine");
        Assert.Same(controller.CurrentSession.Queries, dialogues.Queries);
        Assert.Same(controller.CurrentSession.EventBus, dialogues.Events);
        Assert.NotSame(source.Queries, dialogues.Queries);
        Assert.Equal(StateMachineState.Wilderness, controller.CurrentApplicationState.StateMachineState);
    }

    private static ApplicationController CreateController(ApplicationStateMachine machine, IGameSaveStore store)
    {
        var validator = new GameSaveValidator();
        var controller = new ApplicationController(machine, null!, new GameSessionFactory(new Content(), validator),
            store, validator, NullLogger<ApplicationController>.Instance);
        machine.Fire(ApplicationStateShiftTrigger.Initialize);
        return controller;
    }

    private sealed class PendingStore : IGameSaveStore
    {
        public TaskCompletionSource<GameSave> Completion { get; } = new();
        public Task<GameSave> ReadAsync(int slot, CancellationToken cancellationToken = default) => Completion.Task;
        public Task WriteAsync(int slot, GameSave save, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class Content : IGameIOHandler
    {
        public IEnumerable<string> ReadSiteIdsFromDirectory(string path) => new[] { "village", "mine" };
        public IEnumerable<Quest> ReadQuestListFromFile(string path) => Array.Empty<Quest>();
        public void WriteQuestsToFile(IEnumerable<Quest> quests, string path) => throw new NotSupportedException();
    }

    private sealed class RecordingDialogueFactory : IDialogueSessionFactory
    {
        public IGameQueryService? Queries { get; private set; }
        public QuestEventBus? Events { get; private set; }
        public void Initialize(IGameQueryService queries, QuestEventBus events)
        {
            Queries = queries;
            Events = events;
        }
        public IDialogueSession GetDialogueSession(string npcId) => throw new NotSupportedException();
    }
}
