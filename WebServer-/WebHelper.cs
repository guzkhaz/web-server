using System.Net;
using System.Text;

namespace WebServer;

public static class WebHelper
{
    public static async Task DisplayIndex(HttpListenerContext context, CancellationToken ctx)
    {
        await DisplayFile(@"www\pages\homepage-1.html", context, ctx);
    }

    public static async Task DisplayDefault(HttpListenerContext context, CancellationToken ctx)
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
        Console.WriteLine(basePath);

        await DisplayFile(basePath, context, ctx);
    }

    private static async Task DisplayFile(string path, HttpListenerContext context, CancellationToken ctx)
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

    public static async Task DisplaySignup(HttpListenerContext context, CancellationToken ctx)
    {
        await DisplayFile(@"www\signup.html", context, ctx);
    }

    public static async Task DisplaySignin(HttpListenerContext context, CancellationToken ctx)
    {
        await DisplayFile(@"www\signin.html", context, ctx);
    }
}