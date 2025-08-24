using Microsoft.AspNetCore.SignalR;

namespace PaymentGateway.Worker.Hubs;

public class PaymentHub : Hub
{
    // public async Task TransactionMessage(string message)
    // {
    //     await Clients.All.SendAsync("ReceiveNotifyMessage", message);
    // }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userId = httpContext?.Request.Query["userId"].ToString();
        if (!string.IsNullOrEmpty(userId)) await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        await base.OnConnectedAsync();
    }
}