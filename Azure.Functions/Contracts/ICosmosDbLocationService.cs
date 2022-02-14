using Accelerator.GeoLocation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerator.GeoLocation.Contracts;

public interface ICosmosDbLocationService
{
    public Task<LocationQueryResponse> UpsertPoint(GeoPointModel point);
    public Task<LocationQueryResponse> GetPoint(string dynamicsId);


    public record LocationQueryResponse(GeoPointModel Location, bool Success);
    public record MultipleLocationQueryResponse(List<GeoPointModel> Locations, bool Success);
}