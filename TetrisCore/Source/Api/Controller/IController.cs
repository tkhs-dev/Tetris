namespace TetrisCore.Source.Api
{
    public interface IController : ITetrisGame
    {
        void InitController(Field field);

        void OnTimerTick();
    }
}