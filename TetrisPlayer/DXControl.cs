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
using SharpDX.Direct3D;
using System.Threading;
using SharpDX.Direct2D1;

namespace TetrisPlayer
{
    public partial class DXControl : UserControl,IDisposable
    {
        public DXControl()
        {
            InitializeComponent();

            // デザイナ設定反映
            InitializeComponent();

            // スタイルの指定
            SetStyle(ControlStyles.AllPaintingInWmPaint |// ちらつき抑える
                ControlStyles.Opaque, true);　           // 背景は描画しない

            //Debug
            TetrisPlayer.GetLogger().Info("Loading DXControl.");
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
        #endregion

        /// 
        /// 表示対象ハンドル
        /// 
        protected IntPtr DisplayHandle { get { return Handle; } }

        /// 
        /// 毎フレーム処理
        /// 
        public void Exec()
        {
            Initialize();

            // フォームの生成
            Show();
            // フォームが作成されている間は、ループし続ける
            while (Created)
            {
                MainLoop();

                // イベントがある場合は処理する
                Application.DoEvents();

                // CPUがフル稼働しないようにFPSの制限をかける
                // ※簡易的に、おおよそ秒間60フレーム程度に制限
                Thread.Sleep(16);
            }
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
            SharpDX.DXGI.Factory factory = _SwapChain.GetParent< SharpDX.DXGI.Factory>();
            factory.MakeWindowAssociation(DisplayHandle, WindowAssociationFlags.IgnoreAll);

            // バックバッファーを保持する
            _BackBuffer = Texture2D.FromSwapChain<Texture2D>(_SwapChain, 0);

            // 2D用の初期化を行う
            InitializeDirect2D();
        }

        #region DirectXデバイス基本初期設定
        /// 
        /// Direct2D 関連の初期化
        /// 
        public void InitializeDirect2D()
        {
            // Direct2Dリソースを作成
            _Factory2D = new SharpDX.Direct2D1.Factory();
            using (var surface = _BackBuffer.QueryInterface<SharpDX.DXGI.Surface>())
            {
                _RenderTarget2D = new RenderTarget(_Factory2D, surface, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));
            }
            // 非テキストプリミティブのエッジのレンダリング方法を指定
            _RenderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;
            // テキストの描画に使用されるアンチエイリアスモードについて指定
            _RenderTarget2D.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Cleartype;
        }
        #endregion

        /// 
        /// メインループ処理
        /// 
        public void MainLoop()
        {
            // 毎フレーム行う処理はここに記載する
        }

        /// 
        /// 解放処理
        /// 
        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
