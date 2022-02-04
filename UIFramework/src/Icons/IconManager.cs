using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace UIFramework
{
    public class IconManager
    {
        public const char FOLDER_ICON = '\ue067';
        public const char FILE_ICON = '\uf15b';
        public const char SEARCH_ICON = '\uf002';

        private static Vector4 FOLDER_COLOR = new Vector4(0.921f, 0.78f, 0.376f, 1.0f);

        /// <summary>
        /// Draws an icon with a custom color style.
        /// </summary>
        public static void DrawIcon(char icon)
        {
            Vector4 color = new Vector4(1.0f);

            if (icon == FOLDER_ICON) color = FOLDER_COLOR;

            ImGui.PushStyleColor(ImGuiCol.Text, color);
            ImGui.Text(icon.ToString());
            ImGui.PopStyleColor();
        }
    }
}
