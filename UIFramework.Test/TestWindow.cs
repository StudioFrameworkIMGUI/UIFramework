using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using ImGuiNET;

namespace UIFramework
{
    public class TestWindow : Window
    {
        public override string Name => "Test";

        public TreeView TreeView = new TreeView();

        public TestWindow() {
            FillTreeColumnView();
        }

        private void FillTreeColumnView()
        {
            TreeView.ColumnCount = 2;

            TreeNode root = new TreeNode("Root");
            TreeView.Nodes.Add(root);

            root.AddChild(new ColumnNode("Test1"));
            root.AddChild(new ColumnNode("Test2"));
            root.AddChild(new ColumnNode("Test3"));
        }

        private void FillTreeFiles()
        {
            FolderNode root = new FolderNode("Root");
            TreeView.Nodes.Add(root);
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
            FolderNode node5 = new FolderNode("ContextItems1");
            TreeNode node6 = new FileNode("ContextItems2");
            node6.ContextMenus.Add(new MenuItem("Test2"));
            root.AddChild(node5);
            root.AddChild(node6);

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

        public class FolderNode : TreeNode
        {
            public FolderNode(string name) : base(name)
            {
                ContextMenus.Add(new MenuItem("Rename", () => this.ActivateRename = true));
                ContextMenus.Add(new MenuItem(""));
                ContextMenus.Add(new MenuItem("Add Folder", AddFolder));
                ContextMenus.Add(new MenuItem("Add File", AddFile));

                CanRename = true;
            }

            private void AddFolder()
            {
                AddChild(new FolderNode("NewFolder"));
            }

            private void AddFile()
            {
                AddChild(new FileNode("NewFile"));
            }
        }

        public class ColumnNode : TreeNode
        {
            public float Value = 0;

            public ColumnNode(string name) : base(name)
            {
                HasCheckBox = true;
                CanDragDrop = true;

                RenderOverride += delegate
                {
                    ImGui.Text(this.Header);
                    ImGui.SameLine();

                    ImGui.TextColored(new Vector4(1, 1, 0.2f, 1), $"({Value})");

                    ImGui.NextColumn();

                    ImGui.PushItemWidth(ImGui.GetColumnWidth());
                    ImGui.DragFloat($"##{ID}float", ref Value);
                    ImGui.PopItemWidth();

                    ImGui.NextColumn();
                };
            }
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
