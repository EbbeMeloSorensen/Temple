using Craft.DataStructures.Geometry;
using Craft.DataStructures.MxCifQuadTree;
using Craft.Logging;
using Craft.Math;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;
using System.Collections;

namespace Temple.ViewModel.DD.Exploration
{
    public class GeometryDataStore : IGeometryDataSource
    {
        private MxCifQuadTree<object> _mxCifQuadTree;

        public GeometryDataStore(
            BoundingBox region,
            int maxDepth = 8)
        {
            _mxCifQuadTree = new MxCifQuadTree<object>(
                region, maxDepth, new DummyLogger());
        }

        public IEnumerable Query(
            BoundingBox window)
        {
            return _mxCifQuadTree
                .GetAllIntersecting(window)
                .Select(_ => _.Item);
        }

        public void AddStaticGeometryObject(
            object geometryObject)
        {
            switch (geometryObject)
            {
                //case LineModel line:
                //    _mxCifQuadTree.Insert(new SpatialItem<object>(line.ComputeBoundingBox(), line));
                //    break;
                //case PointModel point:
                //    _mxCifQuadTree.Insert(new SpatialItem<object>(point.ComputeBoundingBox(), point));
                //    break;
                //case CircleModel circle:
                //    _mxCifQuadTree.Insert(new SpatialItem<object>(circle.ComputeBoundingBox(), circle));
                //    break;
                case LineSegment2D lineSegment:
                    _mxCifQuadTree.Insert(new SpatialItem<object>(lineSegment.ComputeBoundingBox(), lineSegment));
                    break;
                case Point2D point:
                    _mxCifQuadTree.Insert(new SpatialItem<object>(point.ComputeBoundingBox(), point));
                    break;
                case Circle2D circle:
                    _mxCifQuadTree.Insert(new SpatialItem<object>(circle.ComputeBoundingBox(), circle));
                    break;
                default:
                    throw new ArgumentException("Object is not a geometry object");
            }
        }
    }
}
