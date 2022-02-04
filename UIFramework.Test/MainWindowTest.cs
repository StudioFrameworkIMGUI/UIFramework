using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIFramework;
using Veldrid.Sdl2;

namespace UIFramework
{
    public class MainWindowTest : MainWindow 
    {
        public MainWindowTest() 
        {
            this.MenuItems.Add(new MenuItem("Test"));
            this.Windows.Add(new TestWindow());
        }

        public override void Render()
        {
            base.Render();
        }
    }
}
