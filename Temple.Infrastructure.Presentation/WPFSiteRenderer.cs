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
                if (siteComponent.ModelId == "event trigger")
                {
                    continue;
                }

                var model = siteComponent.ModelId switch
                {
                    "quad" => GenerateQuad(siteComponent),
                    "wall" => GenerateWall(siteComponent),
                    //"barrel" => GenerateBarrel(scenePart),
                    //"ball" => GenerateBall(scenePart),
                    //"human male" => GenerateHumanMale(scenePart),
                    //"human female" => GenerateHumanFemale(scenePart),
                    _ => throw new NotSupportedException($"Unknown Model ID '{siteComponent.ModelId}'.")
                };

                group.Children.Add(model);
            }

            return new WpfSiteModel(group);
        }

        private Model3D GenerateQuad(
            SiteComponent siteComponent)
        {
            if (siteComponent is not Quad quad)
            {
                throw new InvalidOperationException("Must be a quad");
            }

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

        private Model3D GenerateWall(
            SiteComponent siteComponent)
        {
            if (siteComponent is not Barrier barrier)
            {
                throw new InvalidOperationException("Must be a barrier");
            }

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
    }
}
