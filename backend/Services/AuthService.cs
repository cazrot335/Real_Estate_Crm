using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // 🔐 REGISTER (Viewer)
    public async Task<(bool Success, string Message)> RegisterUserAsync(RegisterRequest request)
{
    if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        return (false, "User already exists");

    // 👇 Fetch Viewer role
    var viewerRole = await _context.Roles
        .FirstAsync(r => r.Name == "Viewer");

    var user = new User
    {
        Email = request.Email,
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        UserRoles = new List<UserRole>
        {
            new UserRole { RoleId = viewerRole.Id }
        }
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return (true, "Viewer registered successfully");
}

    // 🔑 LOGIN (JWT with roles + permissions)
    public async Task<(bool Success, string Token, string Message)> LoginAsync(LoginRequest request)
{
    var user = await _context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
        .FirstOrDefaultAsync(u => u.Email == request.Email);

    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        return (false, "", "Invalid credentials");

    var token = GenerateJwtToken(user);

    return (true, token, "Login successful");
}

    // 👑 ADMIN: CREATE AGENT
    public async Task<(bool Success, string Message)> CreateAgentAsync(CreateAgentRequest request)
{
    if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        return (false, "User already exists");

    // 👇 Fetch Agent role
    var agentRole = await _context.Roles
        .FirstAsync(r => r.Name == "Agent");

    var user = new User
    {
        Email = request.Email,
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        UserRoles = new List<UserRole>
        {
            new UserRole { RoleId = agentRole.Id }
        }
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return (true, "Agent created successfully");
}

    // 🎟️ GENERATE JWT TOKEN
    private string GenerateJwtToken(User user)
{
    var key = _config["JWT_SECRET"];

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Email)
    };

    // 👇 Extract roles
    var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    // Extract permissions
    var permissions = user.UserRoles
        .SelectMany(ur => ur.Role.RolePermissions)
        .Select(rp => rp.Permission.Name)
        .Distinct()
        .ToList();

    claims.AddRange(permissions.Select(p => new Claim("permission", p)));

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddHours(2),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

// 🔄 ASSIGN ROLE TO USER
public async Task<(bool Success, string Message)> AssignRoleToUserAsync(AssignRoleRequest request)
{
    var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
    var roleExists = await _context.Roles.AnyAsync(r => r.Id == request.RoleId);

    if (!userExists || !roleExists)
        return (false, "User or Role not found");

    var alreadyAssigned = await _context.UserRoles
        .AnyAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId);

    if (alreadyAssigned)
        return (false, "Role already assigned");

    _context.UserRoles.Add(new UserRole
    {
        UserId = request.UserId,
        RoleId = request.RoleId
    });

    await _context.SaveChangesAsync();

    return (true, "Role assigned successfully");
}

// 🔑 ADMIN: ASSIGN PERMISSION
public async Task<(bool Success, string Message)> AssignPermissionToRoleAsync(AssignPermissionRequest request)
{
    var roleExists = await _context.Roles.AnyAsync(r => r.Id == request.RoleId);
    var permExists = await _context.Permissions.AnyAsync(p => p.Id == request.PermissionId);

    if (!roleExists || !permExists)
        return (false, "Role or Permission not found");

    var alreadyAssigned = await _context.RolePermissions
        .AnyAsync(rp => rp.RoleId == request.RoleId && rp.PermissionId == request.PermissionId);

    if (alreadyAssigned)
        return (false, "Permission already assigned");

    _context.RolePermissions.Add(new RolePermission
    {
        RoleId = request.RoleId,
        PermissionId = request.PermissionId
    });

    await _context.SaveChangesAsync();

    return (true, "Permission assigned successfully");
}

// 📝 ADMIN: GET ALL USERS
public async Task<List<object>> GetUsersAsync()
{
    return await _context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .Select(u => new
        {
            u.Id,
            u.Email,
            Roles = u.UserRoles.Select(ur => ur.Role.Name)
        })
        .ToListAsync<object>();
}

// 📋 ADMIN: GET ALL ROLES
public async Task<List<Role>> GetRolesAsync()
{
    return await _context.Roles.ToListAsync();
}

// 📋 ADMIN: GET ALL PERMISSIONS
public async Task<List<Permission>> GetPermissionsAsync()
{
    return await _context.Permissions.ToListAsync();
}
}