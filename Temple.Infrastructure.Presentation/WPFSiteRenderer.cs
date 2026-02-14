using System.Windows.Media;
using System.Windows.Media.Media3D;
using Craft.Utils.Linq;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Exploration;
using Barrier = Temple.Domain.Entities.DD.Exploration.Barrier;

namespace Temple.Infrastructure.Presentation
{
    public class WPFSiteRenderer : ISiteRenderer
    {
        public ISiteModel Build(
            SiteData siteData)
        {
            var group = new Model3DGroup();

            foreach (var siteComponent in siteData.SiteComponents)
            {
                // Event triggers are not relevant for the 3D scene
                if (siteComponent is EventTrigger)
                {
                    continue;
                }

                var model = siteComponent switch
                {
                    Quad quad => GenerateQuad(quad),
                    Barrier barrier => GenerateBarrier(barrier),
                    Cylinder cylinder => GenerateCylinder(cylinder),
                    Sphere sphere => GenerateSphere(sphere),
                    ExclamationMark exclamationMark => GenerateExclamationMark(exclamationMark),
                    NPC npc => GenerateNPC(npc),
                    _ => throw new NotSupportedException("unsupported site component")
                };

                group.Children.Add(model);
            }

            return new WpfSiteModel(group);
        }

        private Model3D GenerateQuad(
            Quad quad)
        {
            var mesh = MeshBuilder.CreateQuad(
                new Point3D(quad.Point1.X, quad.Point1.Y, quad.Point1.Z),
                new Point3D(quad.Point2.X, quad.Point2.Y, quad.Point2.Z),
                new Point3D(quad.Point3.X, quad.Point3.Y, quad.Point3.Z),
                new Point3D(quad.Point4.X, quad.Point4.Y, quad.Point4.Z));

            var material = new MaterialGroup();
            material.Children.Add(new DiffuseMaterial(new SolidColorBrush(Colors.LightSalmon)));

            var model = new GeometryModel3D
            {
                Geometry = mesh,
                Material = material
            };

            return model;
        }

        private Model3D GenerateBarrier(
            Barrier barrier)
        {
            var material = new MaterialGroup();
            material.Children.Add(new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(80, 70, 60))));

            var group = new Model3DGroup();

            barrier.BarrierPoints.AdjacentPairs().ToList().ForEach(_ =>
            {
                var p1 = _.Item1;
                var p2 = _.Item2;

                var mesh = MeshBuilder.CreateQuad(
                    new Point3D(p1.X, 1, p1.Z),
                    new Point3D(p2.X, 1, p2.Z),
                    new Point3D(p2.X, 0, p2.Z),
                    new Point3D(p1.X, 0, p1.Z));

                var model = new GeometryModel3D
                {
                    Geometry = mesh,
                    Material = material
                };

                group.Children.Add(model);
            });

            return group;
        }

        private Model3D GenerateCylinder(
            Cylinder cylinder)
        {
            var mesh = MeshBuilder.CreateCylinder(new Point3D(0, cylinder.Length / 2, 0), cylinder.Radius, cylinder.Length, 16);

            var material = new DiffuseMaterial(new SolidColorBrush(Colors.SaddleBrown));

            var model = new GeometryModel3D
            {
                Geometry = mesh,
                Material = material,
                BackMaterial = material
            };

            // Position in this scene
            model.Translate(
                cylinder.Position.X,
                cylinder.Position.Y,
                cylinder.Position.Z);

            return model;
        }

        private Model3D GenerateSphere(
            Sphere sphere)
        {
            var mesh = MeshBuilder.CreateSphere(new Point3D(0, sphere.Radius, 0), sphere.Radius, 8, 8);

            var material = new DiffuseMaterial(new SolidColorBrush(Colors.Orange));

            var model = new GeometryModel3D
            {
                Geometry = mesh,
                Material = material,
                BackMaterial = material
            };

            // Position in this scene
            model.Translate(
                sphere.Position.X,
                sphere.Position.Y,
                sphere.Position.Z);

            return model;
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

        private Model3D GenerateNPC(
            NPC npc)
        {
            return npc.ModelId switch
            {
                "human male" => GenerateHumanMale(npc),
                "human female" => GenerateHumanFemale(npc),
                _ => throw new NotSupportedException("unsupported model id for npc")
            };
        }

        private Model3D GenerateHumanMale(
            SiteComponent siteComponent)
        {
            if (siteComponent is not SiteComponent_Rotatable rotatableScenePart)
            {
                throw new InvalidOperationException("Must be a rotatable site component");
            }

            return ImportMeshFromFile(
                @"DD\Assets\male.stl",
                new DiffuseMaterial(new SolidColorBrush(Colors.LightPink)),
                new Vector3D(1, 0, 0),
                -90,
                new Vector3D(0, 0, 0),
                0.003,
                new Vector3D(
                    rotatableScenePart.Position.X,
                    rotatableScenePart.Position.Y,
                    rotatableScenePart.Position.Z),
                    rotatableScenePart.Orientation);
        }

        private Model3D GenerateHumanFemale(
            SiteComponent siteComponent)
        {
            if (siteComponent is not SiteComponent_Rotatable rotatableScenePart)
            {
                throw new InvalidOperationException("Must be a rotatable site component");
            }

            return ImportMeshFromFile(
                @"DD\Assets\female.stl",
                new DiffuseMaterial(new SolidColorBrush(Colors.LightPink)),
                new Vector3D(1, 0, 0),
                -90,
                new Vector3D(-132.5, 0, 101),
                0.015,
                new Vector3D(
                    rotatableScenePart.Position.X,
                    rotatableScenePart.Position.Y,
                    rotatableScenePart.Position.Z),
                    rotatableScenePart.Orientation);
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
