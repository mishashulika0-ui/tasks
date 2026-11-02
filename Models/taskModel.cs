namespace taskDz.Models;

public class TaskObject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public bool Completed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime Deadline { get; set; }
}