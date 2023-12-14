namespace WebServer.Models;

public class Comment
{
    public string Id { get; set; } = (Guid.NewGuid()).ToString();
    public string Content { get; set; }
    public string Creator { get; set; }
    public string CreatorName { get; set; }
    public string CreatorImage { get; set; }
    public string Date { get; set; }
    public string PostId { get; set; }
}