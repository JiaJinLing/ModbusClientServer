using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Security.Permissions;
using System.IO.Ports;
using System.Security;

namespace Utils
{
    /// <summary>
    /// 串口资源工具类
    /// </summary>
    public class SerialPortUtil
    {
        #region 获取本机串口列表，包括虚拟串口
        /// <summary>
        /// 获取本机串口列表，包括虚拟串口
        /// </summary>
        public static string[] GetCOMList()
        {
            List<string> list = new List<string>();

            foreach (string portName in SerialPort.GetPortNames())
            {
                list.Add(portName);
            }

            return list.ToArray();
        }
        #endregion

        #region 从注册表获取本机串口列表
        /// <summary>
        /// 从注册表获取本机串口列表
        /// </summary>
        public static string[] GetPortNames()
        {
            RegistryKey localMachine = null;
            RegistryKey key2 = null;
            string[] textArray = null;

            //这里有个断言，判断该注册表项是否存在
            new RegistryPermission(RegistryPermissionAccess.Read, @"HKEY_LOCAL_MACHINE\HARDWARE\DEVICEMAP\SERIALCOMM").Assert();

            try
            {
                localMachine = Registry.LocalMachine;
                key2 = localMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM", false);
                if (key2 != null)
                {
                    string[] valueNames = key2.GetValueNames();
                    textArray = new string[valueNames.Length];
                    for (int i = 0; i < valueNames.Length; i++)
                    {
                        textArray[i] = (string)key2.GetValue(valueNames[i]);
                    }
                }
            }
            finally
            {
                if (localMachine != null)
                {
                    localMachine.Close();
                }
                if (key2 != null)
                {
                    key2.Close();
                }
                CodeAccessPermission.RevertAssert();
            }
            if (textArray == null)
            {
                textArray = new string[0];
            }
            return textArray;
        }
        #endregion

    }
}
