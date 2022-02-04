using System;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using ImGuiNET;
using System.Runtime.InteropServices;
using UIFramework;

namespace UIFramework.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            GraphicsBackend graphicsBackend = VeldridStartup.GetPlatformDefaultBackend();
            if (GraphicsDevice.IsBackendSupported(GraphicsBackend.Vulkan))
                graphicsBackend = GraphicsBackend.Vulkan;

            Framework.Run(new MainWindowTest(), "UIFramework.Test", graphicsBackend);
        }
    }
}
