using BonusCalcApi.V1.Domain;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Factories
{
    public static class EntityFactory
    {
        public static Entity ToDomain(this DatabaseEntity databaseEntity)
        {
            //TODO: Map the rest of the fields in the domain object.
            // More information on this can be found here https://github.com/LBHackney-IT/lbh-bonuscalc-api/wiki/Factory-object-mappings

            return new Entity
            {
                Id = databaseEntity.Id,
                CreatedAt = databaseEntity.CreatedAt
            };
        }

        public static DatabaseEntity ToDatabase(this Entity entity)
        {
            //TODO: Map the rest of the fields in the database object.

            return new DatabaseEntity
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
