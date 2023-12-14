using System.Net;

namespace WebServer.WebHelpers;

public static class PageDisplay
{
    private static async Task DisplayFileAsync(string path, HttpListenerContext context, CancellationToken ctx)
    {
            context.Response.StatusCode = 200;
            context.Response.ContentType = Path.GetExtension(path) switch
            {
                ".js" => "application/javascript",
                ".css" => "text/css",
                ".html" => "text/html",
                ".svg" => "image/svg+xml",
                _ => "text/plain"
            };
            var file = await File.ReadAllBytesAsync(path, ctx);
            await context.Response.OutputStream.WriteAsync(file, ctx);
    }
    
    public static async Task DisplayDefaultAsync(HttpListenerContext context, CancellationToken ctx)
    {
        var path = context.Request.Url?.LocalPath
            .Split("/")
            .Skip(1)
            .ToArray();
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "www");
        if (path != null)
        {
            for (var i = 0; i < path.Length - 1; i++)
            {
                basePath = Path.Combine(basePath, $@"{path[i]}\");
            }
        }

        basePath = Path.Combine(basePath, path?[^1] ?? string.Empty);
        if (Path.GetExtension(basePath) == "")
        {
            await DisplayFileAsync(@"www\pages\error.html", context, ctx);
        }
        else
        {
            context.Response.StatusCode = 404;
            await DisplayFileAsync(basePath, context, ctx);
        }
    }
    public static async Task DisplayIndexAsync(HttpListenerContext context, CancellationToken ctx)
    {
        if (context.Request.Cookies["session-id"]?.Value is null)
        {
            await DisplayFileAsync(@"www\pages\homepage-1.html", context, ctx);
        }
        else
        {
            context.Response.StatusCode = 403;
            await DisplayFileAsync(@"www\pages\homepage-2.html", context, ctx);
        }
    }
    public static async Task DisplaySigninAsync(HttpListenerContext context, CancellationToken ctx)
    {
        if (context.Request.Cookies["session-id"]?.Value is null)
        {
            await DisplayFileAsync(@"www\pages\login.html", context, ctx);
        }
        else
        {
            await DisplayFileAsync(@"www\pages\error.html", context, ctx);
        }
    }
    public static async Task DisplaySignup1Async(HttpListenerContext context, CancellationToken ctx)
    {
        if (context.Request.Cookies["session-id"]?.Value is null)
        {
            await DisplayFileAsync(@"www\pages\signup-1.html", context, ctx);
        }
        else
        {
            await DisplayFileAsync(@"www\pages\error.html", context, ctx);
        }
        
    }
    
    public static async Task DisplaySignup2Async(HttpListenerContext context, CancellationToken ctx)
    {
        if (!(context.Request.Cookies["session-id"]?.Value is null))
        {
            await DisplayFileAsync(@"www\pages\signup-2.html", context, ctx);
        }
        else
        {
            await DisplayFileAsync(@"www\pages\error.html", context, ctx);
        }
    }

    public static async Task DisplaySettingsAsync(HttpListenerContext context, CancellationToken ctx)
    {
        if (!(context.Request.Cookies["session-id"]?.Value is null))
        {
            await DisplayFileAsync(@"www\pages\settings.html", context, ctx);
        }   
        else
        {
            await DisplayFileAsync(@"www\pages\error.html", context, ctx);
        }
    }
    public static async Task DisplayMainForumAsync(HttpListenerContext context, CancellationToken ctx)
    {
        if (!(context.Request.Cookies["session-id"]?.Value is null))
        {
            await DisplayFileAsync(@"www\pages\forum-main.html", context, ctx);
        }   
        else
        {
            await DisplayFileAsync(@"www\pages\error.html", context, ctx);
        }
    }
    public static async Task DisplayPostAddAsync(HttpListenerContext context, CancellationToken ctx)
    {
        if (!(context.Request.Cookies["session-id"]?.Value is null))
        {
            await DisplayFileAsync(@"www\pages\forum-add.html", context, ctx);
        }   
        else
        {
            await DisplayFileAsync(@"www\pages\error.html", context, ctx);
        }
    }
    public static async Task DisplayPostAsync(HttpListenerContext context, CancellationToken ctx)
    {
        if (!(context.Request.Cookies["session-id"]?.Value is null))
        {
            await DisplayFileAsync(@"www\pages\forum-item.html", context, ctx);
        }   
        else
        {
            await DisplayFileAsync(@"www\pages\error.html", context, ctx);
        }
    }
    
}