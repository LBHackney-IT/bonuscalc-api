using System.Collections.Generic;
using System.Linq;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Domain;

namespace BonusCalcApi.V1.Factories
{
    public static class ResponseFactory
    {
        //TODO: Map the fields in the domain object(s) to fields in the response object(s).
        // More information on this can be found here https://github.com/LBHackney-IT/lbh-bonuscalc-api/wiki/Factory-object-mappings
        public static OperativeResponse ToResponse(this Entity domain)
        {
            return new OperativeResponse();
        }

        public static List<OperativeResponse> ToResponse(this IEnumerable<Entity> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
