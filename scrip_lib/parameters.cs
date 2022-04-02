using System;
using System.Net;
using System.Collections.Generic;

using Newtonsoft.Json;

/// <summary>
/// Thư viện quản lí data để giao tiếp giữa các thread
/// </summary>
namespace container_capturer.scrip_lib
{
    public struct Command
    {
        public string name;
        public IList<string> args;
    }

    static class parametters
    {
        //command string name
        public const string auto = "auto";
        public const string set = "set";
        public const string get = "get";
        public const string capture = "capture";

        public static bool killThread = false;
        /* ----------------------------------- Image links parametter ----------------------------------- */
        /* rtsp://admin:Dung123@@188.88.76.231/Streaming/Channels/101
         * rtsp://admin:Dung123@@188.88.76.230/Streaming/Channels/101 */
        private static String[] camLink = { "", "", "" };
        private static int camIndex = 3;

        private static String mainFoderLink = "";
        private static String[] capImgLink = { "", "", "" };
        private static String[] outputImgLink = { "", "", "" };
        private static String[] txtLink = { "", "", "" };
        private static String resultLink = "";

        private static String resultSubLink = "/finalResult.jpg";
        private static String[] capturedSubLink = { "/input_", ".jpg" };
        private static String[] outputSubLink = { "/output_", ".jpg" };
        private static String[] txtSubLink = { "/txt_", ".txt" };

        private static String command = "none";

        private static byte[] commandResult;

        /* Get & Set functions */
        /// <summary>
        /// Lưu link connect đến camera
        /// </summary>
        /// <param name="link"></param>
        /// <param name="pose"></param>
        /// <returns></returns>
        public static bool setCamLink(String link, int pose)
        {
            
            if (link != "")
            {
                camIndex = pose + 1;
            }
            else
            {
                camIndex = pose;
            }
            
            camLink[pose] = link;
            Console.WriteLine(link);
            Console.WriteLine(camIndex);
            return true;
        }

        /// <summary>
        /// Trả về số lượng camera được sử dụng để cap ảnh
        /// </summary>
        /// <returns></returns>
        public static int getCamIndex()
        {
            return camIndex;
        }

        /// <summary>
        /// Get link connect đến camera
        /// </summary>
        /// <returns></returns>
        public static String[] getCamLink()
        {
            return camLink;
        }

        /// <summary>
        /// Cài đặt các đường dẫn để lưu ảnh
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public static bool setImageFolder(String link)
        {
            if (link == "")
            {
                return false;
            }

            mainFoderLink = link;
            /* Convert '\' symbol to '/' */
            char[]temp = link.ToCharArray();
            for(int i =0; i<link.Length; i++)
            {
                if(temp[i] == '\\')
                {
                    temp[i] = '/';
                }
            }
            link = new string(temp);

            for(int i = 0; i < camIndex; i++)
            {
                capImgLink[i]    = link + capturedSubLink[0] + (i + 1).ToString() + capturedSubLink[1];
                outputImgLink[i] = link + outputSubLink[0]   + (i + 1).ToString() + outputSubLink[1];
                txtLink[i]       = link + txtSubLink[0]      + (i + 1).ToString() + txtSubLink[1];
                //Console.WriteLine(capImgLink[i]);
                //Console.WriteLine(outputImgLink[i]);
                //Console.WriteLine(txtLink[i]);
                //Console.WriteLine();
            }

            resultLink = link + resultSubLink;
            //Console.WriteLine(resultLink);
            return true;
        }

        /// <summary>
        /// Get đường dẫn ảnh chụp từ camera
        /// </summary>
        /// <returns></returns>
        public static String[] getCapturedImageLink()
        {
            return capImgLink;
        }

        /// <summary>
        /// Get đường dẫn ảnh rời sau khi xử lí
        /// </summary>
        /// <returns></returns>
        public static String[] getOutputImageLink()
        {
            return outputImgLink;
        }

        /// <summary>
        /// Get đường dẫn file txt lưu tham số xử li
        /// </summary>
        /// <returns></returns>
        public static String[] getTxtLink()
        {
            return txtLink;
        }

        /// <summary>
        /// Get đường dẫn lưu ảnh kết quả
        /// </summary>
        /// <returns></returns>
        public static String getResultLink()
        {
            return resultLink;
        }

        /// <summary>
        /// Set lệnh điều khiển
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static bool setCommand(String cmd)
        {
            if(command != "none" && cmd != "none")
                return false;

            command = cmd;
            return true;
        }

        /// <summary>
        /// Get lệnh điều khiển
        /// </summary>
        /// <returns></returns>
        public static string getCommand()
        {
            return command;
        }

        /// <summary>
        /// Lưu kết quả theo dạng byte[]
        /// </summary>
        /// <param name="result"></param>
        public static void setCommandResult(byte[]result)
        {
            try
            {
                commandResult = new byte[result.Length];
                commandResult = result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Get kết quả theo dạng byte[]
        /// </summary>
        /// <returns></returns>
        public static byte[] getCommandResult()
        {
            try
            {
                return commandResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        /* ----------------------------------------------------------------------------------------- */

        /* ----------------------------------- TCP/IP parametter ----------------------------------- */
        private static IPAddress severIP   = IPAddress.Any;
        private static int       severPort = 0;
        private static bool      connectConditionIsAccepted = false;

        public static IPAddress getSeverIP()
        {
            return severIP;
        }

        public static bool setSeverIP(IPAddress svIP)
        {
            severIP = svIP;
            return true;
        }

        public static int getseverPort()
        {
            return severPort;
        }

        public static bool setSeverPort(int _severPort)
        {
            severPort = _severPort;
            return true;
        }

        /// <summary>
        /// Set cờ đã kết nối/chưa kết nối
        /// </summary>
        /// <param name="flg"></param>
        public static void setConnectingFlag(bool flg)
        {
            connectConditionIsAccepted = flg;
        }

        /// <summary>
        /// Đọc cờ đã kết nối/chưa kết nối
        /// </summary>
        /// <param name="flg"></param>
        public static bool getConnectingFlag()
        {
            return connectConditionIsAccepted;
        }


        /* ------------------------------------ Solving by opencv Parameters ----------------------------------- */
        public struct solveImgParametters
        {
            public double cannyThreadhole1,
                          cannyThreadhole2;
            public int blurScale;
            public int container_heigh,
                       len_tols;
            public double v_tollerance,
                          h_tollerance;
            public int crop_scale,
                       lineNums;
        };

        private static double cannyThreadhole1 = 30,
                              cannyThreadhole2 = 100;

        /* blur */
        private static int blurScale = 25;

        private static int container_heigh = 1100,
                           len_tols = 100;
        private static double   v_angle = 0,
                                v_tollerance = 45 * Math.PI / 180,
                                h_angle = 90 * Math.PI / 180,
                                h_tollerance = 20 * Math.PI / 180;

        private static int crop_scale = container_heigh / 10;
        private static int lineNums = 1000;

        public static void setCannyThreshole(double th1, double th2)
        {
            cannyThreadhole1 = th1;
            cannyThreadhole1 = th2;
        }

        public static void setBlurScale(int blrScale)
        {
            blurScale = blrScale;
        }

        public static void setContainerSize(int height, int tollerance)
        {
            container_heigh = height;
            len_tols = tollerance;
            crop_scale = container_heigh / 10;
        }

        public static void setAngleTolerances(double v_tols, double h_tols)
        {
            v_tollerance = v_tols;
            h_tollerance = h_tols;
        }

        public static void setLineNum(int num)
        {
            lineNums = num;
        }

        public static solveImgParametters getOpencvParameters()
        {
            solveImgParametters para = new solveImgParametters();
            para.cannyThreadhole1 = cannyThreadhole1;
            para.cannyThreadhole2 = cannyThreadhole2;
            para.lineNums = lineNums;
            para.v_tollerance = v_tollerance;
            para.h_tollerance = h_tollerance;
            para.blurScale = blurScale;
            para.container_heigh = container_heigh;
            para.len_tols = len_tols;
            para.crop_scale = crop_scale;
            return para;
        }
        /* ---------------------------------------------------------------------------------------------- */
    }
}
