using ImageService.Modal;
using ImageService.Controller.Handlers;
using ImageService.Modal.Event;
using ImageService.Infrastructure.Enums;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Text.RegularExpressions;
using System.Threading;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logger;
        private List<FileSystemWatcher> m_dirWatchers;      // The Watchers of the Dir
        private string m_path;                              // The Path of directory
        #endregion

        public delegate void CloseCommand(object sender, CommandRecievedEventArgs e);
        private Dictionary<int, Delegate> commands;

        // The Event That Notifies that the Directory is being closed.
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;

        public DirectoyHandler(IImageController controller, ILoggingService logger)
        {
            m_controller = controller;
            m_logger = logger;
            m_dirWatchers = new List<FileSystemWatcher>();

            commands = new Dictionary<int, Delegate>()
            {
                {(int)CommandEnum.CloseCommand, new CloseCommand(CloseHandler) }
            };
        }

        public void StartHandleDirectory(string dirPath)
        {
            m_path = dirPath;

            foreach (string extension in new string [] { "*.jpg", "*.png", "*.gif", "*.bmp"})
            {
                FileSystemWatcher watcher = new FileSystemWatcher(m_path, extension);
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                watcher.EnableRaisingEvents = true;
                watcher.Created += new FileSystemEventHandler(OnCreated);
                m_dirWatchers.Add(watcher);
            }
            m_logger.Log($"Watcher has been created for: {m_path}", MessageTypeEnum.INFO);
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string msg = m_controller.ExecuteCommand((int)CommandEnum.NewFileCommand, new string[] { e.FullPath }, out bool resultSuccesful);

            //checking if the execution was succesfull.
            if (resultSuccesful)
            {
                m_logger.Log(msg, MessageTypeEnum.INFO);
            }
            else
            {
                m_logger.Log(msg, MessageTypeEnum.FAIL);
            }
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            //first need to check if the command is mentioned.
            if (e.RequestDirPath.Equals(m_path) || e.RequestDirPath.Equals("*"))
            {
                if (!commands.TryGetValue(e.CommandID, out Delegate command))
                {
                    command.DynamicInvoke(sender, e);
                }
            }
        }

        public void CloseHandler(object sender, CommandRecievedEventArgs e)
        {
            //stop the listening and send message.
            foreach (FileSystemWatcher watcher in m_dirWatchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(m_path, "stopped listening to the following folder: "));
        }
    }
}
