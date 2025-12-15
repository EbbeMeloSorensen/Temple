using System.Windows.Media;
using System.Windows.Media.Media3D;
using Craft.Math;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Utils.Linq;
using LineSegment = Craft.Simulation.Boundaries.LineSegment;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Scene = Craft.Simulation.Scene;

namespace Temple.ViewModel.DD.Exploration;

public static class ExplorationSceneFactory
{
    public static SiteSpecs GetSiteSpecs(
        string siteID)
    {
        return siteID switch
        {
            "Mine" => new SiteSpecs("Mine", new List<List<Point2D>>
            {
                new()
                {
                    new (1, 0),
                    new (1, 2),
                    new (2, 2),
                    new (2, 1),
                    new (4, 1),
                    new (4, 2),
                    new (5, 2),
                    new (5, 1),
                    new (7, 1),
                    new (7, 6),
                    new (8, 6),
                    new (8, 9),
                    new (5, 9),
                    new (5, 7),
                    new (3, 7),
                    new (3, 9),
                    new (-2, 9),
                    new (-2, 5),
                    new (0, 5),
                    new (0, 3),
                    new (-1, 3),
                    new (-1, 4),
                    new (-3, 4),
                    new (-3, 1),
                    new (-1, 1),
                    new (-1, 2),
                    new (0, 2),
                    new (0, 0)
                },
                new ()
                {
                    new (1, 3),
                    new (1, 5),
                    new (3, 5),
                    new (3, 6),
                    new (6, 6),
                    new (6, 5),
                    new (5, 5),
                    new (5, 3),
                    new (4, 3),
                    new (4, 4),
                    new (2, 4),
                    new (2, 3),
                    new (1, 3),
                }
            }, new List<ExplorationEventTrigger>
            {
                new LeaveSiteEventTrigger
                {
                    Point1 = new Point2D(0, 0),
                    Point2 = new Point2D(1, 0),
                    EventID = "Exit_To_Wilderness"
                },
                new ScriptedBattleEventTrigger
                {
                    Point1 = new Point2D(-1, 3),
                    Point2 = new Point2D(-1, 2),
                    EventID = "Dungeon 1, Room A, Goblin"
                },
                new ScriptedBattleEventTrigger
                {
                    Point1 = new Point2D(2, 3),
                    Point2 = new Point2D(2, 2),
                    EventID = "Dungeon 1, Room B, Goblin",
                    EntranceID = "West"
                },
                new ScriptedBattleEventTrigger
                {
                    Point1 = new Point2D(4, 3),
                    Point2 = new Point2D(4, 2),
                    EventID = "Dungeon 1, Room B, Goblin",
                    EntranceID = "East"
                },
                new ScriptedBattleEventTrigger
                {
                    Point1 = new Point2D(1, 5),
                    Point2 = new Point2D(0, 5),
                    EventID = "Final Battle",
                    EntranceID = "South"
                },
                new ScriptedBattleEventTrigger
                {
                    Point1 = new Point2D(3, 7),
                    Point2 = new Point2D(3, 6),
                    EventID = "Final Battle",
                    EntranceID = "East"
                },
            }),
            "Village" => new SiteSpecs("Village", new List<List<Point2D>>
            {
                //new ()
                //{
                //    new (11, 9),
                //    new (10, 9),
                //    new (10, 11),
                //    new (13, 11),
                //    new (13, 9),
                //    new (12, 9),
                //    new (13, 9),
                //    new (13, 11),
                //    new (10, 11),
                //    new (10, 9),
                //    new (11, 9),
                //},
                //new ()
                //{
                //    new (12, 6),
                //    new (13, 6),
                //    new (13, 4),
                //    new (10, 4),
                //    new (10, 6),
                //    new (11, 6),
                //    new (10, 6),
                //    new (10, 4),
                //    new (13, 4),
                //    new (13, 6),
                //    new (12, 6)
                //},
                new ()
                {
                    new (8, 7),
                    new (8, 4),
                    new (4, 4),
                    new (4, 11),
                    new (8, 11),
                    new (8, 8),
                    new (8, 11),
                    new (4, 11),
                    new (4, 4),
                    new (8, 4),
                    new (8, 7)
                },
                new ()
                {
                    new (15, 8),
                    new (15, 12),
                    new (3, 12),
                    new (3, 3),
                    new (15, 3),
                    new (15, 7),
                }
            }, new List<ExplorationEventTrigger>
            {
                new LeaveSiteEventTrigger
                {
                    Point1 = new Point2D(15, 8),
                    Point2 = new Point2D(15, 7),
                    EventID = "Exit_To_Wilderness"
                },
            }),
            _ => throw new InvalidOperationException("Unknown SiteID")
        };
    }

    public static Scene GenerateScene(
        SiteSpecs siteSpecs,
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

        siteSpecs.WallPolyLines.ForEach(wallPolyLine =>
        {
            wallPolyLine.AdjacentPairs().ToList().ForEach(_ =>
            {
                scene.AddBoundary(new LineSegment(
                    new Vector2D(_.Item1.X, -_.Item1.Y),
                    new Vector2D(_.Item2.X, -_.Item2.Y)));
            });
        });

        siteSpecs.ExplorationEventTriggers.ForEach(explorationEventTrigger =>
        {
            switch (explorationEventTrigger)
            {
                case LeaveSiteEventTrigger leaveSiteEventTrigger:
                    scene.AddBoundary(new LineSegment(
                        new Vector2D(leaveSiteEventTrigger.Point1.X, -leaveSiteEventTrigger.Point1.Y), 
                        new Vector2D(leaveSiteEventTrigger.Point2.X, -leaveSiteEventTrigger.Point2.Y),
                        leaveSiteEventTrigger.EventID));
                    break;
                case ScriptedBattleEventTrigger scriptedBattleEventTrigger:
                    AddBattleUnlessWon(scene,
                        scriptedBattleEventTrigger.Point1,
                        scriptedBattleEventTrigger.Point2,
                        scriptedBattleEventTrigger.EventID,
                        scriptedBattleEventTrigger.EntranceID,
                        battlesWon);
                    break;
            }
        });

        return scene;
    }

    public static Model3DGroup Generate3DScene(
        SiteSpecs siteSpecs)
    {
        var floorMesh = MeshBuilder.CreateQuad(
            new Point3D(0, 0, -10),
            new Point3D(0, 0, 10),
            new Point3D(10, 0, 10),
            new Point3D(10, 0, -10));

        var ceilingMesh = MeshBuilder.CreateQuad(
            new Point3D(10, 1, -10),
            new Point3D(10, 1, 10),
            new Point3D(0, 1, 10),
            new Point3D(0, 1, -10));

        var group = new Model3DGroup();
        var material = new DiffuseMaterial(new SolidColorBrush(Colors.Gray));

        siteSpecs.WallPolyLines.ForEach(wallPolyLine =>
        {
            wallPolyLine.AdjacentPairs().ToList().ForEach(_ =>
            {
                var wallMesh = CreateWall(
                    new Point2D(_.Item1.Y, _.Item1.X),
                    new Point2D(_.Item2.Y, _.Item2.X));

                group.Children.Add(new GeometryModel3D(wallMesh, material));
            });
        });

        //group.Children.Add(new GeometryModel3D(floorMesh, material));
        //group.Children.Add(new GeometryModel3D(ceilingMesh, material));

        //if (siteSpecs.)
        //{

        //}

        return group;
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

    private static MeshGeometry3D CreateWall(
        Point2D p1,
        Point2D p2)
    {
        return MeshBuilder.CreateQuad(
            new Point3D(p1.X, 1, p1.Y),
            new Point3D(p2.X, 1, p2.Y),
            new Point3D(p2.X, 0, p2.Y),
            new Point3D(p1.X, 0, p1.Y));
    }
}

