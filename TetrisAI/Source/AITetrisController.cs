using System;
using System.Collections.Generic;
using System.Text;
using TetrisAI.Source.util;
using TetrisCore.Source;
using TetrisCore.Source.Api;

namespace TetrisAI.Source
{
    public class AITetrisController : IController
    {
        TetrisGame Game;
        Field field;



        public void initialize(TetrisGame game)
        {
            Game = game;
        }
        public void InitController(Field field)
        {
            this.field = field;
            field.OnRoundEnd += (object sender) =>
            {

            };
        }

        public void OnFieldUpdate(Field field, BlockObject lastObject, Queue<BlockObject> queue)
        {
            this.field = field;
            EvaluationItem ev = new EvaluationItem(lastObject.GetHeight(),field.GetHoles().Count,0,0,0,0,0);
        }

        public void OnTimerTick()
        {
        }
    }
}
