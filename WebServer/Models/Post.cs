namespace WebServer.Models;

public class Post
{
    public string Id { get; set; } = (Guid.NewGuid()).ToString();
    public string Title { get; set; }
    public string[] Tags { get; set; }
    public string Content { get; set; }
    public string[] Images { get; set; }
    public string Creator { get; set; }
    
    public string CreatorName { get; set; }
    public string CreatorImage { get; set; }
    public string Date { get; set; }
}