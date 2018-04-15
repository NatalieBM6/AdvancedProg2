using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private List<IDirectoryHandler> handlers;
        private string[] handlersPath;
        #endregion

        #region Properties
        //The event that notifies about a new Command being recieved.
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        #endregion

        /************************************************************************
        *The Input: ImageController a LoggingService and the way to a handler.
        *The Output: -
        *The Function operation: The function builds a server.
        *************************************************************************/
        public ImageServer(IImageController controller, ILoggingService logging, string[] handlersPath)
        {
            m_controller = controller;
            m_logging = logging;
            handlers = new List<IDirectoryHandler>();
            this.handlersPath = handlersPath;
            //Creating handlers.
            for (int i = 0; i < handlersPath.Length; i++)
            {
                handlers.Add(new DirectoyHandler(this.m_controller, this.m_logging));
                handlers[i].StartHandleDirectory(handlersPath[i]);
                CommandRecieved += handlers[i].OnCommandRecieved;
                handlers[i].DirectoryClose += OnHandlerClose;
                // Logging each handler into the entry.
                m_logging.Log("Directory Handler created at path:" + handlersPath[i], Logging.Model.MessageTypeEnum.INFO);
            }
        }

        /************************************************************************
        *The Input: -.
        *The Output: -
        *The Function operation: The function does nothing for now.
        *************************************************************************/
        public void SendCommand()
        {
            //
        }

        /************************************************************************
        *The Input: -.
        *The Output: -
        *The Function operation: The function invoke the event.
        *************************************************************************/
        public void CloseServer()
        {
            CommandRecievedEventArgs commandRecEventArgs = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, null);
            CommandRecieved?.Invoke(this, commandRecEventArgs);
        }

        /************************************************************************
        *The Input: sender and an event.
        *The Output: -
        *The Function operation: The function is called when the handler is closed.
        *************************************************************************/
        public void OnHandlerClose(object sender, DirectoryCloseEventArgs e)
        {
            IDirectoryHandler dirHandler = (IDirectoryHandler)sender;
            CommandRecieved -= dirHandler.OnCommandRecieved;
            m_logging.Log("Stop handle directory " + e.Message, Logging.Model.MessageTypeEnum.INFO);
        }
    }
}
