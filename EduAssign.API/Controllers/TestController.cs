using EduAssign.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace EduAssign.API.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
private readonly MongoDbContext _mongoDb;

public TestController(MongoDbContext mongoDb)
{
    _mongoDb = mongoDb;
}

[HttpGet("database")]
public IActionResult TestDatabase()
{
    var collection = _mongoDb.GetCollection<object>("test");

    return Ok(new
    {
        message = "MongoDB connection is configured successfully!"
    });
}

}