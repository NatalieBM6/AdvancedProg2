using ImageService.Infrastructure;
using ImageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModel m_model;
        /************************************************************************
        *The Input: The path of the image and if adding the file was succesful.
        *The Output: a new path.
        *The Function operation: The function returns the path of the file if adding
        * it was succesful.
        *************************************************************************/
        public string Execute(string[] args, out bool result)
        {
            return m_model.AddFile(args[0], out result);
        }

        /************************************************************************
        *The Input: The model logic to run the command.
        *The Output: -.
        *The Function operation: The function builds a new file command.
        *************************************************************************/
        public NewFileCommand(IImageServiceModel model)
        {
            m_model = model;            // Storing the Model
        }
    }
}
