using System.Net;

namespace WebServer
{
    public class Program
    {
        static HttpListener listener = new HttpListener();
        async static Task Main(string[] args)
        {
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            while (listener.IsListening)
            {
                var context = await listener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;
                var ctx = new CancellationTokenSource();
                var localPath = request.Url?.LocalPath;
                if (localPath == "/home")
                {
                    await WebHelper.DisplayIndex(context, ctx.Token);
                }
                else if (localPath == "/signup")
                {
                    await WebHelper.DisplaySignup(context, ctx.Token);
                }
                else if (localPath == "/signin")
                {
                    await WebHelper.DisplaySignin(context, ctx.Token);
                }
                else
                {
                    await WebHelper.DisplayDefault(context, ctx.Token);
                }
                
                response.OutputStream.Close();
            }
            listener.Stop();
            listener.Close();
        }
    }
}
