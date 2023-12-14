using System.Data;
using Npgsql;
using WebServer.Models;

namespace WebServer.DataBase;

public class DatabaseConnectionForum
{
    private readonly NpgsqlConnection _connection = new(ConnectionString);
    private const string ConnectionString = "Host=localhost:5432;" +
                                            "Username=postgres;" +
                                            "Password=12345;" +
                                            "Database=SemWork";
    
    public async Task AddPostAsync(Post post, CancellationToken ctx = default)
    {
        await _connection.OpenAsync(ctx);
        try
        {
            await using (var cmd = new NpgsqlCommand("add_post", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.Parameters.AddWithValue("_id", post.Id);
                cmd.Parameters.AddWithValue("_title", post.Title);
                cmd.Parameters.AddWithValue("_tags", post.Tags);
                cmd.Parameters.AddWithValue("_content", post.Content);
                cmd.Parameters.AddWithValue("_images", post.Images);
                cmd.Parameters.AddWithValue("_creator", post.Creator);
                cmd.Parameters.AddWithValue("_creation_time", post.Date);

                await cmd.ExecuteNonQueryAsync(ctx);
            }
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
    
    public async Task AddCommentAsync(Comment comment, CancellationToken ctx = default)
    {
        await _connection.OpenAsync(ctx);
        try
        {
            await using (var cmd = new NpgsqlCommand("add_comment", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.Parameters.AddWithValue("_id", comment.Id);
                cmd.Parameters.AddWithValue("_content", comment.Content);
                cmd.Parameters.AddWithValue("_creator", comment.Creator);
                cmd.Parameters.AddWithValue("_creation_time", comment.Date);
                cmd.Parameters.AddWithValue("_post_id", comment.PostId);
                await cmd.ExecuteNonQueryAsync(ctx);
            }
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
    
    public async Task<Post[]> GetPostsAsync(CancellationToken ctx = default)
    {
        var posts = new List<Post>();
        await _connection.OpenAsync(ctx);
        try
        {
            string commandText = $@"SELECT * FROM posts";
            
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(ctx))
                    while (await reader.ReadAsync(ctx))
                    {
                        var post = ReadPost(reader);
                        posts.Add(post);
                    }
            }
            
            return posts.ToArray();
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
    
    public async Task<Post?> GetPostByIdAsync(string id, CancellationToken ctx = default)
    {
        await _connection.OpenAsync(ctx);
        try
        {
            string commandText = $@"SELECT * FROM posts where post_id='{id}'";
            
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(ctx))
                    while (await reader.ReadAsync(ctx))
                    {
                        var post = ReadPost(reader);
                        return post;
                    }
            }

            return null;
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
    
    public async Task<Comment[]> GetCommentsAsync(string id, CancellationToken ctx = default)
    {
        var comments = new List<Comment>();
        await _connection.OpenAsync(ctx);
        try
        {
            string commandText = $@"SELECT * FROM comments where post_id='{id}'";
            
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(ctx))
                    while (await reader.ReadAsync(ctx))
                    {
                        var comment = ReadComment(reader);
                        comments.Add(comment);
                    }
            }

            return comments.ToArray();
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
    
    private static Comment ReadComment(NpgsqlDataReader reader)
    {
        var id = reader["comment_id"] as string;
        var content = reader["content"] as string;
        var creator = reader["creator"] as string;
        var date = reader["creation_time"] as string;
        var postId = reader["post_id"] as string;
        
        var comment = new Comment()
        {
            Id = id,
            Content = content,
            Creator = creator,
            Date = date,
            PostId = postId
        };
        return comment;
    }
    
    private static Post ReadPost(NpgsqlDataReader reader)
    {
        var id = reader["post_id"] as string;
        var title = reader["title"] as string;
        var tags = reader["tags"] as string[];
        var content = reader["content"] as string;
        var images = reader["images"] as string[];
        var creator = reader["creator"] as string;
        var date = reader["creation_time"] as string;
        
        var post = new Post()
        {
            Id = id,
            Title = title,
            Tags = tags,
            Content = content,
            Images = images,
            Creator = creator,
            Date = date
        };
        return post;
    }
    
    public async Task<Tag[]> GetTagsAsync(CancellationToken ctx = default)
    {
        var tags = new List<Tag>();
        await _connection.OpenAsync(ctx);
        try
        {
            string commandText = $@"SELECT * FROM tags";
            
            await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
            {
                await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(ctx))
                    while (await reader.ReadAsync(ctx))
                    {
                        var tag = ReadTag(reader);
                        tags.Add(tag);
                    }
            }
            
            return tags.ToArray();
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
    
    private static Tag ReadTag(NpgsqlDataReader reader)
    {
        var value = reader["value"] as string;
        var color = reader["color"] as string;
        var name = reader["name"] as string;
        
        var tag = new Tag()
        {
            Value = value,
            Color = color,
            Name = name
        };
        return tag;
    }
}