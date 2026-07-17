using Craft.Math;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Props;
using Craft.Utils.Linq;
using Temple.Domain.Entities.DD.Common;
using Temple.Domain.Entities.DD.Exploration;
using Temple.ViewModel.DD.Exploration.Bodies;
using Barrier = Temple.Domain.Entities.DD.Exploration.Barrier;
using LineSegment = Craft.Simulation.Boundaries.LineSegment;
using NPC = Temple.Domain.Entities.DD.Exploration.NPC;
using Scene = Craft.Simulation.Scene;

namespace Temple.ViewModel.DD.Exploration;

public static class ExplorationSceneFactory
{
    public static Scene GenerateScene(
        SiteData siteData,
        Vector2D initialPositionOfParty,
        double initialOrientationOfParty,
        IReadOnlySet<string> battlesWon,
        IGameQueryService gameQueryService)
    {
        var ballRadius = 0.16;

        var initialState = new State();

        initialState.AddBodyState(
            new BodyStateClassic(new Player(1, ballRadius), position: initialPositionOfParty)
            {
                Orientation = initialOrientationOfParty
            });

        var standardGravity = 0.0;
        var initialWorldWindowUpperLeft = new Point2D(-1.4, -1.3);
        var initialWorldWindowLowerRight = new Point2D(5, 3);
        var gravitationalConstant = 0.0;
        var coefficientOfFriction = 0.0;
        var timeFactor = 1.0;
        var handleBoundaryCollisions = true;
        var handleBodyCollisions = true;
        var deltaT = 0.001;
        var viewMode = SceneViewMode.FocusOnFirstBody;

        var scene = new Scene(
            null,
            initialWorldWindowUpperLeft,
            initialWorldWindowLowerRight,
            initialState,
            standardGravity,
            gravitationalConstant,
            coefficientOfFriction,
            timeFactor,
            handleBoundaryCollisions,
            handleBodyCollisions,
            deltaT,
            viewMode);

        scene.CollisionBetweenBodyAndBoundaryOccuredCallBack =
            body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

        scene.CollisionBetweenTwoBodiesOccuredCallBack =
            (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.Block;

        scene.CheckForCollisionBetweenBodiesCallback = (body1, body2) =>
        {
            if (body1 is BodyDoor && body2 is BodyDoor)
            {
                return false;
            }

            return body1 is BodyDoor || body2 is BodyDoor;
        };

        var openedDoors = new HashSet<BodyDoor>();
        BodyDoor activatedDoor = null;
        var doorActivationMaxCount = 20;
        var doorActivationCounter = 0;

        var spaceKeyWasPressed = false;

        scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
        {
            spaceKeyWasPressed = keyboardEvents.SpaceDown && keyboardState.SpaceDown;

            var currentStateOfMainBody = currentState.BodyStates.First() as BodyStateClassic;
            var currentRotationalSpeed = currentStateOfMainBody.RotationalSpeed;
            var currentArtificialSpeed = currentStateOfMainBody.ArtificialVelocity.Length;

            if (doorActivationCounter > 0)
            {
                // Door is activated
                doorActivationCounter--;

                var percentageOpen = 100.0 * (doorActivationMaxCount - doorActivationCounter) / doorActivationMaxCount;

                var currentStateOfDoor = currentState.BodyStates.First(bs => bs.Body == activatedDoor) as BodyStateDoor;

                // Dette skal afhænge af, hvor spilleren er i forhold til døren
                // Det skulle du kunne regne ud med et prikprodukt
                currentStateOfDoor.SetOpeningDirection(
                    currentStateOfMainBody.Position);

                currentStateOfDoor.PercentageOpen = percentageOpen;

                // Freeze the player while the door opens
                currentStateOfMainBody.RotationalSpeed = 0;
                currentStateOfMainBody.ArtificialVelocity = new Vector2D(0, 0);

                if (doorActivationCounter == 0)
                {
                    // Final step of activation
                    openedDoors.Add(activatedDoor);
                    activatedDoor = null;
                }

                return true;
            }

            var newRotationalSpeed = 0.0;

            if (keyboardState.LeftArrowDown)
            {
                newRotationalSpeed += Math.PI;
            }

            if (keyboardState.RightArrowDown)
            {
                newRotationalSpeed -= Math.PI;
            }

            var newArtificialSpeed = 0.0;

            if (keyboardState.UpArrowDown)
            {
                newArtificialSpeed += 3.0;
            }

            if (keyboardState.DownArrowDown)
            {
                newArtificialSpeed -= 3.0;
            }

            currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;
            currentStateOfMainBody.ArtificialVelocity = new Vector2D(newArtificialSpeed, 0);

            if (Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01 &&
                Math.Abs(newArtificialSpeed - currentArtificialSpeed) < 0.01)
            {
                return false;
            }

            return true;
        };

        var nextBodyId = 1000;
        var bodyDisposalMap = new Dictionary<int, int>();

        scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
        {
            // Possibly remove probe
            if (bodyDisposalMap.ContainsKey(propagatedState.Index))
            {
                var probe = propagatedState.TryGetBodyState(bodyDisposalMap[propagatedState.Index]);
                propagatedState?.RemoveBodyState(probe);
            }

            var currentStateOfPlayer = propagatedState.TryGetBodyState(1) as BodyStateClassic;

            if (currentStateOfPlayer == null)
            {
                throw new InvalidOperationException("Expected a BodyStateClassic here");
            }

            // Determine if a probe should be launched (for initiating an npc dialog or opening a door)
            if (spaceKeyWasPressed)
            {
                spaceKeyWasPressed = false;

                var lookDirection = new Vector2D(
                    Math.Cos(currentStateOfPlayer!.Orientation),
                    -Math.Sin(currentStateOfPlayer!.Orientation));

                bodyDisposalMap[propagatedState.Index + 150] = nextBodyId;

                var probeRadius = 0.05;

                propagatedState.AddBodyState(new BodyState(
                    new Probe(nextBodyId++, probeRadius), currentStateOfPlayer!.Position)
                {
                    NaturalVelocity = 3.0 * lookDirection
                });
            }

            var response = new PostPropagationResponse();

            // Determine if we triggered a BOUNDARY COLLISION based event such as:
            //   leaving the site
            //   starting a scripted battle
            //   initiating a dialog with an npc
            if (boundaryCollisionReports.Any())
            {
                var bcrWithTag = boundaryCollisionReports.FirstOrDefault(
                    _ => _.Boundary.Tag != null);

                if (bcrWithTag != null)
                {
                    var tag = bcrWithTag.Boundary.Tag;

                    if (bcrWithTag.BodyState.Body is Probe)
                    {
                        if (bcrWithTag.Boundary is Boundaries.NPC)
                        {
                            tag = $"NPC_{tag}";
                            response.Outcome = tag;
                            response.IndexOfLastState = propagatedState.Index;
                        }
                    }
                    else
                    {
                        if (bcrWithTag.Boundary is not Boundaries.NPC)
                        {
                            response.Outcome = tag;
                            response.IndexOfLastState = propagatedState.Index;
                        }
                    }
                }
            }
            // Determine if we triggered a BODY COLLISION based event such as:
            //   Attempting to open a door
            else if (bodyCollisionReports.Any())
            {
                var bodyCollisionReport = bodyCollisionReports.First();

                if ((bodyCollisionReport.Body1 is BodyDoor && bodyCollisionReport.Body2 is Probe) ||
                    (bodyCollisionReport.Body2 is BodyDoor && bodyCollisionReport.Body1 is Probe))
                {
                    var door = bodyCollisionReport.Body2 as BodyDoor;

                    if (!openedDoors.Contains(door))
                    {
                        activatedDoor = door;
                        doorActivationCounter = doorActivationMaxCount;
                    }
                }
            }

            return response;
        };

        var npcId = 2;
        var doorId = 100;

        siteData.SiteComponents.ForEach(siteComponent =>
        {
            switch (siteComponent)
            {
                case Barrier barrier:
                {
                    barrier.BarrierPoints.AdjacentPairs().ToList().ForEach(_ =>
                    {
                        scene.AddBoundary(new LineSegment(
                            new Vector2D(_.Item1.Z, -_.Item1.X),
                            new Vector2D(_.Item2.Z, -_.Item2.X)));
                    });

                    break;
                }
                case Cylinder cylinder:
                {
                    scene.AddBoundary(new Boundaries.Cylinder(new Vector2D(
                        cylinder.Position.Z, -cylinder.Position.X), cylinder.Radius, null, cylinder.Length));

                    scene.Props.Add(new PropCircle(npcId++, cylinder.Radius * 2, new Vector2D(
                        cylinder.Position.Z, -cylinder.Position.X)));

                    break;
                }
                case Door door:
                {
                    var mass = 1.0;
                    var affectedByGravity = true;
                    var affectedByBoundaries = true;
                    var percentageOpen = 0.0;

                    var doorCenter = new Vector2D(door.Position.Z, -door.Position.X);
                    var doorWidth = 0.9;
                    var radians = door.Orientation * Math.PI / 180;

                    var doorHalfVector = new Vector2D(
                        Math.Sin(radians),
                        Math.Cos(radians)) * 0.5 * doorWidth;

                    var point1 = doorCenter - doorHalfVector;
                    var point2 = doorCenter + doorHalfVector;

                    var bodyDoor = new BodyDoor(doorId++, mass, affectedByGravity, affectedByBoundaries, null)
                    {
                        Point1 = point1,
                        Point2 = point2,
                    };

                    initialState.AddBodyState(new BodyStateDoor(bodyDoor, true, percentageOpen));

                    break;
                }
                case NPC npc:
                {
                    var tag = npc.Id;
                    var npcRadius = 0.16;

                    scene.AddBoundary(new Boundaries.NPC(new Vector2D(
                        npc.Position.Z, -npc.Position.X), npcRadius, tag, npc.ModelId, npc.Orientation));

                    scene.Props.Add(new PropCircle(npcId++, npcRadius * 2, new Vector2D(
                        npc.Position.Z, -npc.Position.X)));

                    break;
                }
                case EventTrigger_LeaveSite leaveSiteEventTrigger:
                {
                    scene.AddBoundary(new LineSegment(
                        new Vector2D(leaveSiteEventTrigger.Point1.X, -leaveSiteEventTrigger.Point1.Y),
                        new Vector2D(leaveSiteEventTrigger.Point2.X, -leaveSiteEventTrigger.Point2.Y),
                        leaveSiteEventTrigger.EventID)
                        {
                            Visible = false
                        });

                    break;
                }
                case EventTrigger_ScriptedBattle scriptedBattleEventTrigger:
                {
                    AddBattleUnlessWon(scene,
                        scriptedBattleEventTrigger.Point1,
                        scriptedBattleEventTrigger.Point2,
                        scriptedBattleEventTrigger.EventID,
                        scriptedBattleEventTrigger.EntranceID,
                        battlesWon);
                    break;
                }
            }
        });

        return scene;
    }

    private static void AddBattleUnlessWon(
        Scene scene,
        Point2D point1,
        Point2D point2,
        string battleId,
        string? entranceId,
        IReadOnlySet<string> battlesWon)
    {
        if (battlesWon.Contains(battleId)) return;

        var tag = battleId;

        if (entranceId != null)
        {
            tag = $"{tag};{entranceId}";
        }

        scene.AddBoundary(new LineSegment(
            new Vector2D(point1.X, -point1.Y),
            new Vector2D(point2.X, -point2.Y),
            tag)
            {
                Visible = false
            }); ;
    }

    // Deprecated
    private static void AddCircularBoundary(
        Scene scene,
        Point2D center,
        double radius,
        int segments = 8)
    {
        Enumerable.Range(0, segments + 1)
            .Select(_ => _ * 2 * Math.PI / segments)
            .Select(angle => new Vector2D(
                center.X + radius * Math.Sin(angle),
                -center.Y + radius * Math.Cos(angle)))
            .AdjacentPairs()
            .ToList()
            .ForEach(_ =>
            {
                scene.AddBoundary(new LineSegment(
                    _.Item1,
                    _.Item2));
            });

    }
}

