using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TetrisCore.Source.Api;
using TetrisCore.Source.Config;
using TetrisCore.Source.Extension;
using TetrisCore.Source.Util;
using static TetrisCore.Source.BlockUnit;

namespace TetrisCore.Source
{
    public class TetrisGame : IDisposable
    {
        internal ILog logger;

        private Field field;

        public static WeightedPool<BlockUnit> DefaultObjectPool;

        //使用されるオブジェクトの一覧
        public WeightedPool<BlockUnit> ObjectPool { get; private set; }

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
            DefaultObjectPool = new WeightedPool<BlockUnit>(Enum.GetValues(typeof(Kind)).Cast<Kind>().Select(x => new WeightedPool<BlockUnit>.WeightedItem(1, x.GetObject())).ToList());
        }

        public TetrisGame(ILog logger, int row = 10, int column = 20, WeightedPool<BlockUnit> objectPool = null, Queue<BlockUnit> initialQueue = null)
        {
            this.logger = logger;
            logger.Debug($"TetrisInstance Creating : row{row},column{column}");

            TetrisConfig config = new TetrisConfig();
            config.Load();
            if (!config.Load()) config.Save();
            if (objectPool == null && config.UseCustomObjectList)
            {
                var serializable = (SerializableObjectPool.Load(typeof(SerializableObjectPool), ConfigBase.Directory, config.ObjectListFile) as SerializableObjectPool);
                if (serializable == null)
                {
                    serializable = new SerializableObjectPool() { ObjectPool = DefaultObjectPool };
                    serializable.Save(ConfigBase.Directory, config.ObjectListFile);
                }
                objectPool = serializable.ObjectPool;
            }

            timer = new Timer();
            timer.Interval = TimerSpan;
            timer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) => controller?.OnTimerTick());

            Setting = new GameSetting() { Row = row, Column = column };

            field = new Field(row, column);

            _state = new GameState() { Round = 0, Score = 0, RemovedLines = new Dictionary<int, int>() };

            _gameWatch = new Stopwatch();
            _playData = new GamePlayData();

            ObjectPool = objectPool ?? TetrisGame.DefaultObjectPool;
            _objectQueue = initialQueue ?? new Queue<BlockUnit>();

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
                if (lines.Length != 0)
                {
                    if (!_state.RemovedLines.ContainsKey(lines.Length)) _state.RemovedLines.Add(lines.Length, 0);
                    _state.RemovedLines[lines.Length]++;
                }
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
                _playData.ObjectPool = ObjectPool;
            }
            if (_objectQueue.Count < 2) Enqueue(2);
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
            for (int i = 0; i < num; i++)
            {
                int index = ObjectPool.TakeIndex();
                if (RecordPlayDataEnabled) _playData.SerializableObjectQueue.Add(new GamePlayData.SerializableQueue() { ID = _playData.SerializableObjectQueue.Count, Value = index });
                BlockUnit unit = ObjectPool.Items[index];
                _objectQueue.Enqueue(unit);
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
            bool result;
            lock (field)
            {
                result = field.Move(direction);
            }
            if (RecordPlayDataEnabled && result) _playData.Events.Add(new GamePlayData.GamePlayEvent() { Round = this.State.Round, Time = _gameWatch.Elapsed, Event = GamePlayData.GamePlayEvent.EventType.MOVE, Arg = new GamePlayData.GamePlayEvent.Argument() { Object = direction } });
            return result;
        }

        public void Place()
        {
            if (RecordPlayDataEnabled) _playData.Events.Add(new GamePlayData.GamePlayEvent() { Round = this.State.Round, Time = _gameWatch.Elapsed, Event = GamePlayData.GamePlayEvent.EventType.PLACE, Arg = null });
            field.PlaceImmediately();
        }

        public bool Rotate(bool clockwise)
        {
            bool result = field.Rotate(clockwise ? 1 : -1);
            if (RecordPlayDataEnabled && result) _playData.Events.Add(new GamePlayData.GamePlayEvent() { Round = this.State.Round, Time = _gameWatch.Elapsed, Event = GamePlayData.GamePlayEvent.EventType.ROTATE, Arg = new GamePlayData.GamePlayEvent.Argument() { Object = clockwise } });
            return result;
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
            public Dictionary<int, int> RemovedLines { get; set; }
            public int RemovedLineCount { get => RemovedLines.Values.Sum(); }
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