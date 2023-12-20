namespace AgileMind.Application.UseCases.Backlogs;

public class BacklogDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<UserStoryDto> UserStories { get; set; }

    // Add other properties as needed
}

