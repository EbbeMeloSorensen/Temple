using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Temple.Domain.Entities.C2IEDM.ObjectItems;
using Temple.Persistence.Repositories.C2IEDM.ObjectItems;

namespace Temple.Persistence.EFCore.AppData.Repositories.C2IEDM.ObjectItems
{
    public class ObjectItemRepository : Repository<ObjectItem>, IObjectItemRepository
    {
        public ObjectItemRepository(DbContext context) : base(context)
        {
        }

        public override Task Clear()
        {
            throw new NotImplementedException();
        }

        public override Task Update(ObjectItem entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<ObjectItem> entities)
        {
            throw new NotImplementedException();
        }
    }
}
