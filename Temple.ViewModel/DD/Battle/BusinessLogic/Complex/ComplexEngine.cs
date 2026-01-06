using Craft.Algorithms;
using Craft.DataStructures.Graph;
using Craft.Logging;
using Craft.Math;
using Craft.Utils;
using Temple.Domain.Entities.DD.Battle;

namespace Temple.ViewModel.DD.Battle.BusinessLogic.Complex
{
    public class ComplexEngine : IEngine
    {
        private Dictionary<Creature, int> _creatureIdMap;
        private GraphAdjacencyList<Point2DVertex, EmptyEdge> _wallGraph;
        private Random _random = new Random(0);
        private Scene _scene;
        private HashSet<int> _obstacleIndexes;
        private Queue<Creature> _actingOrder;
        private Queue<EvasionEvent> _evasionEvents;
        private Creature _evadingCreature;
        private int[] _previous;
        private int _battleRoundCount;
        private bool _currentCreatureJustMoved;
        private double _moveDistanceRemaningForCurrentCreature;

        public int[] CurrentCreaturePath { get; private set; }

        public bool BattleroundCompleted { get; private set; }

        public bool BattleDecided { get; private set; }

        public List<Creature> Creatures { get; private set; }

        public Creature CurrentCreature { get; private set; }

        public Creature TargetCreature { get; private set; }

        public ObservableObject<int?> SquareIndexForCurrentCreature { get; }

        public ObservableObject<Dictionary<int, double>> SquareIndexesCurrentCreatureCanMoveTo { get; }

        public ObservableObject<HashSet<int>> SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon { get; }

        public ObservableObject<HashSet<int>> SquareIndexesCurrentCreatureCanAttackWithRangedWeapon { get; }

        public ObservableObject<bool> BattleHasStarted { get; }

        public ObservableObject<bool> BattleHasEnded { get; }

        public ObservableObject<bool> AutoRunning { get; }

        public bool NextEventOccursAutomatically
        {
            get
            {
                return CurrentCreature.IsAutomatic ||
                         !CurrentPlayerControlledCreatureHasAnyOptionsLeft() ||
                         _evasionEvents.Count > 0;
            }
        }

        public ILogger Logger { get; set; }

        public Scene Scene
        {
            get { return _scene; }
            set
            {
                _scene = value;

                if (_scene == null)
                {
                    _obstacleIndexes = new HashSet<int>();
                }
                else
                {
                    Logger?.WriteLine(LogMessageCategory.Information,
                        $"Selected scene: \"{_scene.Name}\"");

                    _obstacleIndexes = new HashSet<int>(_scene.Obstacles
                        .Select(o => o.IndexOfOccupiedSquare(_scene.Columns)));

                    // Establish a graph of the walls for computing visibility
                    var vertices = new List<Point2DVertex>
                    {
                        new Point2DVertex(-0.5, -0.5),
                        new Point2DVertex(_scene.Columns - 0.5, -0.5),
                        new Point2DVertex(_scene.Columns - 0.5, _scene.Rows - 0.5),
                        new Point2DVertex(-0.5, _scene.Rows - 0.5),
                    };

                    _scene.Obstacles
                        .Where(o => o.ObstacleType == ObstacleType.Wall)
                        .Select(o => o.IndexOfOccupiedSquare(_scene.Columns))
                        .Select(i => new Point2D(
                            i.ConvertToXCoordinate(_scene.Columns),
                            i.ConvertToYCoordinate(_scene.Columns)))
                        .ToList()
                        .ForEach(p =>
                        {
                            vertices.Add(new Point2DVertex(p.X - 0.5, p.Y - 0.5));
                            vertices.Add(new Point2DVertex(p.X + 0.5, p.Y - 0.5));
                            vertices.Add(new Point2DVertex(p.X + 0.5, p.Y + 0.5));
                            vertices.Add(new Point2DVertex(p.X - 0.5, p.Y + 0.5));
                        });

                    _wallGraph = new GraphAdjacencyList<Point2DVertex, EmptyEdge>(vertices, false);

                    // Todo: You can eliminate and combine edges
                    Enumerable.Range(0, vertices.Count / 4).ToList().ForEach(i =>
                    {
                        _wallGraph.AddEdge(i * 4, i * 4 + 1);
                        _wallGraph.AddEdge(i * 4 + 1, i * 4 + 2);
                        _wallGraph.AddEdge(i * 4 + 2, i * 4 + 3);
                        _wallGraph.AddEdge(i * 4 + 3, i * 4);
                    });
                }

                InitializeCreatures();
            }
        }

        public BoardTileMode BoardTileMode { get; set; }

        public event EventHandler CreatureKilled;

        public ComplexEngine(
            ILogger logger)
        {
            SquareIndexForCurrentCreature = new ObservableObject<int?>();
            SquareIndexesCurrentCreatureCanMoveTo = new ObservableObject<Dictionary<int, double>>();
            SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon = new ObservableObject<HashSet<int>>();
            SquareIndexesCurrentCreatureCanAttackWithRangedWeapon = new ObservableObject<HashSet<int>>();

            BattleHasStarted = new ObservableObject<bool>();
            BattleHasEnded = new ObservableObject<bool>();
            AutoRunning = new ObservableObject<bool>();
            Logger = logger;
            _actingOrder = new Queue<Creature>();
            _evasionEvents = new Queue<EvasionEvent>();
        }

        public void Randomize()
        {
            _random = new Random((int)DateTime.UtcNow.Ticks);
        }

        public async Task<IBattleEvent> ExecuteNextEvent()
        {
            if (_evasionEvents.Any())
            {
                var evasionEvent = _evasionEvents.Dequeue();

                switch (evasionEvent)
                {
                    case InitiativeSwitch initiativeSwitch:
                        Logger?.WriteLine(LogMessageCategory.Information, $"        Initiative goes to {Tag(initiativeSwitch.Creature)}");
                        CurrentCreature = initiativeSwitch.Creature;
                        SquareIndexForCurrentCreature.Object = CurrentCreature.IndexOfOccupiedSquare(_scene.Columns);
                        return new NoEvent();
                    case Move move:
                        {
                            CurrentCreaturePath = move.Path;

                            MoveCurrentCreature(CurrentCreaturePath.Last());

                            if (!_evasionEvents.Any() && !CurrentCreature.IsAutomatic)
                            {
                                IdentifyOptionsForCurrentPlayerControlledCreature(true);
                            }

                            return new CreatureMove();
                        }
                    case OpportunityAttack opportunityAttack:
                        {
                            var attacker = opportunityAttack.Creature;

                            AttackOpponent(
                                attacker,
                                _evadingCreature,
                                new MeleeAttack("Opportunity attack", 5),
                                false,
                                out var evadingCreatureWasHit,
                                out var evadingCreatureWasKilled);

                            if (evadingCreatureWasKilled)
                            {
                                _evasionEvents.Clear();

                                OnCreatureKilled();

                                if (!OpponentsStillRemaining(attacker))
                                {
                                    SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = null;
                                    SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object = null;
                                    BattleDecided = true;
                                }
                            }

                            TargetCreature = _evadingCreature;

                            return new CreatureAttack();
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (!CurrentCreature.IsAutomatic && !CurrentPlayerControlledCreatureHasAnyOptionsLeft())
            {
                return new CreaturePass();
            }

            MoveCreatureResult moveCreatureResult = null;

            // We peek the next attack rather than deque, because we might not use it in this round
            // such as when no enemies are within range and the creature can only move

            Attack attack;

            // Discard unknown attacks
            while (true)
            {
                CurrentCreature.Attacks.TryPeek(out attack);

                if (attack == null ||
                    attack is MeleeAttack ||
                    attack is RangedAttack)
                {
                    break;
                }

                CurrentCreature.Attacks.Dequeue();
            }

            if (attack != null)
            {
                switch (attack)
                {
                    case MeleeAttack meleeAttack:
                        {
                            var potentialTargetsOfMeleeAttack = IdentifyPotentialTargetsOfMeleeAttack().ToList();

                            if (potentialTargetsOfMeleeAttack.Any())
                            {
                                attack = CurrentCreature.Attacks.Dequeue();
                                var targetCreature = potentialTargetsOfMeleeAttack.First();

                                AttackOpponent(
                                    CurrentCreature,
                                    targetCreature,
                                    attack,
                                    false,
                                    out var opponentWasHit,
                                    out var opponentWasKilled);

                                if (opponentWasKilled)
                                {
                                    OnCreatureKilled();

                                    if (!OpponentsStillRemaining(CurrentCreature))
                                    {
                                        BattleDecided = true;
                                    }
                                }

                                TargetCreature = targetCreature;
                                _currentCreatureJustMoved = false;

                                return new CreatureAttack();
                            }

                            break;
                        }
                    case RangedAttack rangedAttack:
                        {
                            var range = rangedAttack.Range;

                            if (_currentCreatureJustMoved)
                            {
                                return ExecuteRangedAttackOrPass(range);
                            }

                            // Determine if the creature can move to a more suitable position before attacking
                            moveCreatureResult = await DetermineDestinationOfCurrentCreatureWithRangedAttack(range);

                            if (!moveCreatureResult.IndexOfDestinationSquare.HasValue)
                            {
                                return ExecuteRangedAttackOrPass(range);
                            }

                            break;
                        }
                }
            }
            else
            {
                _moveDistanceRemaningForCurrentCreature = 0.0;
            }

            // Hvis vi når hertil, så kan eller vil væsenet ikke angribe.
            // Så er spørgsmålet, om den vil bevæge sig, eller alternativt overlade turen til næste væsen

            if (!_currentCreatureJustMoved && _moveDistanceRemaningForCurrentCreature > 0)
            {
                if (moveCreatureResult == null)
                {
                    moveCreatureResult = await DetermineDestinationOfCurrentCreatureWithMeleeAttack();
                }

                if (moveCreatureResult.IndexOfDestinationSquare.HasValue)
                {
                    return MoveOrEvade(moveCreatureResult);
                }
            }

            return new CreaturePass();
        }

        public IBattleEvent PlayerSelectSquare(
            int squareIndex)
        {
            if (!BattleHasStarted.Object ||
                BattleHasEnded.Object ||
                AutoRunning.Object)
            {
                return null;
            }

            if (SquareIndexesCurrentCreatureCanMoveTo.Object != null &&
                SquareIndexesCurrentCreatureCanMoveTo.Object.Keys.Contains(squareIndex))
            {
                // Player decides to move current creature
                _moveDistanceRemaningForCurrentCreature -= SquareIndexesCurrentCreatureCanMoveTo.Object[squareIndex];
                SquareIndexesCurrentCreatureCanMoveTo.Object = null;

                // 1) Find ud af, hvilke væsener der er ved siden af væsenet, for hvert af de felter, der indgår i væsenets path
                // 2) Find ud af, hvilke step, der involverer, at current creature bevæger sig væk fra en modstander
                // 3) Hvis det allerede er ved første step, så bevæger væsenet sig ikke, og så er udfaldet, at
                //    de væsener, der forlades, får et opportunity attack.
                //    Hvis det først er ved et senere step at man forlader en modstander, så bevæger væsenet sig,
                //    Måske kan man gøre det, at enginen, gemmer en sekvens af moves og opportunity attacks,
                //    f.eks. move/oa/oa/move/oa/move
                //    dvs udfaldet af dette trin sådan set ingenting, men når så man skal til næste trin, så
                //    skal enginen se, om der ligger noget på "kø"...

                var path = _previous.DeterminePath(squareIndex);
                var evadedCreatures = IdentifyEvadedOpponents(path);

                if (evadedCreatures.Any())
                {
                    // Player controlled creature moves in a way that provokes a number of opportunity attacks
                    PopulateEvasionEventQueue(path, evadedCreatures);
                    _evadingCreature = CurrentCreature;
                    Logger?.WriteLine(LogMessageCategory.Information, $"        {Tag(CurrentCreature)} evades");

                    return new NoEvent();
                }

                // Player controlled creature moves ordinarily, without provoking an opportunity attack
                MoveCurrentCreature(squareIndex);
                CurrentCreaturePath = path;
                IdentifyOptionsForCurrentPlayerControlledCreature(true);

                return new CreatureMove();
            }

            if (SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object != null &&
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object.Contains(squareIndex))
            {
                // Player decides to perform a melee attack with current creature
                var attack = CurrentCreature.Attacks.Dequeue();
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = null;

                var opponent = Creatures.Single(c => c.IndexOfOccupiedSquare(Scene.Columns) == squareIndex);

                AttackOpponent(
                    CurrentCreature,
                    opponent,
                    attack,
                    false,
                    out var opponentWasHit,
                    out var opponentWasKilled);

                if (opponentWasKilled && !OpponentsStillRemaining(CurrentCreature))
                {
                    SquareIndexesCurrentCreatureCanMoveTo.Object = null;
                    BattleDecided = true;
                }
                else
                {
                    IdentifyOptionsForCurrentPlayerControlledCreature(false);
                }

                TargetCreature = opponent;

                return new CreatureAttack();
            }

            if (SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object != null &&
                SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object.Contains(squareIndex))
            {
                // Player decides to perform a ranged attack with current creature
                var attack = CurrentCreature.Attacks.Dequeue();
                SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object = null;

                var opponent = Creatures.Single(c => c.IndexOfOccupiedSquare(Scene.Columns) == squareIndex);

                // We need to communicate this to the stake holders for animation of a ranged attack
                TargetCreature = opponent;

                // If the current creature stands next to an opponent, it gets disadvantage
                var opponents = Creatures
                    .Where(c => c.IsHostile != CurrentCreature.IsHostile)
                    .ToList();

                var currentSquareIndex = CurrentCreature.IndexOfOccupiedSquare(_scene.Columns);

                ClosestOpponent(
                    currentSquareIndex,
                    opponents,
                    out var distanceToClosestOpponent);

                var disadvantage = distanceToClosestOpponent < 1.5;

                AttackOpponent(
                    CurrentCreature,
                    opponent,
                    attack,
                    disadvantage,
                    out var opponentWasHit,
                    out var opponentWasKilled);

                if (opponentWasKilled && !OpponentsStillRemaining(CurrentCreature))
                {
                    SquareIndexesCurrentCreatureCanMoveTo.Object = null;
                    BattleDecided = true;
                }
                else
                {
                    IdentifyOptionsForCurrentPlayerControlledCreature(false);
                }

                return new CreatureAttackRanged();
            }

            return null;
        }

        public void StartBattle()
        {
            if (_scene == null)
            {
                throw new InvalidOperationException("Assign a scene to the engine before starting a battle");
            }

            Logger?.WriteLine(LogMessageCategory.Information, "  Starting Battle..");

            BattleDecided = false;
            BattleHasStarted.Object = true;
            _battleRoundCount = 0;

            var message = "    Determining initial acting order of creatures..";
            Logger?.WriteLine(LogMessageCategory.Information, message);

            var creatureTypeToDieRollMap = new Dictionary<CreatureType, int>();
            var dieRollToCreatureMap = new Dictionary<int, Dictionary<CreatureType, List<Creature>>>();

            // Determine the distance from each friendly creature to the closest enemy
            var indexesOfHostileCreatures = Creatures
                .Where(c => c.IsHostile)
                .Select(c => c.IndexOfOccupiedSquare(_scene.Columns))
                .ToArray();

            // Determine the distance from each hostile creature to the closest enemy
            var indexesOfFriendlyCreatures = Creatures
                .Where(c => !c.IsHostile)
                .Select(c => c.IndexOfOccupiedSquare(_scene.Columns))
                .ToArray();

            var graph = new GraphMatrix8Connectivity(_scene.Rows, _scene.Columns);

            graph.ComputeDistances(
                indexesOfHostileCreatures,
                _obstacleIndexes,
                double.MaxValue,
                out var distancesToHostiles,
                out _previous);

            graph.ComputeDistances(
                indexesOfFriendlyCreatures,
                _obstacleIndexes,
                double.MaxValue,
                out var distancesToFriendlies,
                out _previous);

            Creatures.ForEach(c =>
            {
                int dieRoll;
                if (creatureTypeToDieRollMap.ContainsKey(c.CreatureType))
                {
                    dieRoll = creatureTypeToDieRollMap[c.CreatureType];
                }
                else
                {
                    dieRoll = 1 + _random.Next(20) + c.CreatureType.InitiativeModifier;
                    creatureTypeToDieRollMap[c.CreatureType] = dieRoll;
                }

                if (!dieRollToCreatureMap.ContainsKey(dieRoll))
                {
                    dieRollToCreatureMap[dieRoll] = new Dictionary<CreatureType, List<Creature>>();
                }

                if (!dieRollToCreatureMap[dieRoll].ContainsKey(c.CreatureType))
                {
                    dieRollToCreatureMap[dieRoll][c.CreatureType] = new List<Creature>();
                }

                dieRollToCreatureMap[dieRoll][c.CreatureType].Add(c);
            });

            Logger?.WriteLine(LogMessageCategory.Information, "    Initial acting order:");

            var queueNumberInBattleRound = 0;
            foreach (var kvp1 in dieRollToCreatureMap.OrderByDescending(kvp => kvp.Key))
            {
                foreach (var kvp2 in kvp1.Value)
                {
                    var creaturesOfCurrentType = kvp2.Value;

                    if (creaturesOfCurrentType.Count == 1)
                    {
                        var creature = creaturesOfCurrentType.Single();

                        creature.BattleRoundQueueNumber = queueNumberInBattleRound++;

                        Logger?.WriteLine(
                            LogMessageCategory.Information,
                            $"      {queueNumberInBattleRound}: {Tag(creature)}");
                    }
                    else
                    {
                        var distances = creaturesOfCurrentType.First().IsHostile
                            ? distancesToFriendlies
                            : distancesToHostiles;

                        creaturesOfCurrentType
                            .Select(c => new { Creature = c, Distance = distances[c.IndexOfOccupiedSquare(_scene.Columns)] })
                            .OrderBy(cd => cd.Distance)
                            .Select(cd => cd.Creature)
                            .ToList()
                            .ForEach(c =>
                            {
                                c.BattleRoundQueueNumber = queueNumberInBattleRound++;

                                Logger?.WriteLine(
                                    LogMessageCategory.Information,
                                    $"      {queueNumberInBattleRound}: {Tag(c)}");
                            });
                    }
                }
            }

            StartBattleRound();
        }

        public bool CurrentPlayerControlledCreatureHasAnyOptionsLeft()
        {
            return
                SquareIndexesCurrentCreatureCanMoveTo.Object != null &&
                SquareIndexesCurrentCreatureCanMoveTo.Object.Count > 0 ||
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object != null &&
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object.Count > 0 ||
                SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object != null &&
                SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object.Count > 0;
        }

        public bool CanStartBattle()
        {
            return
                Scene != null &&
                !BattleHasStarted.Object &&
                !BattleHasEnded.Object;
        }

        public void InitializeCreatures()
        {
            BattleHasStarted.Object = false;
            BattleHasEnded.Object = false;

            _actingOrder.Clear();
            Creatures = new List<Creature>(_scene?.Creatures);

            InitializeCreatureIdMap();

            CurrentCreature = null;

            SquareIndexForCurrentCreature.Object = null;
            SquareIndexesCurrentCreatureCanMoveTo.Object = null;
            SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = null;
            SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object = null;
        }

        public void StartBattleRound()
        {
            Logger?.WriteLine(LogMessageCategory.Information, $"    Starting battle round {++_battleRoundCount}");

            // Todo: Perform updates for each the round (such as trolls regenerating, poison draining life, etc)

            BattleroundCompleted = false;

            EstablishCreatureActingOrder();
            SwitchToNextCreature();
        }

        public void SwitchToNextCreature()
        {
            // Discard creatures that have been killed
            while (_actingOrder.Count > 0 && _actingOrder.Peek().HitPoints <= 0)
            {
                _actingOrder.Dequeue();
            }

            if (_actingOrder.Count == 0)
            {
                CurrentCreature = null;
                BattleroundCompleted = true;
                return;
            }

            CurrentCreature = _actingOrder.Dequeue();
            SquareIndexForCurrentCreature.Object = CurrentCreature.IndexOfOccupiedSquare(_scene.Columns);

            var message = $"      Turn goes to {Tag(CurrentCreature)}";
            Logger?.WriteLine(LogMessageCategory.Information, message);

            _moveDistanceRemaningForCurrentCreature = CurrentCreature.CreatureType.Movement;

            CurrentCreature.Attacks = new Queue<Attack>(CurrentCreature.CreatureType.Attacks);

            if (!CurrentCreature.IsAutomatic)
            {
                IdentifyOptionsForCurrentPlayerControlledCreature(false);
            }

            _currentCreatureJustMoved = false;
        }

        public string Tag(Creature creature)
        {
            return $"{creature.CreatureType.Name}{_creatureIdMap[creature]}";
        }

        private void EstablishCreatureActingOrder()
        {
            Creatures
                .OrderBy(c => c.BattleRoundQueueNumber)
                .ToList()
                .ForEach(c => _actingOrder.Enqueue(c));
        }

        private async Task<MoveCreatureResult> DetermineDestinationOfCurrentCreatureWithMeleeAttack()
        {
            // If the creature is next to an opponent, it doesn't move
            // Otherwise, it identifies the set of squares that fullfill the following conditions:
            // * The creature can reach them
            // * The walking distance from the square to the closest opponent is as small as possible
            // The optimal destination is the square from this set that has the shortest walking distance
            // from the current position of the creature. If multiple squares in the set share the same
            // shortest distance, it just chooses one of them

            return await Task.Run(() =>
            {
                var currentSquareIndex = CurrentCreature.IndexOfOccupiedSquare(_scene.Columns);

                var opponents = Creatures
                    .Where(c => c.IsHostile != CurrentCreature.IsHostile)
                    .ToList();

                var closestOpponent = ClosestOpponent(
                    currentSquareIndex,
                    opponents,
                    out var distanceToClosestOpponent);

                if (distanceToClosestOpponent < 1.5)
                {
                    return new MoveCreatureResult
                    {
                        IndexOfDestinationSquare = null,
                        WalkingDistanceToDestinationSquare = null,
                        FinalClosestOpponent = closestOpponent,
                        FinalDistanceToClosestOpponent = distanceToClosestOpponent
                    };
                }

                var opponentIndexes = opponents
                    .Select(c => c.IndexOfOccupiedSquare(_scene.Columns))
                    .ToList();

                var allyIndexes = Creatures
                    .Where(c => c.IsHostile == CurrentCreature.IsHostile && !c.Equals(CurrentCreature))
                    .Select(c => c.IndexOfOccupiedSquare(_scene.Columns))
                    .ToList();

                var forbiddenIndexes = new HashSet<int>(_obstacleIndexes.Concat(allyIndexes).Concat(opponentIndexes));

                var graph = new GraphMatrix8Connectivity(_scene.Rows, _scene.Columns);

                // Determine where the current creature can go
                graph.ComputeDistances(
                    new[] { currentSquareIndex },
                    forbiddenIndexes,
                    _moveDistanceRemaningForCurrentCreature,
                    out var walkingDistancesForCurrentCreature,
                    out _previous);

                // Determine the walking distances to the opponents
                graph.ComputeDistances(
                    opponentIndexes,
                    _obstacleIndexes,
                    double.MaxValue,
                    out var walkingDistancesToClosestOpponent);

                // Identify the reachable square(s) that is closest to an opponent
                // (Multiple squares may be equally far from the closest opponent)
                var indexesOfOptimalReachablePositions =
                    walkingDistancesForCurrentCreature.IdentifyIndexesLowerThan(999999)
                        .IdentifyIndexesOfMinimumValue(walkingDistancesToClosestOpponent)
                        .ToList();

                // Select the square that has the shortest walking distance from the square currently
                // occupied by the current creature
                var indexOfDestinationSquare = indexesOfOptimalReachablePositions
                    .IdentifyIndexesOfMinimumValue(walkingDistancesForCurrentCreature)
                    .First();

                // Also determine the walking distance, since we need to subtract it from the
                // remaining walking distance of the current creature for the current turn
                var walkingDistance = walkingDistancesForCurrentCreature[indexOfDestinationSquare];

                closestOpponent = ClosestOpponent(
                    indexOfDestinationSquare,
                    opponents,
                    out var finalDistanceToClosestOpponent);

                var result = new MoveCreatureResult
                {
                    IndexOfDestinationSquare = indexOfDestinationSquare,
                    WalkingDistanceToDestinationSquare = walkingDistance,
                    FinalClosestOpponent = closestOpponent,
                    FinalDistanceToClosestOpponent = finalDistanceToClosestOpponent
                };

                // If the creature is unable to move to a better position (such as when it is boxed in)
                // then IndexOfDestinationSquare should be null

                if (indexOfDestinationSquare == currentSquareIndex)
                {
                    var message = $"        {Tag(CurrentCreature)} passes";
                    Logger?.WriteLine(LogMessageCategory.Information, message);

                    result.IndexOfDestinationSquare = null;
                }

                return result;
            });
        }

        private async Task<MoveCreatureResult> DetermineDestinationOfCurrentCreatureWithRangedAttack(
            double range)
        {
            return await Task.Run(() =>
            {
                var currentSquareIndex = CurrentCreature.IndexOfOccupiedSquare(_scene.Columns);

                var opponents = Creatures
                    .Where(c => c.IsHostile != CurrentCreature.IsHostile)
                    .ToList();

                var opponentIndexes = opponents
                    .Select(c => c.IndexOfOccupiedSquare(_scene.Columns))
                    .ToList();

                var allyIndexes = Creatures
                    .Where(c => c.IsHostile == CurrentCreature.IsHostile && !c.Equals(CurrentCreature))
                    .Select(c => c.IndexOfOccupiedSquare(_scene.Columns))
                    .ToList();

                var forbiddenIndexes = new HashSet<int>(_obstacleIndexes.Concat(allyIndexes).Concat(opponentIndexes));

                var raster1 = IdentifySquaresFromWhichAnOpponentCanBeAttackedWithARangedWeapon(range);
                //raster1.WriteToFile(@"C:\Temp\SquaresFromWhichAnOpponentCanBeAttacked.txt");

                var raster2 = opponentIndexes.ConvertToRaster(_scene.Columns, _scene.Rows);
                raster2.Dilate(1.5);
                raster2.Invert();
                //raster2.WriteToFile(@"C:\Temp\raster2.txt");

                var graph = new GraphMatrix8Connectivity(_scene.Rows, _scene.Columns);

                // Determine where the current creature can go
                graph.ComputeDistances(
                    new[] { currentSquareIndex },
                    forbiddenIndexes,
                    _moveDistanceRemaningForCurrentCreature,
                    out var walkingDistancesForCurrentCreature,
                    out _previous);

                var raster3 = walkingDistancesForCurrentCreature
                    .ConvertTo2DArray(_scene.Columns, _scene.Rows)
                    .Threshold(_moveDistanceRemaningForCurrentCreature);
                //raster3.WriteToFile(@"C:\Temp\Reachable.txt");

                var reachableSquares = raster3.ConvertToIndexes().ToList();
                var squaresFacilitatingAttack = raster1.ConvertToIndexes().Except(_obstacleIndexes).ToList();

                raster1.PixelWiseAnd(raster3);
                var reachableSquaresFacilitatingEnablingAttack = raster1.ConvertToIndexes().ToList();
                //raster1.WriteToFile(@"C:\Temp\FacilitatingAttack.txt");

                raster1.PixelWiseAnd(raster2);
                var reachableSquaresFacilitatingAttackWithoutDisadvantage = raster1.ConvertToIndexes().ToList();
                //raster1.WriteToFile(@"C:\Temp\FacilitatingAttackWithoutDisadvantage.txt");

                // These indexes are the ones that fulfill these criteria:
                // * The current creature can reach them
                // * The current creature can attack an opponent from them
                // * The current creature does not have disadvantage for them

                // If the current creature can reach such a square, it will do so (at first regardless if it provokes an opportunity attack)
                // Otherwise, it will try to reach a square from which it can attack an opponent with disadvantage

                // 1) Kan den nå et optimalt felt? Hvis ja, så gør den det og vælger i øvrigt det med den største afstand til fjenden, ellers:
                // 2) Kan den nå et felt, hvorfra den kan angribe (med disadvantage)? Hvis ja, så gør den det og vælger i øvrigt det, der er tættest på den
                // 3) Bevæg sig så tæt som muligt på et felt, hvorfra den kan angribe

                // Algoritmen, der undersøger mulighederne, afgør, om den flytter sig, eller om den står stille.
                // Hvis den står stille, så angriber den i næste tur

                if (reachableSquaresFacilitatingAttackWithoutDisadvantage.Any())
                {
                    var image = new int[_scene.Rows, _scene.Columns];
                    opponentIndexes.ForEach(i => image[i.ConvertToYCoordinate(_scene.Columns), i.ConvertToXCoordinate(_scene.Columns)] = 1);

                    DistanceTransform.EuclideanDistanceTransform(
                        image,
                        out var distances,
                        out var xValues,
                        out var yValues);

                    // Identify the (reachable) squares that are farthest away from an opponent,
                    // and among these select one that has the minimum walking distance from the
                    // currently occupied square of the current creature
                    var indexOfDestinationSquare = reachableSquaresFacilitatingAttackWithoutDisadvantage
                        .IdentifyIndexesOfMaximumValue(distances.Cast<double>().ToArray())
                        .IdentifyIndexesOfMinimumValue(walkingDistancesForCurrentCreature)
                        .First();

                    // Also determine the walking distance, since we need to subtract it from the
                    // remaining walking distance of the current creature for the current turn
                    var walkingDistance = walkingDistancesForCurrentCreature[indexOfDestinationSquare];

                    var closestOpponent = ClosestOpponent(
                        indexOfDestinationSquare,
                        opponents,
                        out var finalDistanceToClosestOpponent);

                    var result = new MoveCreatureResult
                    {
                        IndexOfDestinationSquare = indexOfDestinationSquare,
                        WalkingDistanceToDestinationSquare = walkingDistance,
                        FinalClosestOpponent = closestOpponent,
                        FinalDistanceToClosestOpponent = finalDistanceToClosestOpponent
                    };

                    if (indexOfDestinationSquare == currentSquareIndex)
                    {
                        result.IndexOfDestinationSquare = null;
                    }

                    return result;
                }

                if (reachableSquaresFacilitatingEnablingAttack.Any())
                {
                    // Select the (reachable) square that is closest to the current position
                    var indexOfDestinationSquare = reachableSquaresFacilitatingEnablingAttack
                        .IdentifyIndexesOfMinimumValue(walkingDistancesForCurrentCreature)
                        .First();

                    // Also determine the walking distance, since we need to subtract it from the
                    // remaining walking distance of the current creature for the current turn
                    var walkingDistance = walkingDistancesForCurrentCreature[indexOfDestinationSquare];

                    var closestOpponent = ClosestOpponent(
                        indexOfDestinationSquare,
                        opponents,
                        out var finalDistanceToClosestOpponent);

                    var result = new MoveCreatureResult
                    {
                        IndexOfDestinationSquare = indexOfDestinationSquare,
                        WalkingDistanceToDestinationSquare = walkingDistance,
                        FinalClosestOpponent = closestOpponent,
                        FinalDistanceToClosestOpponent = finalDistanceToClosestOpponent
                    };

                    if (indexOfDestinationSquare == currentSquareIndex)
                    {
                        result.IndexOfDestinationSquare = null;
                    }

                    return result;
                }
                else
                {
                    // Determine the walking distances to a square from which the current creature can attack an opponent
                    var graph2 = new GraphMatrix8Connectivity(_scene.Rows, _scene.Columns);

                    graph2.ComputeDistances(
                        squaresFacilitatingAttack,
                        _obstacleIndexes,
                        double.MaxValue,
                        out var walkingDistancesToSquaresFromWhichAnOpponentCanBeAttacked);

                    //walkingDistancesToSquaresFromWhichAnOpponentCanBeAttacked.ConvertTo2DArray(_scene.Columns, _scene.Rows).WriteToFile(@"C:\Temp\WalkingDistancesToSquaresFromWhichAnOpponentCanBeAttacked.txt");

                    var indexOfDestinationSquare = reachableSquares
                        .IdentifyIndexesOfMinimumValue(walkingDistancesToSquaresFromWhichAnOpponentCanBeAttacked)
                        .First();

                    // Also determine the walking distance, since we need to subtract it from the
                    // remaining walking distance of the current creature for the current turn
                    var walkingDistance = walkingDistancesForCurrentCreature[indexOfDestinationSquare];

                    var closestOpponent = ClosestOpponent(
                        indexOfDestinationSquare,
                        opponents,
                        out var finalDistanceToClosestOpponent);

                    var result = new MoveCreatureResult
                    {
                        IndexOfDestinationSquare = indexOfDestinationSquare,
                        WalkingDistanceToDestinationSquare = walkingDistance,
                        FinalClosestOpponent = closestOpponent,
                        FinalDistanceToClosestOpponent = finalDistanceToClosestOpponent
                    };

                    if (indexOfDestinationSquare == currentSquareIndex)
                    {
                        result.IndexOfDestinationSquare = null;
                    }

                    return result;
                }
            });
        }

        private void InitializeCreatureIdMap()
        {
            _creatureIdMap = new Dictionary<Creature, int>();

            var creatureTypeCountMap = new Dictionary<CreatureType, int>();

            Creatures?.ForEach(c =>
            {
                if (!creatureTypeCountMap.ContainsKey(c.CreatureType))
                {
                    creatureTypeCountMap[c.CreatureType] = 0;
                }

                _creatureIdMap[c] = ++creatureTypeCountMap[c.CreatureType];
            });
        }

        private void IdentifyOptionsForCurrentPlayerControlledCreature(
            bool creatureJustMoved)
        {
            // Only allow the current creature to move, if it has any attacks left
            // and if its last action was not a move
            if (CurrentCreature.Attacks.Count > 0 && !creatureJustMoved)
            {
                SquareIndexesCurrentCreatureCanMoveTo.Object = new Dictionary<int, double>(
                    IdentifyIndexesOfSquaresThatTheCurrentCreatureCanMoveTo());
            }
            else
            {
                SquareIndexesCurrentCreatureCanMoveTo.Object = new Dictionary<int, double>();
            }

            if (CurrentCreature.Attacks.Count(a => a is MeleeAttack) > 0)
            {
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = new HashSet<int>(
                    IdentifyPotentialTargetsOfMeleeAttack().Select(c => c.IndexOfOccupiedSquare(_scene.Columns)));
            }
            else
            {
                SquareIndexesCurrentCreatureCanAttackWithMeleeWeapon.Object = new HashSet<int>();
            }

            if (CurrentCreature.Attacks.Count(a => a is RangedAttack) > 0)
            {
                var range = ((RangedAttack)CurrentCreature.Attacks.First(a => a is RangedAttack)).Range;

                SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object = new HashSet<int>(
                    IdentifyPotentialTargetsOfRangedAttack(range).Select(c => c.IndexOfOccupiedSquare(_scene.Columns)));
            }
            else
            {
                SquareIndexesCurrentCreatureCanAttackWithRangedWeapon.Object = new HashSet<int>();
            }
        }

        private Dictionary<int, double> IdentifyIndexesOfSquaresThatTheCurrentCreatureCanMoveTo()
        {
            var indexOfCurrentCreature = CurrentCreature.IndexOfOccupiedSquare(_scene.Columns);

            var otherCreatureIndexes = Creatures
                .Where(c => !c.Equals(CurrentCreature))
                .Select(c => c.IndexOfOccupiedSquare(_scene.Columns));

            var graph = new GraphMatrix8Connectivity(_scene.Rows, _scene.Columns);

            var forbiddenIndexes = new HashSet<int>(_obstacleIndexes.Concat(otherCreatureIndexes));

            graph.ComputeDistances(
                new[] { indexOfCurrentCreature },
                forbiddenIndexes,
                _moveDistanceRemaningForCurrentCreature,
                out var distances,
                out _previous);

            var result = distances
                .Select((d, i) => new { Index = i, Distance = d })
                .Where(x => x.Distance < 999999)
                .ToDictionary(x => x.Index, x => x.Distance);

            result.Remove(indexOfCurrentCreature);

            return result;
        }

        private IEnumerable<Creature> IdentifyPotentialTargetsOfMeleeAttack()
        {
            if (CurrentCreature.Attacks.Count(a => a is MeleeAttack) == 0)
            {
                return new List<Creature>();
            }

            return IdentifyAdjacentOpponents(
                CurrentCreature.IndexOfOccupiedSquare(_scene.Columns));
        }

        private IEnumerable<Creature> IdentifyAdjacentOpponents(
            int squareIndex)
        {
            var x = squareIndex.ConvertToXCoordinate(_scene.Columns);
            var y = squareIndex.ConvertToYCoordinate(_scene.Columns);

            return Creatures
                .Where(c => c.IsHostile != CurrentCreature.IsHostile)
                .Select(c => new { Creature = c, Distance = System.Math.Pow(c.PositionX - x, 2) + System.Math.Pow(c.PositionY - y, 2) })
                .Where(cd => cd.Distance < 2.1)
                .Select(cd => cd.Creature);
        }

        private IEnumerable<Creature> IdentifyPotentialTargetsOfRangedAttack(
            double range)
        {
            if (CurrentCreature.Attacks.Count(a => a is RangedAttack) == 0)
            {
                return new List<Creature>();
            }

            // Identify indexes of squares that are visible to the creature
            var viewPoint = new Point2D(
                CurrentCreature.PositionX,
                CurrentCreature.PositionY);

            var triangles = VisibleRegion.IdentifyVisibleRegion(_wallGraph, viewPoint);
            var raster = new int[_scene.Rows, _scene.Columns];

            var circle = new Circle2D(
                new Point2D(CurrentCreature.PositionX, CurrentCreature.PositionY),
                range);

            circle.Rasterize(raster, 0, 1);

            triangles.ForEach(t => t.Rasterize(raster, 1, 2));

            return Creatures
                .Where(c => c.IsHostile != CurrentCreature.IsHostile &&
                            raster[c.PositionY, c.PositionX] == 2);
        }

        private int[,] IdentifySquaresFromWhichAnOpponentCanBeAttackedWithARangedWeapon(
            double range)
        {
            var opponents = Creatures
                .Where(c => c.IsHostile != CurrentCreature.IsHostile)
                .ToList();

            var rasterForAllOpponents = new int[_scene.Rows, _scene.Columns];

            opponents.ForEach(o =>
            {
                var triangles = VisibleRegion.IdentifyVisibleRegion(_wallGraph, new Point2D(o.PositionX, o.PositionY));
                var rasterForIndividualOpponent = new int[_scene.Rows, _scene.Columns];

                var circle = new Circle2D(
                    new Point2D(o.PositionX, o.PositionY), range);

                circle.Rasterize(rasterForIndividualOpponent, 0, 1);

                triangles.ForEach(t => t.Rasterize(rasterForIndividualOpponent, 1, 2));

                rasterForAllOpponents.PixelwiseMax(rasterForIndividualOpponent);
            });

            rasterForAllOpponents.Threshold(2);

            return rasterForAllOpponents;
        }

        private void MoveCurrentCreature(
            int indexOfTargetSquare)
        {
            CurrentCreature.PositionX = indexOfTargetSquare.ConvertToXCoordinate(_scene.Columns);
            CurrentCreature.PositionY = indexOfTargetSquare.ConvertToYCoordinate(_scene.Columns);

            var message =
                $"        {CurrentCreature.CreatureType.Name}{Tag(CurrentCreature)}" +
                $" moves to position (X, Y) = ({CurrentCreature.PositionX}, {CurrentCreature.PositionY})";

            Logger?.WriteLine(LogMessageCategory.Information, message);

            SquareIndexForCurrentCreature.Object = indexOfTargetSquare;
        }

        private Creature ClosestOpponent(
            int squareIndex,
            List<Creature> allOpponents,
            out double distance)
        {
            Creature closestOpponent = null;
            var shortestSqrDistToAnOpponent = double.MaxValue;

            var x = squareIndex.ConvertToXCoordinate(_scene.Columns);
            var y = squareIndex.ConvertToYCoordinate(_scene.Columns);

            allOpponents.ForEach(opponent =>
            {
                var dx = x - opponent.PositionX;
                var dy = y - opponent.PositionY;
                var sqrDist = dx * dx + dy * dy;

                if (sqrDist >= shortestSqrDistToAnOpponent)
                {
                    return;
                }

                closestOpponent = opponent;
                shortestSqrDistToAnOpponent = sqrDist;
            });

            distance = Math.Sqrt(shortestSqrDistToAnOpponent);
            return closestOpponent;
        }

        private void AttackOpponent(
            Creature attacker,
            Creature opponent,
            Attack attack,
            bool disadvantage,
            out bool opponentWasHit,
            out bool opponentWasKilled)
        {
            opponentWasHit = false;
            opponentWasKilled = false;
            var dieRoll = 1 + _random.Next(20);

            if (disadvantage)
            {
                dieRoll = System.Math.Min(dieRoll, 1 + _random.Next(20));
            }

            var message =
                "        " +
                $"{Tag(attacker)} attacks " +
                $"{Tag(opponent)} " +
                $"with a {attack.Name}";

            if (disadvantage)
            {
                message += ", having disadvantage";
            }

            if (attacker.CreatureType.Thaco - dieRoll <= opponent.CreatureType.ArmorClass)
            {
                var damage = 1 + _random.Next(attack.MaxDamage);
                opponent.HitPoints -= damage;

                opponentWasHit = true;

                message += $", causing {damage} in damage";

                if (opponent.HitPoints <= 0)
                {
                    Creatures.Remove(opponent);
                    opponentWasKilled = true;

                    message += $" => {Tag(opponent)} was killed";
                }
            }
            else
            {
                message += $" and misses";
            }

            Logger?.WriteLine(LogMessageCategory.Information, message);
        }

        private bool OpponentsStillRemaining(
            Creature creature)
        {
            return Creatures.Any(c => c.IsHostile != creature.IsHostile);
        }

        private List<Tuple<int, List<Creature>>> IdentifyEvadedOpponents(
            IEnumerable<int> path)
        {
            var temp = path
                .Select(i => IdentifyAdjacentOpponents(i))
                .ToArray();

            var result = new List<Tuple<int, List<Creature>>>();

            for (var i = 0; i < temp.Length - 1; i++)
            {
                var a = temp[i].Except(temp[i + 1]).ToList();

                if (a.Any())
                {
                    result.Add(new Tuple<int, List<Creature>>(i, a));
                }
            }

            return result;
        }

        private IBattleEvent MoveOrEvade(
            MoveCreatureResult moveCreatureResult)
        {
            _currentCreatureJustMoved = true;

            if (moveCreatureResult.WalkingDistanceToDestinationSquare.HasValue)
            {
                _moveDistanceRemaningForCurrentCreature -=
                    moveCreatureResult.WalkingDistanceToDestinationSquare.Value;
            }

            var path = _previous.DeterminePath(moveCreatureResult.IndexOfDestinationSquare.Value);
            var evadedCreatures = IdentifyEvadedOpponents(path);

            if (evadedCreatures.Any())
            {
                // The automatic creature moves in a way that provokes a number of opportunity attacks
                PopulateEvasionEventQueue(path, evadedCreatures);
                _evadingCreature = CurrentCreature;

                Logger?.WriteLine(LogMessageCategory.Information, $"        {Tag(CurrentCreature)} evades");

                return new NoEvent();
            }

            MoveCurrentCreature(moveCreatureResult.IndexOfDestinationSquare.Value);

            CurrentCreaturePath = path;

            return new CreatureMove();
        }

        private IBattleEvent ExecuteRangedAttackOrPass(double range)
        {
            // Identify creatures that can be attacked from the square occupied by the current creature
            var potentialTargetsOfRangedAttack =
                IdentifyPotentialTargetsOfRangedAttack(range).ToList();

            if (!potentialTargetsOfRangedAttack.Any()) return new CreaturePass();

            var attack = CurrentCreature.Attacks.Dequeue();
            var targetCreature = potentialTargetsOfRangedAttack.First();

            AttackOpponent(
                CurrentCreature,
                targetCreature,
                attack,
                false,
                out var opponentWasHit,
                out var opponentWasKilled);

            if (opponentWasKilled && !OpponentsStillRemaining(CurrentCreature))
            {
                BattleDecided = true;
            }

            TargetCreature = targetCreature;
            _currentCreatureJustMoved = false;

            return new CreatureAttackRanged();
        }

        private void PopulateEvasionEventQueue(
            IReadOnlyCollection<int> path,
            IReadOnlyList<Tuple<int, List<Creature>>> evadedCreatures)
        {
            for (var i = 0; i < evadedCreatures.Count; i++)
            {
                var index1 = evadedCreatures[i].Item1;
                var index2 = i < evadedCreatures.Count - 1 ? evadedCreatures[i + 1].Item1 : path.Count - 1;

                if (i == 0 && index1 > 0)
                {
                    _evasionEvents.Enqueue(new Move { Path = path.Take(index1 + 1).ToArray() });
                }

                evadedCreatures[i].Item2.ForEach(c =>
                {
                    _evasionEvents.Enqueue(new InitiativeSwitch { Creature = c });
                    _evasionEvents.Enqueue(new OpportunityAttack { Creature = c });
                });

                _evasionEvents.Enqueue(new InitiativeSwitch { Creature = CurrentCreature });
                _evasionEvents.Enqueue(new Move { Path = path.Skip(index1).Take(index2 - index1 + 1).ToArray() });
            }
        }

        private void OnCreatureKilled()
        {
            var handler = CreatureKilled;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
