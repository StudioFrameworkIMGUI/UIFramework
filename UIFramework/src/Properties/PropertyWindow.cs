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

        public TreeNode SelectedNode;

        public PropertyWindow(DockSpaceWindow space, string name) : base(space)
        {
            this.Name = name;
        }

        public override void Render()
        {
            SelectedNode?.PropertyDrawer?.Invoke();

            if (SelectedNode != null && SelectedNode.Tag != null)
                ImguiBinder.LoadProperties(SelectedNode.Tag, PropertyChanged);
        }
    }
}
