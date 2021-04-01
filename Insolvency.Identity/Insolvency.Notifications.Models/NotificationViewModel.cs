using System;
using System.Collections.Generic;
using Insolvency.Notifications.Models.Json;
using Newtonsoft.Json;

namespace Insolvency.Notifications.Models
{
    public class NotificationViewModel
    {
        public NotificationViewModel()
        { }

        public NotificationViewModel(NotificationMessage message)
        {
            this.Id = message.ExternalId;
            this.CreatedOn = message.CreatedOn;
            var content = JsonConvert.DeserializeObject<APIContent>(message.PayLoad);
            this.PayLoad = content.Personalisation;
            
            MessageType = message.MessageType;
            ServiceName = message.SenderSystem?.Name;
            MessageVersion = message.MessageVersion;
            
        }
        
        public string Id { get; set; }
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime CreatedOn { get; set; }
        public string ServiceName { get; set; }
        public string MessageType { get; set; }
        public string MessageVersion { get; set; }
        public Dictionary<string, object> PayLoad { get; set; }
               
    }
}
