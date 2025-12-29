using System.Windows.Media.Media3D;
using Temple.Application.Interfaces;

namespace Temple.Infrastructure.Presentation;

public class WpfSiteModel : ISiteModel
{
    public Model3D Model3D { get; }

    public WpfSiteModel(
        Model3D model3D)
    {
        Model3D = model3D;
    }
}

