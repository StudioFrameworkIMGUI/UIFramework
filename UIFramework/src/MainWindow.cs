using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ImGuiNET;
using System.ComponentModel;

namespace UIFramework
{
    public class MainWindow : GameWindow
    {
        protected ImGuiController _controller;

        /// <summary>
        /// Windows attached to the main window.
        /// </summary>
        public List<Window> Windows = new List<Window>();

        /// <summary>
        /// The menu items on the top of the main window.
        /// </summary>
        public List<MenuItem> MenuItems = new List<MenuItem>();

        private bool ForceFocus = false;

        //General window info
        float font_scale = 1.0f;
        bool fullscreen = true;
        bool p_open = true;
        ImGuiDockNodeFlags dockspace_flags = ImGuiDockNodeFlags.None;
        bool renderingFrame = false;

        //docking data
        private uint dock_id;
        private unsafe ImGuiWindowClass* window_class;

        public MainWindow(GraphicsMode gMode, string name) : base(1600, 900, gMode, name,
                               GameWindowFlags.Default,
                               DisplayDevice.Default,
                               3, 2, GraphicsContextFlags.Default)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _controller = new ImGuiController(Width, Height);

            //Disable the docking buttons
            ImGui.GetStyle().WindowMenuButtonPosition = ImGuiDir.None;

            //Enable docking support
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            //Enable up/down key navigation
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            //Only move via the title bar instead of the whole window
            ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;
            //Share resources between contexts
            GraphicsContext.ShareContexts = true;

            //Load theme files
            ThemeHandler.Load();

            InitDock();
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            if (renderingFrame) return;

            //Only allow updating the frame once if the frame requires updating in a seperate part of the code

            base.OnRenderFrame(e);

            //Don't render each time if the application is not doing anything
            //This saves on CPU performance
            if (!this.Focused && !ForceFocus) {
                System.Threading.Thread.Sleep(1);
                return;
            }

            renderingFrame = true;

            //Only force the focus once to update the rendered frame
            if (ForceFocus)
                ForceFocus = false;

            //Update the controller
            _controller.Update(this, (float)e.Time);

            RenderWindow();

            _controller.Render();
            SwapBuffers();

            renderingFrame = false;
        }

        private void RenderWindow()
        {
            ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDocking;

            if (fullscreen)
            {
                ImGuiViewportPtr viewport = ImGui.GetMainViewport();
                ImGui.SetNextWindowPos(viewport.WorkPos);
                ImGui.SetNextWindowSize(viewport.WorkSize);
                ImGui.SetNextWindowViewport(viewport.ID);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
                window_flags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
                window_flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
            }

            if ((dockspace_flags & ImGuiDockNodeFlags.PassthruCentralNode) != 0)
                window_flags |= ImGuiWindowFlags.NoBackground;

            //Set the adjustable global font scale
            ImGui.GetIO().FontGlobalScale = font_scale;

            ImGui.Begin("WindowSpace", ref p_open, window_flags);
            ImGui.PopStyleVar(2);

            unsafe
            {
                //Create an inital dock space for docking workspaces.
                ImGui.DockSpace(dock_id, new System.Numerics.Vector2(0.0f, 0.0f), 0, window_class);
            }

            Render();

            ImGui.End();

            //Reset FBO info back to backbuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, Width, Height);
        }

        public virtual void Render()
        {
            if (ImGui.BeginMainMenuBar())
            {
                foreach (var item in MenuItems)
                    ImGuiHelper.DrawMenuItem(item);
                ImGui.EndMenuBar();
            }

            foreach (var window in Windows)
                window.Show();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Tell ImGui of the new size
            _controller.WindowResized(Width, Height);
        }

        protected override void OnFileDrop(FileDropEventArgs e)
        {
            base.OnFileDrop(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            _controller.PressChar(e.KeyChar);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        private unsafe void InitDock()
        {
            uint windowId = ImGui.GetID($"###window_main");

            var nativeConfig = ImGuiNative.ImGuiWindowClass_ImGuiWindowClass();
            (*nativeConfig).ClassId = windowId;
            (*nativeConfig).DockingAllowUnclassed = 0;
            this.window_class = nativeConfig;
        }
    }
}
