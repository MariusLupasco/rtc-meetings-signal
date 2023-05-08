using Microsoft.AspNetCore.SignalR;
using RtcMeetingSignal.App.Models;

namespace RtcMeetingSignal.App.Hubs;

public class SignalingHub : Hub
{
    private const string ReceiveMessage = nameof(ReceiveMessage);
    private const string ReceiveOffer = nameof(ReceiveOffer);
    private const string ReceiveAnswer = nameof(ReceiveAnswer);
    private const string ReceiveCandidate = nameof(ReceiveCandidate);
    
    private static readonly List<string> Connections = new();
    // This will later be refactored
    private static readonly Dictionary <string, List<string>> Rooms = new();

    private readonly ILogger<SignalingHub> _logger;

    public SignalingHub(ILogger<SignalingHub> logger)
    {
        _logger = logger;
    }

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
        // await Clients.Client(connectionId).SendAsync(ReceiveOffer, offer, Context.ConnectionId);
        foreach (var connection in Connections)
        {
            if (Context.ConnectionId != connection)
            {
                await Clients.Client(connection).SendAsync(ReceiveOffer, offer);
            }
        }
    }

    public async Task SendAnswerAsync(object answer, string connectionId)
    {
        await Clients.Client(connectionId).SendAsync(ReceiveAnswer);
    }

    public async Task CreateRoom(string roomName)
    {
        _logger.LogInformation($"Room created {roomName}");

        var sourceConnectionId = Context.ConnectionId;
        
        await Groups.AddToGroupAsync(sourceConnectionId, roomName);
        await Clients.GroupExcept(roomName, new List<string>{ sourceConnectionId })
            .SendAsync("NewUserJoined", Context.ConnectionId);
    }

    public async Task SendIceCandidate(object[] candidates)
    {
        // await Clients.Client(targetConnectionId)
        //     .SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        foreach (var connection in Connections)
        {
            if (Context.ConnectionId != connection)
            {
                await Clients.Client(connection).SendAsync(ReceiveCandidate, candidates);
            }
        }
    }  
}