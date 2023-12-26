namespace AgileMind.Web.Dto;

public class BacklogDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<UserStoryDto> UserStories { get; set; }
}
