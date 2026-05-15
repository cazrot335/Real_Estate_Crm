using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using backend.Models;
using System.Text.Json;

namespace backend.Services;

public class AuthService
{
    private readonly string _filePath;
    private readonly string _jwtKey;

    public AuthService(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "users.json");
        _jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "SUPER_SECRET_KEY_12345";
    }

    // 🔐 Register with hashing
    public async Task<(bool Success, string Message)> RegisterUserAsync(RegisterRequest request)
    {
        var users = await ReadUsers();

        if (users.Any(u => u.Email == request.Email))
            return (false, "User already exists");

        users.Add(new User
        {
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
        });

        await SaveUsers(users);

        return (true, "User registered successfully");
    }

    // 🔑 Login with password check + JWT
    public async Task<(bool Success, string Token, string Message)> LoginAsync(LoginRequest request)
    {
        var users = await ReadUsers();

        var user = users.FirstOrDefault(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return (false, "", "Invalid credentials");

        var token = GenerateJwtToken(user);

        return (true, token, "Login successful");
    }

    // 🎟️ JWT generator
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, "User") // future RBAC
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // 📦 Helpers
    private async Task<List<User>> ReadUsers()
    {
        if (!File.Exists(_filePath))
            return new List<User>();

        var json = await File.ReadAllTextAsync(_filePath);

        return JsonSerializer.Deserialize<List<User>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new List<User>();
    }

    private async Task SaveUsers(List<User> users)
    {
        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);
    }
}