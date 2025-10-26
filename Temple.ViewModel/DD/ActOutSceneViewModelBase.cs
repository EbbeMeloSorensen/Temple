using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Utils;
using Craft.ViewModel.Utils;
using Temple.Domain.Entities.DD;
using Temple.ViewModel.DD.BusinessLogic;

namespace Temple.ViewModel.DD
{
    public abstract class ActOutSceneViewModelBase : ViewModelBase
    {
        private bool _updateBoard;
        private bool _animateMoves;
        private bool _animateAttacks;
        private double _moveAnimationSpeed;
        private double _attackAnimationSpeed;

        protected bool _paused;
        protected ILogger _logger;
        protected IEngine _engine;
        protected BoardViewModelBase _boardViewModel;
        protected RelayCommand _pauseCommand;
        protected RelayCommand _resumeCommand;
        protected RelayCommand _resetCreaturesCommand;
        protected AsyncCommand _startBattleCommand;
        protected AsyncCommand _passCurrentCreatureCommand;
        protected AsyncCommand _automateCurrentCreatureCommand;

        public TeamStatsViewModel TeamStatsViewModel { get; }

        public bool UpdateBoard
        {
            get => _updateBoard;
            set
            {
                _updateBoard = value;
                RaisePropertyChanged();
            }
        }

        public bool AnimateMoves
        {
            get => _animateMoves;
            set
            {
                _animateMoves = value;
                RaisePropertyChanged();
            }
        }

        public bool AnimateAttacks
        {
            get => _animateAttacks;
            set
            {
                _animateAttacks = value;
                RaisePropertyChanged();
            }
        }

        public double MoveAnimationSpeed
        {
            get => _moveAnimationSpeed;
            set
            {
                _moveAnimationSpeed = value;

                _boardViewModel.TicksPrStepForCreatureMoveAnimation =
                    (int)Math.Round(500000 * Math.Pow(10, 1 - _moveAnimationSpeed));

                RaisePropertyChanged();
            }
        }

        public double AttackAnimationSpeed
        {
            get => _attackAnimationSpeed;
            set
            {
                _attackAnimationSpeed = value;

                var timeSpanForAttackAnimation = new TimeSpan((long)Math.Round(500000 * Math.Pow(10, 1 - _attackAnimationSpeed)));
                _boardViewModel.DurationForAttackAnimation = $"0:0:{timeSpanForAttackAnimation.Seconds}.{timeSpanForAttackAnimation.Milliseconds.ToString().PadLeft(3, '0')}";

                RaisePropertyChanged();
            }
        }

        public RelayCommand ResetCreaturesCommand
        {
            get
            {
                return _resetCreaturesCommand ?? (_resetCreaturesCommand = new RelayCommand(
                    ResetCreatures,
                    CanResetCreatures));
            }
        }

        public RelayCommand PauseCommand
        {
            get
            {
                return _pauseCommand ?? (_pauseCommand = new RelayCommand(
                    Pause,
                    CanPause));
            }
        }

        public RelayCommand ResumeCommand
        {
            get
            {
                return _resumeCommand ?? (_resumeCommand = new RelayCommand(
                    Resume,
                    CanResume));
            }
        }

        public AsyncCommand StartBattleCommand
        {
            get
            {
                return _startBattleCommand ?? (_startBattleCommand = new AsyncCommand(
                    StartBattle,
                    CanStartBattle));
            }
        }

        public AsyncCommand PassCurrentCreatureCommand
        {
            get
            {
                return _passCurrentCreatureCommand ?? (_passCurrentCreatureCommand = new AsyncCommand(
                    PassCurrentCreature,
                    CanPassCurrentCreature));
            }
        }

        public AsyncCommand AutomateCurrentCreatureCommand
        {
            get
            {
                return _automateCurrentCreatureCommand ?? (_automateCurrentCreatureCommand = new AsyncCommand(
                    AutomateCurrentCreature,
                    CanAutomateCurrentCreature));
            }
        }

        public ActOutSceneViewModelBase(
            IEngine engine,
            BoardViewModelBase boardViewModel,
            ObservableObject<Scene> selectedScene,
            ILogger logger)
        {
            _updateBoard = true;
            _animateMoves = true;
            _animateAttacks = true;

            _engine = engine;
            _boardViewModel = boardViewModel;
            _logger = logger;

            MoveAnimationSpeed = 0.5;
            AttackAnimationSpeed = 0.5;

            TeamStatsViewModel = new TeamStatsViewModel();

            _engine.BattleHasStarted.PropertyChanged += (s, e) => UpdateCommandStates();
            _engine.BattleHasEnded.PropertyChanged += (s, e) => UpdateCommandStates();
            _engine.AutoRunning.PropertyChanged += (s, e) => UpdateCommandStates();

            _engine.CreatureKilled += (s, e) => TeamStatsViewModel.Update(_engine.Creatures);

            boardViewModel.MoveCreatureAnimationCompleted += async (s, e) => await Proceed();

            boardViewModel.AttackAnimationCompleted += async (s, e) =>
            {
                if (UpdateBoard)
                {
                    _boardViewModel.UpdateCreatureViewModels(
                        _engine.Creatures,
                        _engine.CurrentCreature);
                }

                TeamStatsViewModel.Update(_engine.Creatures);

                if (_engine.BattleDecided)
                {
                    UpdateCommandStates();
                    return;
                }

                await Proceed();
            };

            selectedScene.PropertyChanged += (s, e) =>
            {
                _engine.Scene = (s as ObservableObject<Scene>)?.Object;

                _engine.InitializeCreatures();

                TeamStatsViewModel.Initialize(_engine.Creatures);

                _boardViewModel.UpdateCreatureViewModels(
                    _engine.Creatures,
                    _engine.CurrentCreature);
            };
        }

        protected abstract Task Proceed();

        private void Pause()
        {
            _paused = true;
            UpdateCommandStates();
        }

        private bool CanPause()
        {
            var result =
                _engine.Scene != null &&
                _engine.BattleHasStarted.Object &&
                !_engine.BattleHasEnded.Object &&
                !_paused;

            return
                _engine.Scene != null &&
                _engine.BattleHasStarted.Object &&
                !_engine.BattleHasEnded.Object &&
                !_paused;
        }

        private void Resume()
        {
            if (!_paused)
            {
                return;
            }

            _paused = false;
            UpdateCommandStates();
            Proceed();
        }

        private bool CanResume()
        {
            return
                _engine.Scene != null &&
                _engine.BattleHasStarted.Object &&
                !_engine.BattleHasEnded.Object &&
                _paused;
        }

        private void ResetCreatures()
        {
            var message = $"Resetting scene \"{_engine.Scene.Name}\"";
            _logger?.WriteLine(LogMessageCategory.Information, message);

            _engine.InitializeCreatures();

            TeamStatsViewModel.Initialize(_engine.Creatures);

            _boardViewModel.UpdateCreatureViewModels(
                _engine.Creatures,
                _engine.CurrentCreature);

            _paused = false;
        }

        private bool CanResetCreatures()
        {
            var result =
                _engine.Scene != null &&
                _engine.BattleHasEnded.Object ||
                _engine.BattleHasStarted.Object &&
                _paused;

            return
                _engine.Scene != null &&
                _engine.BattleHasEnded.Object ||
                _engine.BattleHasStarted.Object &&
                _paused;
        }

        private async Task StartBattle()
        {
            _engine.StartBattle();

            if (UpdateBoard)
            {
                _boardViewModel.UpdateCreatureViewModels(
                    _engine.Creatures,
                    _engine.CurrentCreature);
            }

            await Proceed();
        }

        private bool CanStartBattle()
        {
            return _engine.CanStartBattle();
        }

        private async Task PassCurrentCreature()
        {
            var message =
                $"        {_engine.CurrentCreature.CreatureType.Name}{_engine.Tag(_engine.CurrentCreature)} passes";

            _logger?.WriteLine(LogMessageCategory.Information, message);

            _engine.SquareIndexesCurrentCreatureCanMoveTo.Object = null;
            _engine.SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = null;
            _engine.SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object = null;

            _engine.SwitchToNextCreature();

            if (UpdateBoard)
            {
                _boardViewModel.UpdateCreatureViewModels(
                    _engine.Creatures,
                    _engine.CurrentCreature);
            }

            await Proceed();
        }

        private bool CanPassCurrentCreature()
        {
            return
                _engine.BattleHasStarted.Object &&
                !_engine.BattleHasEnded.Object &&
                !_engine.AutoRunning.Object;
        }

        private async Task AutomateCurrentCreature()
        {
            _engine.CurrentCreature.IsAutomatic = true;
            _engine.SquareIndexesCurrentCreatureCanMoveTo.Object = null;
            _engine.SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = null;
            _engine.SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object = null;

            await Proceed();
        }

        private bool CanAutomateCurrentCreature()
        {
            return
                _engine.BattleHasStarted.Object &&
                !_engine.BattleHasEnded.Object &&
                !_engine.AutoRunning.Object;
        }

        private void UpdateCommandStates()
        {
            StartBattleCommand.RaiseCanExecuteChanged();
            PauseCommand.RaiseCanExecuteChanged();
            ResumeCommand.RaiseCanExecuteChanged();
            PassCurrentCreatureCommand.RaiseCanExecuteChanged();
            AutomateCurrentCreatureCommand.RaiseCanExecuteChanged();
            ResetCreaturesCommand.RaiseCanExecuteChanged();
        }
    }
}
