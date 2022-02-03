using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace UIFramework
{
    public class TestWindow : Window
    {
        public override string Name => "Test";

        public TreeView TreeView = new TreeView();

        public TestWindow() {
            FillTree();
        }

        private void FillTree()
        {
            TreeNode root = new TreeNode("Root");
            TreeView.Nodes.Add(root);

            root.AddChild(new FileNode("RenamableNode") { CanRename = true });
            root.AddChild(new FileNode("DragDropNode") { CanDragDrop = true });
            root.AddChild(new FileNode("CheckableNode") { HasCheckBox = true });
            TreeNode node4 = new FileNode("Render Override");
            root.AddChild(node4);

            node4.RenderOverride += delegate
            {
                ImGuiHelper.BeginBoldText();
                ImGui.Text(node4.Header);
                ImGuiHelper.EndBoldText();
            };
        }

        public override void Render()
        {
            TreeView.Render();
        }

        public class FileNode : TreeNode
        {
            public override string Icon => IconManager.FILE_ICON.ToString();

            public FileNode(string name) : base(name)
            {

            }
        }
    }
}
