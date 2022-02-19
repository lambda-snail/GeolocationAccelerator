using Xunit;
using Moq;
using Accelerator.GeoLocation.Contracts;
using Microsoft.Azure.Cosmos;
using Accelerator.GeoLocation.Models;
using Accelerator.GeoLocation.Services;
using System.Threading.Tasks;
using System.Net;
using Azure.Functions.UnitTests.Shared;

namespace Azure.Functions.UnitTests;

public class LocationServiceTests
{
    [Fact]
    public async Task SuccessfulUpsert_ShouldReturnSuccessfulResult()
    {
        // Arrange
        Mock<ItemResponse<GeoPointModel>> response = CosmosMoqUtils<GeoPointModel>.GetMockResponse(HttpStatusCode.OK, new GeoPointModel("1234", 0.1, 0.1));
        Mock<Container> mockContainer = CosmosMoqUtils<GeoPointModel>.GetMockContainer_Upsert(response);
        Mock<CosmosClient> mockCosmosClient = CosmosMoqUtils<GeoPointModel>.GetMockClient(mockContainer);

        CosmosDbLocationService locationService = new(mockCosmosClient.Object);

        // Act
        ICosmosDbLocationService.LocationQueryResponse upsertResponse =
            await locationService.UpsertPoint(new GeoPointModel("1234", 0.1, 0.1));

        // Assert
        Assert.True(upsertResponse.Success);
        Assert.NotNull(upsertResponse.Location);
    }

    [Fact]
    public async Task FailedUpsert_ShouldReturnUnsuccessfulResult()
    {
        // Arrange
        Mock<ItemResponse<GeoPointModel>> response = CosmosMoqUtils<GeoPointModel>.GetMockResponse(HttpStatusCode.BadRequest, (GeoPointModel)null);
        Mock<Container> mockContainer = CosmosMoqUtils<GeoPointModel>.GetMockContainer_Upsert(response);
        Mock<CosmosClient> mockCosmosClient = CosmosMoqUtils<GeoPointModel>.GetMockClient(mockContainer);

        CosmosDbLocationService locationService = new(mockCosmosClient.Object);

        // Act
        ICosmosDbLocationService.LocationQueryResponse upsertResponse =
            await locationService.UpsertPoint(new GeoPointModel("1234", 0.1, 0.1));

        // Assert
        Assert.False(upsertResponse.Success);
        Assert.Null(upsertResponse.Location);
    }

    [Fact]
    public async Task GetPointWithExistingId_ShouldReturnSuccesfullResult()
    {
        string testId = "test";

        // Arrange
        Mock<ItemResponse<GeoPointModel>> response = CosmosMoqUtils<GeoPointModel>.GetMockResponse(HttpStatusCode.OK, new GeoPointModel(testId, 0d, 0d));
        
        Mock<Container> mockContainer = CosmosMoqUtils<GeoPointModel>.GetMockContainer_Get(response);

        Mock<CosmosClient> mockCosmosClient = CosmosMoqUtils<GeoPointModel>.GetMockClient(mockContainer);
        CosmosDbLocationService locationService = new(mockCosmosClient.Object);

        // Act
        ICosmosDbLocationService.LocationQueryResponse getResponse = await locationService.GetPoint(testId);

        // Assert
        Assert.True( getResponse.Success );
        Assert.Equal(testId, getResponse.Location.Id);
        mockContainer.Verify(container => container.ReadItemAsync<GeoPointModel>(testId, It.IsAny<PartitionKey>(), null, default), "When getting the location, the container should be queried with the provided id.");
    }

    [Fact]
    public async Task GetPointNonExistingId_ShouldReturnUnsuccesfullResult()
    {
        string testId = "test";

        // Arrange
        Mock<ItemResponse<GeoPointModel>> response = CosmosMoqUtils<GeoPointModel>.GetMockResponse(HttpStatusCode.BadRequest, (GeoPointModel)null);
        Mock<Container> mockContainer = CosmosMoqUtils<GeoPointModel>.GetMockContainer_Get(response);
        Mock<CosmosClient> mockCosmosClient = CosmosMoqUtils<GeoPointModel>.GetMockClient(mockContainer);
        CosmosDbLocationService locationService = new(mockCosmosClient.Object);

        // Act
        ICosmosDbLocationService.LocationQueryResponse upsertResponse = await locationService.GetPoint(testId);

        // Assert
        Assert.False(upsertResponse.Success);
        Assert.Null(upsertResponse.Location);
        mockContainer.Verify(container => container.ReadItemAsync<GeoPointModel>(testId, It.IsAny<PartitionKey>(), null, default), "When getting the location, the container should be queried with the provided id.");
    }
}