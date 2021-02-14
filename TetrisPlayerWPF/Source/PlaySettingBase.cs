using TetrisPlayerWPF.Source.SettingElement;

namespace TetrisPlayerWPF.Source
{
    public abstract class PlaySettingBase
    {
        [Element("落下間隔", Unit = "ms", Tips = "オブジェクトが落下する間隔")]
        public IntSettingElement FallInterval { get; } = new IntSettingElement(700, 0, 2000);

        [Element("リプレイを保存")]
        public BoolSettingElement RecordPlayDataEnabled { get; } = new BoolSettingElement(false);
    }
}
