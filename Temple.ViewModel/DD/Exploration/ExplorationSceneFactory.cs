using Craft.Math;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Utils.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using LineSegment = Craft.Simulation.Boundaries.LineSegment;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Scene = Craft.Simulation.Scene;

namespace Temple.ViewModel.DD.Exploration;

public static class ExplorationSceneFactory
{
    public static List<List<Point2D>> GetWallPolyLines(
        string site)
    {
        switch (site)
        {
            case "Mine":
                return new List<List<Point2D>>
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
                };
            case "Village":
                return new List<List<Point2D>>
                {
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
                };
        }

        throw new NotImplementedException("Unknown site");
    }

    public static Scene GenerateScene(
        List<List<Point2D>> wallPolyLines,
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

        wallPolyLines.ForEach(wallPolyLine =>
        {
            wallPolyLine.AdjacentPairs().ToList().ForEach(_ =>
            {
                scene.AddBoundary(new LineSegment(
                    new Vector2D(_.Item1.X, -_.Item1.Y),
                    new Vector2D(_.Item2.X, -_.Item2.Y)));
            });
        });

        AddBattleUnlessWon(scene, new Vector2D(-1, -3), new Vector2D(-1, -2), "Dungeon 1, Room A, Goblin", battlesWon);
        AddBattleUnlessWon(scene, new Vector2D(2, -3), new Vector2D(2, -2), "Dungeon 1, Room B, Goblin", battlesWon, "West");
        AddBattleUnlessWon(scene, new Vector2D(4, -3), new Vector2D(4, -2), "Dungeon 1, Room B, Goblin", battlesWon, "East");
        AddBattleUnlessWon(scene, new Vector2D(1, -5), new Vector2D(0, -5), "Final Battle", battlesWon, "South");
        AddBattleUnlessWon(scene, new Vector2D(3, -7), new Vector2D(3, -6), "Final Battle", battlesWon, "East");

        scene.AddBoundary(new LineSegment(new Vector2D(0, 0), new Vector2D(1, 0), "Exit_To_Wilderness"));

        return scene;
    }

    public static Model3DGroup Generate3DScene(
        List<List<Point2D>> wallPolyLines)
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

        wallPolyLines.ForEach(wallPolyLine =>
        {
            wallPolyLine.AdjacentPairs().ToList().ForEach(_ =>
            {
                var wallMesh = CreateWall(
                    new Point2D(_.Item1.Y, _.Item1.X),
                    new Point2D(_.Item2.Y, _.Item2.X));

                group.Children.Add(new GeometryModel3D(wallMesh, material));
            });
        });

        group.Children.Add(new GeometryModel3D(floorMesh, material));
        group.Children.Add(new GeometryModel3D(ceilingMesh, material));

        return group;
    }

    private static void AddBattleUnlessWon(
        Scene scene,
        Craft.Math.Vector2D point1,
        Craft.Math.Vector2D point2,
        string battleId,
        IReadOnlySet<string> battlesWon,
        string? entranceId = null)
    {
        if (battlesWon.Contains(battleId)) return;

        var tag = battleId;

        if (entranceId != null)
        {
            tag = $"{tag};{entranceId}";
        }

        scene.AddBoundary(new LineSegment(point1, point2, tag));
    }

    private static MeshGeometry3D CreateWall(
        Craft.Math.Point2D p1,
        Craft.Math.Point2D p2)
    {
        return MeshBuilder.CreateQuad(
            new Point3D(p1.X, 1, p1.Y),
            new Point3D(p2.X, 1, p2.Y),
            new Point3D(p2.X, 0, p2.Y),
            new Point3D(p1.X, 0, p1.Y));
    }
}

