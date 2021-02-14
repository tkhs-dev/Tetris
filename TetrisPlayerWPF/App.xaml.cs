using log4net;
using System.Windows;

namespace TetrisPlayerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
            GetLogger().Info("Start up TetrisPlayer...");
        }
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static ILog GetLogger()
        {
            return logger;
        }
    }
}
