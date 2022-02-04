using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;

namespace UIFramework
{
    public class TreeView
    {
        /// <summary>
        /// The root nodes of the tree view.
        /// </summary>
        public List<TreeNode> Nodes = new List<TreeNode>();

        /// <summary>
        /// The selected nodes of the tree view.
        /// </summary>
        protected readonly List<TreeNode> SelectedNodes = new List<TreeNode>();

        /// <summary>
        /// Determines if the tree has focus or not.
        /// </summary>
        public bool IsFocused = false;

        /// <summary>
        /// Determines to display the search box or not.
        /// </summary>
        public bool DisplaySearchBox = true;

        public EventHandler OnSelectionChanged;
        public EventHandler OnNodeChecked;
        public EventHandler OnNodeLeftClicked;

        public TreeNode GetDroppedTreeNode() => dragDroppedNode;

        //Scroll handling
        protected float ScrollX;
        protected float ScrollY;

        //Filter operations
        private bool isSearch = false;
        private string _searchText = "";

        //Rename operations
        private TreeNode renameNode;
        private bool isNameEditing = false;
        private string _renameText = "";
        private double renameClickTime;

        //Constants
        const float RENAME_DELAY_TIME = 0.5f;
        const bool RENAME_ENABLE = true;

        //Drag/drop
        private TreeNode dragDroppedNode;

        private UPDATE_FLAGS UpdateFlags;

        public void AddSelection(TreeNode node) {
            SelectedNodes.Add(node);
            OnSelectionChanged?.Invoke(node, EventArgs.Empty);
        }

        public void RemoveSelection(TreeNode node) {
            SelectedNodes.Remove(node);
            OnSelectionChanged?.Invoke(node, EventArgs.Empty);
        }

        public void ScrollToSelected(TreeNode target)
        {
            if (target == null)
                return;

            //Do not scroll to displayed selected node
            if (SelectedNodes.Contains(target))
                return;

            //Expand parents if necessary
            target.ExpandParent();

            var itemHeight = ImGui.GetTextLineHeightWithSpacing() + 3;

            //Calculate position node is at.
            float pos = 0;
            foreach (var node in Nodes) {
                if (GetNodePosition(target, node, ref pos, itemHeight))
                    break;
            }

            ScrollY = pos;
            UpdateFlags |= UPDATE_FLAGS.SCROLL;
        }

        public void UpdateScroll(float scrollX, float scrollY)
        {
            ScrollX = scrollX;
            ScrollY = scrollY;
            UpdateFlags |= UPDATE_FLAGS.SCROLL;
        }

        public void Render()
        {
            var itemHeight = ImGui.GetTextLineHeightWithSpacing() + 3;

            if (DisplaySearchBox)
            DrawSearchBox();

            //Set the same header colors as hovered and active. This makes nav scrolling more seamless looking
            var active = ImGui.GetStyle().Colors[(int)ImGuiCol.Header];
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, active);
            ImGui.PushStyleColor(ImGuiCol.NavHighlight, new Vector4(0));

            //Put the entire control within a child
            ImGui.BeginChild("##tree_view1");
            {
                IsFocused = ImGui.IsWindowFocused();

                //Scroll to specified position
                if (UpdateFlags.HasFlag(UPDATE_FLAGS.SCROLL)) {
                    ImGui.SetScrollX(ScrollX);
                    ImGui.SetScrollY(ScrollY);
                }
                else {
                    //Store current scroll placements in tree
                    ScrollX = ImGui.GetScrollX();
                    ScrollY = ImGui.GetScrollY();
                }

                foreach (var child in Nodes)
                    DrawNode(child, itemHeight);
            }
            ImGui.EndChild();

            ImGui.PopStyleColor(2);

            UpdateFlags = UPDATE_FLAGS.NONE;
        }

        private void DrawSearchBox()
        {
            ImGuiHelper.IncrementCursorPosX(13);

            ImGui.Text(IconManager.SEARCH_ICON.ToString());
            ImGui.SameLine();

            ImGuiHelper.IncrementCursorPosX(11);

            var posX = ImGui.GetCursorPosX();
            var width = ImGui.GetWindowWidth();

            //Span across entire outliner width
            ImGui.PushItemWidth(width - posX - 5);
            if (ImGui.InputText("##search_box", ref _searchText, 200)) {
                isSearch = !string.IsNullOrWhiteSpace(_searchText);
            }
            ImGui.PopItemWidth();
        }

        private void DrawNode(TreeNode node, float itemHeight)
        {
            //Check if the node is within the necessary search filter requirements if search is used
            bool HasText = node.Header != null &&
                 node.Header.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;

            //Node was selected manually outside the outliner so update the list
            if (node.IsSelected && !SelectedNodes.Contains(node))
                AddSelection(node);

            //Node was deselected manually outside the outliner so update the list
            if (!node.IsSelected && SelectedNodes.Contains(node))
                RemoveSelection(node);

            bool isVisible = isSearch && HasText || !isSearch;
            if (isVisible)
                RenderNode(node, itemHeight);

            if (isSearch || node.IsExpanded)
            {
                foreach (var child in node.Children)
                    DrawNode(child, itemHeight);

                if (!isSearch)
                    ImGui.TreePop();
            }
        }

        private void RenderNode(TreeNode node, float itemHeight)
        {
            var flags = PrepareNodeFlags(node);
            bool isRenaming = renameNode == node && isNameEditing;

            //Improve tree node spacing.
            var spacing = ImGui.GetStyle().ItemSpacing;
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(spacing.X, 1));

            //Align the text to improve selection sizing. 
            ImGui.AlignTextToFramePadding();

            //Disable selection view in renaming handler to make text more clear
            if (isRenaming)
            {
                flags &= ~ImGuiTreeNodeFlags.Selected;
                flags &= ~ImGuiTreeNodeFlags.SpanFullWidth;
            }

            //Load the expander or leaf tree node
            if (isSearch) {
                if (ImGui.TreeNodeEx(node.ID, flags, $"")) { ImGui.TreePop(); }
            }
            else
                node.IsExpanded = ImGui.TreeNodeEx(node.ID, flags, $"");

            //Shift position after drawing expander element
            ImGui.SameLine(); ImGuiHelper.IncrementCursorPosX(3);

            //Node events
            bool leftDoubleClicked = ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left);
            bool leftClicked = ImGui.IsItemClicked(ImGuiMouseButton.Left);
            bool rightClicked = ImGui.IsItemClicked(ImGuiMouseButton.Right);
            bool nodeFocused = ImGui.IsItemFocused();
            bool isToggleOpened = ImGui.IsItemToggledOpen();
            bool beginDragDropSource = !isRenaming && node.CanDragDrop && ImGui.BeginDragDropSource();

            if (beginDragDropSource)
                HandleDragDrop(node);

            bool hasContextMenu = node.ContextMenus.Count > 0;
            //Apply a pop up menu for context items. Only do this if the menu has possible items used
            if (hasContextMenu && SelectedNodes.Contains(node))
                HandleContextMenus(node);

            if (node.HasCheckBox)
                HandleCheckbox(node);

            TryDrawNodeIcon(node);

            ImGui.AlignTextToFramePadding();
            if (isRenaming)
                DrawRenamingNode(node);
            else {
                if (node.RenderOverride != null)
                    node.RenderOverride.Invoke(this, EventArgs.Empty);
                else
                    ImGui.Text(node.Header);
            }
            ImGui.PopStyleVar();

            if (!isRenaming)
            {
                //Check for rename selection on selected renamable node
                if (node.IsSelected && node.CanRename && RENAME_ENABLE)
                {
                    bool renameStarting = renameClickTime != 0;
                    bool wasCancelled = false;

                    //Mouse click before editing started cancels the event
                    if (renameStarting && leftClicked)
                    {
                        renameClickTime = 0;
                        renameStarting = false;
                        wasCancelled = true;
                    }

                    //Check for delay
                    if (renameStarting)
                    {
                        //Create a delay between actions. This can be cancelled out during a mouse click
                        var diff = ImGui.GetTime() - renameClickTime;
                        if (diff > RENAME_DELAY_TIME)
                        {
                            //Name edit executed. Setup data for renaming.
                            isNameEditing = true;
                            _renameText = node.Header;
                            renameNode = node;
                            //Reset the time
                            renameClickTime = 0;
                        }
                    }

                    //User has started a rename click. Start a time check
                    if (leftClicked && renameClickTime == 0 && !wasCancelled)
                    {
                        //Do a small delay for the rename event
                        renameClickTime = ImGui.GetTime();
                    }
                }

                //Deselect node during ctrl held when already selected
                if (leftClicked && ImGui.GetIO().KeyCtrl && node.IsSelected)
                {
                    RemoveSelection(node);
                    node.IsSelected = false;
                }
                //Click event executed on item
                else if ((leftClicked || rightClicked) && !isToggleOpened) //Prevent selection change on toggle
                {
                    //Reset all selection unless shift/control held down
                    if (!ImGui.GetIO().KeyCtrl && !ImGui.GetIO().KeyShift)
                    {
                        foreach (var n in SelectedNodes)
                            n.IsSelected = false;
                        SelectedNodes.Clear();
                    }

                    //Add the clicked node to selection.
                    node.IsSelected = true;
                    AddSelection(node);
                }  //Focused during a scroll using arrow keys
                else if (nodeFocused && !isToggleOpened && !node.IsSelected)
                {
                    if (!ImGui.GetIO().KeyCtrl && !ImGui.GetIO().KeyShift)
                    {
                        foreach (var n in SelectedNodes)
                            n.IsSelected = false;
                        SelectedNodes.Clear();
                    }

                    //Add the clicked node to selection.
                    AddSelection(node);
                    node.IsSelected = true;
                }
                if (leftClicked && node.IsSelected)
                    OnNodeLeftClicked?.Invoke(node, EventArgs.Empty);

                //Double click event
                if (leftDoubleClicked && !isToggleOpened && node.IsSelected) {
                    node.OnDoubleClicked();
                }
            }
        }

        private void DrawRenamingNode(TreeNode node)
        {
            var bg = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];

            //Make the textbox frame background blend with the tree background
            //This is so we don't see the highlight color and can see text clearly
            ImGui.PushStyleColor(ImGuiCol.FrameBg, bg);
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(1, 1, 1, 1));
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1);

            var length = ImGui.CalcTextSize(_renameText).X + 20;
            ImGui.PushItemWidth(length);

            if (ImGui.InputText("##RENAME_NODE", ref _renameText, 512,
                ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackCompletion |
                ImGuiInputTextFlags.CallbackHistory | ImGuiInputTextFlags.NoHorizontalScroll))
            {
                node.Header = _renameText;
                node.OnHeaderRenamed?.Invoke(this, EventArgs.Empty);

                isNameEditing = false;
            }
            if (!ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                isNameEditing = false;
            }

            ImGui.PopItemWidth();
            ImGui.PopStyleVar();
            ImGui.PopStyleColor(2);
        }

        private void HandleCheckbox(TreeNode node)
        {
            ImGui.SetItemAllowOverlap();

            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(2, 2));

            bool check = node.IsChecked;

            if (ImGui.Checkbox($"##check{node.ID}", ref check))
            {
                foreach (var n in SelectedNodes)
                    n.IsChecked = check;
                this.OnNodeChecked?.Invoke(this, EventArgs.Empty);
            }
            ImGui.PopStyleVar();

            ImGui.SameLine();
        }

        private void HandleContextMenus(TreeNode node)
        {
            ImGui.PushID(node.Header);

            //Better menu visuals
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(8, 1));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(8, 2));
            ImGui.PushStyleColor(ImGuiCol.Separator, new System.Numerics.Vector4(0.4f, 0.4f, 0.4f, 1.0f));

            if (ImGui.BeginPopupContextItem("##OUTLINER_POPUP", ImGuiPopupFlags.MouseButtonRight))
            {
                DrawRightClickMenu(node);
                ImGui.EndPopup();
            }
            ImGui.PopStyleVar(2);
            ImGui.PopStyleColor(1);

            ImGui.PopID();
        }

        private void HandleDragDrop(TreeNode node)
        {
            //Placeholder pointer data. Instead use drag/drop nodes from GetDragDropNode()
            GCHandle handle1 = GCHandle.Alloc(node.ID);
            ImGui.SetDragDropPayload("OUTLINER_ITEM", (IntPtr)handle1, sizeof(int), ImGuiCond.Once);
            handle1.Free();

            dragDroppedNode = node;

            ImGuiHelper.IncrementCursorPosX(3);

            //Display icon
            ImGuiHelper.IncrementCursorPosX(4);

            ImGui.AlignTextToFramePadding();
            TryDrawNodeIcon(node, true);

            //Display text for item being dragged
            ImGui.AlignTextToFramePadding();
            ImGui.Text($"{node.Header}");
            ImGui.EndDragDropSource();
        }

        private void TryDrawNodeIcon(TreeNode node, bool isDragDrop = false)
        {
            //Ajudst position if using checkbox
            if (node.HasCheckBox && !isDragDrop)
                ImGuiHelper.IncrementCursorPosX(5);

            node.IconDrawer?.Invoke(this, EventArgs.Empty);

            //Assume the icon is just a char
            if (node.Icon?.Length == 1)
            {
                IconManager.DrawIcon(node.Icon[0]);
                ImGui.SameLine();
            }

            //Shift
            ImGuiHelper.IncrementCursorPosX(3);
        }

        private ImGuiTreeNodeFlags PrepareNodeFlags(TreeNode node)
        {
            ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None;
            flags |= ImGuiTreeNodeFlags.SpanFullWidth;

            if (node.Children.Count == 0 || isSearch)
                flags |= ImGuiTreeNodeFlags.Leaf;
            else
            {
                flags |= ImGuiTreeNodeFlags.OpenOnDoubleClick;
                flags |= ImGuiTreeNodeFlags.OpenOnArrow;
            }
            if (node.IsExpanded && !isSearch) {
                //Flags for opening as default settings
                flags |= ImGuiTreeNodeFlags.DefaultOpen;
                //Make sure the "IsExpanded" can force the node to expand
                ImGui.SetNextItemOpen(true);
            }
            if (SelectedNodes.Contains(node))
                flags |= ImGuiTreeNodeFlags.Selected;

            return flags;
        }

        private void DrawRightClickMenu(TreeNode node)
        {
            foreach (var item in node.ContextMenus)
                ImGuiHelper.DrawMenuItem(item);
        }

        private bool GetNodePosition(TreeNode target, TreeNode parent, ref float pos, float itemHeight)
        {
            bool HasText = parent.Header != null &&
              parent.Header.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0;

            //Search is active and node is found but is not in results so skip scrolling
            if (isSearch && parent == target && !HasText)
                return false;
            //Node is found so return
            if (parent == target) {
                return true;
            }
            //Only update results for visible nodes
            if (isSearch && HasText || !isSearch)
                pos += itemHeight;
            if (parent.IsExpanded)
            {
                foreach (var child in parent.Children) {
                    if (GetNodePosition(target, child, ref pos, itemHeight))
                        return true;
                }
            }
            return false;
        }

        enum UPDATE_FLAGS
        {
            NONE,
            SCROLL,
        }
    }
}
