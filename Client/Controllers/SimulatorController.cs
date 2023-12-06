using GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using RandomNameGeneratorLibrary;

namespace Client.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SimulatorController : Controller
{
    
    private readonly IGrainFactory _grainFactory;
        
    public SimulatorController(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }
    
    /// <summary>
    /// Generates 50 users with randomly defined usernames.
    /// </summary>
    /// <returns></returns>
    [HttpGet("massive-generate-users")]
    public IActionResult MassiveGenerateUsers()
    {
        var nameGenerator = new PersonNameGenerator();
        Parallel.ForEach(Enumerable.Repeat(true, 50), async (_, _) =>
        {
            var name = nameGenerator.GenerateRandomFirstName().ToLower();
            var surname = nameGenerator.GenerateRandomLastName().ToLower();
            var friend = _grainFactory.GetGrain<IUser>($"{name}.{surname}");
        });

        return Ok("Successfully generated the new users");
    }
}