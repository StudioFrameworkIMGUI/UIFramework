using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using ImGuiNET;
using System.Numerics;
using System.Reflection;

namespace UIFramework
{
    public partial class ImguiBinder
    {
        public static void LoadProperties(object obj, EventHandler propertyChanged = null)
        {
            var style = ImGui.GetStyle();
            var frameSize = style.FramePadding;
            var itemSpacing = style.ItemSpacing;

           // style.ItemSpacing = (new Vector2(itemSpacing.X, 2));
          //  style.FramePadding = (new Vector2(frameSize.X, 2));

            float width = ImGui.GetWindowWidth();

            Dictionary<string, bool> categories = new Dictionary<string, bool>();
            var properties = obj.GetType().GetProperties();
            string category = "Properties";

            ImGui.Columns(2);

            string activeDropdown = "";
            bool showDropdown = true;

            for (int i = 0; i < properties.Length; i++)
            {
                var categoryAttribute = properties[i].GetCustomAttribute<CategoryAttribute>();
                var descAttribute = properties[i].GetCustomAttribute<DescriptionAttribute>();
                var displayAttribute = properties[i].GetCustomAttribute<DisplayNameAttribute>();
                var browsableAttribute = properties[i].GetCustomAttribute<BrowsableAttribute>();
                var readonlyAttribute = properties[i].GetCustomAttribute<ReadOnlyAttribute>();

                if (browsableAttribute != null && !browsableAttribute.Browsable)
                    continue;

                string label = displayAttribute != null ? displayAttribute.DisplayName : properties[i].Name;
                string desc = descAttribute != null ? descAttribute.Description : properties[i].Name;
                bool readOnly = !properties[i].CanWrite;

                if (readonlyAttribute != null && readonlyAttribute.IsReadOnly)
                    readOnly = true;

                category = "Properties";

                if (categoryAttribute != null)
                    category = categoryAttribute.Category;

                //Draw category dropdown
                if (activeDropdown != category)
                {
                    ImGui.Columns(1);

                    activeDropdown = category;
                    showDropdown = ImGui.CollapsingHeader(activeDropdown, ImGuiTreeNodeFlags.DefaultOpen);

                    ImGui.Columns(2);
                }

                if (!showDropdown)
                    continue;

                ImGui.Text(label);
                ImGui.NextColumn();

                float colwidth = ImGui.GetColumnWidth();
                if (properties[i].PropertyType == typeof(Vector3) ||
                    properties[i].PropertyType == typeof(Vector4))
                {
                    ImGui.SetColumnOffset(1, width * 0.25f);
                }

                ImGui.PushItemWidth(colwidth);

                bool valueChanged = SetPropertyUI(properties[i], obj, label, desc, readOnly);

                if (valueChanged)
                    propertyChanged?.Invoke(properties[i].Name, EventArgs.Empty);

                ImGui.PopItemWidth();

                ImGui.NextColumn();
            }

            style.FramePadding = frameSize;
            style.ItemSpacing = itemSpacing;

            ImGui.Columns(1);
        }

        static bool SetPropertyUI(System.Reflection.PropertyInfo property,
            object obj, string label, string desc, bool readOnly)
        {
            bool valueChanged = false;

            var flags = ImGuiInputTextFlags.None;
            if (readOnly)
                flags |= ImGuiInputTextFlags.ReadOnly;

            label = $"##{property.Name}_{property.PropertyType}";

            if (readOnly)
            {
                var disabled = ImGui.GetStyle().Colors[(int)ImGuiCol.TextDisabled];
                ImGui.PushStyleColor(ImGuiCol.Text, disabled);
            }

            if (property.PropertyType.IsEnum)
            {
                var inputValue = property.GetValue(obj);
                var type = property.PropertyType;
                if (ImGui.BeginCombo(label, inputValue.ToString(), ImGuiComboFlags.NoArrowButton))
                {
                    if (!readOnly)
                    {
                        var values = Enum.GetValues(type);
                        foreach (var val in values)
                        {
                            bool isSelected = inputValue == val;
                            if (ImGui.Selectable(val.ToString(), isSelected))
                            {
                                property.SetValue(obj, val);
                                valueChanged = true;
                            }

                            if (isSelected)
                                ImGui.SetItemDefaultFocus();
                        }
                    }

                    ImGui.EndCombo();
                }
            }
            else if (property.PropertyType == typeof(string))
            {
                var inputValue = (string)property.GetValue(obj);
                if (string.IsNullOrEmpty(inputValue))
                    inputValue = " ";

                if (ImGui.InputText(label, ref inputValue, 0x200, flags))
                {
                    property.SetValue(obj, inputValue);
                    valueChanged = true;
                }
            }
            else if(property.PropertyType == typeof(Vector2))
            {
                var inputValue = (Vector2)property.GetValue(obj);
                if (ImGui.DragFloat2(label, ref inputValue))
                {
                    property.SetValue(obj, inputValue);
                    valueChanged = true;
                }
            }
            else if (property.PropertyType == typeof(Vector3))
            {
                var inputValue = (Vector3)property.GetValue(obj);
                if (ImGui.DragFloat3(label, ref inputValue))
                {
                    property.SetValue(obj, inputValue);
                    valueChanged = true;
                }
            }
            else if (property.PropertyType == typeof(Vector4))
            {
                var inputValue = (Vector4)property.GetValue(obj);
                if (ImGui.DragFloat4(label, ref inputValue))
                {
                    property.SetValue(obj, inputValue);
                    valueChanged = true;
                }
            }
            else if (property.PropertyType == typeof(float))
            {
                var inputValue = (float)property.GetValue(obj);
                if (ImGui.DragFloat(label, ref inputValue))
                {
                    property.SetValue(obj, (float)inputValue);
                    valueChanged = true;
                }
            }
            else if (property.PropertyType == typeof(uint))
            {
                var inputValue = (int)(uint)property.GetValue(obj);
                if (ImGui.InputInt(label, ref inputValue, 0, 0, flags))
                {
                    property.SetValue(obj, (uint)inputValue);
                    valueChanged = true;
                }
            }
            else if (property.PropertyType == typeof(int))
            {
                var inputValue = (int)property.GetValue(obj);
                if (ImGui.InputInt(label, ref inputValue, 0, 0, flags))
                {
                    property.SetValue(obj, (int)inputValue);
                    valueChanged = true;
                }
            }
            else if (property.PropertyType == typeof(bool))
            {
                var inputValue = (bool)property.GetValue(obj);
                if (ImGui.Checkbox(label, ref inputValue))
                {
                    property.SetValue(obj, (bool)inputValue);
                    valueChanged = true;
                }
            }

            if (readOnly)
            {
                ImGui.PopStyleColor();
            }
            return valueChanged;
        }

    }
}