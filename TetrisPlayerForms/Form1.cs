using System;
using System.Windows.Forms;
using TetrisAI.Source;
using TetrisCore.Source;
using TetrisCore.Source.Api;

namespace TetrisPlayer
{
    public partial class Form1 : Form
    {
        private TetrisGame Game;

        public Form1()
        {
            InitializeComponent();
            Game = new TetrisGame(TetrisPlayer.GetLogger());
            this.Shown += initialized;
            TetrisPlayer.GetLogger().Info("Initialization finished.");
        }

        private void initialized(object sender, EventArgs a)
        {
            foreach (Control c in this.Controls)
            {
                if (c is IRenderer)
                {
                    TetrisPlayer.GetLogger().Info("Find a Renderer:" + c.GetType().Name);
                    Game.SetRenderer((IRenderer)c);
                    Game.SetController((IController)new AITetrisController(100));
                }
            }
            Game.Start();
        }
    }
}