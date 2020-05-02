using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace TryMassTransit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ISendEndpointProvider _sendEndpoint;

        public ReportsController(ISendEndpointProvider sendEndpoint)
        {
            _sendEndpoint = sendEndpoint;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            await _sendEndpoint.Send<Shared.CreateReport>(new { ReportId = NewId.NextGuid(), RequestTime = DateTime.Now, EMail = "deneme@gmail.com" });

            return "value";
        }
    }
}