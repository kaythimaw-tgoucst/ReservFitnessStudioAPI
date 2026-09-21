using FitnessStudio.Application.Interfaces.Concurrency;
using FitnessStudio.Application.Interfaces.Gateways;
using FitnessStudio.Application.Interfaces.Identity;
using FitnessStudio.Application.Interfaces.Repositories.ReadOnly;
using FitnessStudio.Application.Interfaces.Repositories.WriteOnly;
using FitnessStudio.Application.UseCase.BookClass;
using FitnessStudio.Application.UseCase.CancelBooking;
using FitnessStudio.Application.UseCase.GetBookingList;
using FitnessStudio.Application.UseCase.GetBusinessStudioList;
using FitnessStudio.Application.UseCase.GetPackageList;
using FitnessStudio.Application.UseCase.GetTimetable;
using FitnessStudio.Application.UseCase.JoinWaitlist;
using FitnessStudio.Application.UseCase.PurchasePackage;
using FitnessStudio.API.Identity;
using FitnessStudio.Infrastructure.Concurrency;
using FitnessStudio.Infrastructure.DataAccess;
using FitnessStudio.Infrastructure.DataAccessWriteOnly;
using FitnessStudio.Infrastructure.Gateways.DateTimeProvider;
using FitnessStudio.Infrastructure.Repository.ReadOnlyRepository;
using FitnessStudio.Infrastructure.Repository.WriteOnlyRepository;
using FitnessStudio.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";

builder.Services.AddDbContext<FitnessStudioDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<FitnessStudioWriteDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisConnectionString));

// Repositories
builder.Services.AddScoped<ITimetableReadOnlyRepository, TimetableRepository>();
builder.Services.AddScoped<IBusinessStudioReadOnlyRepository, BusinessStudioRepository>();
builder.Services.AddScoped<IPackageReadOnlyRepository, PackageRepository>();
builder.Services.AddScoped<IUserReadOnlyRepository, UserRepository>();
builder.Services.AddScoped<IBookingReadOnlyRepository, BookingRepository>();
builder.Services.AddScoped<IPackageWriteRepository, PackageWriteRepository>();
builder.Services.AddScoped<IBookingWriteRepository, BookingWriteRepository>();
builder.Services.AddScoped<IWaitlistWriteRepository, WaitlistWriteRepository>();

// UseCases
builder.Services.AddScoped<IGetTimetableUseCase, GetTimetableUseCase>();
builder.Services.AddScoped<IGetBusinessStudioListUseCase, GetBusinessStudioListUseCase>();
builder.Services.AddScoped<IGetPackageListUseCase, GetPackageListUseCase>();
builder.Services.AddScoped<IPurchasePackageUseCase, PurchasePackageUseCase>();
builder.Services.AddScoped<IBookClassUseCase, BookClassUseCase>();
builder.Services.AddScoped<ICancelBookingUseCase, CancelBookingUseCase>();
builder.Services.AddScoped<IJoinWaitlistUseCase, JoinWaitlistUseCase>();
builder.Services.AddScoped<IGetBookingListUseCase, GetBookingListUseCase>();

// Gateways
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

// Identity
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Concurrency
builder.Services.AddSingleton<IBookingConcurrencyLock, RedisBookingConcurrencyLock>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter a valid JWT token obtained from api/auth/account."
        };

        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });

        return Task.CompletedTask;
    });
});

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

await DatabaseSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "FitnessStudioAPI v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
