using Temple.Domain.Entities.DD.Battle;
using Temple.ViewModel.DD.Battle;
using Temple.ViewModel.DD.Battle.BusinessLogic;
using Temple.ViewModel.DD.Battle.BusinessLogic.Complex;
using Xunit;

namespace Temple.ViewModel.UnitTest;

public class BattleCheatTests
{
    [Fact]
    public void CheatRemovesOnlyHostilesAndReportsVictoryOnce()
    {
        var engine = new ComplexEngine(null!);
        var board = new BoardViewModel(engine, 40, 40, 30, 20);
        var viewModel = new ActOutSceneViewModelComplexEngine(engine, board, null!);
        var scene = new Scene("Test", 5, 5);
        var type = new CreatureType { Name = "Test", MaxHitPoints = 10 };
        var friendly = new Creature(type, false);
        scene.AddCreature(friendly, 0, 0);
        scene.AddCreature(new Creature(type, true), 2, 2);
        scene.AddCreature(new Creature(type, true), 3, 3);
        viewModel.InitializeScene(scene);
        Assert.False(viewModel.CheatCommand.CanExecute(null));
        engine.BattleHasStarted.Object = true;
        var victories = 0;
        viewModel.BattleEnded += (_, e) =>
        {
            Assert.Equal(BattleResult.Victory, e.BattleResult);
            victories++;
        };

        Assert.True(viewModel.CheatCommand.CanExecute(null));
        viewModel.CheatCommand.Execute(null);
        viewModel.CheatCommand.Execute(null);

        Assert.Same(friendly, Assert.Single(engine.Creatures));
        Assert.Equal(3, scene.Creatures.Count);
        Assert.True(engine.BattleDecided);
        Assert.True(engine.BattleHasEnded.Object);
        Assert.Null(engine.CurrentCreature);
        Assert.False(viewModel.CheatCommand.CanExecute(null));
        Assert.Equal(1, victories);
        Assert.Single(board.CreatureViewModels);
        Assert.All(viewModel.TeamStatsViewModel.Hostiles, team => Assert.Equal(0, team.Count));
        Assert.Null(engine.PlayerSelectSquare(0));

        viewModel.ResetCreaturesCommand.Execute(null);
        Assert.Equal(3, engine.Creatures.Count);
        Assert.False(engine.BattleHasEnded.Object);
    }

    [Fact]
    public void CheatIsUnavailableWhileAutomaticActionRuns()
    {
        var engine = new ComplexEngine(null!);
        var board = new BoardViewModel(engine, 40, 40, 30, 20);
        var viewModel = new ActOutSceneViewModelComplexEngine(engine, board, null!);
        viewModel.InitializeScene(new Scene("Test", 5, 5));
        engine.BattleHasStarted.Object = true;
        engine.AutoRunning.Object = true;

        Assert.False(viewModel.CheatCommand.CanExecute(null));
        engine.RemoveHostileCreatures();
        Assert.False(engine.BattleHasEnded.Object);
    }
}
