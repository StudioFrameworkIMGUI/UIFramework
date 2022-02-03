using System;
using UIFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System.Reflection;

namespace UIFramework.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Load the window and run the application
            GraphicsMode mode = new GraphicsMode(new ColorFormat(32), 24, 8, 4, new ColorFormat(32), 2, false);
            MainWindow wnd = new MainWindow(mode, "UIFramework Test");
            wnd.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            wnd.VSync = OpenTK.VSyncMode.On;
            wnd.MenuItems.Add(new MenuItem("Test Item", () =>
            {

            }));
            wnd.Windows.Add(new TestWindow());

            wnd.Run();
        }
    }
}
