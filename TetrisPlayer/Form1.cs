using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TetrisCore.Source;
using TetrisCore.Source.Api;

namespace TetrisPlayer
{
    public partial class Form1 : Form
    {
        TetrisGame Game = new TetrisGame(TetrisPlayer.GetLogger());
        public Form1()
        {
            InitializeComponent();
            this.Shown += initialized;
        }
        private void initialized(object sender,EventArgs a)
        {
            foreach(Control c in this.Controls)
            {
                if(c is IRenderer){
                    TetrisPlayer.GetLogger().Info("Find a Renderer:"+c.GetType().Name);
                    Game.SetRenderer((IRenderer)c);
                }
            }
            Game.Start();
        }
    }
}
