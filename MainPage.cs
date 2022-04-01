using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using container_capturer.scrip_lib;
using System.Net;

namespace container_capturer
{
    public partial class MainPage : Form
    {
        private Thread MainPageThread;

        /// <summary>
        /// hàm chạy trong main của chương trình
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            // Tạo thread của winform
            MainPageThread = new Thread(MainPageLoop);
            MainPageThread.Name = "MainPageThread";
            MainPageThread.IsBackground = true;
            MainPageThread.Start();

            // Enable/ disable quyền thay đổi text trong textbox của camera link
            if (textBoxImgLink1.Text == "")
            {
                textBoxImgLink1.ReadOnly = false;
                textBoxImgLink2.ReadOnly = true;
                textBoxImgLink3.ReadOnly = true;
            }

            // Disable nút thay đổi TCP/IP
            changeButton.Enabled = false;
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            //Tạo thread xử lí ảnh
            autoFunctions auto = new autoFunctions();
            Thread autoControlThread = new Thread(auto.serviceLoop);
            autoControlThread.Name = "AutoControlThread";
            autoControlThread.IsBackground = true;
            autoControlThread.Start();

            //Tạo thread nhận lệnh tcp
            tcpServer myTcp = new tcpServer();
            Thread tcpIpControlThread = new Thread(myTcp.protocolLoop);
            tcpIpControlThread.Name = "TCPControlThread";
            tcpIpControlThread.IsBackground = true;
            tcpIpControlThread.Start();
        }

        private void MainPage_Leave(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Quản lí các thông tin trên bảng điều khiển
        /// </summary>
        private void MainPageLoop()
        {
            while (true)
            {
                try {
                    // Hiển thị cho nút lưu camera links
                    if (textBoxImgLink1.InvokeRequired && setLink1.InvokeRequired)
                    {
                        String txtTemp = "";

                        textBoxImgLink1.Invoke(new MethodInvoker(delegate { txtTemp = textBoxImgLink1.Text; }));

                        setLink1.Invoke(new MethodInvoker(delegate 
                        { 
                            if (txtTemp == parametters.getCamLink()[0])
                            {
                                setLink1.BackColor = Color.LightGray;
                            }
                            else { setLink1.BackColor = Color.LightPink; }                        
                        }));
                    }
                    // Hiển thị cho nút lưu camera links
                    if (textBoxImgLink2.InvokeRequired && setLink2.InvokeRequired)
                    {
                        String txtTemp = "";

                        textBoxImgLink2.Invoke(new MethodInvoker(delegate { txtTemp = textBoxImgLink2.Text; }));

                        setLink2.Invoke(new MethodInvoker(delegate
                        {
                            if (txtTemp == parametters.getCamLink()[1])
                            {
                                setLink2.BackColor = Color.LightGray;
                            }
                            else { setLink2.BackColor = Color.LightPink; }
                        }));
                    }
                    // Hiển thị cho nút lưu camera links
                    if (textBoxImgLink3.InvokeRequired && setLink3.InvokeRequired)
                    {
                        String txtTemp = "";

                        textBoxImgLink3.Invoke(new MethodInvoker(delegate { txtTemp = textBoxImgLink3.Text; }));

                        setLink3.Invoke(new MethodInvoker(delegate
                        {
                            if (txtTemp == parametters.getCamLink()[2])
                            {
                                setLink3.BackColor = Color.LightGray;
                            }
                            else { setLink3.BackColor = Color.LightPink; }
                        }));
                    }

                    // Ẩn các nút lệnh đến khi quá trình setup kết thúc
                    if (captureCommand.InvokeRequired && setCommand.InvokeRequired && AutoComand.InvokeRequired)
                    {
                        bool enable = false;
                        if ((parametters.getCamIndex() == 0) || (parametters.getResultLink() == ""))
                             {enable = false;}
                        else {enable = true; }

                        captureCommand.Invoke(new MethodInvoker(delegate 
                        {
                            if (parametters.getCommand() == "capture") { captureCommand.BackColor = Color.LightGreen; }
                            else { captureCommand.BackColor = Color.LightGray;}
                            captureCommand.Enabled = enable;
                        }));

                        setCommand.Invoke(new MethodInvoker(delegate 
                        {
                            if (parametters.getCommand() == "set") { setCommand.BackColor = Color.LightGreen; }
                            else { setCommand.BackColor = Color.LightGray; }
                            setCommand.Enabled = enable;
                        }));

                        AutoComand.Invoke(new MethodInvoker(delegate {
                            if (parametters.getCommand() == "auto") { AutoComand.BackColor = Color.LightGreen; }
                            else { AutoComand.BackColor = Color.LightGray; }
                            AutoComand.Enabled = enable;
                        }));
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                // Hủy thread khi có lệnh
                if (parametters.killThread)
                {
                    Console.WriteLine("Say goodbye to {0}", Thread.CurrentThread.Name.ToString());
                    break;
                }
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Setup đường dẫn kết nối đến camera số 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setLink1_Click(object sender, EventArgs e)
        {
            parametters.setCamLink(textBoxImgLink1.Text, 0);

            //Cho phép nhập dữ liệu vào hàng kế tiếp 
            if (textBoxImgLink2.Text == "" && textBoxImgLink1.Text != "")
            {
                //Cho phép nhập dữ liệu vào hàng 2 (nếu hàng 2 trống)
                textBoxImgLink1.ReadOnly = true;
                textBoxImgLink2.ReadOnly = false;
                textBoxImgLink3.ReadOnly = true;
            }

        }

        /// <summary>
        /// Setup đường dẫn kết nối đến camera số 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setLink2_Click(object sender, EventArgs e)
        {
            parametters.setCamLink(textBoxImgLink2.Text, 1);

            if (textBoxImgLink2.Text == "")
            {

                //Cho phép nhập dữ liệu vào hàng 1 (nếu hàng 2 trống)
                textBoxImgLink1.ReadOnly = false;
                textBoxImgLink2.ReadOnly = true;
                textBoxImgLink3.ReadOnly = true;
            }
            else if (textBoxImgLink3.Text == "")
            {
                //Cho phép nhập dữ liệu vào hàng 3 (nếu hàng 3 trống)
                textBoxImgLink1.ReadOnly = true;
                textBoxImgLink2.ReadOnly = true;
                textBoxImgLink3.ReadOnly = false;
            }
        }

        /// <summary>
        /// Setup đường dẫn kết nối đến camera số 3 (nếu có)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setLink3_Click(object sender, EventArgs e)
        {
            parametters.setCamLink(textBoxImgLink3.Text, 2);
            if (textBoxImgLink3.Text == "")
            {
                //Cho phép nhập dữ liệu vào hàng 2 (Nếu hàng 3 trống)
                textBoxImgLink1.ReadOnly = true;
                textBoxImgLink2.ReadOnly = false;
                textBoxImgLink3.ReadOnly = true;
            }
        }

        /// <summary>
        /// Setup đường dẫn thư mục lưu các ảnh chụp về và đã/đang xử lí
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setFoder_Click(object sender, EventArgs e)
        {
            if (imageFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                parametters.setImageFolder(imageFolderBrowserDialog.SelectedPath);
                imageFolder.Text = imageFolderBrowserDialog.SelectedPath;
                setLink1.Enabled = false;
                setLink2.Enabled = false;
                setLink3.Enabled = false;
            }
        }

        /// <summary>
        /// Chạy lệnh "capture" - chụp ảnh từ camera theo các đường dẫn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void captureCommand_Click(object sender, EventArgs e)
        {
            if (parametters.setCommand("capture") == false)
                MessageBox.Show("Setting command failed");            
        }

        /// <summary>
        /// Chạy lệnh "set" - thiết lập thông số trong cắt-ghép ảnh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setCommand_Click(object sender, EventArgs e)
        {
            if (parametters.setCommand("set") == false)
            {
                MessageBox.Show("Setting command failed");
            }
            else
            {
                MessageBox.Show("Use mouse to draw zone \n" +
                                "Press c to crop \n" +
                                "Press s to save \n" +
                                "Press q to quit","Help");
            }
        }

        /// <summary>
        /// Chạy lệnh "auto" - tự động cắt ghép ảnh theo thông số đã lưu trong lệnh "set"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoComand_Click(object sender, EventArgs e)
        {
            if (parametters.setCommand("auto") == false)
            {
                MessageBox.Show("Setting command failed");
            }
        }

        /// <summary>
        /// Thiết lập sever tcp nhận lệnh từ các clients theo các thông số đã nhập
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createSeverButton_Click(object sender, EventArgs e)
        {
            byte[] ipData = new byte[4];
            int portData = 0;
            try
            {
                //Lấy dữ liệu
                ipData[0] = byte.Parse(textIp0.Text);
                ipData[1] = byte.Parse(textIp1.Text);
                ipData[2] = byte.Parse(textIp2.Text);
                ipData[3] = byte.Parse(textIp3.Text);
                portData = Int16.Parse(portSeverText.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
                return;
            }

            parametters.setSeverIP(new IPAddress(ipData));
            parametters.setSeverPort(portData);
            parametters.setConnectingFlag(true);
            textIp0.ReadOnly = true;
            textIp1.ReadOnly = true;
            textIp2.ReadOnly = true;
            textIp3.ReadOnly = true;
            portSeverText.ReadOnly = true;
            CreateSeverButton.Enabled = false;
            changeButton.Enabled = true;
        }

        /// <summary>
        /// Thay đổi địa chỉ và cổng của sever đang chạy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeButton_Click(object sender, EventArgs e)
        {
            textIp0.ReadOnly = false;
            textIp1.ReadOnly = false;
            textIp2.ReadOnly = false;
            textIp3.ReadOnly = false;
            portSeverText.ReadOnly = false;
            CreateSeverButton.Enabled = true;
            changeButton.Enabled = false;
        }

        /// <summary>
        /// Show ra màn hình hình ảnh sau khi đã qua xử lí
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowImageButton_Click(object sender, EventArgs e)
        {
            try
            {
                Image tempImage = Image.FromFile(parametters.getResultLink());
                Bitmap tempBitmap = new Bitmap(tempImage);
                pictureBox1.Image = tempBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Đặt lệnh thoát khỏi các thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_Deactivate(object sender, EventArgs e)
        {
            parametters.killThread = true;
        }
    }
}