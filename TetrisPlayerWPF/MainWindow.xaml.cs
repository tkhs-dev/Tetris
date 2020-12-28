using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
using TetrisAI.Source;
using TetrisCore.Source;
using TetrisCore.Source.Api;
using static TetrisAI.Source.Evaluator;

namespace TetrisPlayerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TetrisGame Game;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += initialized;
            App.GetLogger().Info("Initialization finished.");
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
            Game = new TetrisGame(App.GetLogger());
            foreach (Object c in Grid.Children)
            {
                if (c is IRenderer)
                {
                    App.GetLogger().Info("Find a Renderer:" + c.GetType().Name);
                    Game.SetRenderer((IRenderer)c);
                    Game.SetController((IController)new AITetrisController(evaluator, 100));
                    //Game.SetController((IController)new ReplayController(playdata.Events));
                }
            }
            Game.SetRenderer(DXPanel);
            Game.SetController(DXPanel.Controller);
            Game.RecordPlayDataEnabled = false;
            Game.Start();
        }
    }
}
