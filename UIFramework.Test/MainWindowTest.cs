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
            this.MenuItems.Add(new MenuItem("Test", Add));
            this.Windows.Add(new TestWindow());
        }

        private void Add()
        {
            ImguiFileDialog dlg = new ImguiFileDialog();
            dlg.AddFilter(".png", "Portable Network Grahpics");
            dlg.SaveDialog = false;
            dlg.MultiSelect = true;
            if (dlg.ShowDialog())
            {

            }
        }

        public override void Render()
        {
            base.Render();
        }
    }
}
