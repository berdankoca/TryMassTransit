using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using TryMassTransit.Shared;

namespace TryMassTransit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        IPublishEndpoint _publishEndpoint;
        IRequestClient<GetMessages> _client;
        public MessagesController(IPublishEndpoint publishEndpoint, IRequestClient<GetMessages> client)
        {
            _publishEndpoint = publishEndpoint;
            _client = client;
        }

        

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<List<Message>>> GetAsync()
        {
            //Func<Message, bool> expression = p => p.Text.EndsWith("1");
            var response = await _client.GetResponse<MessagesResult>(new { EndWithFilter =  "1" });

            return response.Message.Messages;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            await _publishEndpoint.Publish<Message>(new { Text = $"Hello { id }" });

            return "value";
        }
    }
}
