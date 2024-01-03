using AgileMind.Web.Dto;
using Microsoft.AspNetCore.SignalR;

namespace AgileMind.Infrastructure;

public class BacklogHub : Hub
{
    public async Task SendUserStory(UserStoryDto story)
    {
        await Clients.All.SendAsync("ReceiveUserStory", story);
    }
}
