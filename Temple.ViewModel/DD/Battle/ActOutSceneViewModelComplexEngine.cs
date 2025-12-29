using Craft.Logging;
using Craft.Utils;
using Temple.Domain.Entities.DD.Battle;
using Temple.ViewModel.DD.Battle.BusinessLogic;
using Temple.ViewModel.DD.Battle.BusinessLogic.Complex;

namespace Temple.ViewModel.DD.Battle
{
    public class ActOutSceneViewModelComplexEngine : ActOutSceneViewModelBase
    {
        public ActOutSceneViewModelComplexEngine(
            IEngine engine,
            BoardViewModelBase boardViewModel,
            ObservableObject<Scene> selectedScene,
            ILogger logger) : base(
                engine,
                boardViewModel,
                selectedScene,
                logger)
        {
            boardViewModel.PlayerClickedSquare += async (s, e) =>
            {
                var creatureAction = _engine.PlayerSelectSquare(e.SquareIndex);

                if (creatureAction == null)
                {
                    // Occurs e.g. when the user clicks a non-highlighted square that doesn't trigger anything
                    return;
                }

                _boardViewModel.ClearPlayerOptions();

                switch (creatureAction)
                {
                    case CreatureMove:
                        _boardViewModel.MoveCurrentCreature(
                            _engine.CurrentCreature,
                            _engine.CurrentCreaturePath);
                        break;
                    case NoEvent:
                        await Proceed();
                        break;
                    case CreatureAttackRanged:
                        _boardViewModel.AnimateAttack(
                            _engine.CurrentCreature,
                            _engine.TargetCreature,
                            true);
                        break;
                    case CreatureAttack:
                        _boardViewModel.AnimateAttack(
                            _engine.CurrentCreature,
                            _engine.TargetCreature,
                            false);
                        break;
                }
            };
        }

        protected override async Task Proceed()
        {
            if (_paused)
            {
                return;
            }

            while (!_engine.BattleDecided)
            {
                if (_engine.BattleroundCompleted)
                {
                    _engine.StartBattleRound();

                    _boardViewModel.UpdateCreatureViewModels(
                        _engine.Creatures,
                        _engine.CurrentCreature);
                }

                if (_engine.NextEventOccursAutomatically)
                {
                    _engine.AutoRunning.Object = true;
                    var nextEvent = await _engine.ExecuteNextEvent();

                    switch (nextEvent)
                    {
                        case CreaturePass:
                            _engine.SwitchToNextCreature();
                            _boardViewModel.UpdateCreatureViewModels(
                                _engine.Creatures,
                                _engine.CurrentCreature);
                            continue;
                        // Bemærk, at vi for de næste 3 ikke kalder continue men derimod break, dvs vi træder ud af løkken og dermed hele Proceed
                        // metoden. Den kaldes igen, når vi håndterer disse evenst: MoveCreatureAnimationCompleted, AttackAnimationCompleted
                        case CreatureMove:
                            _boardViewModel.MoveCurrentCreature(
                                _engine.CurrentCreature,
                                _engine.CurrentCreaturePath);
                            break;
                        case CreatureAttackRanged:
                            _boardViewModel.AnimateAttack(
                                _engine.CurrentCreature,
                                _engine.TargetCreature,
                                true);
                            break;
                        case CreatureAttack:
                            _boardViewModel.AnimateAttack(
                                _engine.CurrentCreature,
                                _engine.TargetCreature,
                                false);
                            break;
                        case NoEvent:
                            _boardViewModel.UpdateCreatureViewModels(
                                _engine.Creatures,
                                _engine.CurrentCreature);
                            continue;
                        default:
                            throw new ArgumentException("unexpected battle event");
                    }
                }
                else
                {
                    _engine.AutoRunning.Object = false;

                    _boardViewModel.HighlightPlayerOptions(
                        _engine.SquareIndexForCurrentCreature.Object.Value,
                        _engine.SquareIndexesCurrentCreatureCanMoveTo.Object.Keys.ToHashSet(),
                        _engine.SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object,
                        _engine.SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object);

                    // Diagnostics
                    //_logger.WriteLine(LogMessageCategory.Information, "(Proceed method about to exit - Initiative will go to the player)");
                }

                break;
            }

            if (_engine.BattleDecided)
            {
                _logger?.WriteLine(LogMessageCategory.Information, "Battle was decided");
            }
            else
            {
                // Diagnostics
                //_logger.WriteLine(LogMessageCategory.Information, "(Proceed method exiting)");
            }
        }
    }
}
