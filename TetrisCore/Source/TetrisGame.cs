using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using TetrisCore.Source.Api;
using TetrisCore.Source.Extension;
using static TetrisCore.Source.BlockObject;

namespace TetrisCore.Source
{
    public class TetrisGame : IDisposable
    {
        internal ILog logger;

        private Field field;

        public static List<BlockObject> DefaultObjectPool;
        //使用されるオブジェクトの一覧
        public List<BlockObject> ObjectPool;

        //キュー
        private Queue<BlockObject> _objectQueue;
        public Queue<BlockObject> ObjectQueue => _objectQueue;

        private Timer timer;
        public bool TimerEnabled { get; set; }

        public readonly int ROW;
        public readonly int COLUMN;

        private IRenderer renderer;
        private IController controller;

        static TetrisGame()
        {
            DefaultObjectPool = new List<BlockObject>(Enum.GetValues(typeof(Kind)).Cast<Kind>().Select(x=>x.GetObject()).ToList());
        }
        public TetrisGame(ILog logger,int row = 10,int column = 20)
        {
            this.logger = logger;
            logger.Info($"TetrisInstance Creating : row{row},column{column}");


            ObjectPool = TetrisGame.DefaultObjectPool;
            _objectQueue = new Queue<BlockObject>(ObjectPool.OrderBy(x => Guid.NewGuid()).Take(2));

            timer = new Timer();
            timer.Interval = 300;
            timer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) => controller?.OnTimerTick());

            ROW = row;
            COLUMN = column;

            field = new Field(row, column);


            field.OnBlockChanged += (object sender,Point point)=>
            {
                logger.Debug($"Block was changed:{point}");
                Draw();
            };
            field.OnBlockPlaced += (object sender, BlockObject obj) => 
            {
                logger.Debug("Block was placed");
                controller.OnFieldUpdate(field,obj,_objectQueue);
                field.SetObject((BlockObject)_objectQueue.Dequeue().Clone());
                timer.Stop();
                timer.Start();
                _objectQueue.Enqueue(ObjectPool.GetRandom());
            };
        }
        public void SetRenderer(IRenderer renderer)
        {
            this.renderer = renderer;
            renderer.initialize(this);
            renderer.InitRender();
        }
        public void SetController(IController controller)
        {
            this.controller = controller;
            controller.initialize(this);
            controller.InitController();
        }
        public void Start()
        {
            field.SetObject((BlockObject)ObjectPool.GetRandom().Clone());
            if (TimerEnabled) timer.Start();
            Draw();
        }

        //操作
        public bool Move(BlockObject.Directions direction)
        {
            return field.Move(direction);
        }
        public void Place()
        {
            field.PlaceImmediately();
        }
        public bool Rotate()
        {
            return field.Rotate(1);
        }
        private void Draw()
        {
            renderer.Render(field);
        }

        public void Dispose()
        {
            logger.Debug("Dispose");
            ((IDisposable)timer).Dispose();
        }
    }
}
