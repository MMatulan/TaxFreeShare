using Microsoft.AspNetCore.SignalR;

namespace TaxFreeShareAPI.ChatHub;

public class ChatHub : Hub
{
    // Bli med i en gruppe (f.eks. chat mellom kjøper og selger)
    public async Task JoinChat(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    // Forlat gruppe om ønskelig
    public async Task LeaveChat(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    // Send melding til én gruppe
    public async Task SendMessageToGroup(string groupName, string senderName, string message)
    {
        await Clients.Group(groupName).SendAsync("ReceiveMessage", senderName, message);
    }
}
