using System.Collections.Generic;
using System.Windows.Input;

namespace VNDBUpdater.Models
{
    public class MenuItem
    {
        public string Header { get; set; }
        public List<MenuItem> Children { get; set; }
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }

        public MenuItem(string header, ICommand command = null, object commandParameter = null)
        {
            Header = header;
            Command = command;
            CommandParameter = commandParameter;
            Children = new List<MenuItem>();
        }
    }
}
