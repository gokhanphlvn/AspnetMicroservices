using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetPreconfiguredOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderContext).Name);
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>
            {
                new Order() {
                    UserName = "swn",
                    FirstName = "Gökhan",
                    LastName = "Pehlivan",
                    EmailAddress = "gokhanpehlivan06@gmail.com",
                    AddressLine = "Bahcelievler",
                    Country = "Turkey",
                    TotalPrice = 350,
                    State ="a",
                    ZipCode="a",
                    CardName="a",
                    CardNumber="a",
                    Expiration="a",
                    CVV="a",
                    LastModifiedBy="swn",
                    LastModifiedDate=DateTime.Now
                }
            };
        }
    }
}
