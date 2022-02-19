using Accelerator.GeoLocation.Models;
using System.Threading.Tasks;

namespace Accelerator.GeoLocation.Contracts;

public interface ICosmosDbRegionService : IGeoQueryable<GeoRegionModel>
{
    //Task<GeoQueryResponse<GeoRegionModel>> GetRegion(string id);
    //Task<GeoQueryResponse<GeoRegionModel>> UpsertRegion(GeoRegionModel region);
}
