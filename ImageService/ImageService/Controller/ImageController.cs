using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModel m_model; // The Model Object
        private Dictionary<int, ICommand> commands;

        public ImageController(IImageServiceModel model)
        {
            m_model = model; // Storing the Model Of The System
            commands = new Dictionary<int, ICommand>
            {
                { (int)CommandEnum.NewFileCommand, new NewFileCommand(m_model) }
            };
        }

        /************************************************************************
        *The Input: The command's descriptors.
        *The Output: The path of the file if the creation was succesful.
        *The Function operation: The function executes the given command.
        *************************************************************************/
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            {
                if (!commands.ContainsKey(commandID))
                {
                    resultSuccesful = false;
                    return "Command not found";
                }
                ICommand command = commands[commandID];

                // Defining thread for moving files.
                Task<Tuple<string, bool>> executeTask = new Task<Tuple<string, bool>>(() =>
                {
                    bool result;
                    string retVal = commands[commandID].Execute(args, out result);
                    return Tuple.Create(retVal, result);
                });

                executeTask.Start();
                resultSuccesful = executeTask.Result.Item2;
                return executeTask.Result.Item1;
            }
        }
    }
}