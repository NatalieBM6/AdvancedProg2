using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using ImageService.Modal.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private List<DirectoyHandler> handlersList;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        public event EventHandler<DirectoryCloseEventArgs> CloseCommand;
        #endregion

        public ImageServer(ILoggingService logging, IImageServiceModal modal)
        {
            this.m_logging = logging;
            this.m_controller = new ImageController(modal);

            handlersList = new List<DirectoyHandler>();
            DirectoyHandler current;

            string directories = ConfigurationManager.AppSettings["Handler"];
            string[] handlersDirectories = directories.Split(';');
            for (int i = 0; i < handlersDirectories.Length; i++)
            {
                current = CreateHandler(handlersDirectories[i]);
                handlersList.Add(current);
            }
        }

        public DirectoyHandler CreateHandler(String directory)
        {
            DirectoyHandler h = new DirectoyHandler(directory, m_controller, m_logging);

            if (h.StartHandleDirectory(directory))
            {
                CommandRecieved += h.OnCommandRecieved;
                h.DirectoryClose += CloseCommand;
                m_logging.Log("Starting handler for the directory: " + directory, Logging.Modal.MessageTypeEnum.INFO);

                return h;
            }
            else
            {
                return null;
            }
        }

        public void SendCommand(CommandRecievedEventArgs e)
        {
            CommandRecieved?.Invoke(this, e);
        }

        public void CloseServer()
        {
            m_logging.Log("Closing the server.", Logging.Modal.MessageTypeEnum.INFO);
            CloseCommand?.Invoke(this, null);
        }

        public void OnCloseServer(object sender, DirectoryCloseEventArgs e)
        {
            DirectoyHandler dh = (DirectoyHandler)sender;
            CommandRecieved -= dh.OnCommandRecieved;
            dh.DirectoryClose -= OnCloseServer;
        }
    }
}
