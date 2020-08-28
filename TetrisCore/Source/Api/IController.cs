using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisCore.Source.Api
{
    public interface IController : ITetrisGame
    {
        void InitController();
        void OnFieldUpdate(Field field,BlockObject lastObject,Queue<BlockObject> queue);
        void OnTimerTick();
    }
}
