using Craft.Logging;
using Craft.Utils;
using Temple.Domain.Entities.DD.Battle;

namespace Temple.ViewModel.DD.Battle.BusinessLogic
{
    public interface IEngine
    {
        int[] CurrentCreaturePath { get; }

        bool BattleroundCompleted { get; }

        bool BattleDecided { get; }

        List<Creature> Creatures { get; }

        Creature CurrentCreature { get; }

        Creature TargetCreature { get; }

        ObservableObject<int?> SquareIndexForCurrentCreature { get; }

        ObservableObject<Dictionary<int, double>> SquareIndexesCurrentCreatureCanMoveTo { get; }

        ObservableObject<HashSet<int>> SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon { get; }

        ObservableObject<HashSet<int>> SquareIndexesCurrentCreatureCanAttackWithRangedWeapon { get; }

        ObservableObject<bool> BattleHasStarted { get; }

        ObservableObject<bool> BattleHasEnded { get; }

        ObservableObject<bool> AutoRunning { get; }

        bool NextEventOccursAutomatically { get; }

        ILogger Logger { get; set; }

        Scene Scene { get; set; }

        BoardTileMode BoardTileMode { get; set; }

        event EventHandler CreatureKilled;

        void Randomize();

        Task<IBattleEvent> ExecuteNextEvent();

        IBattleEvent? PlayerSelectSquare(
            int squareIndex);

        void StartBattle();

        bool CurrentPlayerControlledCreatureHasAnyOptionsLeft();

        bool CanStartBattle();

        void InitializeCreatures();

        void StartBattleRound();

        void SwitchToNextCreature();

        string Tag(
            Creature creature);
    }
}
