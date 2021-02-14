using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using TetrisPlayerWPF.Source;
using TetrisPlayerWPF.Source.SettingElement;

namespace TetrisPlayerWPF
{
    /// <summary>
    /// PlaySettingPage.xaml の相互作用ロジック
    /// </summary>
    public partial class PlaySettingPage : Page, ITabablePage
    {
        private PlaySettingBase setting;
        private Type type { get => setting.GetType(); }
        public PlaySettingPage(PlayType playType)
        {
            InitializeComponent();
            switch (playType)
            {
                case PlayType.SINGLE:
                    setting = new SinglePlaySetting();
                    break;
                case PlayType.AI:
                    setting = new AIPlaySetting();
                    break;
                case PlayType.REPLAY:
                    setting = new RePlaySetting();
                    break;
            }
            this.Loaded += loaded;
        }
        protected void loaded(object sender, EventArgs args)
        {
            foreach (UIElement e in CreateUIElement()) Panel.Children.Add(e);
        }

        private List<UIElement> CreateUIElement()
        {
            List<UIElement> result = new List<UIElement>();
            foreach (var v in type.GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(ElementAttribute))))
            {
                StackPanel element = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 10, 10, 10) };
                ElementAttribute att = Attribute.GetCustomAttribute(v, typeof(ElementAttribute)) as ElementAttribute;
                element.Children.Add(new TextBlock() { Text = att.Name, ToolTip = att.Tips, FontSize = 15, Margin = new Thickness(10, 10, 10, 10) });
                UIElement input = null;
                if (v.PropertyType == typeof(IntSettingElement))
                {
                    IntSettingElement s = v.GetValue(setting) as IntSettingElement;
                    input = new NumericTextbox() { Text = s.Value.ToString(), FontSize = 20, TextAlignment = TextAlignment.Center, MinWidth = 100, Minimum = s.MinValue ?? int.MinValue, Maximum = s.MaxValue ?? int.MaxValue };
                    (input as NumericTextbox).TextChanged += (object sender, TextChangedEventArgs e) =>
                    {
                        if (!int.TryParse((input as NumericTextbox).Text, out s.Value)) s.Value = 0;
                    };
                }
                else if (v.PropertyType == typeof(BoolSettingElement))
                {
                    BoolSettingElement s = v.GetValue(setting) as BoolSettingElement;
                    input = new CheckBox() { IsChecked = s.Default, LayoutTransform = new ScaleTransform(1.5, 1.5), Margin = new Thickness(10, 10, 10, 10) };
                    (input as CheckBox).Click += (object sender, RoutedEventArgs e) =>
                    {
                        s.Value = (bool)(input as CheckBox).IsChecked;
                    };
                }
                else if (v.PropertyType == typeof(FileSettingElement))
                {
                    FileSettingElement s = v.GetValue(setting) as FileSettingElement;
                    Button button = new Button() { Content = "ファイルを開く", FontSize = 20, Padding = new Thickness(10, 10, 10, 10) };
                    StackPanel p = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 10, 10, 10) };
                    p.Children.Add(button);
                    p.Children.Add(new TextBlock() { Text = "" });
                    input = p;
                    button.Click += (object sender, RoutedEventArgs e) =>
                      {
                          var dialog = new System.Windows.Forms.OpenFileDialog();
                          dialog.Filter = $"{s.FileDescription} (*.{s.FileExtension})|*.{s.FileExtension}";

                          // ダイアログを表示する
                          if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                          {
                              s.Value = new FileInfo(dialog.FileName);
                              p.Children.OfType<TextBlock>().First().Text = s.Value.FullName;
                          }
                      };
                }
                if (input != null)
                {
                    if (v.CustomAttributes.Any(x => x.AttributeType == typeof(DisabledElementAttribute))) input.IsEnabled = false;
                    element.Children.Add(input);
                }
                if (!string.IsNullOrEmpty(att.Unit)) element.Children.Add(new TextBlock() { Text = $"({att.Unit})", FontSize = 15 });
                result.Add(element);
            }
            return result;
        }
        public void Submit()
        {
            NavigationService.GetNavigationService(this).Navigate(new PlayPage(setting));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        public string GetSubmitText()
        {
            return "スタート";
        }

        public enum PlayType
        {
            SINGLE,
            AI,
            REPLAY
        }
    }
}
