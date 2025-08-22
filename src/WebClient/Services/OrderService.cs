using System.Net;
using System.Text;
using Newtonsoft.Json;
using WebClient.Helpers;
using WebClient.Models;

namespace WebClient.Services;

public class OrderService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OrderService>  _logger;

    public OrderService(IHttpClientFactory httpClientFactory, ILogger<OrderService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<Result<CreateOrderResponse>> CreateOrderAsync(OrderRequest orderRequest)
    {
        var result = new Result<CreateOrderResponse>();
        var url = "api/v1/Orders";
        using (var client = _httpClientFactory.CreateClient("backend"))
        {
            using (var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(orderRequest), 
                       Encoding.UTF8, "application/json")))
            {
                var body = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<Result<CreateOrderResponse>>(body);
                }
                else
                {
                    result.code = (int) response.StatusCode;
                    result.message = body;
                }
                _logger.LogInformation("{tag} request: {url} - param : {param} - response: {response}", "backend", url, 
                    JsonConvert.SerializeObject(orderRequest), body);
            }
        }

        return result;
    }
    
    public async Task<Result<bool>> NotifyIpnAsync(OrderIpnRequest request)
    {
        var result = new Result<bool>();
        var url = "api/v1/Orders/ipn";
        using (var client = _httpClientFactory.CreateClient("backend"))
        {
            using (var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(request), 
                       Encoding.UTF8, "application/json")))
            {
                var body = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<Result<bool>>(body);
                }
                else
                {
                    result.code = (int) response.StatusCode;
                    result.message = body;
                }
                _logger.LogInformation("{tag} request: {url} - param : {param} - response: {response}", "backend", url, 
                    JsonConvert.SerializeObject(request), body);
            }
        }

        return result;
    }
    
    public async Task<Result<OrderPaging>> FilterAsync(FilterOrderRequest request)
    {
        var result = new Result<OrderPaging>();
        var url = $"api/v1/Orders?{request.ToQueryObject()}";
        using (var client = _httpClientFactory.CreateClient("backend"))
        {
            using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseContentRead))
            {
                var body = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<Result<OrderPaging>>(body);
                }
                else
                {
                    result.code = (int) response.StatusCode;
                    result.message = body;
                }
                _logger.LogInformation("{tag} request: {url} - param : {param} - response: {response}", "backend", url, 
                    JsonConvert.SerializeObject(request), body);
            }
        }

        return result;
    }
}