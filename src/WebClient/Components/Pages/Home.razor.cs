using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using WebClient.Helpers;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Components.Pages;

public partial class Home
{
    private bool connected = false;
    private OrderRequest  orderRequest = new () // fake data to test
    {
        Amount = 100000,
        UserId = "D57B3468-5A64-4E6E-BBBA-606BFE11F22F"
    };
    private bool isShowQrCode = false;
    private bool isPaymentProcessing = false;
    private CreateOrderResponse qrCode = new ();
    
    private HubConnection _hubConnection;
    
    [Inject]
    private IJSRuntime _js { get; set; }
    
    [Inject]
    private OrderService _orderService { get; set; }
    
    [Inject]
    private IConfiguration _configuration { get; set; }

    private void SelectedUserChanged(ChangeEventArgs e)
    {
        if (e.Value is not null)
        {
            orderRequest.UserId = e.Value.ToString();
        }
    }
    
    private void SelectedPaymentMethodChanged(ChangeEventArgs e)
    {
        if (e.Value is not null)
        {
            orderRequest.PaymentMethod = int.Parse(e.Value.ToString());
        }
    }

    private async Task CreateOrder()
    {
        Console.WriteLine($"Creating order: {JsonConvert.SerializeObject(orderRequest)}");
        var errorMessage = orderRequest.ValidateObject();
        if (!string.IsNullOrEmpty(errorMessage))
        {
            await _js.InvokeVoidAsync("error", errorMessage);
            return;
        }
        // create order
        var orderResponse = await _orderService.CreateOrderAsync(orderRequest);
        if (orderResponse.code == 0) // temp fix code
        {
            // show QR code
            qrCode = orderResponse.data;
            qrCode.QrCode = orderResponse.data.QrCode.ToQRCodeImageSrc(10);
            isShowQrCode = true;
            // Connect to hub to get transaction status
            // Connect to notify hub
            _hubConnection = new HubConnectionBuilder().WithUrl($"{_configuration["Hub:BaseUrl"]}/{_configuration["Hub:Path"]}?userId={orderRequest.UserId}").Build();
            _hubConnection.On<string>("ReceiveNotifyMessage", (payload) => HandleConnect(payload));
            await _hubConnection.StartAsync();
            connected = true;
            StateHasChanged(); 
            return;
        }
        await _js.InvokeVoidAsync("error", orderResponse.message);
    }

    private void CloseQR()
    {
        // Cancel pay
        isShowQrCode = false;
        qrCode = new();
        isPaymentProcessing = false;
    }

    private async Task PayNow()
    {
        var data = new OrderIpnRequest()
        {
            TransactionId = Guid.NewGuid().ToString(),
            Status = 1,
            RefId = qrCode.Id
        };
        var ipnResponse = await _orderService.NotifyIpnAsync(data);
        isPaymentProcessing = true;
    }
    
    private async Task HandleConnect(string payload)
    {
        var data = JsonConvert.DeserializeObject<OrderMessage>(payload);
        if (data?.Status == 1) // temp fix code
        {
            await _js.InvokeVoidAsync("success", $"Payment Successful Order - {data.Id}");
            CloseQR();
        }
        else
        {
            await _js.InvokeVoidAsync("error", $"Payment Failed Order - {data.Id}");
        }
        await InvokeAsync(StateHasChanged);
    }
    
    

    // public async ValueTask DisposeAsync()
    // {
    //     await _hubConnection.DisposeAsync();
    // }
}