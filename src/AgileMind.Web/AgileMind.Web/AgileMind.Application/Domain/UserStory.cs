namespace AgileMind.Application.Domain;

public class UserStory
{
    public string Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool IsCompleted { get; private set; }

    public UserStory(string title, string description, int complexity)
    {
        Id = Guid.NewGuid().ToString();
        Title = title;
        Description = description;
        IsCompleted = false;
    }

    public void UpdateTitle(string newTitle)
    {
        Title = newTitle;
    }

    public void UpdateDescription(string newDescription)
    {
        Description = newDescription;
    }

    public void MarkAsCompleted()
    {
        IsCompleted = true;
    }
}