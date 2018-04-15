using ImageService.Logging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        /************************************************************************
        *The Input: a message and it's type.
        *The Output: -
        *The Function operation: The function Notifies all of the subscribers about
        *a new message recieved and logging it into the entry log.
        *************************************************************************/
        public void Log(string message, MessageTypeEnum type)
        {
            //Upadting the messsages' arguments.
            MessageRecievedEventArgs msgRec = new MessageRecievedEventArgs
            {
                Status = type,
                Message = message
            };
            //Notify all the subscribers of the recieved message.
            MessageRecieved?.Invoke(this, msgRec);
        }
    }
}
