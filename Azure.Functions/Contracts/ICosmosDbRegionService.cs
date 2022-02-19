using Accelerator.GeoLocation.Models;
using System.Threading.Tasks;

namespace Accelerator.GeoLocation.Contracts;

public interface ICosmosDbRegionService
{
    Task<RegionQueryResponse> GetRegion(string id);
    Task<RegionQueryResponse> UpsertRegion(GeoRegionModel region);

    public record RegionQueryResponse(GeoRegionModel Region, bool Success);
}
