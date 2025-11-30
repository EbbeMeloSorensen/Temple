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
            SceneType.Exploration,
            ScenePayload.ForExploration("ForestEntry"));

        _machine = new StateMachine<SceneType, Trigger>(
            () => CurrentScene.Type,
            s => CurrentScene = CurrentScene with { Type = s }
        );

        Configure();
    }

    private void Configure()
    {
        _machine.Configure(SceneType.Exploration)
            .PermitDynamic(Trigger.EncounterEnemy, () =>
            {
                var payload = ScenePayload.ForBattle("Wolves");
                var next = new GameScene(SceneType.Battle, payload);
                ChangeScene(next);
                return next.Type;
            })
            .PermitDynamic(Trigger.TalkToNpc, () =>
            {
                var payload = ScenePayload.ForDialog("ElderNPC");
                var next = new GameScene(SceneType.Dialog, payload);
                ChangeScene(next);
                return next.Type;
            });

        _machine.Configure(SceneType.Battle)
            .PermitDynamic(Trigger.EndBattle, () =>
            {
                var next = new GameScene(
                    SceneType.Exploration,
                    ScenePayload.ForExploration("ForestEntry"));
                ChangeScene(next);
                return next.Type;
            });

        _machine.Configure(SceneType.Dialog)
            .PermitDynamic(Trigger.EndDialog, () =>
            {
                var next = new GameScene(
                    SceneType.Exploration,
                    ScenePayload.ForExploration("VillageSquare"));
                ChangeScene(next);
                return next.Type;
            });
    }

    private void ChangeScene(GameScene scene)
    {
        CurrentScene = scene;
        SceneChanged?.Invoke(scene);
    }

    public Task FireAsync(Trigger t) => _machine.FireAsync(t);
}