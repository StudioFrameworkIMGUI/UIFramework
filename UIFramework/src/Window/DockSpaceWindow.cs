using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace UIFramework
{
    public class DockSpaceWindow : Window
    {
        public unsafe ImGuiWindowClass* window_class;

        DockSpace dockSpace = null;

        public DockSpaceWindow(string name) 
        {
            Name = name;
            dockSpace = new DockSpace($"##{name}DockSpace");
        }

        public void AddDock(DockWindow dock) {
            dockSpace.DockedWindows.Add(dock);
        }

        public void RemoveDock(DockWindow dock) {
            dockSpace.DockedWindows.Remove(dock);
        }

        public void ClearDocks() {
            dockSpace.DockedWindows.Clear();
        }

        public override void Render()
        {
            unsafe
            {
                dockSpace.Render(window_class);
            }
        }

        public override void OnLoad()
        {
            unsafe
            {
                uint windowId = ImGui.GetID($"###window_{this.Name}");

                ImGuiWindowClass windowClass = new ImGuiWindowClass();
                windowClass.ClassId = windowId;
                windowClass.DockingAllowUnclassed = 0;
                this.window_class = &windowClass;
            }
        }
    }
}
