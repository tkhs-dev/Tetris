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
    public partial class TetrisDX : UserControl,IDisposable,IRenderer,IController
    {
        private bool Initialized;

        private int Row;
        private int Column;

        private int size;

        private TetrisGame game;
        private Field field;
        public TetrisDX()
        {
            // デザイナ設定反映
            InitializeComponent();
            this.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);

            // スタイルの指定
            SetStyle(ControlStyles.AllPaintingInWmPaint |// ちらつき抑える
                ControlStyles.Opaque, true);            // 背景は描画しない

            System.Windows.Media.CompositionTarget.Rendering += RenderingEvent;
        }
        //--------------------------------------------------------------//
        //                         DirectX設定                          //
        //--------------------------------------------------------------//
        /// 
        /// Direct3Dのデバイス
        /// 
        public SharpDX.Direct3D11.Device Device { get { return _device; } }
        private SharpDX.Direct3D11.Device _device = null;

        /// 
        /// スワップチェーン
        /// ※デバイスが描いた画像をウィンドウに表示する機能
        /// 
        SwapChain _SwapChain;
        Texture2D _BackBuffer;

        private Keyboard _keyboard;

        #region Direct2D関連
        /// 
        /// レンダーターゲット2D
        /// 
        public RenderTarget RenderTarget2D { get { return _RenderTarget2D; } }
        private RenderTarget _RenderTarget2D;
        /// 
        /// Direct2Dで描画用のファクトリーオブジェクト
        /// 
        private SharpDX.Direct2D1.Factory _Factory2D;

        /// 
        /// DirectWriteで描画用のファクトリーオブジェクト
        /// 
        private SharpDX.DirectWrite.Factory _FactoryDWrite;
        /// 
        /// 描画ブラシ
        /// 
        private SolidColorBrush _ColorBrush;
        #endregion

        /// 
        /// 表示対象ハンドル
        /// 
        protected IntPtr DisplayHandle { get { return Handle; } }

        void RenderingEvent(object sender, EventArgs e)
        {
            MainLoop();
        }
        /// 
        /// 毎フレーム処理
        /// 
        public void Exec()
        {
            Initialize();
            // フォームの生成
            Show();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Exec();
        }


        /// 
        /// DirectXデバイスの初期化
        /// 
        public void Initialize()
        {
            // スワップチェーン設定
            var desc = new SwapChainDescription()
            {
                // バッファ数
                // ※ダブルバッファリングを行う場合は2を指定
                BufferCount = 1,
                // 描画情報
                ModeDescription = new ModeDescription(ClientSize.Width, ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                // ウィンドウモードの有効・無効
                IsWindowed = true,
                // 描画対象ハンドル
                OutputHandle = DisplayHandle,
                // マルチサンプル方法の指定
                SampleDescription = new SampleDescription(1, 0),
                // 描画後の表示バッファの扱い方法の指定
                SwapEffect = SwapEffect.Discard,
                // 描画画像の使用方法
                Usage = Usage.RenderTargetOutput
            };

            // デバイスとスワップチェーンを生成
            SharpDX.Direct3D11.Device.CreateWithSwapChain(
                // デバイスの種類
                DriverType.Hardware,
                // ランタイムレイヤーの有効にするリスト
                DeviceCreationFlags.BgraSupport,
                // フィーチャーレベル
                // ※ある程度のハードウェアのレベルを規定して，それぞれのレベルにあわせたプログラムを書ける仕組み
                // ※DirectX の世代を指定
                new[] { SharpDX.Direct3D.FeatureLevel.Level_11_0 },
                // スワップチェーン設定
                desc,
                // 生成した変数を返す
                out _device, out _SwapChain);

            // Windowsの不要なイベントを無効にする
            var factory = _SwapChain.GetParent<SharpDX.DXGI.Factory>();
            factory.MakeWindowAssociation(DisplayHandle, WindowAssociationFlags.IgnoreAll);

            // バックバッファーを保持する
            _BackBuffer = Texture2D.FromSwapChain<Texture2D>(_SwapChain, 0);

            // 2D用の初期化を行う
            InitializeDirect2D();

            //キーボードを初期化
            DirectInput dinput = new DirectInput();
            if (dinput != null)
            {
                _keyboard = new Keyboard(dinput);
                if (_keyboard != null)
                {
                    // バッファサイズを指定
                    _keyboard.Properties.BufferSize = 128;
                }
            }
        }

        #region DirectXデバイス基本初期設定
        /// 
        /// Direct2D 関連の初期化
        /// 
        public void InitializeDirect2D()
        {
            // Direct2Dリソースを作成
            _Factory2D = new SharpDX.Direct2D1.Factory();
            using (var surface = _BackBuffer.QueryInterface<Surface>())
            {
                _RenderTarget2D = new RenderTarget(_Factory2D, surface, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));
            }
            // 非テキストプリミティブのエッジのレンダリング方法を指定
            _RenderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;
            // テキストの描画に使用されるアンチエイリアスモードについて指定
            _RenderTarget2D.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Cleartype;


            // DirectWriteオブジェクトを生成するために必要なファクトリオブジェクトを生成
            _FactoryDWrite = new SharpDX.DirectWrite.Factory();

            // ブラシを生成
            _ColorBrush = new SolidColorBrush(_RenderTarget2D, SharpDX.Color.Red);
            // RGBAで色を指定する場合は下記のように行う
            //_ColorBrush = new SolidColorBrush(_RenderTarget2D, new SharpDX.Color(255, 255, 255, 255));
        }
        #endregion

        /// 
        /// メインループ処理
        /// 
        public void MainLoop()
        {
            //入力処理
            InputCheck();

            _RenderTarget2D?.BeginDraw();
            // 画面を特定の色(例．灰色)で初期化
            _RenderTarget2D?.Clear(SharpDX.Color.LightGray);

            // 図形描画位置
            if (_RenderTarget2D != null && Initialized && field!=null)
            {
                size = this.Height / Column - 1;
                RenderFlame(Row,Column,10,10);
                RenderBlocks(10,10);
                if(field.Object!=null) RenderObject(field.Object, field.ObjectPoint, 10,10);
                RenderNextObject(size*Row+50,10);
            }

            _RenderTarget2D?.EndDraw();
            _SwapChain?.Present(0, PresentFlags.None);
        }
        delegate bool FocusedCheck();
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
        private void RenderFlame(int row,int column,int x,int y)
        {
            for (int i = 0; i < row; i++)
            {
                for (int i2 = 0; i2 < column; i2++)
                {
                    _ColorBrush.Color = SharpDX.Color.Gray;
                    _RenderTarget2D.DrawRectangle(new SharpDX.Mathematics.Interop.RawRectangleF(x+size * i, y+size * i2, x+size * i + size, y+size * i2 + size), _ColorBrush);
                }
            }
        }
        private void RenderBlocks(int x,int y)
        {
            for (int i = 0; i < Row; i++)
            {
                for (int i2 = 0; i2 < Column; i2++)
                {
                    Cell cell = field.GetCell(new System.Drawing.Point(i, i2));
                    if (cell.HasBlock())
                    {
                        RenderBlock(cell.Block,new System.Drawing.Point(i,i2),x,y);
                    }
                }
            }
        }
        private void RenderBlock(Block block, System.Drawing.Point point,int x, int y)
        {
            _ColorBrush.Color = Source.Util.ColorConverter.GetDXColor(block.Color);
            _RenderTarget2D.FillRectangle(new SharpDX.Mathematics.Interop.RawRectangleF(x+size * point.X + 1, y+size * point.Y + 1, x+size * point.X + size - 1, y+size * point.Y + size - 1), _ColorBrush);
        }
        private void RenderObject(BlockObject obj, System.Drawing.Point point, int x, int y)
        {
            foreach(Block b in obj.GetBlocks(point))
            {
                RenderBlock(b,b.Point,x,y);
            }
        }
        private void RenderNextObject(int x, int y)
        {
            RenderFlame(4,4,x,y);
            RenderObject(game.ObjectQueue.ToArray()[0], System.Drawing.Point.Empty, x, y);
            RenderFlame(4, 4, x, y*2+size*4);
            RenderObject(game.ObjectQueue.ToArray()[1], System.Drawing.Point.Empty, x, y*2+size*4);
        }

        /// 
        /// 解放処理
        /// 
        public new void Dispose()
        {
            game.Dispose();
            base.Dispose();
        }

        public void Render(Field field)
        {
            this.field = field;
        }

        public void initialize(TetrisGame game)
        {
            this.game = game;

            //初期化処理
            Row = game.ROW;
            Column = game.COLUMN;

            //タイマーON
            game.TimerEnabled = true;

            Initialized = true;
        }

        public void OnTimerTick()
        {
            game.Move(BlockObject.Directions.SOUTH);
        }
    }
}
