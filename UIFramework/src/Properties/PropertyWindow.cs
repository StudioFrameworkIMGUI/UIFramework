using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIFramework
{
    public class PropertyWindow : DockWindow
    {
        public EventHandler PropertyChanged;

        public object SelectedProperty;

        public PropertyWindow(DockSpaceWindow space, string name) : base(space)
        {
            this.Name = name;
        }

        public override void Render()
        {
            if (SelectedProperty != null)
                ImguiBinder.LoadProperties(SelectedProperty, PropertyChanged);
        }
    }
}
