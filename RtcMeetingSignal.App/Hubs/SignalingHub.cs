using Microsoft.AspNetCore.SignalR;
using RtcMeetingSignal.App.Models;

namespace RtcMeetingSignal.App.Hubs;

public class SignalingHub : Hub
{
    private const string ReceiveMessage = nameof(ReceiveMessage);
    private const string ReceiveOffer = nameof(ReceiveOffer);
    private const string ReceiveAnswer = nameof(ReceiveAnswer);
    
    private static readonly List<string> Connections = new();
    // This will later be refactored
    private static readonly Dictionary <string, List<string>> Rooms = new();

    public override Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        Connections.Add(connectionId);
        
        return base.OnConnectedAsync();
    }

    public async Task SendMessageAsync(string message)
    {
        foreach (var connection in Connections)
        {
            if (Context.ConnectionId != connection)
            {
                await Clients.Client(connection).SendAsync(ReceiveMessage, message);
            }
        }
    }

    // TODO Decide on offer structure
    public async Task SendOfferAsync(SessionOffer offer)
    {
        await Clients.All.SendAsync(ReceiveOffer, offer);
    }

    public async Task SendAnswerAsync(object answer, string connectionId)
    {
        await Clients.Client(connectionId).SendAsync(ReceiveAnswer);
    }
    
}