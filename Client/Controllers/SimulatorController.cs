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
    /// Generates 20 users with randomly defined usernames.
    /// </summary>
    /// <returns></returns>
    [HttpGet("massive-generate-users")]
    public async Task<IActionResult> MassiveGenerateUsers()
    {
        var nameGenerator = new PersonNameGenerator();
        
        await Parallel.ForEachAsync(Enumerable.Repeat(true, 20), async (_, _) =>
        {
            var name = nameGenerator.GenerateRandomFirstName();
            var surname = nameGenerator.GenerateRandomLastName();
            var completeName = $"{name} {surname}";
            var username = $"{name.ToLower()}.{surname.ToLower()}";
            var newUser = _grainFactory.GetGrain<IUser>(username);

            await newUser.TryCreateUser(completeName, username);
        });

        return Ok(new {Message="Successfully generated the new users"});
    }
}