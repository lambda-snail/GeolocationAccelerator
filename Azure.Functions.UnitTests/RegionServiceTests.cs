using Xunit;
using Moq;
using Accelerator.GeoLocation.Contracts;
using Microsoft.Azure.Cosmos;
using Accelerator.GeoLocation.Models;
using Accelerator.GeoLocation.Services;
using System.Threading.Tasks;
using System.Net;
using Azure.Functions.UnitTests.Shared;
using System.Collections.Generic;

namespace Azure.Functions.UnitTests;

public class RegionServiceTests
{
    [Fact]
    public async void SuccessfullUpsert_ShouldReturnSuccessfullResult()
    {
        // Arrange
        string testRegionId = "1234";
        GeoRegionModel upsertRegion = 
            new GeoRegionModel(testRegionId, new List<CoordinatePair> 
                                       { 
                                            new CoordinatePair { Longitude = 0.1, Latitude = 0.1 }
                                       });
        
        Mock<ItemResponse<GeoRegionModel>> response = CosmosMoqUtils<GeoRegionModel>.GetMockResponse(HttpStatusCode.OK, upsertRegion);
        Mock<Container> mockContainer = CosmosMoqUtils<GeoRegionModel>.GetMockContainer_Upsert(response);
        Mock<CosmosClient> mockCosmosClient = CosmosMoqUtils<GeoRegionModel>.GetMockClient(mockContainer);

        CosmosDbRegionService locationService = new(mockCosmosClient.Object);

        // Act
        ICosmosDbRegionService.RegionQueryResponse upsertResponse =
            await locationService.UpsertRegion(upsertRegion);

        // Assert
        Assert.True(upsertResponse.Success);
        Assert.NotNull(upsertResponse.Region);
        mockContainer.Verify(container => container.UpsertItemAsync<GeoRegionModel>(upsertRegion, It.IsAny<PartitionKey>(), null, default), "When upserting the region, the container should attempt to upsert data into the database." );
    }

    [Fact]
    public async Task FailedUpsert_ShouldReturnUnsuccessfulResult()
    {
        GeoRegionModel upsertRegion =
            new GeoRegionModel("1234", new List<CoordinatePair>
                                       {
                                            new CoordinatePair { Longitude = 0.1, Latitude = 0.1 }
                                       });

        // Arrange
        Mock<ItemResponse<GeoRegionModel>> response = CosmosMoqUtils<GeoRegionModel>.GetMockResponse(HttpStatusCode.BadRequest, (GeoRegionModel)null);
        Mock<Container> mockContainer = CosmosMoqUtils<GeoRegionModel>.GetMockContainer_Upsert(response);
        Mock<CosmosClient> mockCosmosClient = CosmosMoqUtils<GeoRegionModel>.GetMockClient(mockContainer);

        CosmosDbRegionService regionService = new(mockCosmosClient.Object);

        // Act
        ICosmosDbRegionService.RegionQueryResponse upsertResponse =
            await regionService.UpsertRegion(upsertRegion);

        // Assert
        Assert.False(upsertResponse.Success);
        Assert.Null(upsertResponse.Region);
        mockContainer.Verify(container => container.UpsertItemAsync<GeoRegionModel>(upsertRegion, It.IsAny<PartitionKey>(), null, default), "When upserting the region, the container should attempt to upsert data into the database.");
    }

    [Fact]
    public async Task GetRegionWithExistingId_ShouldReturnSuccesfullResult()
    {
        string testId = "test";
        GeoRegionModel region =
            new GeoRegionModel(testId, new List<CoordinatePair>
                                       {
                                            new CoordinatePair { Longitude = 0.1, Latitude = 0.1 }
                                       });

        // Arrange
        Mock<ItemResponse<GeoRegionModel>> response = CosmosMoqUtils<GeoRegionModel>.GetMockResponse(HttpStatusCode.OK, region);

        Mock<Container> mockContainer = CosmosMoqUtils<GeoRegionModel>.GetMockContainer_Get(response);

        Mock<CosmosClient> mockCosmosClient = CosmosMoqUtils<GeoRegionModel>.GetMockClient(mockContainer);
        CosmosDbRegionService locationService = new(mockCosmosClient.Object);

        // Act
        ICosmosDbRegionService.RegionQueryResponse getResponse = await locationService.GetRegion(testId);

        // Assert
        Assert.True(getResponse.Success);
        Assert.Equal(testId, getResponse.Region.Id);
        mockContainer.Verify(container => container.ReadItemAsync<GeoRegionModel>(testId, It.IsAny<PartitionKey>(), null, default), "When getting the region, the container should be queried with the provided id.");
    }

    [Fact]
    public async Task GetRegionNonExistingId_ShouldReturnUnsuccesfullResult()
    {
        string testId = "test";

        // Arrange
        Mock<ItemResponse<GeoRegionModel>> response = CosmosMoqUtils<GeoRegionModel>.GetMockResponse(HttpStatusCode.BadRequest, (GeoRegionModel)null);
        Mock<Container> mockContainer = CosmosMoqUtils<GeoRegionModel>.GetMockContainer_Get(response);
        Mock<CosmosClient> mockCosmosClient = CosmosMoqUtils<GeoRegionModel>.GetMockClient(mockContainer);
        CosmosDbRegionService regionService = new(mockCosmosClient.Object);

        // Act
        ICosmosDbRegionService.RegionQueryResponse upsertResponse = await regionService.GetRegion(testId);

        // Assert
        Assert.False(upsertResponse.Success);
        Assert.Null(upsertResponse.Region);
        mockContainer.Verify(container => container.ReadItemAsync<GeoRegionModel>(testId, It.IsAny<PartitionKey>(), null, default), "When getting the region, the container should be queried with the provided id.");
    }
}
