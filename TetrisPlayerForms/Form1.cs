using System;
using System.IO;
using System.Windows.Forms;
using TetrisAI.Source;
using TetrisCore.Source;
using TetrisCore.Source.Api;
using static TetrisAI.Source.Evaluator;

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
            EvaluationNNParameter parameter = new EvaluationNNParameter(
                new float[] {
                    -0.1f,-0.1f,-0.1f,-0.1f, -0.1f, -0.1f, -0.1f, -0.1f, 0.1f,//objectHeight
                    -0.2f, -0.25f, -0.25f, -0.25f, -0.25f, -0.25f, -0.25f, -0.25f, -0.25f,//numHole
                    -0.25f, -0.2f, -0.25f, -0.25f, -0.2f, -0.25f, -0.25f, -0.25f, -0.25f,//holeDepth
                    -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f,//numDeadSpace
                    -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f, -0.5f,//wells
                    2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f,//erodedPiece
                    -0.15f, -0.15f, -0.15f, -0.15f, -0.15f, -0.15f, -0.15f, -0.15f, -0.15f,//numRowWithHole
                    -0.35f, -0.35f, -0.35f, -0.35f, -0.35f, -0.35f, -0.35f, -0.35f, -0.35f,//rowTrans
                    -0.35f, -0.35f, -0.35f, -0.35f, -0.35f, -0.35f, -0.35f, -0.35f, -0.35f 
                },
                new float[] { 
                    0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f 
                },
                new float[] { 
                    0.25f, 0.25f, 0.25f, 0.25f, 0.25f 
                });
            string path = "parameters";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            ParameterConfig config = ParameterConfig.Load(path,"params.xml");
            if (config != null) parameter = config.Parameter;
            Evaluator evaluator = new Evaluator(parameter);
            foreach (Control c in this.Controls)
            {
                if (c is IRenderer)
                {
                    TetrisPlayer.GetLogger().Info("Find a Renderer:" + c.GetType().Name);
                    Game.SetRenderer((IRenderer)c);
                    Game.SetController((IController)new AITetrisController(evaluator,100));
                }
            }
            Game.Start();
        }
    }
}