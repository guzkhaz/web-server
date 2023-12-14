﻿namespace WebServer.Models;

public class User
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string? Location { get; set; }
    public string? Image { get; set; }
}