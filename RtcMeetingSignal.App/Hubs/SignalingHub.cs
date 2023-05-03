using Microsoft.AspNetCore.SignalR;

namespace RtcMeetingSignal.App.Hubs;

public class SignalingHub : Hub
{
    private static List<string> _connections = new();

    public override Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        _connections.Add(connectionId);
        
        return base.OnConnectedAsync();
    }

    public async Task SendMessage(string message)
    {
        foreach (var connection in _connections)
        {
            if (Context.ConnectionId != connection)
            {
                await Clients.Client(connection).SendAsync("ReceiveMessage", message);
            }
        }
    }
}