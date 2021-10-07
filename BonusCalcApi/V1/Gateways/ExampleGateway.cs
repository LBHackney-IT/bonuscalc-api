using System.Collections.Generic;
using BonusCalcApi.V1.Domain;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways
{
    //TODO: Rename to match the data source that is being accessed in the gateway eg. MosaicGateway
    public class ExampleGateway : IExampleGateway
    {
        private readonly BonusCalcContext _databaseContext;

        public ExampleGateway(BonusCalcContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Entity GetEntityById(int id)
        {
            var result = _databaseContext.DatabaseEntities.Find(id);

            return result?.ToDomain();
        }

        public List<Entity> GetAll()
        {
            return new List<Entity>();
        }
    }
}
