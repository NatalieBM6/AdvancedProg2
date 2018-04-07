using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging.Modal
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageTypeEnum Status { get; set; }
        public string Message { get; set; }

        public MessageRecievedEventArgs(MessageTypeEnum stat, String mes)
        {
            this.Status = stat;
            this.Message = mes;
        }
    }
}