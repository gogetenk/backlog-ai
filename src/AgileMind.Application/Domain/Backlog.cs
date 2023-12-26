namespace AgileMind.Application.Domain;

public class Backlog
{
    public string Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public List<UserStory> UserStories { get; private set; }

    public Backlog(string title, string description)
    {
        Id = Guid.NewGuid().ToString();
        Title = title;
        Description = description;
        UserStories = new List<UserStory>();
    }

    public void UpdateTitle(string newTitle)
    {
        Title = newTitle;
        // Additional business logic can be added here
    }

    public void UpdateDescription(string newDescription)
    {
        Description = newDescription;
        // Additional business logic can be added here
    }

    public void AddUserStory(UserStory userStory)
    {
        UserStories.Add(userStory);
        // Additional business logic can be added here
    }

    // Other domain behaviors and methods as needed
}
