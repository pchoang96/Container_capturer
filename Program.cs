using container_capturer.scrip_lib;
using System;
using System.Threading;
using System.Windows.Forms;

namespace container_capturer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [Obsolete]
        static int Main()
        {
            try
            {                
                //Thread của winform
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainPage());               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            /*
            String[] imgLink = {"D:/SITCDV/container_capturer/images/r5.jpg"
                                };
            opencvProcess solveImg = new opencvProcess();
            solveImg.fullProcess(imgLink);
            */
            return 1;
        }
    }
}
