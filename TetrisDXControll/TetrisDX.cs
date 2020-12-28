using SharpDX.DirectInput;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TetrisAI.Source;
using TetrisCore;
using TetrisCore.Source;
using TetrisCore.Source.Api;
using TetrisCore.Source.Util;
using static TetrisAI.Source.Evaluator;
using static TetrisCore.Source.BlockObject;

namespace TetrisDXControll
{
    public partial class TetrisDX : TetrisDXBase, IDisposable
    {
        public DXPlayerController Controller;
        public TetrisDX()
        {
            // デザイナ設定反映
            InitializeComponent();
            this.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);

            //初期化
            Controller = new DXPlayerController();
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
                if (field.Object != null) RenderObject(field.Object, 10, 10);
                RenderNextObject(size * Row + 50, 10);
                RenderGameState(size*Row+50,10);
                RenderObjectAxis(10, 10);
                RenderPlaceablePosition(10, 10);
                RenderHoles(10, 10);
                RenderDeadSpace(10,10);
                RenderWells(10, 10);
            }

            RenderTarget2D?.EndDraw();
            _SwapChain?.Present(0, PresentFlags.None);
        }

        private void RenderHoles(int x, int y)
        {
            foreach (System.Drawing.Point p in field.GetHoles())
            {
                RenderDiagonalWires(SharpDX.Color.Red, p, x, y,false);
            }
        }
        private void RenderDeadSpace(int x,int y)
        {
            foreach (System.Drawing.Point p in field.GetDeadSpace())
            {
                RenderDiagonalWires(SharpDX.Color.HotPink, p, x, y, true);
            }
        }

        private void RenderWells(int x, int y)
        {
            foreach (List<System.Drawing.Point> pl in field.GetWells())
            {
                foreach (System.Drawing.Point p in pl) RenderDiagonalWires(SharpDX.Color.Green, p, x, y);
            }
        }

        private void RenderObjectAxis(int x, int y)
        {
            RenderDiagonalWires(SharpDX.Color.Cyan, field.Object.Point, x, y);
        }

        private void RenderPlaceablePosition(int x, int y)
        {
            List<BlockPosition> point = field.GetPlaceablePositions(field.Object.Unit);
            foreach (var p in point.Where(x=>x.Direction.Equals(field.Object.Direction)).Select(x=>x.Point))
            {
                RenderDiagonalWires(SharpDX.Color.Orange, p, x, y);
            }
        }

        public override void Render()
        {
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
                        Controller.Move(BlockUnit.Directions.WEST);
                        break;

                    case Key.Right:
                        Controller.Move(BlockUnit.Directions.EAST);
                        break;

                    case Key.Up:
                        Controller.Rotate(true);
                        break;

                    case Key.Down:
                        Controller.Rotate(false);
                        break;

                    case Key.Return:
                    case Key.NumberPadEnter:
                        Controller.Place();
                        break;
                }
            }
        }
    }
}