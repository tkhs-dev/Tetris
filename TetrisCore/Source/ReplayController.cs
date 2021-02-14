using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TetrisCore.Source.Api;
using static TetrisCore.Source.BlockUnit;
using static TetrisCore.Source.GamePlayData.GamePlayEvent.EventType;

namespace TetrisCore.Source
{
    public class ReplayController : ControllerBase
    {
        private IReadOnlyList<GamePlayData.GamePlayEvent> _events;

        public ReplayController(IReadOnlyList<GamePlayData.GamePlayEvent> events)
        {
            _events = events;
        }

        public override void initialize(TetrisGame game)
        {
            base.initialize(game);
            game.RecordPlayDataEnabled = false;
        }
        public override void InitController(Field field)
        {
            base.InitController(field);
            field.OnRoundStart += OnRoundStart;
        }
        public async void OnRoundStart(object sender)
        {
            await Task.Run(() =>
            {
                TimeSpan lastTime = TimeSpan.Zero;
                foreach (GamePlayData.GamePlayEvent ev in _events.Where(x => x.Round == _game.State.Round).OrderBy(x => x.Time))
                {
                    TimeSpan span = ev.Time - lastTime;
                    lastTime = ev.Time;
                    double interval = span.TotalMilliseconds;
                    Task.Delay(span).Wait();
                    DoEvent(ev);
                }
            });
        }
        public void DoEvent(GamePlayData.GamePlayEvent ev)
        {
            switch (ev.Event)
            {
                case PLACE:
                    Place();
                    break;
                case MOVE:
                    Move((Directions)ev.Arg.Object);
                    break;
                case ROTATE:
                    Rotate((bool)ev.Arg.Object);
                    break;
            }
        }
    }
}