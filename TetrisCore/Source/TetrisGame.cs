using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TetrisCore.Source.Api;
using TetrisCore.Source.Extension;
using static TetrisCore.Source.BlockUnit;

namespace TetrisCore.Source
{
    public class TetrisGame : IDisposable
    {
        internal ILog logger;

        private Field field;

        public static List<BlockUnit> DefaultObjectPool;

        //使用されるオブジェクトの一覧
        public List<BlockUnit> ObjectPool;

        //キュー
        private Queue<BlockUnit> _objectQueue;

        public Queue<BlockUnit> ObjectQueue => _objectQueue;

        public bool TimerEnabled { get; set; }
        private Timer timer;

        public int MaxRound { get; set; }

        private IRenderer renderer;
        private IController controller;

        //Setting
        public readonly GameSetting Setting;

        //State
        private GameState _state;

        public GameState State => _state;

        public delegate void OnGameEndEvent(object sender);
        public event OnGameEndEvent OnGameEnd;

        static TetrisGame()
        {
            DefaultObjectPool = new List<BlockUnit>(Enum.GetValues(typeof(Kind)).Cast<Kind>().Select(x => x.GetObject()).ToList());
        }

        public TetrisGame(ILog logger, int row = 10, int column = 20)
        {
            this.logger = logger;
            logger.Debug($"TetrisInstance Creating : row{row},column{column}");

            ObjectPool = TetrisGame.DefaultObjectPool;
            _objectQueue = new Queue<BlockUnit>(ObjectPool.OrderBy(x => Guid.NewGuid()).Take(2));

            timer = new Timer();
            timer.Interval = 700;
            timer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) => controller?.OnTimerTick());

            Setting = new GameSetting(row, column);

            field = new Field(row, column);

            _state = new GameState() { Round = 0, Score = 0, RemovedLines = 0 };

            field.OnBlockChanged += (object sender, Point point) =>
            {
                //logger.Debug($"Block was changed:{point}");
            };
            field.OnRoundStart += (object sender) =>
            {
                lock (_objectQueue)
                {
                    _objectQueue.Enqueue(ObjectPool.GetRandom());
                    field.SetObject(_objectQueue.Dequeue());
                }
            };
            field.OnBlockPlaced += (object sender, BlockObject obj) =>
            {
                //logger.Debug("Block was placed");
                Draw();
            };
            field.OnLinesRemoved += (object sender, int[] lines, int eroded) =>
            {
                _state.RemovedLines += lines.Length;
            };
            field.OnRoundEnd += (object sender, RoundResult result) =>
             {
                 logger.Debug($"Round {_state.Round} End");
                 _state.Round++;
                 _state.Score += result.Score;
                 if (MaxRound > 0 && _state.Round >= MaxRound)
                 {
                     timer.Stop();
                     timer.Dispose();
                     OnGameEnd?.Invoke(this);
                     return;
                 }
                     field.StartRound();
             };
            field.OnGameOver += (object sender) =>
            {
                logger.Debug("Game Over");
                timer.Stop();
                timer.Dispose();
                OnGameEnd?.Invoke(this);
            };
            OnGameEnd += (object sender) =>
            {
                logger.Debug(State.Score);
            };
        }

        public void SetRenderer(IRenderer renderer)
        {
            this.renderer = renderer;
            renderer.initialize(this);
            renderer.InitRender(field);
        }

        public void SetController(IController controller)
        {
            this.controller = controller;
            controller.initialize(this);
            controller.InitController(field);
        }

        public void Start()
        {
            field.SetObject(ObjectPool.GetRandom());
            field.StartRound();
            if (TimerEnabled) timer.Start();
            Draw();
        }

        public Task<GameResult> WhenGameEnd()
        {
            var tcs = new TaskCompletionSource<GameResult>();
            OnGameEnd += (object sender) =>
            {
                tcs.TrySetResult(new GameResult() { Score = State.Score ,Round = State.Round});
            };
            return tcs.Task;
        }

        //操作
        public bool Move(BlockUnit.Directions direction)
        {
            return field.Move(direction);
        }

        public void Place()
        {
            field.PlaceImmediately();
        }

        public bool Rotate(bool clockwise)
        {
            return field.Rotate(clockwise ? 1 : -1);
        }

        private void Draw()
        {
            renderer?.Render(field);
        }

        public void Dispose()
        {
            logger.Debug("Dispose");
            ((IDisposable)timer).Dispose();
        }

        public class GameState
        {
            public int Score { get; set; }
            public int Round { get; set; }
            public int RemovedLines { get; set; }
        }

        public class GameSetting
        {
            public readonly int ROW;
            public readonly int COLUMN;

            public GameSetting(int row, int column)
            {
                ROW = row;
                COLUMN = column;
            }
        }
    }
}