using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TetrisPlayer
{
    static class TetrisPlayer
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            GetLogger().Info("Start up TetrisPlayer...");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form main = new Form1();
            Application.Run(main);
        }

        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ILog GetLogger()
        {
            return logger;
        }
    }
}
