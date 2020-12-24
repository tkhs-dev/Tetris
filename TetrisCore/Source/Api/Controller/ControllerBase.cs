namespace TetrisCore.Source.Api
{
    public abstract class ControllerBase : IController
    {
        protected TetrisGame _game;
        protected Field _field;

        public virtual void initialize(TetrisGame game)
        {
            _game = game;
        }

        public virtual void InitController(Field field)
        {
            _field = field;
        }
        public virtual void OnTimerTick()
        {
            _game.Move(BlockUnit.Directions.SOUTH);
        }

        //コントロール
        public bool Move(BlockUnit.Directions direction)
        {
            return _game.Move(direction);
        }

        public void Place()
        {
            _game.Place();
        }

        public bool Rotate(bool clockwise)
        {
            return _game.Rotate(clockwise);
        }
    }
}