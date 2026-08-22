using System.Collections;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Domain.Geometry;
using Temple.Application.Interfaces;

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
                            new Point3D(p1.X, -p1.Y, 2.5),
                            new Point3D(p2.X, -p2.Y, 2.5),
                            new Point3D(p2.X, -p2.Y, 0),
                            new Point3D(p1.X, -p1.Y, 0));

                        var model = new GeometryModel3D
                        {
                            Geometry = mesh,
                            Material = material_Walls,
                        };

                        model3DGroup.Children.Add(model);
                        break;

                    case Circle2D_NPC circle2D_npc:

                        var modelNPC = MeshBuilder.ImportModelFromFile(
                            circle2D_npc.ModelId,
                            new DiffuseMaterial(new SolidColorBrush(Colors.LightPink)),
                            new Vector3D(
                                circle2D_npc.Center.X,
                                -circle2D_npc.Center.Y,
                                0),
                            circle2D_npc.Orientation);

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
                            circle2D_cylinder.Center.X,
                            0.0,
                            circle2D_cylinder.Center.Y);

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
    }
}
