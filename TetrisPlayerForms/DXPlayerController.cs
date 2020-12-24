using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisCore.Source;
using TetrisCore.Source.Api;

namespace TetrisPlayer
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
