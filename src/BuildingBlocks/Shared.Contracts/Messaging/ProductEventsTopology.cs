namespace Shared.Contracts.Messaging;

public static class ProductEventsTopology
{
    public const string Exchange = "product.events";

    public static class RoutingKeys
    {
        public const string ProductCreated = "product.created";
    }

    public static class Queues
    {
        public const string LogProductCreated = "log.product-created";
    }
}