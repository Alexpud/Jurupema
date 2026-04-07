using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jurupema.Api.Application.Models;
using Jurupema.Api.Domain.Entities;
using Jurupema.Api.Domain.Enums;
using Jurupema.Api.Infrastructure.Data;
using Jurupema.Api.Integration.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace Jurupema.Api.Integration.Tests.Orders;

public class OrderEndpointsTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    public OrderEndpointsTests(ApiWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Post_orders_returns_created_with_location_and_body()
    {
        // Arrange
        Guid productAId;
        Guid productBId;
        await using (var arrangeScope = _factory.Services.CreateAsyncScope())
        {
            var db = arrangeScope.ServiceProvider.GetRequiredService<JurupemaDbContext>();
            var productA = new Product("IntA", "d", 3m);
            var productB = new Product("IntB", "d", 4m);
            db.Products.AddRange(productA, productB);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            productAId = productA.Id;
            productBId = productB.Id;
        }

        var client = _factory.CreateClient();
        var request = new CreateOrderParameter
        {
            Products =
            [
                new CreateOrderLineItem { ProductId = productAId, Quantity = 1 },
                new CreateOrderLineItem { ProductId = productBId, Quantity = 2 }
            ],
            Payment = new CreateOrderPaymentInfo
            {
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentStatus = PaymentStatus.Pending
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/orders", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        var result = await response.Content.ReadFromJsonAsync<CreateOrderResult>(
            jsonOptions,
            TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Multiple(
            () => Assert.NotEqual(Guid.Empty, result.OrderId),
            () => Assert.Equal(11m, result.TotalPrice),
            () => Assert.Equal(2, result.Lines.Count),
            () => Assert.Equal(PaymentMethod.CreditCard, result.Payment.PaymentMethod),
            () => Assert.Equal(PaymentStatus.Pending, result.Payment.PaymentStatus),
            () => Assert.Contains($"/orders/{result.OrderId}", response.Headers.Location?.ToString() ?? "", StringComparison.Ordinal));

        await using var assertScope = _factory.Services.CreateAsyncScope();
        var dbRead = assertScope.ServiceProvider.GetRequiredService<JurupemaDbContext>();
        var orderInDb = await dbRead.Orders.AsNoTracking()
            .Include(o => o.ProductOrders)
            .FirstOrDefaultAsync(o => o.Id == result.OrderId, TestContext.Current.CancellationToken);
        Assert.NotNull(orderInDb);
        Assert.Multiple(
            () => Assert.Equal(11m, orderInDb.TotalPrice),
            () => Assert.Equal(PaymentMethod.CreditCard, orderInDb.PaymentMethod),
            () => Assert.Equal(PaymentStatus.Pending, orderInDb.PaymentStatus));
    }

    [Fact]
    public async Task Post_orders_when_product_not_found_returns_400_problemdetails_with_code()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new CreateOrderParameter
        {
            Products =
            [
                new CreateOrderLineItem
                {
                    ProductId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                    Quantity = 1
                }
            ],
            Payment = new CreateOrderPaymentInfo
            {
                PaymentMethod = PaymentMethod.Cash,
                PaymentStatus = PaymentStatus.Pending
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/orders", request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);

        var hasCode = problem.Extensions.TryGetValue("code", out var codeObj);

        Assert.Multiple(
            () => Assert.Equal(400, problem.Status),
            () => Assert.Equal("Domain validation error", problem.Title),
            () => Assert.False(string.IsNullOrWhiteSpace(problem.Detail)),
            () => Assert.True(hasCode),
            () => Assert.Equal("product_not_found", codeObj?.ToString()));
    }
}
