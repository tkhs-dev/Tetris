namespace TetrisCore.Source.Api
{
    public interface IRenderer : ITetrisGame
    {
        void InitRender(Field field);

        void Render(Field field);
    }
}