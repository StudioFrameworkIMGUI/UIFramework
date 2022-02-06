using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIFramework;
using Veldrid.Sdl2;
using ImGuiNET;

namespace UIFramework
{
    public class MainWindowTest : MainWindow 
    {
        DockSpaceWindow DockSpace = new DockSpaceWindow("DockSpace");

        public MainWindowTest() 
        {
            this.MenuItems.Add(new MenuItem("Test", Add));

            var window = new TestWindow();
            window.DockDirection = ImGuiNET.ImGuiDir.Left;
            window.SplitRatio = 0.4f;
            DockSpace.AddDock(window);
        }

        private void Add()
        {
            ImguiFileDialog dlg = new ImguiFileDialog();
            dlg.AddFilter(".png", "Portable Network Grahpics");
            dlg.SaveDialog = false;
            dlg.MultiSelect = true;
            if (dlg.ShowDialog())
            {

            }
        }

        public override void Render()
        {
            base.Render();

            var contentSize = ImGui.GetWindowSize();

            unsafe
            {
                //Constrain the docked windows within a workspace using window classes
                ImGui.SetNextWindowClass(window_class);
                //Set the window size on load
                ImGui.SetNextWindowSize(contentSize, ImGuiCond.Once);
            }

            DockSpace.Show();
        }
    }
}
