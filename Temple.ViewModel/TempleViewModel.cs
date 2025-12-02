using GalaSoft.MvvmLight;
using Temple.Application.State;

namespace Temple.ViewModel
{
    public abstract class TempleViewModel : ViewModelBase
    {
        public virtual TempleViewModel Init(ApplicationStatePayload payload) => this;
    }
}
