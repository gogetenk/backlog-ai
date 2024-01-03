namespace AgileMind.Infrastructure.Gpt;

internal class BacklogData
{
    public string BacklogTitle { get; set; }
    public string BacklogDescription { get; set; }
}

internal class UserStoryData
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int Complexity { get; set; }
}