using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMClient
{
    /// <summary>
    /// 约定
    /// </summary>
    public enum Protocol
    {
        Client端发送文件名 = 0,
        Client端发送数据块 = 1,
        Client端发送最后一个数据块 = 2,

        Server端本次接收完毕 = 3,
        Server端结束 = 4
    }
}
