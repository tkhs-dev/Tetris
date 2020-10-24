using log4net;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TetrisAI.Source.util;
using TetrisCore;
using TetrisCore.Source;
using TetrisCore.Source.Api;

namespace TetrisAI.Source
{
    public class AITetrisController : IController
    {
        private ILog logger;
        private TetrisGame Game;
        private Field field;

        private int erodedObjectCells;

        public AITetrisController()
        {
            logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void initialize(TetrisGame game)
        {
            Game = game;
            erodedObjectCells = 0;
        }

        public void InitController(Field field)
        {
            this.field = field;
            field.OnRoundEnd += OnRoundEnd;
            field.OnRoundStart += OnRoundStart;
        }

        public void OnRoundStart(object sender)
        {
            Field field = (Field)sender;
            this.logger.Debug("Round Start");
        }

        public void OnRoundEnd(object sender, RoundResult result)
        {
            Field field = result.FieldAtEnd;
            List<Point> holes = field.GetHoles();
            logger.Debug("Round End");
            Enumerable.Range(0, Game.ROW - 1);
            EvaluationItem ev = new EvaluationItem(result.Object.GetHeight(), holes.Count, result.FieldAtEnd.GetWells().Select(x => x.ToArray()).ToArray().Length, erodedObjectCells, 0, 0, 0);
        }

        public void OnTimerTick()
        {
        }
    }
}