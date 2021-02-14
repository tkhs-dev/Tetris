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
            this.Shown += initialized;
            TetrisPlayer.GetLogger().Info("Initialization finished.");
        }

        private void initialized(object sender, EventArgs a)
        {
            EvaluationNNParameter parameter = new EvaluationNNParameter(
                new float[] {
                    0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f
                },
                new float[] {
                    0.25f, 0.25f, 0.25f, 0.25f, 0.25f
                });
            string path = "parameters";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var p = EvaluationNNParameter.Load(typeof(EvaluationNNParameter), path, "params.xml") as EvaluationNNParameter;
            if (p != null) parameter = p;
            Evaluator evaluator = new Evaluator(parameter);

            GamePlayData playdata = (GamePlayData)GamePlayData.Load(typeof(GamePlayData), "playdata", "data.xml");

            //Game = new TetrisGame(TetrisPlayer.GetLogger(),10,20,playdata.ObjectPool,playdata.ObjectQueue);
            Game = new TetrisGame(TetrisPlayer.GetLogger());
            foreach (Control c in this.Controls)
            {
                if (c is IRenderer)
                {
                    TetrisPlayer.GetLogger().Info("Find a Renderer:" + c.GetType().Name);
                    Game.SetRenderer((IRenderer)c);
                    Game.SetController((IController)new AITetrisController(evaluator, 100));
                    //Game.SetController((IController)new ReplayController(playdata.Events));
                }
            }
            Game.RecordPlayDataEnabled = false;
            Game.Start();
        }
    }
}