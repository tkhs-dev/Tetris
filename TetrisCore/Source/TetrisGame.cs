using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using TetrisCore.Source.Api;

namespace TetrisCore.Source
{
    public class TetrisGame
    {
        internal ILog logger;

        private Field field;

        public readonly int ROW;
        public readonly int COLUMN;

        private IRenderer renderer;
        private IController controller;

        public TetrisGame(ILog logger,int row = 10,int column = 20)
        {
            this.logger = logger;
            logger.Info("TetrisInstance Creating");
            ROW = row;
            COLUMN = column;
            field = new Field(row, column);
            field.OnBlockChanged += Draw;
        }
        public void SetRenderer(IRenderer renderer)
        {
            this.renderer = renderer;
            renderer.initialize(this);
        }
        public void SetController(IController controller)
        {
            this.controller = controller;
        }
        private void Draw(Object sender,Field field)
        {
            renderer.Render(field);
        }
    }
}
