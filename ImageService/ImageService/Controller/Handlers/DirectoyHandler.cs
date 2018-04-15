using ImageService.Model;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Model;
using System.Text.RegularExpressions;
using System.Threading;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller.
        private ILoggingService m_logging;                  // The eventLogger.
        private List<FileSystemWatcher> m_listWatchers = new List<FileSystemWatcher>();
        private string m_path;                              // The Path of directory.
        #endregion

        // The Event That Notifies that the Directory is being closed.
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;

       /************************************************************************
       *The Input: a controller and a logging service.
       *The Output: -.
       *The Function operation: The function builds a DirectoryHandler.
       *************************************************************************/
        public DirectoyHandler(IImageController controller, ILoggingService logging)
        {
            m_logging = logging;
            m_controller = controller;
        }

       /************************************************************************
       *The Input: a sender and an event.
       *The Output: -.
       *The Function operation: The function run when a command is recieved.
       *************************************************************************/
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (e.CommandID == (int)CommandEnum.CloseCommand)
            {
                HandlerClose();
            }
        }

        //Closing the handler, the watcher and the end of the service.
        public void HandlerClose()
        {
            foreach (FileSystemWatcher watcher in m_listWatchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }

            DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(m_path, "dir " + m_path + " directory closed"));
        }

       /************************************************************************
       *The Input: The file path to watch.
       *The Output: -.
       *The Function operation: The function starts the handler and watches for new files.
       *************************************************************************/
        public void StartHandleDirectory(string dirPath)
        {
            // Creating our watcher and notifying accordingly.
            m_path = dirPath;
            FileSystemWatcher watcher = new FileSystemWatcher(m_path);
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Changed += new FileSystemEventHandler(OnCreated);
            watcher.EnableRaisingEvents = true;
            this.m_listWatchers.Add(watcher);
            m_logging.Log(" watcher added in " + dirPath, MessageTypeEnum.INFO);
        }

       /************************************************************************
       *The Input: a sender and an event.
       *The Output: -.
       *The Function operation: The function runs when a new file is being watched.
       *************************************************************************/
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string[] path = { e.FullPath };
            //Possible endings for images.
            string[] filters = { ".jpg", ".png", ".gif", ".bmp" };
            string extension = Path.GetExtension(e.FullPath);
            bool result;
            //Checking whether the new file is an image we should handle.
            foreach (string f in filters)
            {
                // If it is we'll execute the new file command and logg accordingly.
                if (extension.Equals(f, StringComparison.InvariantCultureIgnoreCase))
                {
                    Thread.Sleep(100);
                    string msg = m_controller.ExecuteCommand((int)CommandEnum.NewFileCommand, path, out result);
                    if (result)
                    {
                        m_logging.Log("file in new path:" + msg, MessageTypeEnum.INFO);
                    }
                    else
                    {
                        m_logging.Log("Error in moving the new file. Reason: " + msg, MessageTypeEnum.FAIL);
                    }
                }
            }
        }

    }
}
