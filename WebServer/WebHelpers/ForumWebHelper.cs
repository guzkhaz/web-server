using System.Net;
using System.Text;
using System.Text.Json;
using WebServer.DataBase;
using WebServer.Models;
using WebServer.Validation;

namespace WebServer.WebHelpers;

public static class ForumWebHelper
{
    
    public static async Task AddPostAsync(HttpListenerContext context, CancellationToken ctx)
    {
        using var sr = new StreamReader(context.Request.InputStream);
        var userStr = await sr.ReadToEndAsync().ConfigureAwait(false);

        var post = JsonSerializer.Deserialize<Post>(userStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        var postValidator = new PostValidator();

        var validationResult = await postValidator.ValidateAsync(post, ctx);

        if (validationResult.IsValid)
        {
            post.Images = post.Images.Where(i => !string.IsNullOrWhiteSpace(i)).Distinct().ToArray();
            if (post.Images.Length != 0)
            {
                for (int i = 0; i < post.Images.Length; i++)
                {
                    var filePath = $@"C:\Users\Guzel\Documents\INF HOMEWORK\web-server\WebServer\www\photos\{post.Id}-{i}.png";
                        byte[] imageData = Convert.FromBase64String(post.Images[i].Split(",")[1]);
                        await File.WriteAllBytesAsync(filePath, imageData, ctx);
                        post.Images[i] = filePath;
                }
            }

            var dbContext = new DatabaseConnectionForum();
            try
            {
                post.Creator = CacheManager.Get<User>(context.Request.Cookies["session-id"].Value).Email;
                await dbContext.AddPostAsync(post, ctx);
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.StatusCode = 200;
            }
            catch (Exception)
            {
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.StatusCode = 500;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                    .GetBytes(JsonSerializer.Serialize("Ошибка сервера")), ctx);
            }
        }
        else
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = 406;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Данные не прошли проверку на сервере")), ctx);
        }
        
    }
    public static async Task GetPostAsync(HttpListenerContext context, CancellationToken ctx)
    {
        try
        {
            var dbContext = new DatabaseConnectionForum();
            var dbContextUser = new DatabaseConnectionUser();
            
            var posts = dbContext.GetPostsAsync(ctx).Result;

            foreach (var post in posts)
            {
                var user = await dbContextUser.GetUserAsync(post.Creator, ctx);
                post.Creator = user.Email;
                post.CreatorName = $"{user.Surname} {user.Name}";
                if (!string.IsNullOrEmpty(user.Image))
                {
                    post.CreatorImage = "data:image/png;base64," +
                                        Convert.ToBase64String(await File.ReadAllBytesAsync(user.Image, ctx));
                }
                for (var i = 0; i < post.Images.Length; i++)
                {
                    var path = post.Images[i];
                    if (!string.IsNullOrEmpty(path))
                    {
                        post.Images[i] = "data:image/png;base64," +
                                         Convert.ToBase64String(await File.ReadAllBytesAsync(path, ctx));
                    }
                }
            }
            context.Response.ContentType = "text/plain; charset=utf-8";

            var json = JsonSerializer.Serialize(posts);
            var bytes = Encoding.UTF8.GetBytes(json);
            await context.Response.OutputStream.WriteAsync(bytes, ctx);
            context.Response.StatusCode = 200;
        }
        catch (Exception)
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            //context.Response.StatusCode = 500;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Ошибка сервера")), ctx);
        }
    }
    
    public static async Task GetTagsAsync(HttpListenerContext context, CancellationToken ctx)
    {
        try
        {
            var dbContext = new DatabaseConnectionForum();
            var tags = dbContext.GetTagsAsync(ctx).Result;
          
            context.Response.ContentType = "text/plain; charset=utf-8";

            var json = JsonSerializer.Serialize(tags);
            var bytes = Encoding.UTF8.GetBytes(json);
            await context.Response.OutputStream.WriteAsync(bytes, ctx);
        }
        catch (Exception)
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = 500;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Ошибка сервера")), ctx);
        }
    }
    
    public static async Task GetPostByIdAsync(HttpListenerContext context, CancellationToken ctx)
    {
        try
        {
            var postId = (context.Request.Url?.Query).Substring(4);
            var dbContext = new DatabaseConnectionForum();
            var post = dbContext.GetPostByIdAsync(postId, ctx).Result;
            
            for (var i = 0; i < post.Images.Length; i++)
            {
                var path = post.Images[i];
                if (!string.IsNullOrEmpty(path))
                {
                    post.Images[i] = "data:image/png;base64," +
                                     Convert.ToBase64String(await File.ReadAllBytesAsync(path, ctx));
                }
            }
            
            var dbContextUser = new DatabaseConnectionUser();
            var user = await dbContextUser.GetUserAsync(post.Creator, ctx);
            post.CreatorName = $"{user.Surname} {user.Name}";
            if (!string.IsNullOrEmpty(user.Image))
            {
                post.CreatorImage = "data:image/png;base64," +
                                    Convert.ToBase64String(await File.ReadAllBytesAsync(user.Image, ctx));
            }
          
            context.Response.ContentType = "text/plain; charset=utf-8";
            var json = JsonSerializer.Serialize(post);
            var bytes = Encoding.UTF8.GetBytes(json);
            await context.Response.OutputStream.WriteAsync(bytes, ctx);
        }
        catch (Exception)
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = 500;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Ошибка сервера")), ctx);
        }
    }
    
    public static async Task AddCommentAsync(HttpListenerContext context, CancellationToken ctx)
    {
        using var sr = new StreamReader(context.Request.InputStream);
        var postId = (context.Request.Headers.Get("Post-Id"));
        var userStr = await sr.ReadToEndAsync().ConfigureAwait(false);

        var comment = JsonSerializer.Deserialize<Comment>(userStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        var commentValidator = new CommentValidator();

        var validationResult = await commentValidator.ValidateAsync(comment, ctx);

        if (validationResult.IsValid)
        {
            var dbContext = new DatabaseConnectionForum();
            try
            {
                comment.PostId = postId;
                comment.Creator = CacheManager.Get<User>(context.Request.Cookies["session-id"].Value).Email;
                
                await dbContext.AddCommentAsync(comment, ctx);
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.StatusCode = 200;
            }
            catch (Exception)
            {
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.StatusCode = 500;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                    .GetBytes(JsonSerializer.Serialize("Ошибка сервера")), ctx);
            }
        }
        else
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = 406;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Данные не прошли проверку на сервере")), ctx);
        }
    }
    
    public static async Task GetCommentsAsync(HttpListenerContext context, CancellationToken ctx)
    {
        try
        {
            var postId = (context.Request.Url?.Query).Substring(4);
            var dbContext = new DatabaseConnectionForum();
            var dbContextUser = new DatabaseConnectionUser();
            
            var comments = dbContext.GetCommentsAsync(postId, ctx).Result;
            
            foreach (var comment in comments)
            {
                var user = await dbContextUser.GetUserAsync(comment.Creator, ctx);
                comment.CreatorName = $"{user.Surname} {user.Name}";
                if (!string.IsNullOrEmpty(user.Image))
                {
                    comment.CreatorImage = "data:image/png;base64," +
                                        Convert.ToBase64String(await File.ReadAllBytesAsync(user.Image, ctx));
                }
            }
            
            context.Response.ContentType = "text/plain; charset=utf-8";

            var json = JsonSerializer.Serialize(comments);
            var bytes = Encoding.UTF8.GetBytes(json);
            await context.Response.OutputStream.WriteAsync(bytes, ctx);
            context.Response.StatusCode = 200;
        }
        catch (Exception)
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            //context.Response.StatusCode = 500;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Ошибка сервера")), ctx);
        }
    }
}