using GrainInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatSilo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        //private readonly IUser _user;

        private readonly IGrainFactory _grainFactory;


        /*public UsersController(IUser user)
        {
            _user = user;  
        }*/

        public UsersController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }


    }
}
