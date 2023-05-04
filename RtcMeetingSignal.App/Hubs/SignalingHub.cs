using Microsoft.AspNetCore.SignalR;

namespace RtcMeetingSignal.App.Hubs;

public class SignalingHub : Hub
{
    private const string ReceiveMessage = nameof(ReceiveMessage);
    private const string ReceiveOffer = nameof(ReceiveOffer);
    private const string ReceiveAnswer = nameof(ReceiveAnswer);
    
    private static List<string> _connections = new();
        
    public override Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        _connections.Add(connectionId);
        
        return base.OnConnectedAsync();
    }

    public async Task SendMessageAsync(string message)
    {
        foreach (var connection in _connections)
        {
            if (Context.ConnectionId != connection)
            {
                await Clients.Client(connection).SendAsync(ReceiveMessage, message);
            }
        }
    }

    // TODO Decide on offer structure
    public async Task SendOfferAsync(object offer, string connectionId)
    {
        await Clients.Client(connectionId).SendAsync(ReceiveOffer, offer);
    }

    public async Task SendAnswerAsync(object answer, string connectionId)
    {
        await Clients.Client(connectionId).SendAsync(ReceiveAnswer);
    }
    
}