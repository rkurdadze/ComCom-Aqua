using System.Windows.Forms;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Aqua
{
    public partial class MainWindow : Form
    {
       

        public MainWindow()
        {
            InitializeComponent();
#if GL
            Text = "MonoGame.Forms.GL";
#elif DX
            Text = "MonoGame.Forms.DX";
#endif

            CanvasControl.CubesValueChanged += new CubesChangedEventHandler(CubesValueChaged);
        }

        private void CubesValueChaged(object sender, StringValueChangedEventArgs e)
        {
            toolStripStatusLabel1.Text = e.NewValue;
        }
    }
}
