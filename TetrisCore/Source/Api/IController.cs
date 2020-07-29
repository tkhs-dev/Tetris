using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisCore.Source.Api
{
    public interface IController : ITetrisGame
    {
        void OnTimerTick();
    }
}
