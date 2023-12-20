namespace AgileMind.Application.UseCases.Backlogs;

public class UserStoryDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }

    // Additional properties relevant for the client-side
}
