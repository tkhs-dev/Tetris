using System;
using System.Collections.Generic;
using System.Text;
using TetrisCore.Source;
using TetrisCore.Source.Api;

namespace TetrisAI.Source
{
    public class AITetrisController : IController
    {
        TetrisGame Game;

        public void initialize(TetrisGame game)
        {
            Game = game;
        }
        public void InitController()
        {
        }

        public void OnFieldUpdate(Field field, BlockObject lastObject, Queue<BlockObject> queue)
        {
            throw new NotImplementedException();
        }

        public void OnTimerTick()
        {
        }
    }
}
