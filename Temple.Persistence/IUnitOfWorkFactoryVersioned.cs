using System;

namespace Temple.Persistence
{
    public interface IUnitOfWorkFactoryVersioned : IUnitOfWorkFactory
    {
        DateTime? DatabaseTime { get; set; }
    }
}
