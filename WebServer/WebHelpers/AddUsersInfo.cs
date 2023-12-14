using WebServer.Models;
using WebServer.Validation;

namespace WebServer.WebHelpers;

public static class AddUsersInfo
{
    public static async Task AddInfoAsync(User user, User userInfo, CancellationToken ctx)
    {
        var locationValidator = new LocationValidator();
        var nameValidator = new NameValidator();
        var surnameValidator = new SurnameValidator();
        if (!string.IsNullOrEmpty(userInfo.Location) && (await locationValidator.ValidateAsync(userInfo, ctx)).IsValid)
        {
            user.Location = userInfo.Location;
        }
        if ((await nameValidator.ValidateAsync(userInfo, ctx)).IsValid)
        {
            user.Name = userInfo.Name;
        }
        if ((await surnameValidator.ValidateAsync(userInfo, ctx)).IsValid)
        {
            user.Surname = userInfo.Surname;
        }
        if (!string.IsNullOrEmpty(userInfo.Image))
        {
            var filePath =
                $@"C:\Users\Guzel\Documents\INF HOMEWORK\web-server\WebServer\www\files\{user.Email.Split(".")[0]}.png";
            byte[] imageData = Convert.FromBase64String(userInfo.Image.Split(",")[1]);
            await File.WriteAllBytesAsync(filePath, imageData, ctx);
            user.Image = filePath;
        }
    }
}