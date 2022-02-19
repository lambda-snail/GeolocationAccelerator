using Accelerator.GeoLocation.Models;
using Microsoft.Azure.Cosmos;
using Moq;
using System.Net;

namespace Azure.Functions.UnitTests.Shared;

public static class CosmosMoqUtils<T>
{
    public static Mock<ItemResponse<T>> GetMockResponse(HttpStatusCode returnCode, T returnPoint)
    {
        Mock<ItemResponse<T>> response = new Mock<ItemResponse<T>>();
        response.Setup(response => response.StatusCode).Returns(returnCode);
        response.Setup(response => response.Resource).Returns(returnPoint);
        return response;
    }

    public static Mock<Container> GetMockContainer_Upsert(Mock<ItemResponse<T>> mockResponse)
    {
        Mock<Container> mockContainer = new Mock<Container>();
        mockContainer.Setup(container => container.UpsertItemAsync(It.IsAny<T>(), It.IsAny<PartitionKey>(), null, default).Result).Returns(mockResponse.Object);
        return mockContainer;
    }

    public static Mock<Container> GetMockContainer_Get(Mock<ItemResponse<T>> mockResponse)
    {
        Mock<Container> mockContainer = new Mock<Container>();
        mockContainer.Setup(container => container.ReadItemAsync<T>(It.IsAny<string>(), It.IsAny<PartitionKey>(), null, default).Result).Returns(mockResponse.Object);
        return mockContainer;
    }

    public static Mock<CosmosClient> GetMockClient(Mock<Container> container)
    {
        Mock<CosmosClient> mockCosmosClient = new Mock<CosmosClient>();
        mockCosmosClient.Setup(client => client.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(container.Object);
        return mockCosmosClient;
    }
}
