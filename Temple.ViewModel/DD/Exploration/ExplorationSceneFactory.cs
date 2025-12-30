using Craft.Math;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Utils.Linq;
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
        IReadOnlySet<string> battlesWon)
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
            handleBodyCollisions,
            deltaT,
            viewMode);

        // Denne callback returnerer en værdi, der angiver, hvad der skal ske, når en body kolliderer med en boundary
        scene.CollisionBetweenBodyAndBoundaryOccuredCallBack =
            body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

        // Denne callback returnerer en værdi, der angiver, hvad der skal ske, når to bodies kolliderer
        scene.CollisionBetweenTwoBodiesOccuredCallBack =
            (body1, body2) => OutcomeOfCollisionBetweenTwoBodies.Ignore;

        scene.CheckForCollisionBetweenBodiesCallback = (body1, body2) =>
        {
            // Vi checker for om en probe rammer en NPC
            if (body1 is Bodies.NPC || body2 is Bodies.NPC)
            {
                if (body1 is Probe || body2 is Probe)
                {
                    return true;
                }
            }

            // Ellers foretages ikke noget check
            return false;
        };

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

        var nextBodyId = 2;
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

                bodyDisposalMap[propagatedState.Index + 100] = nextBodyId;

                propagatedState.AddBodyState(new BodyState(
                    new Probe(nextBodyId++, 0.05), currentStateOfPlayer!.Position)
                {
                    NaturalVelocity = 3.0 * lookDirection
                });
            }

            var response = new PostPropagationResponse();

            // Determine if we triggered a dialog with an npc
            if (bodyCollisionReports.Any())
            {
                var bcr = bodyCollisionReports.First();
                var tag = bcr.Body1 is Bodies.NPC
                    ? bcr.Body1.Tag
                    : bcr.Body2.Tag;

                response.Outcome = $"NPC_{tag}";
                response.IndexOfLastState = propagatedState.Index + 10;
                return response;
            }

            // Determine if we triggered an event such as leaving the site or starting a scripted battle
            if (!boundaryCollisionReports.Any()) return response;

            var boundary = boundaryCollisionReports.First().Boundary;

            if (string.IsNullOrEmpty(boundary.Tag)) return response;

            response.Outcome = boundary.Tag;
            response.IndexOfLastState = propagatedState.Index;

            return response;
        };

        siteData.SiteComponents.ToList().ForEach(siteComponent =>
        {
            switch (siteComponent)
            {
                case Barrier barrier:
                {
                    barrier.BoundaryPoints.AdjacentPairs().ToList().ForEach(_ =>
                    {
                        scene.AddBoundary(new LineSegment(
                            _.Item1,
                            _.Item2));
                    });
                    break;
                }
                case Barrel barrel:
                {
                    AddCircularBoundary(
                        scene, 
                        new Point2D(
                            barrel.Position.Z,
                            barrel.Position.X),
                        0.2);

                    break;
                }
                case NPC npc:
                {
                    initialState.AddBodyState(
                        new BodyState(new Bodies.NPC(nextBodyId++, 0.16, npc.Tag), new Vector2D(npc.Position.Z, -npc.Position.X)));

                    AddCircularBoundary(
                    scene,
                    new Point2D(
                        npc.Position.Z,
                        npc.Position.X),
                    0.16);

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

