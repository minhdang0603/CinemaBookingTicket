using API.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace API.Configurations
{
    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
          //  var key = configuration.GetValue<string>("JwtSettings:Secret");

          //  services.AddAuthentication(option =>
          //  {
          //      option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          //      option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
          //  })
          //.AddJwtBearer(option =>
          //{
          //    option.RequireHttpsMetadata = false;
          //    option.SaveToken = true;
          //    option.TokenValidationParameters = new TokenValidationParameters()
          //    {
          //        ValidateIssuerSigningKey = true,
          //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
          //        ValidateIssuer = true,
          //        ValidateAudience = true,
          //        ValidIssuer = configuration.GetValue<string>("JwtSettings:ValidIssuer"),
          //        ValidAudience = configuration.GetValue<string>("JwtSettings:ValidAudience")
          //    };

          //    option.Events = new JwtBearerEvents
          //    {
          //        OnChallenge = context =>
          //        {
          //            context.HandleResponse();
          //            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
          //            context.HttpContext.Response.ContentType = "application/json";
          //            var response = APIResponse<object>.Builder()
          //                .WithStatusCode(HttpStatusCode.Unauthorized)
          //                .WithErrorMessages(new List<string> { "Authentication failed" })
          //                .WithSuccess(false)
          //                .Build();
          //            return context.HttpContext.Response.WriteAsJsonAsync(response);
          //        },
          //        OnForbidden = context =>
          //        {
          //            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
          //            context.HttpContext.Response.ContentType = "application/json";
          //            var response = APIResponse<object>.Builder()
          //                .WithStatusCode(HttpStatusCode.Forbidden)
          //                .WithErrorMessages(new List<string> { "You do not have permission to access this resource." })
          //                .WithSuccess(false)
          //                .Build();
          //            return context.HttpContext.Response.WriteAsJsonAsync(response);
          //        }
          //    };

          //});
        }
    }
}
