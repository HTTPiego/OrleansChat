using Client.Repositories;
using Client.Repositories.Interfaces;
using GrainInterfaces;
using Grains;
using Grains.GrainState;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Orleans;
using RandomNameGeneratorLibrary;

namespace Client.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SimulatorController : Controller
{
    
    private readonly IGrainFactory _grainFactory;

    private readonly IUserRepository _userRepository;

    public SimulatorController(IGrainFactory grainFactory, IUserRepository userRepository)
    {
        _grainFactory = grainFactory;
        _userRepository = userRepository;
    }
    
    /// <summary>
    /// Generates 20 users with randomly defined usernames.
    /// </summary>
    /// <returns></returns>
    [HttpGet("massive-generate-users")]
    public IActionResult MassiveGenerateUsers()
    {
        var nameGenerator = new PersonNameGenerator();

        for (int i = 0; i < 20; i++)
        {
            var name = nameGenerator.GenerateRandomFirstName();
            var surname = nameGenerator.GenerateRandomLastName();
            var username = $"{name.ToLower()}.{surname.ToLower()}";
            var completeName = $"{name} {surname}";
            var newUser = _grainFactory.GetGrain<IUser>(username);

            newUser.TryCreateUserRetState(completeName, username); 
            var a = new UserDB(username);

            var userAdded = _userRepository.AddUser(a);

        }
        return Ok(new {Message="Successfully generated the new users"});
    }
}

/*await Parallel.ForEachAsync(Enumerable.Repeat(true, 20), async(_, _) =>
        {
            var name = nameGenerator.GenerateRandomFirstName();
var surname = nameGenerator.GenerateRandomLastName();
var completeName = $"{name} {surname}";
var username = $"{name.ToLower()}.{surname.ToLower()}";
var newUser = _grainFactory.GetGrain<IUser>(username);

await newUser.TryCreateUser(completeName, username);

await _userRepository.AddUser(await newUser.GetUserState());

        });*/