using Accelerator.GeoLocation.Models;
using System.Threading.Tasks;

namespace Accelerator.GeoLocation.Contracts;

public interface ICosmosDbLocationService : IGeoQueryable<GeoPointModel>
{
    // TODO: Location specific queries here
}