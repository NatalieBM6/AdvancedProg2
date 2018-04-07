using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Modal;

namespace ImageService.Commands
{
    class CloseHandlerCommand : ICommand
    {
        private IImageServiceModal m_modal;

        public CloseHandlerCommand(IImageServiceModal modal)
        {
            m_modal = modal;            // Storing the Modal
        }


        // change!!!!!
        public string Execute(string[] args, out bool result)
        {
            result = true;
            return "ok";
        }
    }
}