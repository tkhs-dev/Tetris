using log4net;
using TetrisAI.Source;
using TetrisCore.Source;
using static TetrisAI.Source.Evaluator;

namespace TetrisAI_Trainer.Source
{
    public class Trainer
    {
        private ILog logger;

        private TetrisGame game;
        private AITetrisController controller;

        public Trainer()
        {
            logger = TetrisAITrainer.Logger;
            game = new TetrisGame(logger);
            controller = new AITetrisController(new Evaluator(EvaluationNNParameter.CreateNew()),2);

            game.SetController(controller);
        }

        public void Start()
        {
            game.Start();
            game.WhenGameEnd().Wait();
        }
    }
}