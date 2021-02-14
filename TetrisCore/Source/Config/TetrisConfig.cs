namespace TetrisCore.Source.Config
{
    public class TetrisConfig : ConfigBase
    {
        public bool UseCustomObjectList { get; set; }
        public string ObjectListFile { get; set; }
        public TetrisConfig() : base("TetrisConfig")
        {

        }
        public override ConfigBase GetDefault()
        {
            return new TetrisConfig() { UseCustomObjectList = false, ObjectListFile = "ObjectList" };
        }
    }
}
