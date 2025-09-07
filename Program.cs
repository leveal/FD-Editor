using System;
using System.Windows.Forms;

namespace FR_Operator
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool terminalOnly = false;
            bool enableVisualStyles = true;
            if (args != null)
            {
                foreach (string a in args)
                {
                    string arg = a.ToLower();
                    if (arg.ToLower().StartsWith("external_log="))
                    {
                        LogHandle.ExternalLogDir = arg.Substring(13);
                    }
                    else if (arg.ToLower().Equals("term"))
                    {
                        terminalOnly = true;
                    }
                    else if (arg.ToLower().Equals("xp"))
                    {
                        enableVisualStyles = false;
                    }
                }
            }
            AppSettings.LoadSettings();
            if (enableVisualStyles)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }

            if (terminalOnly)
            {
                TerminalFnExchange lib = new TerminalFnExchange();
                lib.ShowSettings();
            }
            else
            {
                if (!System.IO.File.Exists("Atol.Drivers10.Fptr.dll"))
                {
                    LogHandle.ol("Отсутсвует обертка для работы с атолом Atol.Drivers10.Fptr.dll" + Environment.NewLine +
                        "Работа с атолом заблокирована. Возможно программа некорректно извлечена из архива. Для разблокировки работы с атолом добавьте в директорию с программой обертку Atol.Drivers10.Fptr.dll"+Environment.NewLine+
                        "Для разблокировки скачайте fs.atol.ru полный архив драйвера, скопируйте 10.10.6.0\\wrappers\\csharp\\Atol.Drivers10.Fptr.dll  в папку с программой и перезапустите ее" );
                    AppSettings.AtolAbility = false;
                }
                if (!System.IO.File.Exists("Newtonsoft.Json.dll"))
                {
                    LogHandle.ol("Отсутвует библиотека для работы с JSON форматом Newtonsoft.Json.dll, работа с данным форматом блокируется");
                    AppSettings.jsonAvailable = false;
                }

                Application.Run(new MainForm());
            }

            
        }
        
    }
}
