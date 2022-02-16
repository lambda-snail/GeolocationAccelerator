using Xunit;
using Moq;
using Accelerator.GeoLocation.Contracts;
using Microsoft.Azure.Cosmos;
using Accelerator.GeoLocation.Models;
using Microsoft.Azure.Cosmos.Spatial;
using Accelerator.GeoLocation.Services;
using System.Threading.Tasks;
using System;
using System.Net;

namespace Azure.Functions.UnitTests;

public class LocationServiceTests
{
    [Fact]
    public async Task SuccessfulUpsert_ShouldReturnSuccessfulResult()
    {
        // Arrange
        Mock<ItemResponse<GeoPointModel>> response = GetMockResponse(HttpStatusCode.OK, new GeoPointModel("1234", 0.1, 0.1));
        Mock<Container> mockContainer = GetMockContainer(response);
        Mock<CosmosClient> mockCosmosClient = GetMockClient(mockContainer);

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
        Mock<ItemResponse<GeoPointModel>> response = GetMockResponse(HttpStatusCode.BadRequest, (GeoPointModel)null);
        Mock<Container> mockContainer = GetMockContainer(response);
        Mock<CosmosClient> mockCosmosClient = GetMockClient(mockContainer);

        CosmosDbLocationService locationService = new(mockCosmosClient.Object);

        // Act
        ICosmosDbLocationService.LocationQueryResponse upsertResponse =
            await locationService.UpsertPoint(new GeoPointModel("1234", 0.1, 0.1));

        // Assert
        Assert.False(upsertResponse.Success);
        Assert.Null(upsertResponse.Location);
    }

    private Mock<ItemResponse<GeoPointModel>> GetMockResponse(HttpStatusCode returnCode, GeoPointModel returnPoint)
    {
        Mock<ItemResponse<GeoPointModel>> response = new Mock<ItemResponse<GeoPointModel>>();
        response.Setup(response => response.StatusCode).Returns(returnCode);
        response.Setup(response => response.Resource).Returns(returnPoint);
        return response;
    }

    Mock<Container> GetMockContainer(Mock<ItemResponse<GeoPointModel>> mockResponse)
    {
        Mock<Container> mockContainer = new Mock<Container>();
        mockContainer.Setup(container => container.UpsertItemAsync(It.IsAny<GeoPointModel>(), null, null, default).Result).Returns(mockResponse.Object);
        return mockContainer;
    }

    Mock<CosmosClient> GetMockClient(Mock<Container> container)
    {
        Mock<CosmosClient> mockCosmosClient = new Mock<CosmosClient>();
        mockCosmosClient.Setup(client => client.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(container.Object);
        return mockCosmosClient;
    }
}