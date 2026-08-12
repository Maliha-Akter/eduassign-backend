using EduAssign.API.Data;
using EduAssign.API.Services;
using EduAssign.API.DTOs.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();


// MongoDB
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped(
    sp => sp.GetRequiredService<MongoDbContext>().Database
);


// Services
builder.Services.AddScoped<
    IAssignmentService,
    AssignmentService
>();


// Better Auth
builder.Services.AddHttpClient();
builder.Services.AddAuthentication("BetterAuth")
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
            .WithOrigins("http://localhost:3000")
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