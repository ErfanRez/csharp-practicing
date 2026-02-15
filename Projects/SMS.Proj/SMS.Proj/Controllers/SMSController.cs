using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMS.Gateway.DTOs;
using SMS.Proj.DTOs;

namespace SMS.Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<SMSController> _logger;

        public SMSController(IPublishEndpoint publishEndpoint, ILogger<SMSController> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult SendSingleSms([FromBody] SingleSMSDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Sender) || string.IsNullOrEmpty(request.Receiver))
            {
                return BadRequest("Invalid request.");
            }

            _publishEndpoint.Publish(request);

            return Ok("Single SMS sent successfully.");
        }

        [HttpPost("/bulk")]
        public IActionResult SendSms([FromBody] BulkSmsDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Sender) || request.Receiver.Count == 0)
            {
                return BadRequest("Invalid request.");
            }

            foreach (var item in request.Receiver)
            {
                var singleRequest = new SingleSMSDto
                {
                    Sender = request.Sender,
                    Receiver = item,
                    Text = request.Text
                };
                _publishEndpoint.Publish(singleRequest);
            }

            return Ok("Bulk SMS sent successfully.");
        }
    }
}
