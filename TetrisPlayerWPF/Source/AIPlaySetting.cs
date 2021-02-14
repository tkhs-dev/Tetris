using TetrisPlayerWPF.Source.SettingElement;

namespace TetrisPlayerWPF.Source
{
    public class AIPlaySetting : PlaySettingBase
    {
        [Element("AIの操作間隔", Tips = "AIが移動などの操作を行う間隔", Unit = "ms")]
        public IntSettingElement AiControllInterval { get; } = new IntSettingElement(100, 0, 1000);

        [Element("学習データ", Tips = "AIの学習済みデータファイル")]
        public FileSettingElement AiTrainingFile { get; } = new FileSettingElement(null, "学習ファイル", "nnprm");
    }
}
