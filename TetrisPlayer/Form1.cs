﻿using System;
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
        TetrisGame Game;
        public Form1()
        {
            InitializeComponent();
            Game = new TetrisGame(TetrisPlayer.GetLogger());
            this.Shown += initialized;
            TetrisPlayer.GetLogger().Info("Initialization finished.");
        }
        private void initialized(object sender,EventArgs a)
        {
            foreach(Control c in this.Controls)
            {
                if(c is IRenderer){
                    TetrisPlayer.GetLogger().Info("Find a Renderer:"+c.GetType().Name);
                    Game.SetRenderer((IRenderer)c);
                    Game.SetController((IController)c);
                }
            }
            Game.Start();
        }
    }
}
