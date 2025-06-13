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
        var user = _dbContext.Users.SingleOrDefault(u => u.Email.ToLower() == loginRequest.Email.ToLower());

        var isValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

        if (user == null || !isValid)
        {
            throw new AppException(ErrorCodes.InvalidCredentials());
        }

        // Generate JWT token
        var expiration = DateTime.UtcNow.AddMinutes(tokenExpirationInMinutes);
        var token = await GenerateJwtToken(user, expiration);

        return new LoginResponseDTO
        {
            Token = token,
            Expiration = expiration,
            User = _mapper.Map<UserDTO>(user)
        };
    }

    public async Task<LoginResponseDTO> RegisterAsync(UserCreateDTO userCreateDTO)
    {
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
            EmailConfirmed = false,
            PhoneNumber = userCreateDTO.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, userCreateDTO.Password);
        if (!result.Succeeded)
        {
            throw new AppException(ErrorCodes.UserCreationFailed());
        }

        await _userManager.AddToRoleAsync(user, Constant.Role_Customer);

        // Send confirmation email
        await _emailService.SendWelcomeEmailAsync(user.Email, user.Name);

        return new LoginResponseDTO
        {
            Token = await GenerateJwtToken(user, DateTime.UtcNow.AddMinutes(tokenExpirationInMinutes)),
            Expiration = DateTime.UtcNow.AddMinutes(tokenExpirationInMinutes),
            User = _mapper.Map<UserDTO>(user)
        };
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user, DateTime expiration)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Email.ToString()),
            new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
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
