using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;

namespace UIFramework
{
    /// <summary>
    /// Represents a window that supports docking.
    /// </summary>
    public class DockWindow : Window
    {
        /// <summary>
        /// The direction of the docking window within the parent docking host.
        /// </summary>
        public ImGuiDir DockDirection = ImGuiDir.None;

        /// <summary>
        /// The split ratio within the docking host.
        /// </summary>
        public float SplitRatio = 0.0f;

        /// <summary>
        /// The parent dock to parent as a docking host.
        /// </summary>
        public DockWindow ParentDock;

        /// <summary>
        /// The docking ID to identify this docking layout.
        /// </summary>
        public uint DockID;

        public override string ToString()
        {
            return $"{Name}_{DockDirection}_{SplitRatio}_{DockID}";
        }
    }
}
