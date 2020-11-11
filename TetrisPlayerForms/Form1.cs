using System;
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
            parameter = new EvaluationNNParameter(
                new float[] { 0.23214602f, 0.37655354f, 0.94237113f, -0.4917581f, -0.7450013f, 0.11374831f, -0.9399138f, 0.36454725f, -0.05010283f, 0.036227703f, 0.2850113f, -0.21503127f, -0.64786696f, -0.8472132f, -0.22908473f, 0.17756069f, 0.21729195f, -0.45580405f, -0.24754316f, 0.5636325f, 0.39910638f, -0.5957318f, -0.34143484f, 0.5915102f, 0.38965416f, -0.039565682f, 0.51423776f, -0.65069723f, -0.67759496f, -0.62713444f, -0.5674633f, 0.8629664f, 0.48954594f, 0.9463831f, 0.2982906f, 0.807472f, -0.94503796f, 0.8797451f, -0.49487728f, 0.6142807f, -0.06694317f, 0.5910989f, -0.63534296f, 0.81078374f, -0.0057765245f, 0.6525917f, 0.95764637f, -0.4035489f, -0.6652111f, -0.9920791f, -0.9675733f, -0.35851938f, -0.3435105f, 0.16826344f, 0.27687585f, -0.5978645f, -0.40561694f, 0.65659714f, 0.83526325f, -0.38618422f, 0.57659805f, -0.67263883f, 0.5554663f, 0.08960545f, 0.9269755f, -0.8237728f, -0.39135468f, -0.15480512f, -0.950732f, 0.7185949f, -0.18775779f, 0.9107655f, -0.9611256f, -0.96474457f, 0.5172442f, 0.822896f, -0.73322666f, -0.9939027f, -0.10907638f, 0.49136806f, 0.3588164f }
                ,new float[] { -0.059703648f, 0.17841876f, 0.7632127f, -0.22320312f, -0.50907314f, -0.11780739f, 0.70955443f, 0.7335521f, -0.053162217f, -0.15801579f, -0.10729563f, 0.9992496f, -0.8777785f, -0.3535915f, 0.8691312f, 0.48100555f, 0.4663043f, 0.32654405f, -0.91246027f, -0.7098743f, -0.0678184f, 0.9784125f, -0.945633f, 0.22066641f, 0.02145493f, 0.47436345f, -0.895573f, 0.26019073f, -0.64501673f, 0.3175063f, -0.37746185f, 0.11227262f, -0.9627803f, 0.26448727f, 0.63017416f, -0.023714423f, -0.072077274f, 0.0731951f, -0.84212965f, 0.7451601f, 0.56746614f, 0.53852606f, 0.51756024f, 0.43890774f, -0.010107458f }
                ,new float[] { 0.30896604f, -0.15392143f, 0.1624595f, -0.7763811f, -0.3916123f }
                );
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