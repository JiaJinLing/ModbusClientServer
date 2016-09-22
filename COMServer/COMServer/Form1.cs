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

namespace COMServer
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
            serialPort = new SerialPort("COM2");
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

        /// <summary>
        /// 接收串口数据事件
        /// </summary>
        public void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort.BytesToRead == 0) return;

            #region 接收数据
            int bt = serialPort.ReadByte();
            List<byte> bList = new List<byte>();
            while (serialPort.BytesToRead > 0)
            {
                byte[] bArr = new byte[serialPort.BytesToRead];
                serialPort.Read(bArr, 0, bArr.Length);
                bList.AddRange(bArr);
            }
            byte[] receiveData = bList.ToArray();
            #endregion

            switch (bt)
            {
                case (int)Protocol.Client端发送文件名:
                    string path = ASCIIEncoding.UTF8.GetString(receiveData);
                    string fileName = Path.GetFileName(path);
                    fs = new FileStream(@"d:\_临时文件\COM测试" + fileName, FileMode.Create, FileAccess.Write);
                    byte[] bArr = new byte[1];
                    bArr[0] = (int)Protocol.Server端本次接收完毕;
                    serialPort.Write(bArr, 0, bArr.Length);
                    break;
                case (int)Protocol.Client端发送数据块:
                    fs.Write(receiveData, 0, receiveData.Length);//通过串口对象将数据传送至Modbus客户端
                    fs.Flush();
                    bArr = new byte[1];
                    bArr[0] = (int)Protocol.Server端本次接收完毕;
                    serialPort.Write(bArr, 0, bArr.Length);
                    break;
                case (int)Protocol.Client端发送最后一个数据块:
                    fs.Write(receiveData, 0, receiveData.Length);
                    fs.Flush();
                    bArr = new byte[1];
                    bArr[0] = (int)Protocol.Server端结束;
                    serialPort.Write(bArr, 0, bArr.Length);
                    fs.Close();
                    fs.Dispose();
                    break;
            }
        }

    }
}
