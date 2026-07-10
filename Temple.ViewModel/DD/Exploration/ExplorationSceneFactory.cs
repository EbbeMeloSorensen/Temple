using Craft.Math;
using Craft.Simulation;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
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
        var handleBodyCollisions = false;
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

        // Denne callback returnerer en værdi, der angiver, hvad der skal ske, når en body kolliderer med en boundary
        scene.CollisionBetweenBodyAndBoundaryOccuredCallBack =
            body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

        var spaceKeyWasPressed = false;

        scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
        {
            spaceKeyWasPressed = keyboardEvents.SpaceDown && keyboardState.SpaceDown;

            var currentStateOfMainBody = currentState.BodyStates.First() as BodyStateClassic;
            var currentRotationalSpeed = currentStateOfMainBody.RotationalSpeed;
            var currentArtificialSpeed = currentStateOfMainBody.ArtificialVelocity.Length;

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

            // Determine if a probe should be launched (for initiating an npc dialog)
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

            // Determine if we triggered an event such as leaving the site or starting a scripted battle
            if (!boundaryCollisionReports.Any())
            {
                return response;
            }

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

            return response;
        };

        var npcId = 2;

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
                    scene.AddBoundary(new CircularBoundary(new Vector2D(
                        cylinder.Position.Z, -cylinder.Position.X), cylinder.Radius));

                    scene.Props.Add(new PropCircle(npcId++, cylinder.Radius * 2, new Vector2D(
                        cylinder.Position.Z, -cylinder.Position.X)));

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
                        leaveSiteEventTrigger.EventID));

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
            tag));
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

