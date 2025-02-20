using ChatALot.API.Hubs;
using ChatALot.Data.Models.DTOs.Message;
using ChatALot.Data.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace ChatALot.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageServices _message;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(IMessageServices message, IHubContext<ChatHub> hubContext)
        {
            _message = message;
            _hubContext = hubContext;
        }

        [HttpGet("{senderID}/{receiverID}")]
        public async Task<IActionResult> ReceiveMessage(Guid senderID, Guid receiverID)
        {
            try
            {
                Log.Information($"{senderID} is receiving a message from {receiverID}");

                var response = await _message.ReceiveMessage(senderID, receiverID);

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var response = await _message.SendMessage(request);

                if(response)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", request.SenderId, request.ReceiverId, request.Content);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }
    }
}
