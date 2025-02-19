using ChatALot.Data.Models.Domains;
using ChatALot.Data.Models.DTOs.Message;
using ChatALot.Data.Repository.Interface;
using ChatALot.Data.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatALot.Data.Services.Implementation
{
    public class MessageServices : IMessageServices
    {
        private readonly IMessageRepository _messageRepository;

        public MessageServices(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<IEnumerable<ReceiveMessageRequest>> ReceiveMessage(Guid user1, Guid user2)
        {
            try
            {
                var request = await _messageRepository.ReceiveAsync(user1, user2);

                var response = new List<ReceiveMessageRequest>();

                foreach ( var message in request)
                {
                    var msg = new ReceiveMessageRequest
                    {
                        Id = message.Id,
                        SenderId = message.SenderId,
                        ReceiverId = message.ReceiverId,
                        Content = message.Content
                    };

                    response.Add(msg);
                }

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> SendMessage(SendMessageRequest message)
        {
            try
            {
                var request = new Message
                {
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    Content = message.Content,
                };
                
                var response = await _messageRepository.SendAsync(request);

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
