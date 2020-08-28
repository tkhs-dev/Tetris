﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisCore.Source.Api
{
    public interface IRenderer : ITetrisGame
    {
        void InitRender();
        void Render(Field field);
    }
}
