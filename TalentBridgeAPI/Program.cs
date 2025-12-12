using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Security.Claims;
using TalentBridge.API.DTOs.CommunityDTOs;
using TalentBridge.API.DTOs.MentorshipDTOs;
using TalentBridge.API.Interfaces.ICommunity;
using TalentBridge.API.Interfaces.IMentorship;
using TalentBridge.API.Services;
using TalentBridge.API.Services.MentorshipServices;
using TalentBridge.API.Validations;
using TalentBridge.API.Validations.MentorshipValidators;
using TalentBridge.Application.DTOs.CommentsDTOs;
using TalentBridge.Application.DTOs.CVDTOs;
using TalentBridge.Application.DTOs.UserDTOs;
using TalentBridge.Application.Interfaces;
using TalentBridge.Application.Interfaces.IUser;
using TalentBridge.Application.Services;
using TalentBridge.Application.Settings;
using TalentBridge.Application.Validations;
using TalentBridge.Infrastructure.Data;
using TalentBridge.Infrastructure.Repositories;
using TalentBridge.Infrastructure.Repositories.MentorshipRepositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
{
    throw new InvalidOperationException("JWT Settings are not configured properly");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role
    };
    
    // EVENTOS DE DEBUG
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"❌ JWT FAILED: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var claims = context.Principal?.Claims;
            var claimsList = claims?.Select(c => $"{c.Type}={c.Value}").ToList() ?? new();
            Console.WriteLine($"✅ JWT VALID. Claims: {string.Join(" | ", claimsList)}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"⚠️ JWT CHALLENGE: {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
builder.Services.AddScoped<IValidator<RefreshTokenRequestDto>, RefreshTokenRequestValidator>();
builder.Services.AddScoped<IValidator<RevokeTokenRequest>, RevokeTokenRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateCVDto>, UpdateCVDtoValidator>();
builder.Services.AddScoped<IValidator<CreateAdminDto>, CreateAdminDtoValidator>();
builder.Services.AddScoped<IValidator<CreateUniversityDto>, CreateUniversityValidator>();
builder.Services.AddScoped<IValidator<UpdateUniversityDto>, UpdateUniversityValidator>();
builder.Services.AddScoped<IValidator<CreateCareerDto>, CreateCareerValidator>();
builder.Services.AddScoped<IValidator<CreateCommunityDto>, CreateCommunityValidator>();
builder.Services.AddScoped<IValidator<UpdateCommunityDto>, UpdateCommunityValidator>();
builder.Services.AddScoped<IValidator<CreatePostDto>, CreatePostValidator>();
builder.Services.AddScoped<IValidator<UpdatePostDto>, UpdatePostValidator>();
builder.Services.AddScoped<IValidator<CreateCommentDto>, CreateCommentValidator>();
builder.Services.AddScoped<IValidator<UpdateCommentDto>, UpdateCommentValidator>();
builder.Services.AddScoped<IValidator<UpdateMemberRoleDto>, UpdateMemberRoleValidator>();
builder.Services.AddScoped<IValidator<CreateMentorshipDto>, CreateMentorshipValidator>();
builder.Services.AddScoped<IValidator<UpdateMentorshipDto>, UpdateMentorshipValidator>();
builder.Services.AddScoped<IValidator<CreateSessionDto>, CreateSessionValidator>();
builder.Services.AddScoped<IValidator<UpdateSessionDto>, UpdateSessionValidator>();
builder.Services.AddScoped<IValidator<UpdateAttendanceDto>, UpdateAttendanceValidator>();
builder.Services.AddScoped<IValidator<CreateMilestoneDto>, CreateMilestoneValidator>();
builder.Services.AddScoped<IValidator<UpdateMilestoneDto>, UpdateMilestoneValidator>();
builder.Services.AddScoped<IValidator<CreateResourceDto>, CreateResourceValidator>();
builder.Services.AddScoped<IValidator<UpdateResourceDto>, UpdateResourceValidator>();
builder.Services.AddScoped<IValidator<CreateMentorshipRequestDto>, CreateMentorshipRequestValidator>();
builder.Services.AddScoped<IValidator<CreateMentorApplicationDto>, CreateMentorApplicationValidator>();
builder.Services.AddScoped<IValidator<UpdateMentorProfileDto>, UpdateMentorProfileValidator>();
builder.Services.AddScoped<IValidator<SearchMentorsDto>, SearchMentorsValidator>();

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        fv.AutomaticValidationEnabled = true;
        fv.ImplicitlyValidateChildProperties = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TalentBridge API",
        Version = "v1",
        Description = "API for TalentBridge Application with JWT Authentication"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// SERVICIOS DE AUTENTICACIÓN Y USUARIO
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICVRepository, CVRepository>();
builder.Services.AddScoped<ICVService, CVService>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();
builder.Services.AddScoped<IUniversityCareerRepository, UniversityCareerRepository>();
builder.Services.AddScoped<IUniversityService, UniversityService>();
builder.Services.AddScoped<ICommunityRepository, CommunityRepository>();
builder.Services.AddScoped<ICommunityMemberRepository, CommunityMemberRepository>();
builder.Services.AddScoped<ICommunityPostRepository, CommunityPostRepository>();
builder.Services.AddScoped<ICommunityCommentRepository, CommunityCommentRepository>();
builder.Services.AddScoped<IPostLikeRepository, PostLikeRepository>();
builder.Services.AddScoped<ICommentLikeRepository, CommentLikeRepository>();
builder.Services.AddScoped<ICommunityService, CommunityService>();
builder.Services.AddScoped<ICommunityPostService, CommunityPostService>();
builder.Services.AddScoped<ICommunityCommentService, CommunityCommentService>();

// SERVICIOS DE MENTORÍA
builder.Services.AddScoped<IMentorshipRepository, MentorshipRepository>();
builder.Services.AddScoped<IMentorshipSessionRepository, MentorshipSessionRepository>();
builder.Services.AddScoped<IMentorshipMilestoneRepository, MentorshipMilestoneRepository>();
builder.Services.AddScoped<IMentorshipResourceRepository, MentorshipResourceRepository>();
builder.Services.AddScoped<IMentorshipRequestRepository, MentorshipRequestRepository>();
builder.Services.AddScoped<IMentorApplicationRepository, MentorApplicationRepository>();
builder.Services.AddScoped<ISessionAttendanceRepository, SessionAttendanceRepository>();
builder.Services.AddScoped<IMentorshipService, MentorshipService>();

builder.Services.AddScoped<DatabaseSeeder>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAdminAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TalentBridge API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();