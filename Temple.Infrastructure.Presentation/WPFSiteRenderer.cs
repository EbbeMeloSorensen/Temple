using System.Collections;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Craft.Utils.Linq;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Domain.Geometry;
using Barrier = Temple.Domain.Entities.DD.Exploration.Barrier;

namespace Temple.Infrastructure.Presentation
{
    public class WPFSiteRenderer : ISiteRenderer
    {
        public ISiteModel Build(
            IEnumerable geometricObjects)
        {
            var model3DGroup = new Model3DGroup();
            var material_Walls = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(80, 70, 60)));

            foreach (var geometricObject in geometricObjects)
            {
                switch (geometricObject)
                {
                    case LineSegment2D_Trigger:
                        // These are not to be rendered in the 3d model
                        break;

                    case Craft.Math.LineSegment2D lineSegment2D:
                        var p1 = lineSegment2D.Point1;
                        var p2 = lineSegment2D.Point2;

                        var mesh = MeshBuilder.CreateQuad(
                            new Point3D(-p1.Y, 2, p1.X),
                            new Point3D(-p2.Y, 2, p2.X),
                            new Point3D(-p2.Y, 0, p2.X),
                            new Point3D(-p1.Y, 0, p1.X));

                        var model = new GeometryModel3D
                        {
                            Geometry = mesh,
                            Material = material_Walls,
                        };

                        model3DGroup.Children.Add(model);
                        break;

                    case Circle2D_NPC circle2D_npc:
                        var modelNPC = GenerateHumanMale(circle2D_npc);
                        model3DGroup.Children.Add(modelNPC);
                        break;

                    case Circle2D_Cylinder circle2D_cylinder:
                        var mesh2 = MeshBuilder.CreateCylinder(
                            new Point3D(0, circle2D_cylinder.Length / 2, 0),
                            circle2D_cylinder.Radius,
                            circle2D_cylinder.Length, 16);

                        var material2 = new DiffuseMaterial(new SolidColorBrush(Colors.SaddleBrown));

                        var model2 = new GeometryModel3D
                        {
                            Geometry = mesh2,
                            Material = material2,
                            BackMaterial = material2
                        };

                        // Position in this scene
                        model2.Translate(
                            -circle2D_cylinder.Center.Y,
                            0.0,
                            circle2D_cylinder.Center.X);

                        model3DGroup.Children.Add(model2);
                        break;
                }
            }

            return new WpfSiteModel(model3DGroup);
        }

        private Model3D GenerateExclamationMark(
            ExclamationMark exclamationMark)
        {
            var radius = 0.01;
            var cylinderHeight = 0.08;
            var material = new DiffuseMaterial(new SolidColorBrush(Colors.DarkSlateGray));
            var group = new Model3DGroup();

            group.Children.Add(new GeometryModel3D
            {
                Geometry = MeshBuilder.CreateSphere(new Point3D(0, radius, 0), radius, 8, 8),
                Material = material,
                BackMaterial = material
            });

            group.Children.Add(new GeometryModel3D
            {
                Geometry = MeshBuilder.CreateCylinder(new Point3D(0, 2 * radius + cylinderHeight / 2 + 0.005, 0), radius, cylinderHeight, 8),
                Material = material,
                BackMaterial = material
            });

            // Position in this scene
            group.Translate(
                exclamationMark.Position.X,
                exclamationMark.Position.Y,
                exclamationMark.Position.Z);

            return group;
        }

        private Model3D GenerateHumanMale(
            Circle2D_NPC circle2D_NPC)
        {
            string path = null;
            var basicRotationAxis = new Vector3D(1, 0, 0);
            var basicRotationAngle = -90.0;
            var basicTranslation = new Vector3D(0, 0, 0);
            //var basicScaleFactor = 0.3;
            var basicScaleFactor = 1;

            switch (circle2D_NPC.ModelId)
            {
                case "human male":
                    path = @"DD\Assets\male_corrected.stl";
                    break;
                case "human female":
                    path = @"DD\Assets\female_corrected.stl";
                    break;
            }

            return ImportMeshFromFile(
                path,
                new DiffuseMaterial(new SolidColorBrush(Colors.LightPink)),
                basicRotationAxis,
                basicRotationAngle,
                basicTranslation,
                basicScaleFactor,
                new Vector3D(
                    -circle2D_NPC.Center.Y,
                    0,
                    circle2D_NPC.Center.X),
                    circle2D_NPC.Orientation);
        }

        private GeometryModel3D ImportMeshFromFile(
            string path,
            Material material,
            Vector3D basicRotationAxis,
            double basicRotationAngle,
            Vector3D basicTranslation,
            double basicScaleFactor,
            Vector3D position,
            double orientation = 0)
        {
            var mesh = StlMeshLoader.Load(path);

            var model = new GeometryModel3D
            {
                Geometry = mesh,
                Material = material
            };

            // Basic transform to normalize the model in this coordinate system
            model.Rotate(basicRotationAxis, basicRotationAngle);
            model.Translate(basicTranslation.X, basicTranslation.Y, basicTranslation.Z);
            model.Scale(basicScaleFactor, basicScaleFactor, basicScaleFactor);

            // Position in this scene
            if (Math.Abs(orientation) > 0.00001)
            {
                model.Rotate(new Vector3D(0, 1, 0), orientation);
            }

            model.Translate(position.X, position.Y, position.Z);

            return model;
        }
    }
}
