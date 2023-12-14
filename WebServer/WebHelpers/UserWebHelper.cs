using System.Net;
using System.Text;
using System.Text.Json;
using WebServer.DataBase;
using WebServer.Helpers;
using WebServer.Models;
using WebServer.Validation;
namespace WebServer.WebHelpers;

public static class UserWebHelper
{
    public static async Task AddUserAsync(HttpListenerContext context, CancellationToken ctx)
    {
        using var sr = new StreamReader(context.Request.InputStream);
        var userStr = await sr.ReadToEndAsync().ConfigureAwait(false);

        var user = JsonSerializer.Deserialize<User>(userStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var dbContext = new DatabaseConnectionUser();

        if (!dbContext.UserExistAsync(user, ctx).Result)
        {
            var userValidator = new UserValidatorMain();

            var validationResult = await userValidator.ValidateAsync(user, ctx);

            if (!validationResult.IsValid)
            {
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.StatusCode = 406;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                    .GetBytes(JsonSerializer.Serialize("Данные не прошли проверку на сервере")), ctx);
            }
            else
            {
                user.Password = PasswordHasher.Hash(user.Password);

                await dbContext.AddUserAsync(user, ctx);

                var sessionId = Guid.NewGuid().ToString();
                CacheManager.Set(sessionId, user);
                context.Response.Cookies.Add(new Cookie
                {
                    Name = "session-id",
                    Value = sessionId,
                    Expires = DateTime.UtcNow.AddDays(1d)
                });
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.StatusCode = 200;
            }
        }
        else
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = 406;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Пользователь с таким адресом почты уже существует")), ctx);
        }
    }

    public static async Task GetUserAsync(HttpListenerContext context, CancellationToken ctx)
    {
        using var sr = new StreamReader(context.Request.InputStream);
        var userStr = await sr.ReadToEndAsync().ConfigureAwait(false);
        var userLoginModel = JsonSerializer.Deserialize<LoginModel>(userStr, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

        var loginValidator = new LoginValidator();

        var validationResult = await loginValidator.ValidateAsync(userLoginModel, ctx);

        if (validationResult.IsValid)
        {
            var connection = new DatabaseConnectionUser();
            var user = await connection.GetUserAsync(userLoginModel!.Email, ctx);
            if (user != null)
            {
                var sessionId = Guid.NewGuid().ToString();
                if (PasswordHasher.Verify(user.Password, userLoginModel.Password))
                {
                    CacheManager.Set(sessionId, user);
                    context.Response.Cookies.Add(new Cookie
                    {
                        Name = "session-id",
                        Value = sessionId,
                        Expires = DateTime.UtcNow.AddMinutes(30d)
                    });
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    context.Response.StatusCode = 200;
                }
                else
                {
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    context.Response.StatusCode = 400;
                    await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                        .GetBytes(JsonSerializer.Serialize("Неправильно введен пароль")), ctx);
                }
            }
            else
            {
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.StatusCode = 406;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                    .GetBytes(JsonSerializer.Serialize("Пользователь с таким адресом почты не существует")), ctx);
            }
        }
        else
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = 400;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Данные не прошли проверку на сервере")), ctx);
        }
    }

    public static async Task SupplementUserAsync(HttpListenerContext context, CancellationToken ctx)
    {
        using var sr = new StreamReader(context.Request.InputStream);
        var userStr = await sr.ReadToEndAsync().ConfigureAwait(false);

        var userInfo = JsonSerializer.Deserialize<User>(userStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        var connection = new DatabaseConnectionUser();
        var sessionId = context.Request.Cookies["session-id"]?.Value;
        var user = CacheManager.Get<User>(sessionId);
        try
        {
            if (!string.IsNullOrEmpty(userInfo.Image))
            {
                var filePath =
                    $@"C:\Users\Guzel\Documents\INF HOMEWORK\web-server\WebServer\www\files\{user.Email.Split(".")[0]}.png";
                byte[] imageData = Convert.FromBase64String(userInfo.Image.Split(",")[1]);
                await File.WriteAllBytesAsync(filePath, imageData, ctx);
                user.Image = filePath;
            }

            var locationValidator = new LocationValidator();
            if ((await locationValidator.ValidateAsync(userInfo, ctx)).IsValid)
            {
                user.Location = userInfo.Location;
            }
            else
            {
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.StatusCode = 400;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                    .GetBytes(JsonSerializer.Serialize("Данные не прошли проверку на сервере")), ctx);
            }

            await connection.SupplementUserAsync(user, ctx);
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = 200;
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = 500;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Произошла ошибка на сервере")), ctx);
        }
    }


    public static async Task Exit(HttpListenerContext context, CancellationToken ctx)
    {
        try
        {
            var sessionId = context.Request.Cookies["session-id"]?.Value;
            CacheManager.Remove(sessionId);
            context.Response.Cookies.Add(new Cookie
            {
                Name = "session-id",
                Value = sessionId,
                Expires = DateTime.UtcNow.AddDays(-1d)
            });
            context.Response.Redirect("http://localhost:8010/home");
        }
        catch (Exception e)
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = 500;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Ошибка сервера")), ctx);
        }
    }

    public static async Task GetInfoAsync(HttpListenerContext context, CancellationToken ctx)
    {
        try
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            var user = CacheManager.Get<User>(context.Request.Cookies["session-id"].Value);
            var userCopy = new User()
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Image = user.Image,
                Location = user.Location,
                Password = user.Password
            };
            var path = userCopy.Image;
            if (!string.IsNullOrEmpty(path))
            {
                userCopy.Image = "data:image/png;base64," + Convert.ToBase64String(await File.ReadAllBytesAsync(path, ctx));
            }

            var json = JsonSerializer.Serialize(userCopy);
            var bytes = Encoding.UTF8.GetBytes(json);
            await context.Response.OutputStream.WriteAsync(bytes, ctx);
        }
        catch (Exception e)
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = 500;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Ошибка сервера")), ctx);
        }
    }
    
    public static async Task EditUserAsync(HttpListenerContext context, CancellationToken ctx)
    {
        try
        {
            var sessionId = context.Request.Cookies["session-id"]?.Value;
            var user = CacheManager.Get<User>(sessionId);
            
            using var sr = new StreamReader(context.Request.InputStream);
            var userStr = await sr.ReadToEndAsync().ConfigureAwait(false);
            var userModel = JsonSerializer.Deserialize<User>(userStr, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            await AddUsersInfo.AddInfoAsync(user, userModel, ctx);
            var connection = new DatabaseConnectionUser();
            await connection.EditUserAsync(user, ctx);
        }
        catch (Exception e)
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = 500;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Ошибка сервера")), ctx);
        }
    }

    public static async Task DeleteUserAsync(HttpListenerContext context, CancellationToken ctx)
    {
        try
        {
            var sessionId = context.Request.Cookies["session-id"]?.Value;
            var user = CacheManager.Get<User>(sessionId);
            var connection = new DatabaseConnectionUser();
            connection.DeleteUserAsync(user.Email, ctx);
            Exit(context, ctx);
        }
        catch (Exception e)
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = 500;
            await context.Response.OutputStream.WriteAsync(Encoding.UTF8
                .GetBytes(JsonSerializer.Serialize("Ошибка сервера")), ctx);
        }
    }
}