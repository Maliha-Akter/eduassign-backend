using EduAssign.API.Data;
using EduAssign.API.Services;
using EduAssign.API.DTOs.Auth;

var builder = WebApplication.CreateBuilder(args);

// Controllers & OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// MongoDB
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped(sp => sp.GetRequiredService<MongoDbContext>().Database);

// Assignment service
builder.Services.AddScoped<IAssignmentService, AssignmentService>();

// Better Auth Custom Handler
builder.Services.AddHttpClient(); 
builder.Services.AddAuthentication("BetterAuth")
    .AddScheme<BetterAuthOptions, BetterAuthHandler>("BetterAuth", options => { });

builder.Services.AddAuthorization();

// CORS (Configured for Better Auth Cookies)
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Enables cookies across ports
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Frontend");

// Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();