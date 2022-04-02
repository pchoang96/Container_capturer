using OpenCvSharp;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace container_capturer.scrip_lib
{
    /// <summary>
    /// Thư viện chứa các hàm liên quan đến chụp camera
    /// </summary>
    internal class cameraCapture
    {
        /* =================================== Variables =================================== */

        private VideoCapture _capture;
        private string linkCamera = "";

        /* =================================== Contructor =================================== */

        /// <summary>
        /// Tạo object đọc camera từ link
        /// </summary>
        /// <param name="connectCameraLink"></param> Link connect đến camera
        public cameraCapture(string connectCameraLink)
        {
            Console.WriteLine("\n This is camera reader \n");
            linkCamera = connectCameraLink;
            _capture = new VideoCapture(linkCamera);
        }

        /// <summary>
        /// Kết nối thẳng đến cam của máy nếu không có link
        /// </summary>
        public cameraCapture()
        {
            Console.WriteLine("\n This is camera reader \n");
            _capture = new VideoCapture(0);
        }

        /*  ===================================  Private =================================== */
        /*  ===================================  Public  =================================== */

        /// <summary>
        /// Chờ kết nối đến camera
        /// </summary>
        /// <returns></returns>
        public bool openCamera()
        {
            int counter = 0;
            while (!_capture.IsOpened() && counter < 5)
            {
                counter++;
                Console.WriteLine("connecting to {0}", linkCamera);
                Thread.Sleep(500);
            }

            string ouput = "";

            if (counter > 4)
            {
                ouput = "Connect: " + linkCamera + " failed.";
                ProcessOuput.AppendTextRichTextBox(ouput);
                return false;
            }

            ouput = "Connected: " + linkCamera + " success.";
            ProcessOuput.AppendTextRichTextBox(ouput);
            return true;
        }

        /// <summary>
        /// Stream camera as video
        /// </summary>
        public void streamCamera()
        {
            Mat streamFrame = new Mat();

            while (true)
            {
                if (_capture.Read(streamFrame))
                {
                    Cv2.ImShow("Lovely C#", streamFrame);
                }

                if (Cv2.WaitKey(1) == 'q')
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Lưu ảnh chụp camera vào đường dẫn
        /// </summary>
        /// <param name="imLink"></param> Link đường dẫn
        /// <param name="scale"></param> Scale ảnh
        /// <returns></returns>
        public bool saveFrameTo(string imLink, OpenCvSharp.Size scale)
        {
            Mat img = new Mat();
            _capture.Read(img);
            Cv2.Resize(img, img, scale, 1, 1);
            return (Cv2.ImWrite(imLink, img));
        }
    }

    /// <summary>
    /// Thư viện chứ các hàm vẽ/cắt/ghép ảnh
    /// </summary>
    internal class drawFunctions
    {
        /* =================================== Private variables =================================== */
        static bool drawing = false;
        static int ix = 0, iy = 0;
        static int holdX = 0, holdY = 0;
        static int resize_scale = 1;
        public static string[] characters = { "point1-", "point2-" };

        /* =================================== Public variables =================================== */
        /* =================================== Contructors =================================== */
        /* =================================== Private functions =================================== */

        /// <summary>
        /// Hàm được gọi ra khi có hoạt động của chuột
        /// </summary>
        /// <param name="_event"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="flags"></param>
        /// <param name="userData"></param>
        private static void Mice(MouseEventTypes _event, int x, int y, MouseEventFlags flags, IntPtr userData)
        {
            switch (_event)
            {
                case MouseEventTypes.LButtonDown:
                    drawing = true;
                    ix = x; iy = y; //starting point
                    break;
                case MouseEventTypes.LButtonUp:
                    drawing = false;
                    break;
                case MouseEventTypes.MouseMove:
                    if (drawing)
                    {
                        holdX = x; holdY = y; //moving 
                    }
                    break;
            }
        }

        /// <summary>
        /// Hàm vẽ hình chữ nhật với hai điểm
        /// </summary>
        /// <param name="img"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <returns></returns>
        private static Mat drawRectangle(Mat img, int x, int y, int xx, int yy)
        {
            int thickness = 2;

            Mat temp_img = img;
            OpenCvSharp.Point topLeft, topRight, botLeft, botRight;
            topLeft = new OpenCvSharp.Point(Math.Min(x, xx), Math.Min(y, yy));
            topRight = new OpenCvSharp.Point(Math.Max(x, xx), Math.Min(y, yy));
            botLeft = new OpenCvSharp.Point(Math.Min(x, xx), Math.Max(y, yy));
            botRight = new OpenCvSharp.Point(Math.Max(x, xx), Math.Max(y, yy));

            Cv2.Line(temp_img, topLeft, topRight, new Scalar(0, 149, 0), thickness);
            Cv2.Line(temp_img, topLeft, botLeft, new Scalar(0, 149, 0), thickness);
            Cv2.Line(temp_img, topRight, botRight, new Scalar(0, 149, 0), thickness);
            Cv2.Line(temp_img, botRight, botLeft, new Scalar(0, 149, 0), thickness);

            return temp_img;
        }

        /// <summary>
        /// Cắt ảnh dựa vào hai điểm và tỉ lệ
        /// </summary>
        /// <param name="img"></param>  ảnh đầu vào
        /// <param name="x"></param>    tọa độ điểm
        /// <param name="y"></param>    tọa độ điểm
        /// <param name="xx"></param>   tọa độ điểm
        /// <param name="yy"></param>   tọa độ điểm
        /// <param name="scale"></param>tỉ lệ (resize ảnh sau khi cắt)
        /// <returns></returns> ảnh kết quả
        public static Mat cropImage(Mat img, int x, int y, int xx, int yy, int scale = 0)
        {
            //Console.Write("Scale is : {0}\n", scale);

            int minX = Math.Min(x, xx) - scale,
                minY = Math.Min(y, yy) - scale,
                maxX = Math.Max(x, xx) + scale,
                maxY = Math.Max(y, yy) + scale;

            minX = Math.Max(minX, 0);
            minY = Math.Max(minY, 0);
            maxX = Math.Min(maxX, img.Width);
            maxY = Math.Min(maxY, img.Height);
            if (x == xx || y == yy)
            {
                return img;
            }
            Mat img_cropped = img[minY, maxY, minX, maxX];
            return img_cropped;
        }

        /// <summary>
        /// Lưu ảnh và text(chứa thông số để cắt ảnh) vào đường dẫn
        /// </summary>
        /// <param name="img"></param>
        /// <param name="imageLink"></param>
        /// <param name="textLink"></param>
        /// <returns></returns>
        private static bool saveImageAndData(Mat img, string imageLink, string textLink)
        {
            //Lưu ảnh
            if (false == Cv2.ImWrite(imageLink, img))
            {
                return false;
            }

            //Thông số cắt ảnh
            ix *= resize_scale;
            iy *= resize_scale;
            holdY *= resize_scale;
            holdX *= resize_scale;

            ix = ix < 0 ? 0 : ix;
            iy = iy < 0 ? 0 : iy;
            ix = ix > img.Width ? img.Width : ix;
            iy = iy > img.Height ? img.Height : iy;

            string line = "";
            line += characters[0] + ((int)(ix)).ToString() + ":" + ((int)(iy)).ToString()
            + "\n" + characters[1] + ((int)(holdX)).ToString() + ":" + ((int)(holdY)).ToString();

            File.WriteAllText(textLink, line);

            return true;
        }

        /* =================================== Public functions =================================== */

        /// <summary>
        /// Cắt ảnh theo chiều dọc
        /// </summary>
        /// <param name="img"></param>
        /// <param name="x"></param>
        /// <param name="xx"></param>
        /// <returns></returns>
        public static Mat cropingImageVerticly(Mat img, int x, int xx)
        {
            Mat img_cropped = img.Clone();
            if (xx != x)
            {
                img_cropped = cropImage(img_cropped, x, 0, xx, img_cropped.Height);
            }
            return img_cropped;
        }

        /// <summary>
        /// Ghép hai ảnh với nhau theo phương ngang
        /// </summary>
        /// <param name="imLeft"></param>
        /// <param name="imRight"></param>
        /// <returns></returns>
        public static Mat addTwoImages(Mat imLeft, Mat imRight)
        {
            if (imLeft.Size().Height != imRight.Size().Height)
            {
                Console.Write("Images must have same High");
                Console.WriteLine("hl: {0} and hr: {1} ", imLeft.Height, imRight.Height);
                return null;
            }

            Mat addingResult = new Mat();
            Cv2.HConcat(imLeft, imRight, addingResult);
            return addingResult;
        }

        /// <summary>
        /// Vẽ đường biểu thị ngăn cách cho ảnh
        /// </summary>
        /// <param name="img"></param>
        /// <param name="x"></param>
        /// <param name="Length"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static Mat drawingSeparateLine(Mat img, int x, int Length = 50, int thickness = 20)
        {
            Mat temp_img = img.Clone();
            int h = temp_img.Height;
            Cv2.Line(temp_img, new OpenCvSharp.Point(x, 0), new OpenCvSharp.Point(x, Length), new Scalar(0, 255, 0), thickness);
            Cv2.Line(temp_img, new OpenCvSharp.Point(x, h), new OpenCvSharp.Point(x, h - Length), new Scalar(0, 255, 0), thickness);
            return temp_img;
        }

        /// <summary>
        /// Vòng lặp vẽ ảnh, lưu ảnh và tự động ghép ảnh
        /// </summary>
        /// <param name="img"></param>
        /// <param name="resultLink"></param>
        /// <param name="txtLink"></param>
        /// <returns></returns>
        public static bool drawingLoop(Mat img, string resultLink, string txtLink)
        {
            const string settingWindown = "Setting image";
            bool rep = false;
            Mat img_temp = img.Clone();
            Mat temp = new Mat();
            int w = img_temp.Width, h = img_temp.Height;

            if (w > 800)
            {
                resize_scale = w / 800;
                Cv2.Resize(img_temp, img_temp, new OpenCvSharp.Size(img_temp.Width / resize_scale, img_temp.Height / resize_scale));
            }

            Cv2.ImShow(settingWindown, img_temp);
            Cv2.SetMouseCallback(settingWindown, Mice);

            bool looping = true;
            while (looping)
            {
                if (Cv2.GetWindowProperty(settingWindown, WindowPropertyFlags.Visible) < 1)
                {
                    Console.WriteLine("Windown closed");
                    return false;
                }

                if (drawing)
                {
                    try
                    {
                        img_temp.CopyTo(temp);
                        Cv2.ImShow("Setting image", drawRectangle(temp, ix, iy, holdX, holdY));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }                    
                }
                int keyPress = Cv2.WaitKey(1) & 0xFF;
                switch (keyPress)
                {
                    case 'c':                        
                        img_temp.CopyTo(temp);
                        temp = cropingImageVerticly(temp, ix, holdX);
                        Console.WriteLine("cx: {0} - choldX: {1}", ix, holdX);
                        Cv2.ImShow("Setting image", temp);
                        break;
                    case 's':
                        saveImageAndData(img, resultLink, txtLink);
                        ix = 0; holdX = 0;
                        iy = 0; holdY = 0;
                        rep = true;
                        looping = false;
                        break;
                    case 'q':
                        rep = false;
                        ix = 0; holdX = 0;
                        iy = 0; holdY = 0;
                        looping = false;
                        break;
                    default:
                        if (keyPress != 255)
                        {
                            Console.WriteLine("Pressed: {0}", keyPress);
                        }
                        break;
                }
            }
            Cv2.DestroyWindow(settingWindown);
            return rep;
        }

        /// <summary>
        /// Vẽ đường thẳng
        /// </summary>
        /// <param name="img"></param>
        /// <param name="line"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Mat drawLine(Mat img, LineSegmentPolar line, Scalar color)
        {
            double rho = line.Rho,
                   theta = line.Theta;
            Console.WriteLine("Rho: {0} and Theta: {1}", rho, theta);
            int len = 5000;
            double a = Math.Cos(theta);
            double b = Math.Sin(theta);

            double x0 = a * rho,
                   y0 = b * rho,
                   x1 = (x0 + len * (-b)),
                   y1 = y0 + len * (a),
                   x2 = x0 - len * (-b),
                   y2 = y0 - len * (a);
            Cv2.Line(img, new OpenCvSharp.Point(x1, y1), new OpenCvSharp.Point(x2, y2), color, 10);
            return img;
        }

        /// <summary>
        /// Vẽ tập hợp các điểm 
        /// </summary>
        /// <param name="img"></param>
        /// <param name="point"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Mat drawPoint(Mat img, OpenCvSharp.Point[] point, Scalar color)
        {
            for (int i = 0; i < point.Length; i++)
            {
                img = drawPoint(img, point[i], color);
            }
            return img;
        }
        public static Mat drawPoint(Mat img, OpenCvSharp.Point point, Scalar color)
        {
            if (point.X > img.Width || point.Y > img.Height || point.X < 0 || point.Y < 0)
            {
                return img;
            }
            Cv2.Circle(img, point.X, point.Y, 2, color, 5);
            return img;
        }
    }

    /// <summary>
    /// Thư viện quản lí các hàm xử lí ảnh của openCV
    /// </summary>
    internal class opencvProcess
    {
        /* =================================== Structs =================================== */

        /// <summary>
        /// Struct lưu kết quả xử lí ảnh
        /// </summary>
        public struct openCVResult
        {
            public LineSegmentPolar[] horizon;
            public LineSegmentPolar[] verticle;
            public Mat resultImg;
            public OpenCvSharp.Point[] cropPoints;
            public openCVResult(int size = 2)
            {
                horizon = new LineSegmentPolar[size];
                verticle = new LineSegmentPolar[size];
                verticle[0].Rho = 99999;
                verticle[1].Rho = 0;

                resultImg = new Mat();
                cropPoints = new OpenCvSharp.Point[size];
            }
        }

        /* =================================== Variables =================================== */

        private static double cannyThreshole1 = 30,
                               cannyThreshole2 = 100;

        /* blur */
        private static int blurScale = 25;

        private static int container_heigh = 1100,
                           len_tols = 100;
        private static double v_angle = 0,
                            v_tollerance = 45 * Math.PI / 180,
                            h_angle = 90 * Math.PI / 180,
                            h_tollerance = 20 * Math.PI / 180;

        /* solve */
        private static double solve_scale = 1,
                              show_scale = 0.3 / solve_scale;
        private static int crop_scale = container_heigh / 10;
        private static int lineNums = 1000;

        private static LineSegmentPoint[] list_cup1 = new LineSegmentPoint[lineNums];
        private static LineSegmentPoint[] list_cup2 = new LineSegmentPoint[lineNums];


        /* =================================== Contructors =================================== */

        /// <summary>
        /// Thiết lập chiều cao container trong ảnh (pixels)
        /// </summary>
        /// <param name="height"></param>
        public static void setContainerHeight(int height)
        {
            container_heigh = Math.Abs(height);
            Console.WriteLine("Container height chaned to {0}", container_heigh);
        }

        /* =================================== Sub Functions =================================== */

        /// <summary>
        /// hàm tính góc AOB tạo bởi 3 điểm
        /// </summary>
        /// <param name="A"></param>
        /// <param name="O"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private static float getAngleFrom3Points(OpenCvSharp.Point A, OpenCvSharp.Point O, OpenCvSharp.Point B)
        {
            float v1x = B.X - O.X;
            float v1y = B.Y - O.Y;
            float v2x = A.X - O.X;
            float v2y = A.Y - O.Y;

            float angle = (float)(Math.Atan2(v1x, v1y) - Math.Atan2(v2x, v2y));
            return angle;/*in range of -pi ~ pi */
        }

        /// <summary>
        /// Hàm chuyển đổi đường thẳng (từ LineSegmentPoint sang LineSegmentPolar)
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static LineSegmentPolar convertToLineSegmentPolar(LineSegmentPoint line)
        {
            LineSegmentPolar result = new LineSegmentPolar(0, 0);

            if (line.P1.X == line.P2.X)
            {
                result.Rho = 0;
                result.Theta = line.P1.X;
            }
            else if (line.P1.Y == line.P2.Y)
            {
                result.Rho = (float)(Math.PI) / 2;
                result.Theta = line.P1.Y;
            }
            else
            {
                float crossOy = line.P1.Y - line.P1.X * (line.P2.Y - line.P1.Y) / (line.P2.X - line.P1.X);
                float theta = getAngleFrom3Points(line.P1, new OpenCvSharp.Point(0, crossOy), new OpenCvSharp.Point(0, 0));
                result.Rho = (float)(Math.Sin(theta) * crossOy);
                result.Theta = theta;
            }

            return result;
        }

        /// <summary>
        /// Hàm tính số đường gần xung quanh đường thẳng (điểm phổ biến)
        /// </summary>
        /// <param name="groupLines"></param>
        /// <param name="mainLine"></param>
        /// <returns></returns> 
        private static int getLinePopularFactor(LineSegmentPoint[] groupLines, LineSegmentPoint mainLine)
        {
            double angleScale = 180 * (Math.PI / 180);
            int distanceScale = 10;
            int count = 0;


            LineSegmentPolar polarMainLine = convertToLineSegmentPolar(mainLine);
            LineSegmentPolar mainLineCompare = polarMainLine;
            if (polarMainLine.Rho < 0)
            {
                mainLineCompare.Rho = Math.Abs(polarMainLine.Rho);
            }

            for (int i = 0; i < groupLines.Length; i++)
            {
                LineSegmentPolar subLine = convertToLineSegmentPolar(groupLines[i]);
                LineSegmentPolar subLineCompare = subLine;
                if (subLine.Rho < 0)
                {
                    subLineCompare.Rho = Math.Abs(subLine.Rho);
                }
                if ((Math.Abs(subLineCompare.Rho - mainLineCompare.Rho) < distanceScale) &&
                   (Math.Abs(subLineCompare.Theta - mainLineCompare.Theta) < angleScale))
                {
                    count++;
                }
            }
            return count;
        }

        /* =================================== Main functions =================================== */

        /// <summary>
        /// Hàm tìm kiếm đường thẳng trong ảnh
        /// </summary>
        /// <param name="img"></param>
        /// <param name="numLines"></param>
        /// <param name="scale"></param>
        /// <param name="minLen"></param>
        /// <param name="maxGap"></param>
        /// <returns></returns>
        private static LineSegmentPoint[] find_line(Mat img, int numLines, int scale, int minLen, int maxGap)
        {
            Mat temp = img.Clone();
            //Cv2.CvtColor(temp, temp, ColorConversionCodes.BGR2GRAY);
            Cv2.Canny(temp, temp, cannyThreshole1, cannyThreshole2);

            var sss = new Mat();
            int threshholeValue = 1;
            LineSegmentPoint[] lines;
            while (true)
            {
                lines = Cv2.HoughLinesP(temp, 1, Math.PI / 180, threshholeValue, minLen, maxGap);
                if (lines.Length > numLines)
                {
                    threshholeValue += scale;
                }
                else
                {
                    Console.WriteLine("Number of lines: {0}", lines.Length);
                    break;
                }
            }
            return lines;
        }

        /// <summary>
        /// Hàm in ảnh ra cửa sổ để xem
        /// </summary>
        /// <param name="img"></param> ảnh
        /// <param name="name"></param> tên cửa sổ
        private void checkImage(Mat img, string name = "debug")
        {
            var temp = img.Clone();
            Cv2.Resize(temp, temp, new OpenCvSharp.Size(0, 0), show_scale, show_scale);
            Cv2.ImShow(name, temp);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        /// <summary>
        /// Hàm tìm các đường bao vật thể (contours) trong ảnh và in ra một ảnh đen trắng chỉ chứa các đường bao
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private static Mat contourDefine(Mat img)
        {
            Mat blank = Mat.Zeros(img.Height, img.Width, MatType.CV_8UC1);

            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchyIndexes;

            // Blur ảnh
            Mat temp = new Mat();
            Cv2.MedianBlur(img, temp, blurScale);

            // Đưa ảnh về các khối vuống 50*50 để xử lí cho dễ
            Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(50, 50));
            Cv2.MorphologyEx(temp, temp, MorphTypes.Open, element);
            Cv2.MorphologyEx(temp, temp, MorphTypes.Close, element);

            // Convert ảnh về đen trắng và xử lí canny
            Cv2.CvtColor(temp, temp, ColorConversionCodes.BGR2GRAY);
            Cv2.Canny(temp, temp, cannyThreshole1, cannyThreshole2);

            // Tìm các đường contours
            Cv2.FindContours(
                        temp,
                        out contours,
                        out hierarchyIndexes,
                        mode: RetrievalModes.External,
                        method: ContourApproximationModes.ApproxSimple);
            Console.WriteLine("Contour raw = {0}", contours.Length);

            // Lọc và in các đường contours có độ dài lớn hơn 100 vào ảnh trống (blank)
            int counter = 0;
            var contourIndex = 0;
            while ((contourIndex >= 0))
            {
                var contour = contours[contourIndex];

                double epsilon = 0.00001 * Cv2.ArcLength(contour, true);
                Mat outPutMat = new Mat();
                var output = OutputArray.Create(outPutMat);
                Cv2.ApproxPolyDP(InputArray.Create(contour), output, epsilon, true);
                double length = Cv2.ArcLength(output.GetMat(), true);
                if (length > 100)
                {
                    Console.WriteLine("Length = {0}", length);
                    Cv2.DrawContours(blank, contours, contourIndex, Scalar.White, 2);
                    counter++;
                }
                else
                {

                }
                contourIndex = hierarchyIndexes[contourIndex].Next;
            }

            //checkImage(blank,"Blank");
            return blank;
        }

        /// <summary>
        /// Xứ lí tìm kiếm đường bao container trong ảnh
        /// </summary>
        /// <param name="img"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static openCVResult solveImage(Mat img, LineSegmentPoint[] lines)
        {
            openCVResult result = new openCVResult(2);

            // ------------------------------ Tính toán độ phổ biến trung bình ------------------------------
            var popularFactor = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                LineSegmentPolar template = convertToLineSegmentPolar(lines[i]);
                //Console.WriteLine("rho = {0}; theta = {1}", template.Rho, template.Theta);
                popularFactor += getLinePopularFactor(lines, lines[i]);
            }
            popularFactor /= lines.Length;
            //------------------------------------------------------------------------------------------------

            // ------------------------ Lọc các đường có độ phổ biến thấp hơn trung bình ---------------------
            LineSegmentPoint[] popularLines = new LineSegmentPoint[lines.Length];
            int popularScale = 4;
            int popularLinesCounter = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                LineSegmentPolar template = convertToLineSegmentPolar(lines[i]);
                if (getLinePopularFactor(lines, lines[i]) >= popularFactor / popularScale)
                {
                    popularLines[popularLinesCounter] = lines[i];
                    popularLinesCounter++;
                }
            }
            //------------------------------------------------------------------------------------------------

            // ------------------------------ Find 4 lines that shapes the container -------------------------
            int count = 0;
            for (int i = 0; i < popularLinesCounter; i++)
            {
                LineSegmentPolar mainLine = convertToLineSegmentPolar(popularLines[i]);
                LineSegmentPolar template = mainLine;
                //Console.WriteLine("r = {0} , th = {1}", template.Rho, template.Theta);
                if (mainLine.Rho < 0)
                {
                    template.Rho = Math.Abs(mainLine.Rho);
                }

                //--------------------------- find 2 verticle line --------------------------------------------
                if ((template.Theta > h_angle + h_tollerance) || (template.Theta < h_angle - h_tollerance))
                {
                    if (Math.Abs(template.Theta - v_angle) < v_tollerance)
                    {
                        if (Math.Abs(result.verticle[0].Rho) > template.Rho)
                        {
                            result.verticle[0] = mainLine;
                        }
                        if (Math.Abs(result.verticle[1].Rho) < template.Rho)
                        {
                            result.verticle[1] = mainLine;
                        }
                    }
                    continue;
                }
                //-------------------------------------------------------------------------------------------

                for (int j = 0; j < lines.Length; j++)
                {
                    LineSegmentPolar subLine = convertToLineSegmentPolar(lines[j]);

                    if ((subLine.Theta < mainLine.Theta + h_tollerance) && (subLine.Theta > mainLine.Theta - h_tollerance))
                    {
                        if (((mainLine.Rho - subLine.Rho) > (container_heigh - len_tols)) && ((mainLine.Rho - subLine.Rho) < (container_heigh + len_tols)))
                        {
                            list_cup1[count] = lines[i];
                            list_cup2[count] = lines[j];
                            //Console.WriteLine("Main line: R = {0} _ Th = {1}", mainLine.Rho, mainLine.Theta);
                            //Console.WriteLine("Sub line: R = {0} _ Th = {1}", subLine.Rho, subLine.Theta);
                            count += 1;
                        }
                    }
                }
            }

            // find two most-parallel couple lines 
            var showing = img.Clone();
            Cv2.CvtColor(showing, showing, ColorConversionCodes.GRAY2BGR);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    LineSegmentPolar temp1 = convertToLineSegmentPolar(list_cup1[i]);
                    LineSegmentPolar temp2 = convertToLineSegmentPolar(list_cup2[i]);
                    result.horizon[0].Rho += temp1.Rho;
                    result.horizon[1].Rho += temp2.Rho;
                    result.horizon[0].Theta += temp1.Theta;
                    result.horizon[1].Theta += temp2.Theta;
                }
                result.horizon[0].Rho /= count;
                result.horizon[1].Rho /= count;
                result.horizon[0].Theta /= count;
                result.horizon[1].Theta /= count;

                showing = drawFunctions.drawLine(showing, result.horizon[0], Scalar.Green);
                showing = drawFunctions.drawLine(showing, result.horizon[1], Scalar.Green);
                showing = drawFunctions.drawLine(showing, result.verticle[0], Scalar.Green);
                showing = drawFunctions.drawLine(showing, result.verticle[1], Scalar.Green);
            }

            var A = result.horizon[0].LineIntersection(result.verticle[0]);
            var B = result.horizon[0].LineIntersection(result.verticle[1]);
            var C = result.horizon[1].LineIntersection(result.verticle[0]);
            var D = result.horizon[1].LineIntersection(result.verticle[1]);

            if (A.HasValue && B.HasValue && C.HasValue && D.HasValue && D.HasValue)
            {
                result.cropPoints[0].X = Math.Min(A.Value.X, D.Value.X);
                result.cropPoints[0].Y = Math.Min(A.Value.Y, D.Value.Y);
                result.cropPoints[1].X = Math.Max(B.Value.X, C.Value.X);
                result.cropPoints[1].Y = Math.Max(B.Value.Y, C.Value.Y);

                drawFunctions.drawPoint(showing, new OpenCvSharp.Point(A.Value.X, A.Value.Y), Scalar.Pink);
                drawFunctions.drawPoint(showing, new OpenCvSharp.Point(B.Value.X, B.Value.Y), Scalar.Pink);
                drawFunctions.drawPoint(showing, new OpenCvSharp.Point(C.Value.X, C.Value.Y), Scalar.Pink);
                drawFunctions.drawPoint(showing, new OpenCvSharp.Point(D.Value.X, D.Value.Y), Scalar.Pink);
                drawFunctions.drawPoint(showing, result.cropPoints, Scalar.LightPink);
            }
            else
            {
                Console.WriteLine("Can't find the lines");
            }

            //Cv2.Resize(showing, showing, new OpenCvSharp.Size(0, 0), show_scale, show_scale);
            //Cv2.ImShow("mid-result", showing);
            //Cv2.WaitKey(0);

            result.resultImg = img;

            return result;
        }

        /// <summary>
        /// Xứ lí ảnh để tìm kiếm các cạnh của container và crop ảnh vừa với conainer
        /// </summary>
        /// <param name="imageLinks"></param>
        public static void fullProcess(string imageLinks)
        {
            var img = Cv2.ImRead(imageLinks);

            var temp = contourDefine(img);
            var result = solveImage(temp, find_line(temp, lineNums, 5, 50, 10));

            temp = result.resultImg;

            var r = Cv2.GetRotationMatrix2D(new OpenCvSharp.Point(temp.Width / 2, temp.Height / 2), (result.horizon[0].Theta - Math.PI / 2), 1);
            Cv2.WarpAffine(temp, temp, r, new OpenCvSharp.Size(0, 0));

            temp = drawFunctions.cropImage(img, result.cropPoints[0].X, result.cropPoints[0].Y, result.cropPoints[1].X, result.cropPoints[1].Y, crop_scale);
            Cv2.ImWrite(imageLinks, temp);

            /*
            string name = "Result of " + imageLinks[i];
            Cv2.Resize(temp, temp, new OpenCvSharp.Size(0, 0), show_scale, show_scale);
            Cv2.ImShow(name, temp);
            if (Cv2.WaitKey(0) == 'q')
            {
                Cv2.DestroyAllWindows();
                break;
            }
            */
        }
    }

    /// <summary>
    /// Thư viện quản lí các hàm chạy tự động
    /// </summary>
    internal class autoFunctions
    {
        /* =================================== Variables =================================== */

        /* =================================== Contructors =================================== */
        public autoFunctions()
        {

        }
        /* =================================== Functions =================================== */

        /// <summary>
        /// Đọc file text để lấy dữ liệu xử lí ảnh
        /// </summary>
        /// <param name="fileLink"></param> link file text
        /// <param name="output"></param> dữ liệu đã lọc ra
        private void decoder(string fileLink, int[] output)
        {
            //Các biến xử lí
            string hold = "";
            string[] hold_X = { "", "" };
            string[] hold_Y = { "", "" };
            byte holdFlag = 0;

            // đọc file
            string[] result = File.ReadAllLines(fileLink);

            // lọc chuỗi
            for (int i = 0; i < result.Length; i++)
            {
                char[] charArray = result[i].ToCharArray();
                for (int j = 0; j < charArray.Length; j++)
                {
                    if (0 == holdFlag)
                    {
                        hold += charArray[j];
                        if (drawFunctions.characters[i] == hold)
                        {
                            hold = "";
                            holdFlag = 1;
                        }
                    }
                    else if (charArray[j] != ':' && holdFlag == 1)
                    {
                        hold_X[i] += charArray[j];
                    }
                    else if (charArray[j] != ':' && holdFlag == 2)
                    {
                        hold_Y[i] += charArray[j];
                    }
                    else
                    {
                        holdFlag = 2;
                    }
                }
                holdFlag = 0;
                hold = "";
            }
            //Console.WriteLine("\nxr: {0} - holdXr: {1}", hold_X[0], hold_X[1]);
            //Console.WriteLine("yr: {0} - holdYr: {1}", hold_Y[0], hold_Y[1]);
            output[0] = int.Parse(hold_Y[0]);
            output[1] = int.Parse(hold_Y[1]);
            output[2] = int.Parse(hold_X[0]);
            output[3] = int.Parse(hold_X[1]);
        }

        /// <summary>
        /// Tự động cap và lưu ảnh từ các camera
        /// </summary>
        /// <param name="cameraLink"></param> link to connect with camera
        /// <param name="imageStorage"></param> directories to store captured images
        /// <returns></returns>
        private bool AutoCapturingPhotosFromCamera(string[] cameraLink, string[] imageStorage)
        {
            if (cameraLink.Length != imageStorage.Length)
            {
                Console.WriteLine("cameraLink and imageStorage array must have same OpenCvSharp.Size");
                return false;
            }

            for (int i = 0; i < parametters.getCamIndex(); i++)
            {
                if ("" == cameraLink[i] || "" == imageStorage[i])
                {
                    continue;
                }
                cameraCapture camRead = new cameraCapture(cameraLink[i]);
                if (camRead.openCamera())
                {
                    if (camRead.saveFrameTo(imageStorage[i], new OpenCvSharp.Size(0, 0)))
                        Console.WriteLine("Save done at {0}", i);
                    else
                    {
                        Console.WriteLine("Write failed at {0}", i);
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Getting camera failed at {0}", i);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// auto tự động cắt/vẽ -> ghép ảnh -> lưu ảnh từ các đường dẫn
        /// </summary>
        /// <param name="inputDirectory"></param> đường dẫn đến ảnh input
        /// <param name="outputDirectory"></param> đường dẫn chỗ lưu ảnh rời đã xử lí (output)
        /// <param name="txtDirectory"></param> đường dẫn chỗ lưu ảnh kết quả (đã ghép)
        /// <returns></returns> result Mat type
        private Mat autoSolvingImages(string[] inputDirectory, string[] outputDirectory, string[] txtDirectory, string resultDirectory)
        {
            try
            {
                // Kiểm tra điều kiện để chạy hàm: các đường dẫn phải tương ứng với nhau
                if (inputDirectory.Length != outputDirectory.Length || outputDirectory.Length != txtDirectory.Length )
                {
                    Console.WriteLine("Directory arrays must have same size");
                    return null;
                }

                #region Đọc file ảnh và file dữ liệu xử lí -> xử lí cắt/vẽ đường phân chia lên ảnh rời -> lưu ảnh rời đã qua xử lí vào output
                for (int i = 0; i < parametters.getCamIndex(); i++)
                {
                    if (("" == inputDirectory[i] || "" == outputDirectory[i] || "" == txtDirectory[i]))
                    {
                        continue;
                    }

                    // Đọc file ảnh và file dữ liệu xử lí
                    int[] coefficient = { 0, 0, 0, 0 };
                    decoder(txtDirectory[i], coefficient);
                    Mat img = Cv2.ImRead(inputDirectory[i]);

                    #region Xử lí cắt/vẽ đường phân chia lên ảnh rời
                    // cắt ảnh theo thông số chiều ngang
                    img = drawFunctions.cropingImageVerticly(img,coefficient[2], coefficient[3]); 

                    // vẽ đường chia ảnh theo thông số chiều ngang
                    img = drawFunctions.drawingSeparateLine(img, coefficient[2], 50, 10);
                    img = drawFunctions.drawingSeparateLine(img, coefficient[3], 50, 10);
                    #endregion

                    //cài đặt thông số chiều cao của container
                    opencvProcess.setContainerHeight(coefficient[0] - coefficient[1]);

                    // lưu ảnh rời đã qua xử lí vào output
                    Cv2.ImWrite(outputDirectory[i], img);
                }
                #endregion

                #region Ghép các ảnh ở output vào làm 1
                Mat combineImg = new Mat();
                for (int i = 0; i < parametters.getCamIndex(); i++)
                {
                    if ("" == outputDirectory[i])
                    {
                        continue;
                    }

                    //Quá trình ghép ảnh
                    if (0 == i)
                    {
                        combineImg = Cv2.ImRead(outputDirectory[i]); // xử lí ảnh đầu tiên
                    }
                    else
                    {
                        combineImg = drawFunctions.addTwoImages(combineImg, Cv2.ImRead(outputDirectory[i])); // xử lí các ảnh kế tiếp
                    }
                }
                #endregion

                Cv2.ImWrite(resultDirectory, combineImg); //lưu ảnh vào vị trí đã chọn
                return combineImg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// Thiết lập các thông số cắt ảnh bằng thư viện opencv
        /// </summary>
        /// <param name="inputDirectory"></param>
        /// <param name="outputDirectory"></param>
        /// <param name="txtDirectory"></param>
        /// <returns></returns>
        private bool settingCoefficient(string[] inputDirectory, string[] outputDirectory, string[] txtDirectory)
        {
            if (inputDirectory.Length != outputDirectory.Length || outputDirectory.Length != txtDirectory.Length)
            {
                Console.WriteLine("Directory arrays must have same size");
                return false;
            }

            for (int i = 0; i < parametters.getCamIndex(); i++)
            {
                if (("" == inputDirectory[i] || "" == outputDirectory[i] || "" == txtDirectory[i]))
                {
                    continue;
                }

                if (false == drawFunctions.drawingLoop(Cv2.ImRead(inputDirectory[i]), outputDirectory[i], txtDirectory[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Thay đổi kích thước ảnh
        /// </summary>
        /// <param name="image">The image to resize.</param> ảnh đầu vào
        /// <param name="width">The width to resize to.</param> thông số chiều dài (pixels)
        /// <param name="height">The height to resize to.</param> thông số chiều rộng (pixels)
        /// <returns>The resized image.</returns> trả về ảnh đã resize
        private Bitmap ResizeImage(Image image, int width, int height)
        {
            Bitmap resize = resizeImage(image, new System.Drawing.Size(width, height));
            return resize;
        }

        private static Bitmap resizeImage(Image imgToResize, System.Drawing.Size size)
        {
            //Get the image current width  
            int sourceWidth = imgToResize.Width;
            //Get the image current height  
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size  
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size  
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width  
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height  
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return b;
        }

        /// <summary>
        /// Chuyển ảnh thành dạng Base64String -> byte để gửi đi qua TCP socket
        /// </summary>
        /// <param name="linkOfResult"></param>
        /// <returns></returns>
        private bool sendingResult(string linkOfResult)
        {
            try
            {
                int sWidth = 800;
                int sHight = 600;
                // Chuyển ảnh thành base64String
                Bitmap bmp = new Bitmap(@linkOfResult);
                bmp = ResizeImage(bmp, sWidth, sHight);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] byteImage = ms.ToArray();
                string img = Convert.ToBase64String(byteImage);

                // Chuyển base64String thành byte[]
                byte[] sendBytes = Convert.FromBase64String(img);

                // Lưu kết quả vào biến lưu trữ (sẽ được truy cập tại hàm gửi sau này)
                parametters.setCommandResult(sendBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Hàm quản lí và thực hiện các lệnh
        /// </summary>
        public void serviceLoop()
        {
            while (true)
            {
                if ("none" != parametters.getCommand())
                {
                    Console.WriteLine(" >>Command: {0}", parametters.getCommand());
                }

                switch (parametters.getCommand())
                {
                    case "capture": // lệnh chụp ảnh
                        AutoCapturingPhotosFromCamera(parametters.getCamLink(), parametters.getCapturedImageLink());
                        parametters.setCommand("none");
                        break;
                    case "auto": // lệnh tự động xử lí ảnh
                        autoSolvingImages(parametters.getCapturedImageLink(),
                                            parametters.getOutputImageLink(),
                                            parametters.getTxtLink(),
                                            parametters.getResultLink()
                                         );
                        //opencvProcess.fullProcess(parametters.getResultLink());
                        parametters.setCommand("none");
                        break;
                    case "set": // lệnh cài đặt thông số
                        settingCoefficient(parametters.getCapturedImageLink(),
                                            parametters.getOutputImageLink(),
                                            parametters.getTxtLink()
                                          );
                        parametters.setCommand("none");
                        break;
                    case "get": // lệnh gửi ảnh qua tcp
                        if (sendingResult(parametters.getResultLink()))
                        {
                            // Làm gì đó khi get ảnh thành công
                        }
                        parametters.setCommand("none");
                        break;
                    default:
                        parametters.setCommand("none");
                        break;
                }
                if (parametters.killThread)
                {
                    Console.WriteLine("Say goodbye to {0}", Thread.CurrentThread.Name.ToString());
                    break;
                }
                Thread.Sleep(200);
            }
        }
    }
}