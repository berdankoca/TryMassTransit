using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using TryMassTransit.Api.Contracts;
using TryMassTransit.Shared;

namespace TryMassTransit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRequestClient<OrderUpdating> _requestClient;

        public OrdersController(IMediator mediator, IRequestClient<OrderUpdating> requestClient)
        {
            _mediator = mediator;
            _requestClient = requestClient;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            
            //Response<UpdatingResult> x = await _requestClient.GetResponse<UpdatingResult>(new { OrderId = id });
            await _mediator.Publish<OrderUpdating>(new { OrderId = id });

            return "value";
        }
    }
}