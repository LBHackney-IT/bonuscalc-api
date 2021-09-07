using System.Collections.Generic;
using BonusCalcApi.V1.Domain;

namespace BonusCalcApi.V1.Gateways
{
    public interface IExampleGateway
    {
        Entity GetEntityById(int id);

        List<Entity> GetAll();
    }
}
