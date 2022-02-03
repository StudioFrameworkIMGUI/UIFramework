using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace UIFramework
{
    public class MenuItem : INotifyPropertyChanged
    {
        private string _header;
        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                RaisePropertyChanged("Header");
            }
        }

        /// <summary>
        /// The list of sub menu items parented to this menu item.
        /// </summary>
        public List<MenuItem> MenuItems = new List<MenuItem>();

        /// <summary>
        /// Determines if the menu item is checked or not.
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Determines if the menu item can be checked during a click operation.
        /// </summary>
        public bool CanCheck { get; set; }

        /// <summary>
        /// The icon of the menu item to display next to the header.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The tool tip to show during a hover of the menu item.
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// Event for when the menu properties are changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        //Command to execute during a click operation
        private Action _command;

        public MenuItem(string name) {
            _header = name;
        }

        public MenuItem(string name, EventHandler clicked)
        {
            _header = name;
            _command = () => { clicked?.Invoke(this, EventArgs.Empty); };
        }

        public MenuItem(string name, Action clicked)
        {
            _header = name;
            _command = clicked;
        }

        public void Execute() {
            _command?.Invoke();
        }

        protected void RaisePropertyChanged(string memberName = "")
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }
        }
    }
}
