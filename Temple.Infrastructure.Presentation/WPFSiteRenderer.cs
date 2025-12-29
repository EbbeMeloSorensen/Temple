
using System.Windows.Media.Media3D;
using Temple.Application.Interfaces;

namespace Temple.Infrastructure.Presentation
{
    public class WPFSiteRenderer : ISiteRenderer
    {
        public ISiteModel Build()
        {
            Model3D model = new Model3DGroup();
            return new WpfSceneModel(model);
        }
    }
}
