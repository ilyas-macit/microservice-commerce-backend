using System.Net.Http.Json;
using MediatR;
using Microsoft.Extensions.Configuration;
using ProductService.Domain.Events;

namespace ProductService.Application.EventHandlers;

public class ProductAddedEventHandler : INotificationHandler<ProductAddedEvent>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ProductAddedEventHandler(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task Handle(ProductAddedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var logServiceUrl = _configuration["Services:LogServiceUrl"];
            if (string.IsNullOrWhiteSpace(logServiceUrl))
            {
                Console.WriteLine("LogServiceUrl not configured.");
                return;
            }

            var client = _httpClientFactory.CreateClient();
            var payload = new
            {
                level = "INFO",
                message = $"Product added: {notification.ProductName} (Id: {notification.ProductId})",
                serviceName = "ProductService"
            };

            var response = await client.PostAsJsonAsync($"{logServiceUrl.TrimEnd('/')}/api/logs", payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to send log to LogService. Status: {(int)response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while sending log to LogService: {ex.Message}");
        }
    }
}
