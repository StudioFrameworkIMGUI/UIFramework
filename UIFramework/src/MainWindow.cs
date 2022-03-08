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
    public class MainWindow : DockSpaceWindow
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
        ImGuiWindowFlags window_flags;

        public override ImGuiWindowFlags Flags => window_flags;

        public MainWindow() : base("dock_main")
        {}

        internal void Init(Sdl2Window window) => _window = window;

        public void OnApplicationLoad()
        {
            this.Name = "WindowSpace";

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
            
            OnLoad();
        }

        public void OnRenderFrame()
        {
            window_flags = ImGuiWindowFlags.NoDocking;

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
                window_flags |= ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
            }

            if ((dockspace_flags & ImGuiDockNodeFlags.PassthruCentralNode) != 0)
                window_flags |= ImGuiWindowFlags.NoBackground;

            //Set the adjustable global font scale
            ImGui.GetIO().FontGlobalScale = font_scale;

            this.Show();

            ImGui.PopStyleVar(2);
        }

        public override void Render()
        {
            //Use window as a parent dock if no docks added
            if (this.DockedWindows.Count == 0)
            {
                var dock_id = ImGui.GetID("##DockspaceRoot");

                unsafe
                {
                    //Create an inital dock space for docking workspaces.
                    ImGui.DockSpace(dock_id, new System.Numerics.Vector2(0.0f, 0.0f), 0, window_class);
                }
            }

            if (ImGui.BeginMainMenuBar())
            {
                foreach (var item in MenuItems)
                    ImGuiHelper.DrawMenuItem(item);
                ImGui.EndMainMenuBar();
            }

            foreach (var window in Windows)
                window.Show();

            base.Render();
        }

        public virtual void OnResize(int width, int height)
        {

        }

        public virtual void OnFileDrop(string fileName)
        {
        }
    }
}
