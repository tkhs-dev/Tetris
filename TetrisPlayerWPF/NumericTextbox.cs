using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TetrisPlayerWPF
{
    public partial class NumericTextbox : TextBox
    {
        public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            nameof(Minimum),
            typeof(int),
            typeof(NumericTextbox),
            new UIPropertyMetadata(null)
        );

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            nameof(Maximum),
            typeof(int),
            typeof(NumericTextbox),
            new UIPropertyMetadata(null)
        );

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public NumericTextbox()
        {
            InitializeComponent();
        }

        public NumericTextbox(IContainer container)
        {
            container.Add((IComponent)this);
            InitializeComponent();
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            bool yes_parse = false;
            {
                {
                    int xx;
                    var tmp = Text + e.Text;
                    yes_parse = int.TryParse(tmp, out xx) && (Minimum <= xx && xx <= Maximum);
                }
            }
            e.Handled = !yes_parse;
        }
    }
}