using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace UIFramework
{
    public static class ImGuiHelper
    {
        /// <summary>
        /// Makes any font used UI element as bold.
        /// </summary>
        public static void BeginBoldText() {
            ImGui.PushFont(ImGuiController.DefaultFontBold);
        }

        /// <summary>
        /// Closes the BeginBoldText()
        /// </summary>
        public static void EndBoldText() {
            ImGui.PopFont();
        }

        /// <summary>
        /// Increases the cursor position on the X direction.
        /// </summary>
        public static void IncrementCursorPosX(float amount) {
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + amount);
        }

        /// <summary>
        /// Increases the cursor position on the Y direction.
        /// </summary>
        public static void IncrementCursorPosY(float amount) {
            ImGui.SetCursorPosX(ImGui.GetCursorPosY() + amount);
        }

        /// <summary>
        /// Draws a menu item. Must be inside a current popup or menubar.
        /// </summary>
        public static void DrawMenuItem(MenuItem item, bool alignFramePadding = true)
        {
            string header = item.Header;

            if (item.Icon?.Length == 1) {
                IconManager.DrawIcon(item.Icon[0]);
            }

            if (string.IsNullOrEmpty(header))
            {
                ImGui.Separator();
                return;
            }

            if (alignFramePadding)
                ImGui.AlignTextToFramePadding();

            bool opened = false;
            if (item.MenuItems.Count == 0)
            {
                if (ImGui.MenuItem(header, "", item.IsChecked))
                {
                    if (item.CanCheck)
                        item.IsChecked = !item.IsChecked;
                    item.Execute();
                }
            }
            else
            {
                opened = ImGui.BeginMenu(header);
            }

            if (ImGui.IsItemHovered() && !string.IsNullOrEmpty(item.ToolTip))
                ImGui.SetTooltip(item.ToolTip);

            if (opened)
            {
                foreach (var child in item.MenuItems)
                    DrawMenuItem(child);

                ImGui.EndMenu();
            }
        }
    }
}
