namespace AgileMind.Web.Dto;

public class UserStoryDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public int Complexity { get; set; }
}
