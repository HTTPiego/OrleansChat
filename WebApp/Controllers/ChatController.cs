
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace WebApp.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class ChatController: ControllerBase
    {
        private readonly IGrainFactory _grainFactory;

        public ChatController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }
    }