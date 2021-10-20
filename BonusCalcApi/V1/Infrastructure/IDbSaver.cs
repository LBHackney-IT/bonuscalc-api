using System.Threading.Tasks;

namespace BonusCalcApi.V1.Infrastructure
{
    public interface IDbSaver
    {
        public Task SaveChangesAsync();
    }

}
