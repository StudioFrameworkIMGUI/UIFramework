using System;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using ImGuiNET;
using System.Runtime.InteropServices;

namespace UIFramework
{
    public class Framework
    {
        /// <summary>
        /// The main window of the application.
        /// </summary>
        public static MainWindow ApplicationWindow;

        private static Sdl2Window _window;
        private static GraphicsDevice _gd;
        private static CommandList _cl;
        private static ImGuiController _controller;

        // UI state
        private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);

        private static bool vsync = true;
        private static bool debug = false;

        public static MainWindow CreateWindow()
        {
            return new MainWindow();
        }

        public static void Run(MainWindow main_window, string name, GraphicsBackend graphicsBackend)
        {
            var window = new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, $"{name}: {graphicsBackend}");
            var options = new GraphicsDeviceOptions(debug, null, vsync, ResourceBindingModel.Improved, true, true);

            _window = VeldridStartup.CreateWindow(window);
            _gd = VeldridStartup.CreateGraphicsDevice(_window, options, graphicsBackend);

            main_window.Init(_window);

            ApplicationWindow = main_window;
            _window.Resized += () =>
            {
                _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
                _controller.WindowResized(_window.Width, _window.Height);

                ApplicationWindow.OnResize(_window.Width, _window.Height);
            };
            _window.DragDrop += (e) =>
            {
                ApplicationWindow.OnFileDrop(e.File);
            };

            _cl = _gd.ResourceFactory.CreateCommandList();
            _controller = new ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);

            ApplicationWindow.OnLoad();

            // Main application loop
            while (_window.Exists)
            {
                bool focused = _window.Focused;

                float deltaSeconds = focused ? 1f / 60f : 1f / 60f;

                InputSnapshot snapshot = _window.PumpEvents();
                if (!_window.Exists) { break; }

                //Thread sleep when application is not used.
                if (!focused)
                    System.Threading.Thread.Sleep(1);

                //Update render if a force focus is also used (animation playback)
                bool render = focused || MainWindow.ForceFocus;

                _controller.Update(deltaSeconds, snapshot, render); // Feed the input events to our ImGui controller, which passes them through to ImGui.
                if (render)
                {
                    ApplicationWindow.OnRenderFrame();

                    _cl.Begin();
                    _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
                    _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
                    _controller.Render(_gd, _cl);
                    _cl.End();
                    _gd.SubmitCommands(_cl);
                    _gd.SwapBuffers(_gd.MainSwapchain);
                }
            }

            // Clean up Veldrid resources
            _gd.WaitForIdle();
            _controller.Dispose();
            _cl.Dispose();
            _gd.Dispose();
        }
    }
}
