using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using TetrisAI.Source;
using TetrisCore.Source;
using TetrisCore.Source.Extension;
using TetrisCore.Source.Util;
using TetrisPlayerWPF.Source;
using TetrisPlayerWPF.Source.SettingElement;
using static TetrisAI.Source.Evaluator;

namespace TetrisPlayerWPF
{
    /// <summary>
    /// PlayPage.xaml の相互作用ロジック
    /// </summary>
    public partial class PlayPage : Page, IDisposable
    {
        private TetrisGame Game;
        private PlaySettingBase Setting;

        public PlayPage(PlaySettingBase setting)
        {
            InitializeComponent();
            this.Loaded += initialized;
            App.GetLogger().Info("Initialization finished.");
            Setting = setting;
        }

        public void Dispose()
        {
            DXPanel.Dispose();
            Game.Dispose();
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
            if(Setting is SinglePlaySetting)
            {
                SinglePlaySetting setting = Setting as SinglePlaySetting;
                Game = new TetrisGame(App.GetLogger());
                Game.SetRenderer(DXPanel);
                Game.SetController(DXPanel.Controller);
                Game.TimerSpan = setting.FallInterval.Value;
                Game.RecordPlayDataEnabled = setting.RecordPlayDataEnabled.Value;
                Game.Start();
            }else if(Setting is AIPlaySetting)
            {
                AIPlaySetting setting = Setting as AIPlaySetting;
                var p = EvaluationNNParameter.Load(typeof(EvaluationNNParameter), setting.AiTrainingFile.Value.DirectoryName, setting.AiTrainingFile.Value.Name) as EvaluationNNParameter;
                if (p != null) parameter = p;
                Evaluator evaluator = new Evaluator(parameter);
                Game = new TetrisGame(App.GetLogger());
                Game.SetRenderer(DXPanel);
                Game.SetController(new AITetrisController(evaluator,setting.AiControllInterval.Value));
                Game.TimerSpan = setting.FallInterval.Value;
                Game.RecordPlayDataEnabled = setting.RecordPlayDataEnabled.Value;
                Game.Start();
            }else if(Setting is RePlaySetting)
            {
                RePlaySetting setting = Setting as RePlaySetting;
                GamePlayData playdata = (GamePlayData)GamePlayData.Load(typeof(GamePlayData), setting.PlayDataFile.Value.DirectoryName, setting.PlayDataFile.Value.Name);
                Game = new TetrisGame(App.GetLogger(), 10, 20, playdata.ObjectPool, playdata.ObjectQueue);
                Game.SetRenderer(DXPanel);
                Game.SetController(new ReplayController(playdata.Events));
                Game.Start();
                
            }
            //Game = new TetrisGame(TetrisPlayer.GetLogger(),10,20,playdata.ObjectPool,playdata.ObjectQueue);
            
        }
    }
}