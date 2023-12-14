using System.Net;
using WebServer.WebHelpers;
namespace WebServer
{
    public class Program
    {
        
        static HttpListener listener = new HttpListener();
        async static Task Main(string[] args)
        {
            listener.Prefixes.Add("http://localhost:8010/");
            listener.Start();

            while (listener.IsListening)
            {
                var context = await listener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;
                var ctx = new CancellationTokenSource();
                var localPath = request.Url?.LocalPath;

                switch (localPath)
                {
                    case "/home":
                        await PageDisplay.DisplayIndexAsync(context, ctx.Token);
                        break;
                    case "/signin":
                        await PageDisplay.DisplaySigninAsync(context, ctx.Token);
                        break;
                    case "/signup-1":
                        await PageDisplay.DisplaySignup1Async(context, ctx.Token);
                        break;
                    case "/signup-2":
                        await PageDisplay.DisplaySignup2Async(context, ctx.Token);
                        break;
                    case "/signup-1-registration":
                        await UserWebHelper.AddUserAsync(context, ctx.Token);
                        break;
                    case "/signup-1-supplement":
                        await UserWebHelper.SupplementUserAsync(context, ctx.Token);
                        break;
                    case "/signin-enter":
                        await UserWebHelper.GetUserAsync(context, ctx.Token);
                        break;
                    case "/exit":
                        await UserWebHelper.Exit(context, ctx.Token);
                        break;
                    case "/get-info":
                        await UserWebHelper.GetInfoAsync(context, ctx.Token);
                        break;
                    case "/settings":
                        await PageDisplay.DisplaySettingsAsync(context, ctx.Token);
                        break;
                    case "/edit":
                        await UserWebHelper.EditUserAsync(context, ctx.Token);
                        break;
                    case "/delete-account":
                        await UserWebHelper.DeleteUserAsync(context, ctx.Token);
                        break;
                    case "/forum":
                        await PageDisplay.DisplayMainForumAsync(context, ctx.Token);
                        break;
                    case "/forum-add":
                        await PageDisplay.DisplayPostAddAsync(context, ctx.Token);
                        break;
                    case "/forum-post":
                        await PageDisplay.DisplayPostAsync(context, ctx.Token);
                        break;
                    case "/get-post-by-id":
                        await ForumWebHelper.GetPostByIdAsync(context, ctx.Token);
                        break;
                    case "/add-post":
                        await ForumWebHelper.AddPostAsync(context, ctx.Token);
                        break;
                    case "/add-comment":
                        await ForumWebHelper.AddCommentAsync(context, ctx.Token);
                        break;
                    case "/get-comments":
                        await ForumWebHelper.GetCommentsAsync(context, ctx.Token);
                        break;
                    case "/get-posts":
                        await ForumWebHelper.GetPostAsync(context, ctx.Token);
                        break;
                    case "/get-tags":
                        await ForumWebHelper.GetTagsAsync(context, ctx.Token);
                        break;
                    default:
                        await PageDisplay.DisplayDefaultAsync(context, ctx.Token);
                        break;
                }
                
                response.OutputStream.Close();
                response.Close();
            }
            listener.Stop();
            listener.Close();
        }
    }
}
