using TetrisCore.Source;
using TetrisCore.Source.Api;

namespace TetrisDXControll
{
    public class DXPlayerController : ControllerBase
    {
        public override void InitController(Field field)
        {
            base.InitController(field);

            //タイマーON
            _game.TimerEnabled = true;
        }
    }
}
