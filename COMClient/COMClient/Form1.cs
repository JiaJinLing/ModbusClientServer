using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace COMClient
{
    public partial class Form1 : Form
    {
        #region 变量
        /// <summary>
        /// 串口资源
        /// </summary>
        private static SerialPort serialPort = null;
        /// <summary>
        /// 文件
        /// </summary>
        private static FileStream fs = null;
        private static long index = 0;
        private static long blockCount;
        private static int size = 4095;
        private static DateTime dt;
        #endregion

        #region Form1
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region Form1_Load
        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort = new SerialPort("COM1");
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            try
            {
                serialPort.Open();
            }
            catch
            {

            }

        }
        #endregion

        #region btnConn_Click
        private void btnConn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                dt = DateTime.Now;
                fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                blockCount = (fs.Length - 1) / size + 1;

                List<byte> bList = new List<byte>();
                bList.Add((int)Protocol.Client端发送文件名);
                bList.AddRange(ASCIIEncoding.UTF8.GetBytes(openFileDialog1.FileName));
                byte[] bArr = bList.ToArray();
                serialPort.Write(bArr, 0, bArr.Length);
            }
        }
        #endregion

        /// <summary>
        /// 接收串口数据事件
        /// </summary>
        public void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort.BytesToRead == 0) return;

            int bt = serialPort.ReadByte();

            switch (bt)
            {
                case (int)Protocol.Server端本次接收完毕:
                    if (index != blockCount - 1)
                    {
                        byte[] bArr = new byte[size + 1];
                        bArr[0] = (int)Protocol.Client端发送数据块;
                        fs.Read(bArr, 1, size);
                        index++;
                        serialPort.Write(bArr, 0, bArr.Length);
                    }
                    else
                    {
                        byte[] bArr = new byte[fs.Length - (size * index) + 1];
                        bArr[0] = (int)Protocol.Client端发送最后一个数据块;
                        fs.Read(bArr, 1, bArr.Length - 1);
                        index++;
                        serialPort.Write(bArr, 0, bArr.Length);
                    }
                    break;
                case (int)Protocol.Server端结束:
                    index = 0;
                    double totalSeconds = DateTime.Now.Subtract(dt).TotalSeconds;
                    MessageBox.Show("完成，耗时" + totalSeconds + "秒，速度" + (fs.Length / 1024.0 / totalSeconds).ToString("#.0") + "KB/S");
                    fs.Close();
                    fs.Dispose();
                    break;
            }
        }

    }
}
