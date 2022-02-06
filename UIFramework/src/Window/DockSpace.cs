using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace UIFramework
{
    /// <summary>
    /// Represents a region to dock multiple docking windows.
    /// </summary>
    public class DockSpace
    {
        /// <summary>
        /// A list of dockable windows that can dock to this dockspace.
        /// </summary>
        public List<DockWindow> DockedWindows = new List<DockWindow>();

        /// <summary>
        /// Determines to reload the dock layout or not.
        /// </summary>
        public bool UpdateDockLayout = true;

        public bool Opened = true;

        private uint dockspaceId = 0;

        private string Name;

        public DockSpace(string name) {
            Name = name;
        }

        //Set a unique dock space ID
        private void Init() => dockspaceId = ImGui.GetID($"{Name}ds");
        
        public unsafe void Render(ImGuiWindowClass* window_class)
        {
            if (dockspaceId == 0)
                Init();

            unsafe
            {
                //Check if the dock has been created or needs to be updated
                if (ImGui.DockBuilderGetNode(dockspaceId).NativePtr == null || this.UpdateDockLayout) {
                    ReloadDockLayout();
                }
                ImGui.DockSpace(dockspaceId, new System.Numerics.Vector2(0, 0),
                    ImGuiDockNodeFlags.CentralNode, window_class);
            }

            foreach (var window in DockedWindows)
                window.Show();
        }

        public void ReloadDockLayout()
        {
            ImGuiDockNodeFlags dockspace_flags = ImGuiDockNodeFlags.None;

            ImGui.DockBuilderRemoveNode(dockspaceId); // Clear out existing layout
            ImGui.DockBuilderAddNode(dockspaceId, dockspace_flags); // Add empty node

            //This variable will track the document node
            uint dock_main_id = dockspaceId;
            //Reset IDs
            foreach (var dock in DockedWindows)
                dock.DockID = 0;

            foreach (var dock in DockedWindows)
            {
                if (dock.DockDirection == ImGuiDir.None)
                    dock.DockID = dock_main_id;
                else
                {
                    //Search for the same dock ID to reuse if possible
                    var dockedWindow = DockedWindows.FirstOrDefault(x => x != dock && x.DockDirection == dock.DockDirection && x.SplitRatio == dock.SplitRatio && x.ParentDock == dock.ParentDock);
                    if (dockedWindow != null && dockedWindow.DockID != 0)
                        dock.DockID = dockedWindow.DockID;
                    else if (dock.ParentDock != null)
                        dock.DockID = ImGui.DockBuilderSplitNode(dock.ParentDock.DockID, dock.DockDirection, dock.SplitRatio, out uint dockOut, out dock.ParentDock.DockID);
                    else
                        dock.DockID = ImGui.DockBuilderSplitNode(dock_main_id, dock.DockDirection, dock.SplitRatio, out uint dockOut, out dock_main_id);
                }
                ImGui.DockBuilderDockWindow(dock.Name, dock.DockID);
            }
            ImGui.DockBuilderFinish(dockspaceId);

            UpdateDockLayout = false;
        }
    }
}
