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
                new float[] { 0.8183296f, 0.38890624f, 0.40890932f, -0.29137295f, 0.108611345f, -0.93645227f, 0.18760645f, -0.9520677f, 0.4470123f, -0.42095333f, -0.7528384f, -0.4251545f, -0.86679745f, -0.8741549f, -0.8954503f, -0.96680593f, 0.5196415f, 0.92288625f, -0.8280602f, 0.8042444f, -0.60987985f, 0.21100497f, 0.12107432f, 0.91218305f, 0.4981072f, -0.48696917f, -0.8272456f, 0.27317262f, -0.20926577f, -0.54112047f, 0.65664864f, -0.84227216f, -0.57481563f, 0.81682706f, 0.5274862f, -0.34164888f, 0.8046477f, 0.98999655f, -0.80819845f, -0.16841996f, -0.6487147f, 0.49439335f, 0.5089283f, -0.7343436f, 0.56659114f, -0.80402356f, 0.89838696f, 0.7025782f, 0.18745399f, 0.5096487f, 0.83033895f, 0.5302665f, -0.12249488f, 0.81369233f, 0.9970912f, 0.006399989f, 0.8150419f, -0.15228081f, -0.6811196f, -0.5075685f, 0.7526717f, -0.56533396f, 0.181687f, 0.18554568f, 0.85840714f, 0.5270382f, 0.25794053f, 0.57151985f, -0.34751576f, 0.3933363f, 0.50344265f, 0.8203099f, -0.0713889f, -0.9159263f, -0.46527892f, -0.3528294f, 0.50237703f, -0.15141147f, -0.08206093f, 0.16035795f, 0.63602793f }
                ,new float[] { -0.7945786f, 0.15922678f, -0.6381672f, -0.2991721f, -0.2518896f, -0.9405304f, 0.7085171f, 0.1398561f, -0.5459409f, -0.22580177f, 0.057913065f, 0.8079171f, 0.37249708f, -0.5049263f, 0.9097253f, -0.28075188f, -0.43755734f, 0.6598004f, 0.9374453f, -0.79897666f, 0.83523595f, 0.22124755f, 0.28556406f, -0.37818742f, 0.3387419f, -0.8547374f, -0.7067491f, -0.56707245f, -0.21373028f, -0.20960128f, 0.43821025f, -0.61272526f, 0.48200858f, -0.39244485f, 0.2447033f, -0.5107032f, 0.72646654f, -0.42648667f, 0.49132216f, -0.5920825f, 0.812317f, -0.5184488f, 0.9005163f, -0.023837566f, 0.62294567f }
                ,new float[] { 0.8201655f, -0.23329103f, 0.86151326f, -0.808007f, 0.5422691f }
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