namespace TetrisPlayerWPF.Source.SettingElement
{
    public class IntSettingElement : SettingElementBase<int>
    {
        public int? MinValue { get => _min_value; }
        private int? _min_value;
        public int? MaxValue { get => _max_value; }
        private int? _max_value;
        public IntSettingElement(int value, int? min_value = null, int? max_value = null) : base(value)
        {
            _min_value = min_value;
            _max_value = max_value;
        }
    }
}
