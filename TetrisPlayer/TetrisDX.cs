using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.Direct2D1;
using System.Threading;
using SharpDX.Direct3D;
using SharpDX;
using TetrisCore.Source.Api;
using TetrisCore.Source;
using log4net;
using SharpDX.Mathematics.Interop;
using SharpDX.DirectInput;
using System.Runtime.InteropServices;

namespace TetrisPlayer
{
    public partial class TetrisDX : TetrisDXBase,IDisposable,IRenderer,IController
    {
        public TetrisDX()
        {
            // デザイナ設定反映
            InitializeComponent();
            this.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
        }
        public void InitRender() { }
        public void InitController()
        {
            //タイマーON
            game.TimerEnabled = true;
        }
        public void OnFieldUpdate(Field field,BlockObject lastObject,Queue<BlockObject> queue)
        {
        }
        public void OnTimerTick()
        {
            game.Move(BlockObject.Directions.SOUTH);
        }
        public override void MainLoop()
        {
            InputCheck();
            RenderTarget2D?.BeginDraw();
            // 画面を特定の色(例．灰色)で初期化
            RenderTarget2D?.Clear(SharpDX.Color.LightGray);

            // 図形描画位置
            if (RenderTarget2D != null && Initialized && field != null)
            {
                size = this.Height / Column - 1;
                RenderFlame(Row, Column, 10, 10);
                RenderBlocks(10, 10);
                if (field.Object != null) RenderObject(field.Object, field.ObjectPoint, 10, 10);
                RenderNextObject(size * Row + 50, 10);
            }

            RenderTarget2D?.EndDraw();
            _SwapChain?.Present(0, PresentFlags.None);
        }
        private void InputCheck()
        {
            if (!this.Focused || _keyboard == null) return;

            _keyboard.Acquire();
            _keyboard.Poll();

            // デバイスからデータを取得する
            var state = _keyboard.GetBufferedData();

            if (state == null) { return; }
            // 押されたキーデータを抽出
            var isPressedKeys = state.Where(n => n.IsPressed);
            // 押されたキーデータが合った場合
            foreach (var key in isPressedKeys)
            {
                switch (key.Key)
                {
                    case Key.Left:
                        game.Move(BlockObject.Directions.WEST);
                        break;
                    case Key.Right:
                        game.Move(BlockObject.Directions.EAST);
                        break;
                    case Key.Up:
                        game.Rotate();
                        break;
                    case Key.Return:
                    case Key.NumberPadEnter:
                        game.Place();
                        break;
                }
            }
        }
    }
}
