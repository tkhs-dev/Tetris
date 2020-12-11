using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisCore.Source.Api
{
    public abstract class ControllerBase : IController
    {
        public abstract void InitController(Field field);
        public abstract void initialize(TetrisGame game);
        public abstract void OnTimerTick();

        //コントロール
        public bool Move(BlockUnit.Directions direction)
        {
            return field.Move(direction);
        }

        public void Place()
        {
            field.PlaceImmediately();
        }

        public bool Rotate(bool clockwise)
        {
            return field.Rotate(clockwise ? 1 : -1);
        }
    }
}
