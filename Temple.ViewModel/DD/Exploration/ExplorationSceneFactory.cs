using Craft.Math;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Utils.Linq;
using Temple.Domain.Entities.DD.Exploration;
using Barrier = Temple.Domain.Entities.DD.Exploration.Barrier;
using LineSegment = Craft.Simulation.Boundaries.LineSegment;
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
            new BodyStateClassic(new CircularBody(1, ballRadius, 1, false), position: initialPositionOfParty)
            {
                Orientation = initialOrientationOfParty
            });

        var standardGravity = 0.0;
        var initialWorldWindowUpperLeft = new Point2D(-1.4, -1.3);
        var initialWorldWindowLowerRight = new Point2D(5, 3);
        var gravitationalConstant = 0.0;
        var coefficientOfFriction = 0.0;
        var timeFactor = 1.0;
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
            handleBodyCollisions,
            deltaT,
            viewMode);

        scene.CollisionBetweenBodyAndBoundaryOccuredCallBack =
            body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

        scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
        {
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

        scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
        {
            var currentStateOfMainBody = propagatedState.TryGetBodyState(1) as BodyStateClassic;

            if (currentStateOfMainBody == null)
            {
                throw new InvalidOperationException("Expected a bodystate here");
            }

            var response = new PostPropagationResponse();

            if (!boundaryCollisionReports.Any()) return response;

            var boundary = boundaryCollisionReports.First().Boundary;

            if (string.IsNullOrEmpty(boundary.Tag)) return response;

            response.Outcome = boundary.Tag;
            response.IndexOfLastState = propagatedState.Index;

            return response;
        };

        siteData.SiteComponents.ToList().ForEach(sceneComponent =>
        {
            switch (sceneComponent)
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
                    var nBoundarySegments = 8;
                    var barrelRadius = 0.2;

                    Enumerable.Range(0, nBoundarySegments + 1)
                        .Select(_ => _ * 2 * Math.PI / nBoundarySegments)
                        .Select(angle => new Vector2D(
                            barrel.Position.Z + barrelRadius * Math.Sin(angle),
                            -barrel.Position.X + barrelRadius * Math.Cos(angle)))
                        .AdjacentPairs()
                        .ToList()
                        .ForEach(_ =>
                        {
                            scene.AddBoundary(new LineSegment(
                                _.Item1,
                                _.Item2));
                        });

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
}

