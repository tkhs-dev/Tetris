namespace TetrisDXControll.Source.Util
{
    public class ColorConverter
    {
        public static SharpDX.Color GetDXColor(System.Drawing.Color color)
        {
            return new SharpDX.Color(color.ToArgb());
        }
    }
}