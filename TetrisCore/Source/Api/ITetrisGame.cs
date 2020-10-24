using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisCore.Source.Api
{
    public interface ITetrisGame
    {
        void ITetrisGame(ILog logger);
        void initialize(TetrisGame game);
    }
}
