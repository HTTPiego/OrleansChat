using GrainInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatSilo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomsController : ControllerBase
    {

        private readonly IGrainFactory _grainFactory;

        public ChatRoomsController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }



    }
}
