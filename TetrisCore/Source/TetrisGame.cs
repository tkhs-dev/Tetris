using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static IReadOnlyList<BlockUnit> DefaultObjectPool;

        //使用されるオブジェクトの一覧
        public IReadOnlyList<BlockUnit> ObjectPool { get; private set; }

        //キュー
        private Queue<BlockUnit> _objectQueue;

        public Queue<BlockUnit> ObjectQueue => _objectQueue;

        public bool TimerEnabled { get; set; }
        public int TimerSpan { get; set; } = 700;
        private Timer timer;

        public int MaxRound { get; set; }

        private IRenderer renderer;
        private IController controller;

        //Setting
        public readonly GameSetting Setting;

        //State
        private GameState _state;

        public GameState State => _state;

        //ゲームのプレイデータ(リプレイなどに使用)
        public bool RecordPlayDataEnabled { get; set; }

        private GamePlayData _playData;

        private Stopwatch _gameWatch;

        //イベント
        public delegate void OnGameEndEvent(object sender, GameResult result);

        public event OnGameEndEvent OnGameEnd;

        static TetrisGame()
        {
            DefaultObjectPool = new List<BlockUnit>(Enum.GetValues(typeof(Kind)).Cast<Kind>().Select(x => x.GetObject()).ToList()).AsReadOnly();
        }
        public TetrisGame(ILog logger, int row=10, int column=20,IReadOnlyList<BlockUnit> objectPool=null,Queue<BlockUnit> initialQueue=null)
        {
            this.logger = logger;
            logger.Debug($"TetrisInstance Creating : row{row},column{column}");

            ObjectPool = objectPool ?? TetrisGame.DefaultObjectPool;
            _objectQueue = initialQueue ?? new Queue<BlockUnit>();
            if (_objectQueue.Count < 2) Enqueue(2);

            timer = new Timer();
            timer.Interval = TimerSpan;
            timer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) => controller?.OnTimerTick());

            Setting = new GameSetting() { Row = row, Column = column };

            field = new Field(row, column);

            _state = new GameState() { Round = 0, Score = 0, RemovedLines = 0 };

            _gameWatch = new Stopwatch();
            _playData = new GamePlayData();

            field.OnBlockChanged += (object sender, Point point) =>
            {
                //logger.Debug($"Block was changed:{point}");
            };
            field.OnRoundStart += (object sender) =>
            {
                if (RecordPlayDataEnabled) _gameWatch.Restart();
                lock (_objectQueue)
                {
                    field.SetObject(Dequeue());
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
                     OnGameEnd?.Invoke(this, new GameResult() { Score = State.Score, Round = State.Round });
                     return;
                 }
                 if (RecordPlayDataEnabled) _playData.Save();
                 field.StartRound();
             };
            field.OnGameOver += (object sender) =>
            {
                logger.Debug("Game Over");
                timer.Stop();
                OnGameEnd?.Invoke(this, new GameResult() { Score = State.Score, Round = State.Round });
            };
            OnGameEnd += (object sender, GameResult result) =>
            {
                logger.Debug(State.Score);
                _gameWatch.Stop();
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
            if (RecordPlayDataEnabled)
            {
                _playData.Date = DateTime.Now;
                _playData.Setting = Setting;
                _playData.ObjectPool = ObjectPool.Select((x,Index)=> { var u = GamePlayData.SerializableBlockUnit.FromBlockUnit(x); u.ID = Index;return u; }).ToList();
            }
            field.SetObject(Dequeue());
            field.StartRound();
            if (TimerEnabled)
            {
                timer.Interval = TimerSpan;
                timer.Start();
            }
            Draw();
        }

        public Task<GameResult> WhenGameEnd()
        {
            var tcs = new TaskCompletionSource<GameResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            OnGameEnd += (object sender, GameResult result) =>
            {
                tcs.TrySetResult(result);
            };
            return tcs.Task;
        }
        private void Enqueue(int num = 1)
        {
            for(int i = 0; i < num; i++)
            {
                Random random = new Random();
                int index = random.Next(0, ObjectPool.Count);
                BlockUnit unit = ObjectPool[index];
                _objectQueue.Enqueue(unit);
                if (RecordPlayDataEnabled) _playData.ObjectsQueue.Add(index);
            }
        }
        private BlockUnit Dequeue()
        {
            if (_objectQueue.Count <= 2) Enqueue();
            return _objectQueue.Dequeue();
        }

        //操作
        public bool Move(BlockUnit.Directions direction)
        {
            if (RecordPlayDataEnabled) _playData.Events.Add(new GamePlayData.GamePlayEvent() { Round=this.State.Round, Time = _gameWatch.Elapsed, Event = GamePlayData.GamePlayEvent.EventType.MOVE, Arg = new GamePlayData.GamePlayEvent.Argument() { Type = typeof(Directions), Object = direction } });
            return field.Move(direction);
        }

        public void Place()
        {
            if (RecordPlayDataEnabled) _playData.Events.Add(new GamePlayData.GamePlayEvent() { Round = this.State.Round, Time = _gameWatch.Elapsed, Event = GamePlayData.GamePlayEvent.EventType.PLACE, Arg = null });
            field.PlaceImmediately();
        }

        public bool Rotate(bool clockwise)
        {
            if (RecordPlayDataEnabled) _playData.Events.Add(new GamePlayData.GamePlayEvent() { Round = this.State.Round, Time = _gameWatch.Elapsed, Event = GamePlayData.GamePlayEvent.EventType.ROTATE, Arg = new GamePlayData.GamePlayEvent.Argument() { Type = typeof(bool), Object = clockwise } });
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
            _gameWatch.Stop();
        }

        public class GameState
        {
            public int Score { get; set; }
            public int Round { get; set; }
            public int RemovedLines { get; set; }
        }

        public class GameSetting
        {
            public int Row { get; set; }
            public int Column { get; set; }

            public GameSetting()
            {
            }
        }
    }
}