using Stateless;

namespace Temple.Application.State.NewPrinciple;

public class GameStateMachine
{
    private readonly StateMachine<SceneType, Trigger> _machine;

    public GameScene CurrentScene { get; private set; }

    public event Action<GameScene>? SceneChanged;

    public GameStateMachine()
    {
        CurrentScene = new GameScene(
            SceneType.Starting);

        _machine = new StateMachine<SceneType, Trigger>(
            () => CurrentScene.Type,
            s => CurrentScene = CurrentScene with { Type = s }
        );

        Configure();
    }

    private void Configure()
    {
        _machine.Configure(SceneType.Starting)
            .PermitDynamic(Trigger.Initialize, () =>
            {
                var next = new GameScene(SceneType.MainMenu);
                ChangeScene(next);
                return next.Type;
            });

        _machine.Configure(SceneType.MainMenu)
            .PermitDynamic(Trigger.GoToSmurfManagement, () =>
            {
                var next = new GameScene(SceneType.SmurfManagement);
                ChangeScene(next);
                return next.Type;
            })
            .PermitDynamic(Trigger.GoToPeopleManagement, () =>
            {
                var next = new GameScene(SceneType.PeopleManagement);
                ChangeScene(next);
                return next.Type;
            })
            .PermitDynamic(Trigger.StartNewGame, () =>
            {
                var next = new GameScene(SceneType.Intro);
                ChangeScene(next);
                return next.Type;
            })
            .PermitDynamic(Trigger.ShutdownRequested, () =>
            {
                var next = new GameScene(SceneType.ShuttingDown);
                ChangeScene(next);
                return next.Type;
            });

        _machine.Configure(SceneType.SmurfManagement)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = new GameScene(SceneType.MainMenu);
                ChangeScene(next);
                return next.Type;
            });

        _machine.Configure(SceneType.PeopleManagement)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = new GameScene(SceneType.MainMenu);
                ChangeScene(next);
                return next.Type;
            });

        _machine.Configure(SceneType.Intro)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = new GameScene(SceneType.Battle_First);
                ChangeScene(next);
                return next.Type;
            });

        _machine.Configure(SceneType.Battle_First)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = new GameScene(SceneType.ExploreArea_AfterFirstBattle);
                ChangeScene(next);
                return next.Type;
            })
            .PermitDynamic(Trigger.GoToDefeat, () =>
            {
                var next = new GameScene(SceneType.Defeat);
                ChangeScene(next);
                return next.Type;
            });

        _machine.Configure(SceneType.ExploreArea_AfterFirstBattle)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = new GameScene(SceneType.Battle_Final);
                ChangeScene(next);
                return next.Type;
            });

        _machine.Configure(SceneType.Battle_Final)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = new GameScene(SceneType.Victory);
                ChangeScene(next);
                return next.Type;
            })
            .PermitDynamic(Trigger.GoToDefeat, () =>
            {
                var next = new GameScene(SceneType.Defeat);
                ChangeScene(next);
                return next.Type;
            });

        _machine.Configure(SceneType.Defeat)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = new GameScene(SceneType.MainMenu);
                ChangeScene(next);
                return next.Type;
            });

        _machine.Configure(SceneType.Victory)
            .PermitDynamic(Trigger.ExitState, () =>
            {
                var next = new GameScene(SceneType.MainMenu);
                ChangeScene(next);
                return next.Type;
            });


        //_machine.Configure(SceneType.Exploration)
        //    .PermitDynamic(Trigger.EncounterEnemy, () =>
        //    {
        //        var payload = ScenePayload.ForBattle("Wolves");
        //        var next = new GameScene(SceneType.Battle, payload);
        //        ChangeScene(next);
        //        return next.Type;
        //    })
        //    .PermitDynamic(Trigger.TalkToNpc, () =>
        //    {
        //        var payload = ScenePayload.ForDialog("ElderNPC");
        //        var next = new GameScene(SceneType.Dialog, payload);
        //        ChangeScene(next);
        //        return next.Type;
        //    });

        //_machine.Configure(SceneType.Battle)
        //    .PermitDynamic(Trigger.EndBattle, () =>
        //    {
        //        var next = new GameScene(
        //            SceneType.Exploration,
        //            ScenePayload.ForExploration("ForestEntry"));
        //        ChangeScene(next);
        //        return next.Type;
        //    });

        //_machine.Configure(SceneType.Dialog)
        //    .PermitDynamic(Trigger.EndDialog, () =>
        //    {
        //        var next = new GameScene(
        //            SceneType.Exploration,
        //            ScenePayload.ForExploration("VillageSquare"));
        //        ChangeScene(next);
        //        return next.Type;
        //    });
    }

    private void ChangeScene(GameScene scene)
    {
        CurrentScene = scene;
        SceneChanged?.Invoke(scene);
    }

    public Task FireAsync(Trigger t) => _machine.FireAsync(t);
}