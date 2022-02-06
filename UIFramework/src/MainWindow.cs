using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.Sdl2;
using ImGuiNET;
using System.ComponentModel;

namespace UIFramework
{
    public class MainWindow 
    {
        /// <summary>
        /// Windows attached to the main window.
        /// </summary>
        public List<Window> Windows = new List<Window>();

        /// <summary>
        /// The menu items on the top of the main window.
        /// </summary>
        public List<MenuItem> MenuItems = new List<MenuItem>();

        public static bool ForceFocus = false;

        //General window info
        private static Sdl2Window _window;
        float font_scale = 1.0f;
        bool fullscreen = true;
        bool p_open = true;
        ImGuiDockNodeFlags dockspace_flags = ImGuiDockNodeFlags.None;
        bool renderingFrame = false;

        //docking data
        public unsafe ImGuiWindowClass* window_class;

        public MainWindow() {}

        internal void Init(Sdl2Window window) => _window = window;

        public void OnLoad()
        {
            //Disable the docking buttons
            ImGui.GetStyle().WindowMenuButtonPosition = ImGuiDir.None;

            //Enable docking support
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            //Enable up/down key navigation
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            //Only move via the title bar instead of the whole window
            ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;

            //Load theme files
            ThemeHandler.Load();
            
            InitDock();
        }

        public void OnRenderFrame()
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

            var dock_id = ImGui.GetID("##DockspaceRoot");

            unsafe
            {
                //Create an inital dock space for docking workspaces.
                ImGui.DockSpace(dock_id, new System.Numerics.Vector2(0.0f, 0.0f), 0, window_class);
            }

            Render();

            ImGui.End();
        }

        public virtual void Render()
        {
            if (ImGui.BeginMainMenuBar())
            {
                foreach (var item in MenuItems)
                    ImGuiHelper.DrawMenuItem(item);
                ImGui.EndMainMenuBar();
            }

            foreach (var window in Windows)
                window.Show();
        }

        public virtual void OnResize(int width, int height)
        {

        }

        public virtual void OnFileDrop(string fileName)
        {
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
