using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace basicClasses
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        private void SplashScreen_Shown(object sender, EventArgs e)
        {
            GuiContextLoader.callbackFrm = this;
            guiGelegate gui = new guiGelegate(hideSplash);
            GuiContextLoader.gui = gui;

            var th = new Thread(new ThreadStart(GuiContextLoader.LoadCont));
            th.IsBackground = true;
            th.Start();

        }

        public void hideSplash()
        {
            var frm1 = new Form1();
            frm1.Show();
            this.Hide();
        }

      
    }


    public class GuiContextLoader
    {
        public static SplashScreen callbackFrm;
        public static guiGelegate gui;

        public static void updateGui()
        {
            if (gui != null && callbackFrm != null)
            {
                callbackFrm.Invoke(gui);
            }
        }

        public static void LoadCont()
        {
            Parser.ContextGlobal = new opis();
            Parser.LoadEnvironment();

            updateGui();
        }

    }
}
