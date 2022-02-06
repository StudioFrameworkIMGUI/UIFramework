using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;

namespace UIFramework
{
    /// <summary>
    /// Represents a window instance for rendering UI elements in.
    /// </summary>
    public class Window
    {
        /// <summary>
        /// The name of the window.
        /// </summary>
        public virtual string Name { get; set; } = "Window";

        /// <summary>
        /// The flags of the window.
        /// </summary>
        public virtual ImGuiWindowFlags Flags { get; }

        /// <summary>
        /// Determines if the window is opened or not.
        /// </summary>
        public bool Opened = true;

        private bool _windowClosing = false;
        private bool loaded = false;

        /// <summary>
        /// Displays the window and renders it. This must be called during a render loop.
        /// </summary>
        public void Show()
        {
            if (!Opened)
                return;

            //Method for setting up stuff on load
            if (!loaded) {
                OnLoad();
                loaded = true;
            }

            bool visible = ImGui.Begin(this.Name, ref Opened, Flags);
            //Window is no longer opened so call the closing method
            if (!Opened && !_windowClosing) {
                _windowClosing = true;
                OnWindowClosing();
            }
            if (visible) {
                Render();
                ImGui.End();
            }
        }

        public virtual void OnLoad()
        {

        }

        public void Close() => this.Opened = false;

        /// <summary>
        /// Renders the UI elements in the window.
        /// </summary>
        public virtual void Render()
        {

        }

        /// <summary>
        /// Called when the window is about to close.
        /// </summary>
        public virtual void OnWindowClosing()
        {
        }
    }
}
