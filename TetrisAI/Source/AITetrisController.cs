using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TetrisAI.Source.util;
using TetrisCore;
using TetrisCore.Source;
using TetrisCore.Source.Api;

namespace TetrisAI.Source
{
    public class AITetrisController : IController
    {
        TetrisGame Game;
        Field field;

        int erodedObjectCells;

        public void initialize(TetrisGame game)
        {
            Game = game;
            erodedObjectCells = 0;
        }
        public void InitController(Field field)
        {
            this.field = field;
            field.OnRoundEnd += (object sender, RoundResult result) =>
            {

            };
        }

        public void OnRoundEnd(RoundResult result)
        {
            EvaluationItem ev = new EvaluationItem(result.Object.GetHeight(),result.FieldAtEnd.GetHoles().Count,result.FieldAtEnd.GetWells().Select(x=>x.ToArray()).ToArray().Length,erodedObjectCells,0,0,0);
        }

        public void OnTimerTick()
        {
        }
    }
}
