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
        List<DockSpaceWindow> DockSpaces = new List<DockSpaceWindow>();

        PropertyWindow PropertyWindow;

        public MainWindowTest() 
        {
         //   Windows.Add(new Window("TEST_WINDOW"));
            DockSpaces.Add(LoadDockSpace("Test1"));
         //   DockSpaces.Add(LoadDockSpace("Test2"));
        }

        private DockSpaceWindow LoadDockSpace(string name)
        {
            DockSpaceWindow DockSpace = new DockSpaceWindow(name);

            var window = new TestWindow(DockSpace);
            window.DockDirection = ImGuiDir.Left;
            window.SplitRatio = 0.25f;
            DockSpace.DockedWindows.Add(window);

            PropertyWindow = new PropertyWindow(DockSpace, "Properties")
            {
                DockDirection = ImGuiDir.Right,
                SplitRatio = 0.25f,
            };

            //Update properties
            window.TreeView.OnSelectionChanged += (o, e) =>
            {
                var node = o as TreeNode;
                if (node != null)
                    PropertyWindow.SelectedProperty = node.Tag;
            };

            DockSpace.DockedWindows.Add(new DockWindow(DockSpace, "Document")
            {
                DockDirection = ImGuiDir.None,
            });
            DockSpace.DockedWindows.Add(PropertyWindow);
            DockSpace.DockedWindows.Add(new DockWindow(DockSpace, "Console")
            {
                DockDirection = ImGuiDir.Down,
                SplitRatio = 0.25f,
            });
            return DockSpace;
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

            foreach (var space in DockSpaces)
            {
                unsafe
                {
                    //Constrain the docked windows within a workspace using window classes
                    ImGui.SetNextWindowClass(window_class);
                    //Set the window size on load
                    ImGui.SetNextWindowSize(contentSize, ImGuiCond.Once);
                }

                space.Show();
            }
        }
    }
}
