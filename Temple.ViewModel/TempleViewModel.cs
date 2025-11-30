using GalaSoft.MvvmLight;
using Temple.Application.State.NewPrinciple;

namespace Temple.ViewModel
{
    public abstract class TempleViewModel : ViewModelBase
    {
        public virtual TempleViewModel Init(ScenePayload payload) => this;
    }
}
