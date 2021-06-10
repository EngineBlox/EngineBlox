using EngineBlox.Test.Utility.Configuration;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace EngineBlox.Api.Test.Unit
{
    public interface IOrders
    {
        Task GetOrderSummariesAsync();
        Task StartPickingAsync();
        Task SubmitPickAsync(PickedItems pickedItems);
        void NoAsyncSuffixTest();
    }

    public class PickedItems
    {
        public int ItemId { get; set; }
    }

    public class Orders : IOrders
    {
        public static IConfiguration BuildConfiguration(string? overrideName = null, string? overrideValue = null)
        {
            var builder = new InMemoryConfigurationBuilder()
                             .AddOrUpdateConfiguration("Api:Orders:BaseAddress", "http://www.test.co.uk")
                             .AddOrUpdateConfiguration("Api:Orders:GetOrderSummaries", "/orders")
                             .AddOrUpdateConfiguration("Api:Orders:StartPicking", "/orders/{orderId}")
                             .AddOrUpdateConfiguration("Api:Orders:SubmitPick", "/orders/{orderId}")
                             .AddOrUpdateConfiguration("Api:Orders:NoAsyncSuffixTest", "/no/async/test");

            if (overrideName != null) builder.AddOrUpdateConfiguration(overrideName, overrideValue ?? "");

            return builder.Build();
        }

        public Task GetOrderSummariesAsync() => throw new System.NotImplementedException();
        public Task StartPickingAsync() => throw new System.NotImplementedException();
        public Task SubmitPickAsync(PickedItems pickedItems) => throw new System.NotImplementedException();
        public void NoAsyncSuffixTest() => throw new System.NotImplementedException();
    }
}
