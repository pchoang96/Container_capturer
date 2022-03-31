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
                //Tạo thread xử lí ảnh
                autoFunctions auto = new autoFunctions();
                Thread autoControlThread = new Thread(auto.serviceLoop);
                autoControlThread.Name = "Auto control thread";
                autoControlThread.IsBackground = true;
                autoControlThread.Start();

                //Tạo thread nhận lệnh tcp
                tcpServer myTcp = new tcpServer();
                Thread tcpIpControlThread = new Thread(myTcp.protocolLoop);
                tcpIpControlThread.Name = "TCP control thread";
                tcpIpControlThread.IsBackground = true;
                tcpIpControlThread.Start();

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
