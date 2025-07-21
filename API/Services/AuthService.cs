using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Utility;

namespace API.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private string secretKey;
    private string issuer;
    private string audience;
    private int tokenExpirationInMinutes;

    public AuthService(IConfiguration configuration, UserManager<ApplicationUser> userManager,
                        IMapper mapper, ApplicationDbContext dbContext,
                        IEmailService emailService)
    {
        _configuration = configuration;
        _userManager = userManager;
        _mapper = mapper;
        _dbContext = dbContext;
        _emailService = emailService;
        secretKey = _configuration.GetValue<string>("JwtSettings:Secret") ?? "";
        issuer = _configuration.GetValue<string>("JwtSettings:ValidIssuer") ?? "";
        audience = _configuration.GetValue<string>("JwtSettings:ValidAudience") ?? "";
        tokenExpirationInMinutes = _configuration.GetValue<int>("JwtSettings:DurationInMinutes");
    }


    public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest)
    {
        if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
        {
            throw new AppException(ErrorCodes.InvalidCredentials());
        }

        var user = _dbContext.Users.SingleOrDefault(u => u.Email != null && u.Email.ToLower() == loginRequest.Email.ToLower());

        var isValid = user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password);

        if (user == null || !isValid)
        {
            throw new AppException(ErrorCodes.InvalidCredentials());
        }

        // Check if email is confirmed
        if (!user.EmailConfirmed)
        {
            throw new AppException(ErrorCodes.EmailNotConfirmed());
        }

        // Generate JWT token
        var expiration = DateTime.UtcNow.AddMinutes(tokenExpirationInMinutes);
        var token = await GenerateJwtToken(user, expiration);

        return new LoginResponseDTO
        {
            Token = token,
            Expiration = expiration
        };
    }

    public async Task<string> RegisterAsync(UserCreateDTO userCreateDTO)
    {
        // Validate input
        if (string.IsNullOrEmpty(userCreateDTO.Email) || string.IsNullOrEmpty(userCreateDTO.Password) || string.IsNullOrEmpty(userCreateDTO.Name))
        {
            throw new AppException(ErrorCodes.UserCreationFailed());
        }

        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(userCreateDTO.Email);
        if (existingUser != null)
        {
            throw new AppException(ErrorCodes.UserAlreadyExists(userCreateDTO.Email));
        }

        var user = new ApplicationUser
        {
            UserName = userCreateDTO.Email,
            Email = userCreateDTO.Email,
            NormalizedEmail = userCreateDTO.Email.ToUpper(),
            Name = userCreateDTO.Name,
            EmailConfirmed = false, // Set to false initially
            PhoneNumber = userCreateDTO.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, userCreateDTO.Password);
        if (!result.Succeeded)
        {
            throw new AppException(ErrorCodes.UserCreationFailed());
        }

        await _userManager.AddToRoleAsync(user, Constant.Role_Customer);

        // Generate email confirmation token using Identity
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // Send confirmation email with userId and token
        await _emailService.SendEmailConfirmationAsync(user.Email, user.Name, $"{user.Id}:{token}");

        return "Registration successful! Please check your email to verify your account.";
    }

    public async Task<bool> VerifyEmailAsync(string userId, string token)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                // Send welcome email after successful verification
                await _emailService.SendWelcomeEmailAsync(user.Email!, user.Name!);
            }

            return result.Succeeded;
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user, DateTime expiration)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Name ?? ""),
            new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? ""),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
