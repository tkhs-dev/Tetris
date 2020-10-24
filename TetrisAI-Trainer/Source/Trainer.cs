using log4net;
using TetrisAI.Source;
using TetrisCore.Source;

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
            controller = new AITetrisController();

            game.SetController(controller);
        }

        public void Start()
        {
            game.Start();
        }
    }
}