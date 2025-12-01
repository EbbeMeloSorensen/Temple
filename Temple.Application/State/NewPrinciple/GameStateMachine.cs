using Stateless;

namespace Temple.Application.State.NewPrinciple;

public class GameStateMachine
{
    internal readonly StateMachine<GameScene, Trigger> _machine;

    public GameScene CurrentScene { get; private set; }

    public event Action<GameScene>? SceneChanged;

    public GameStateMachine()
    {
        CurrentScene = new GameScene(
            SceneType.Starting);

        var starting = new GameScene(SceneType.Starting);
        var mainMenu = new GameScene(SceneType.MainMenu);
        var smurfManagement = new GameScene(SceneType.SmurfManagement);
        var peopleManagement = new GameScene(SceneType.PeopleManagement);
        var shuttingDown = new GameScene(SceneType.ShuttingDown);
        var intro = new GameScene(SceneType.Intro);
        var battleFirst = new GameScene(SceneType.Battle_First);
        var battleFinal= new GameScene(SceneType.Battle_Final);
        var exploreAreaAfterFirstBattle = new GameScene(SceneType.ExploreArea_AfterFirstBattle);
        var defeat = new GameScene(SceneType.Defeat);
        var victory = new GameScene(SceneType.Victory);

        _machine = new StateMachine<GameScene, Trigger>(starting);

        _machine.Configure(starting)
            .PermitDynamic(Trigger.Initialize, () =>
            {
                var next = mainMenu;
                ChangeScene(next);
                return next;
            });

        _machine.Configure(mainMenu)
            .PermitDynamic(Trigger.GoToSmurfManagement, () =>
            {
                var next = smurfManagement;
                ChangeScene(next);
                return next;
            })
            .PermitDynamic(Trigger.GoToPeopleManagement, () =>
            {
                var next = peopleManagement;
                ChangeScene(next);
                return next;
            })
            .PermitDynamic(Trigger.StartNewGame, () =>
            {
                var next = intro;
                ChangeScene(next);
                return next;
            })
            .PermitDynamic(Trigger.ShutdownRequested, () =>
            {
                var next = shuttingDown;
                ChangeScene(next);
                return next;
            });

        _machine.Configure(smurfManagement)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = mainMenu;
                ChangeScene(next);
                return next;
            });

        _machine.Configure(peopleManagement)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = mainMenu;
                ChangeScene(next);
                return next;
            });

        _machine.Configure(intro)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = battleFirst;
                ChangeScene(next);
                return next;
            });

        _machine.Configure(battleFirst)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = exploreAreaAfterFirstBattle;
                ChangeScene(next);
                return next;
            })
            .PermitDynamic(Trigger.GoToDefeat, () =>
            {
                var next = defeat;
                ChangeScene(next);
                return next;
            });

        _machine.Configure(exploreAreaAfterFirstBattle)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = battleFinal;
                ChangeScene(next);
                return next;
            });

        _machine.Configure(battleFinal)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = victory;
                ChangeScene(next);
                return next;
            })
            .PermitDynamic(Trigger.GoToDefeat, () =>
            {
                var next = defeat;
                ChangeScene(next);
                return next;
            });

        _machine.Configure(defeat)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = mainMenu;
                ChangeScene(next);
                return next;
            });

        _machine.Configure(victory)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = mainMenu;
                ChangeScene(next);
                return next;
            });
    }

    private void ChangeScene(GameScene scene)
    {
        CurrentScene = scene;
        SceneChanged?.Invoke(scene);
    }

    public Task FireAsync(Trigger t) => _machine.FireAsync(t);
}