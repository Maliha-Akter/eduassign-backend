using EduAssign.API.Data;
using EduAssign.API.Services;
using EduAssign.API.DTOs.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// ADDED: Memory Cache for caching BetterAuth sessions (~1ms response time after first check)
builder.Services.AddMemoryCache();

// MongoDB
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped(
    sp => sp.GetRequiredService<MongoDbContext>().Database
);

// Services
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();

// Better Auth
builder.Services.AddHttpClient();

// ADDED: Explicitly specify DefaultAuthenticateScheme and DefaultChallengeScheme
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "BetterAuth";
    options.DefaultChallengeScheme = "BetterAuth";
})
.AddScheme<BetterAuthOptions, BetterAuthHandler>("BetterAuth", options => 
{
    options.AuthServerUrl = "http://localhost:3000";
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Frontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();