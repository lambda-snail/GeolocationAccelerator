using Accelerator.GeoLocation.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerator.GeoLocation.Models;

public interface IGeoQueryable<T> where T : IModel
{
    public Task<GeoQueryResponse<T>> UpsertItem(T item);
    public Task<GeoQueryResponse<T>> GetItem(string id);
}

public record GeoQueryResponse<Q>(Q Item, bool Success);
public record GeoMultipleQueryResponse<Q>(List<Q> Item, bool Success);