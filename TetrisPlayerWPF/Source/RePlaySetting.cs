using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisPlayerWPF.Source.SettingElement;

namespace TetrisPlayerWPF.Source
{
    public class RePlaySetting:PlaySettingBase
    {
        public new BoolSettingElement RecordPlayDataEnabled { get; }
        public new IntSettingElement FallInterval { get; }

        [Element("リプレイデータ",Tips ="リプレイデータファイル")]
        public FileSettingElement PlayDataFile { get; } = new FileSettingElement(null,"リプレイデータファイル","rpldt");
    }
}
