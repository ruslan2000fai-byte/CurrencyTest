using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Commands.RegisterUser;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CQRS: разделение контекстов для чтения и записи
builder.Services.AddDbContext<AppReadDbContext>(options =>
	{
		options.UseNpgsql(builder.Configuration.GetConnectionString("ReadConnection"));
		options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
	});

builder.Services.AddDbContext<AppWriteDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("WriteConnection")));

builder.Services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.MapInboundClaims = false;
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtSettings["Issuer"],
			ValidAudience = jwtSettings["Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
		};
		options.Events = new JwtBearerEvents
		{
			OnTokenValidated = async context =>
			{
				var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
				var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
				if (jti != null && await tokenService.IsTokenRevokedAsync(jti, context.HttpContext.RequestAborted))
					context.Fail("Token has been revoked.");
			}
		};
	});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler(errApp =>
{
	errApp.Run(async context =>
	{
		var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = exception switch
		{
			InvalidOperationException => StatusCodes.Status409Conflict,
			UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
			KeyNotFoundException => StatusCodes.Status404NotFound,
			_ => StatusCodes.Status500InternalServerError
		};
		await context.Response.WriteAsJsonAsync(new { error = exception?.Message });
	});
});

app.Run();
