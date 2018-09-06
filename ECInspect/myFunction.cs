﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Euresys.Open_eVision_1_2;
using System.Drawing.Imaging;

namespace ECInspect
{
    class myFunction
    {
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(
                            IntPtr hwnd,
                            int wMsg,
                            IntPtr wParam,
                            IntPtr lParam);

        //[DllImport("User32.dll", EntryPoint = "SendMessage")]
        //public static extern int SendMessage(int hWnd, int Msg, int wParam, ref Adapter.COPYDATASTRUCT lParam);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int Index);

        /// <summary>
        /// 函数锁定键盘及鼠标
        /// </summary>
        /// <param name="Block"></param>
        [DllImport("user32.dll")]
        internal static extern void BlockInput(bool Block);

        /// <summary>
        /// 调用windows的系统锁定
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="fEnable"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "EnableWindow")]
        internal static extern long EnableWindow(IntPtr hwnd, bool fEnable);

        Logs log = Logs.LogsT();

        #region 配置文件
        //配置文件的路径
        private static string GetConfigIniPath()
        {
            string dllpath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            dllpath = dllpath.Substring(8, dllpath.Length - 8);    // 8是 file:// 的长度
            char sep = System.IO.Path.DirectorySeparatorChar;
            return System.IO.Path.GetDirectoryName(dllpath) + sep + "Config" + sep + "SANTEC.ini";
        }

        //配置文件的读取
        public static bool GetIniString(string section, string key, out string value)
        {
            string iniPath = GetConfigIniPath();
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, "", sb, 1024, iniPath);
            value = sb.ToString();
            if (value.Length > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="section">Section范围</param>
        /// <param name="key">Key关键字</param>
        /// <param name="value">值</param>
        public static void WriteIniString(string section, string key, string value)
        {
            string iniPath = GetConfigIniPath();
            WritePrivateProfileString(section, key, value, iniPath);
        }
        #endregion

        #region 读取SANTEC.ini文件
        internal void ReadSANTECIni(string FileName)
        {
            string lastfile = string.Empty;//最新的文件
            string dllpath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            dllpath = dllpath.Substring(8, dllpath.Length - 8);    // 8是 file:// 的长度
            char sep = System.IO.Path.DirectorySeparatorChar;
            string dir = System.IO.Path.GetDirectoryName(dllpath) + sep + GlobalVar.Folder_Config + sep;
            if (!Directory.Exists(dir)) throw new Exception("读取SANTEC.ini的路径不存在");
            lastfile = dir + FileName;

            ReadIni_Value(lastfile, INIFileValue.gl_inisection_COM, INIFileValue.gl_iniKey_EC_COM, ref INIFileValue.EC_COM);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_COM, INIFileValue.gl_iniKey_PLC_COM, ref INIFileValue.PLC_COM);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_COM, INIFileValue.gl_iniKey_ReadCard_COM, ref INIFileValue.ReadCard_COM);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_COM, INIFileValue.gl_iniKey_Scan_COM, ref INIFileValue.Scan_COM);
            int Max, Min;
            Max = Min = 0;
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_AXIS, INIFileValue.gl_iniKey_DotAxis_Max, ref Max);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_AXIS, INIFileValue.gl_iniKey_DotAxis_Min, ref Min);
            INIFileValue.DotAxisRange = new Range_Int(Max, Min);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_AXIS, INIFileValue.gl_iniKey_XAxis_Max, ref Max);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_AXIS, INIFileValue.gl_iniKey_XAxis_Min, ref Min);
            INIFileValue.XAxisRange = new Range_Int(Max, Min);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_AXIS, INIFileValue.gl_iniKey_YAxis_Max, ref Max);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_AXIS, INIFileValue.gl_iniKey_YAxis_Min, ref Min);
            INIFileValue.YAxisRange = new Range_Int(Max, Min);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_AXIS, INIFileValue.gl_iniKey_CarryAxis_Max, ref Max);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_AXIS, INIFileValue.gl_iniKey_CarryAxis_Min, ref Min);
            INIFileValue.CarryAxisRange = new Range_Int(Max, Min);

            StringBuilder tmp = new StringBuilder(1024);
            GetPrivateProfileString(INIFileValue.gl_inisection_AXIS, INIFileValue.gl_iniKey_CCDAxis_Max, "420", tmp, 1024, lastfile);
            Max = int.Parse(tmp.ToString());
            tmp = new StringBuilder(1024);
            //GetPrivateProfileString(INIFileValue.gl_inisection_AXIS, INIFileValue.gl_iniKey_Sin, "0", tmp, 1024, lastfile);
            //Min = int.Parse(tmp.ToString());

            INIFileValue.CCDAxisRange = new Range_Int(Max, Min);
            
            //ReadIni_Value(lastfile, INIFileValue.gl_inisection_CCD, INIFileValue.gl_iniKey_AxisX_Std, ref INIFileValue.AxisX_Std);
            //ReadIni_Value(lastfile, INIFileValue.gl_inisection_CCD, INIFileValue.gl_iniKey_AxisY_Std, ref INIFileValue.AxisY_Std);

            ReadIni_Value(lastfile, INIFileValue.gl_inisection_OpeneVision, INIFileValue.gl_iniKey_MatchFile, ref INIFileValue.FindModelFile);

            ReadIni_Value(lastfile, INIFileValue.gl_inisection_CCD, INIFileValue.gl_iniKey_MarkX_Std, ref INIFileValue.MarkX_Std);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_CCD, INIFileValue.gl_iniKey_MarkY_Std, ref INIFileValue.MarkY_Std);

            ReadIni_Value(lastfile, INIFileValue.gl_inisection_CCD, INIFileValue.gl_iniKey_CCDSN, ref INIFileValue.CCDSN);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_BarcodeGun, INIFileValue.gl_iniKey_BarcodeGunEnable, ref INIFileValue.BarcodeGunEnable, true);
            ReadIni_Value(lastfile, INIFileValue.gl_inisection_BarcodeScan, INIFileValue.gl_iniKey_BarcodeScanEnable, ref INIFileValue.BarcodeScanEnable, true);

            ReadIni_Value(lastfile,INIFileValue.gl_inisection_UpdateSql,INIFileValue.gl_iniKey_FlowId,ref INIFileValue.FlowID);

            //获取角度补正值 [2018.3.28 lqz]
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(INIFileValue.gl_inisection_Tan, INIFileValue.gl_iniKey_Dot_Coordinate_Angle, "0", sb, 1024, lastfile);
            GlobalVar.Dot_Coordinate_Radian = double.Parse(sb.ToString());
            sb = new StringBuilder(1024);
            GetPrivateProfileString(INIFileValue.gl_inisection_UpdateSql, INIFileValue.gl_iniKey_UpdateSql, "false", sb, 1024, lastfile);
            GlobalVar.gl_UpSql = bool.Parse(sb.ToString());
            sb = new StringBuilder(1024);
            GetPrivateProfileString(INIFileValue.gl_inisection_UpdateSql, INIFileValue.gl_iniKey_SqlName, "A82TFLEX", sb, 1024, lastfile);
            GlobalVar.gl_str_product = sb.ToString();

            sb = new StringBuilder(1024);
            GetPrivateProfileString(INIFileValue.gl_inisection_CCDOrigin, INIFileValue.gl_iniKey_CCDOrigin_X, "", sb, 1024, lastfile);
            GlobalVar.gl_origin_X = double.Parse(sb.ToString());
            sb = new StringBuilder(1024);
            GetPrivateProfileString(INIFileValue.gl_inisection_CCDOrigin, INIFileValue.gl_iniKey_CCDOrigin_Y, "", sb, 1024, lastfile);
            GlobalVar.gl_origin_Y = double.Parse(sb.ToString());



        }
        #endregion

        #region 读取Product.ini文件
        /// <summary>
        /// 读取Product.ini文件
        /// </summary>
        internal void ReadProductIni()
        {
            string loadfile = string.Empty;
            string dllpath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            dllpath = dllpath.Substring(8, dllpath.Length - 8);    // 8是 file:// 的长度
            char sep = System.IO.Path.DirectorySeparatorChar;
            string dir = System.IO.Path.GetDirectoryName(dllpath) + sep + GlobalVar.Folder_Config + sep;
            if (!Directory.Exists(dir)) throw new Exception(string.Format("读取{0}的路径不存在", GlobalVar.File_LOAD));
            loadfile = dir + GlobalVar.File_LOAD;

            ReadIni_Value(loadfile, INIFileValue.gl_inisection_STEP, INIFileValue.gl_iniKey_N, ref INIFileValue.Product_N);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_STEP, INIFileValue.gl_iniKey_MAC_XY, ref INIFileValue.Product_MAC_XY, true);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_PRODUCT, INIFileValue.gl_iniKey_NAME, ref INIFileValue.Product_NAME);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_PRODUCT, INIFileValue.gl_iniKey_GROUP, ref INIFileValue.Product_GROUP);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_PRODUCT, INIFileValue.gl_iniKey_OKURI, ref INIFileValue.Product_OKURI);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_PRODUCT, INIFileValue.gl_iniKey_PITCH, ref INIFileValue.Product_PITCH);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_PRODUCT, INIFileValue.gl_iniKey_TOMBO, ref INIFileValue.Product_TOMBO);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_PRODUCT, INIFileValue.gl_iniKey_TRL, ref INIFileValue.Product_TRL);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_PRODUCT, INIFileValue.gl_iniKey_NK, ref INIFileValue.Product_NK);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_OFFSET, INIFileValue.gl_iniKey_JIGU_OFFSET, ref INIFileValue.Product_JIGU_OFFSET);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_OFFSET, INIFileValue.gl_iniKey_SEARCH_OFFSET, ref INIFileValue.Product_SEARCH_OFFSET);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_POINTSET, INIFileValue.gl_iniKey_AL_MARK, ref INIFileValue.Product_AL_MARK);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_POINTSET, INIFileValue.gl_iniKey_AL_MODE, ref INIFileValue.Product_AL_MODE);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_POINTSET, INIFileValue.gl_iniKey_AL_FINE, ref INIFileValue.Product_AL_FINE);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_POINTSET, INIFileValue.gl_iniKey_AL_NUM, ref INIFileValue.Product_AL_NUM);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_MARKSET, INIFileValue.gl_iniKey_STAMP_ENABLE, ref INIFileValue.Product_STAMP_ENABLE);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_MARKSET, INIFileValue.gl_iniKey_STAMP_ON, ref INIFileValue.Product_STAMP_ON);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_MARKSET, INIFileValue.gl_iniKey_AIR_BROW, ref INIFileValue.Product_AIR_BROW);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_MARKSET, INIFileValue.gl_iniKey_STAMP_TON, ref INIFileValue.Product_STAMP_TON);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_VIEWSET, INIFileValue.gl_iniKey_VIEW_MODE, ref INIFileValue.Product_VIEW_MODE);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_VIEWSET, INIFileValue.gl_iniKey_VIEW_MARK_X, ref INIFileValue.Product_VIEW_MARK_X);
            ReadIni_Value(loadfile, INIFileValue.gl_inisection_VIEWSET, INIFileValue.gl_iniKey_VIEW_MARK_Y, ref INIFileValue.Product_VIEW_MARK_Y);

            ReadIni_Value(loadfile, INIFileValue.gl_inisection_Image, INIFileValue.gl_iniKey_CM_MODE, ref INIFileValue.Product_CM_MODE, true);
            if (INIFileValue.Product_CM_MODE > 0)//需要使用相机时，才读取并使用下列参数
            {
                ReadIni_Value(loadfile, INIFileValue.gl_inisection_Image, INIFileValue.gl_iniKey_CM_DELAY, ref INIFileValue.Product_CM_DELAY);
                ReadIni_Value(loadfile, INIFileValue.gl_inisection_Image, INIFileValue.gl_iniKey_CM_RETRY, ref INIFileValue.Product_CM_RETRY);
                ReadIni_Value(loadfile, INIFileValue.gl_inisection_PRODUCT, INIFileValue.gl_iniKey_MARK_X_STD, ref INIFileValue.Product_MARK_X_STD);
                ReadIni_Value(loadfile, INIFileValue.gl_inisection_PRODUCT, INIFileValue.gl_iniKey_MARK_Y_STD, ref INIFileValue.Product_MARK_Y_STD);

            }

            /***读取坐标，因为没有key，采取全部读取的方式****/
            string[] _iniText = File.ReadAllLines(loadfile, Encoding.Default);
            bool Add = false;
            string X = "X";
            string Y = "Y";
            int Index = 0;//第几个区块对应的打点位置
            List<Point> mark = new List<Point>();//一个区块对应的打点位置
            foreach (string str in _iniText)
            {
                if (str.Contains(INIFileValue.gl_inisection_LOCATIONSET))
                {
                    Add = true;
                    continue;
                }
                else if (str.Contains("EOF"))
                {
                    Add = false;
                    break;
                }

                if (Add)
                {
                    int _X;
                    int _Y;
                    double tempd;//临时存储变量
                    if (str.Contains(X.ToUpper()))
                    {
                        if (double.TryParse(str.Substring(str.IndexOf(X.ToUpper()) + 1, str.IndexOf(Y.ToUpper()) - str.IndexOf(X.ToUpper()) - 1), out tempd))
                            _X = (int)(tempd * 10);
                        else throw new Exception("读取到的打点坐标X不是数字");
                        if (double.TryParse(str.Substring(str.IndexOf(Y.ToUpper()) + 1), out tempd))
                            _Y = (int)(tempd * 10);
                        else throw new Exception("读取到的打点坐标Y不是数字");

                        mark.Add(new Point(_X, _Y));//打点的坐标
                    }
                    else if (str.Contains(X.ToLower()))
                    {
                        if (double.TryParse(str.Substring(str.IndexOf(X.ToLower()) + 1, str.IndexOf(Y.ToLower()) - str.IndexOf(X.ToLower()) - 1), out tempd))
                            _X = (int)tempd;
                        else throw new Exception("读取到的显示区块坐标X不是数字");
                        if (double.TryParse(str.Substring(str.IndexOf(Y.ToLower()) + 1), out tempd))
                            _Y = (int)tempd;
                        else throw new Exception("读取到的显示区块坐标Y不是数字");

                        INIFileValue.BlockPoint.Add(new Point(_X, _Y));

                        INIFileValue.MarkPoint.Add(Index++, mark.ToArray());//打点序号、打点的位置
                        mark.Clear();
                    }
                }
            }
        }

        //配置文件的读取
        private bool GetIniString(string iniPath, string section, string key, out string value)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, "", sb, 1024, iniPath);
            value = sb.ToString();
            if (value.Length > 0)
                return true;
            else
                return false;
        }

        private void ReadIni_Value(string path, string section, string Key, ref string param, bool IgnoreErr = false)
        {
            string value = string.Empty;
            if (GetIniString(path, section, Key, out value))
            {
                param = value;
            }
            else
            {
                if (!IgnoreErr) throw new Exception(string.Format("{0} {1}\t 参数读取失败", path, Key));
            }
        }

        private void ReadIni_Value(string path, string section, string Key, ref int param, bool IgnoreErr = false)
        {
            string value = string.Empty;
            if (GetIniString(path, section, Key, out value))
            {
                int temp = 0;
                if (int.TryParse(value, out temp)) param = temp;
                else throw new Exception(string.Format("{0} {1}\t 参数读取成功,转换失败", path, Key));
            }
            else
            {
                if (!IgnoreErr) throw new Exception(string.Format("{0} {1}\t 参数读取失败", path, Key));
            }
        }

        private void ReadIni_Value(string path, string section, string Key, ref double param, bool IgnoreErr = false)
        {
            string value = string.Empty;
            if (GetIniString(path, section, Key, out value))
            {
                double temp = 0;
                if (double.TryParse(value, out temp)) param = temp;
                else throw new Exception(string.Format("{0} {1}\t 参数读取成功,转换失败", path, Key));
            }
            else
            {
                if (!IgnoreErr) throw new Exception(string.Format("{0} {1}\t 参数读取失败", path, Key));
            }
        }

        private void ReadIni_Value(string path, string section, string Key, ref bool param, bool IgnoreErr = false)
        {
            string value = string.Empty;
            if (GetIniString(path, section, Key, out value))
            {
                bool temp = false;
                if (bool.TryParse(value, out temp)) param = temp;
                else throw new Exception(string.Format("{0} {1}\t 参数读取成功,转换失败", path, Key));
            }
            else
            {
                if (!IgnoreErr) throw new Exception(string.Format("{0} {1}\t 参数读取失败", path, Key));
            }
        }
        #endregion
        #region 读取mapping.ini文件
        internal void ReadMapping()
        {
            if (INIFileValue.FlowID == "35") INIFileValue.MType = "TO2";
            else INIFileValue.MType = "TO1";
            if (!(new DBQuery().AcuqaintPcs()))
            {
                string fileName = GlobalVar.gl_str_product + ".ini";
                string lastfile = string.Empty;//最新的文件
                string dllpath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                dllpath = dllpath.Substring(8, dllpath.Length - 8);    // 8是 file:// 的长度
                char sep = System.IO.Path.DirectorySeparatorChar;
                string dir = System.IO.Path.GetDirectoryName(dllpath) + sep + GlobalVar.Folder_Config + sep + "MAPPING" + sep;
                lastfile = dir + fileName;
                if (!Directory.Exists(dir)) throw new Exception(string.Format("读取{0}的MAPPING文件路径不存在", GlobalVar.gl_str_product));
                if (INIFileValue.Product_GROUP > 0)
                {
                    GlobalVar.gl_mapping.Clear();//清空原有数据
                    for (int i = 1; i <= INIFileValue.Product_GROUP; i++)//读取对应值
                    {
                        string pcs_str = "";
                        StringBuilder str_temp = new StringBuilder(100);
                        GetPrivateProfileString("MAPPING", i.ToString(), "0", str_temp, 50, lastfile);
                        pcs_str = str_temp.ToString();
                        if (!GlobalVar.gl_mapping.ContainsKey(i))
                        {
                            int pcs = int.Parse(pcs_str);
                            GlobalVar.gl_mapping.Add(i, pcs);
                        }
                    }
                }
                else throw new Exception(string.Format("读取{0}的MAPPING文件异常", GlobalVar.gl_str_product));
            }
        }
        /// <summary>
        /// 更改本地mapping文件
        /// </summary>
        internal void WriteMapping()
        {
            string fileName = GlobalVar.gl_str_product + ".ini";
            string lastfile = string.Empty;//最新的文件
            string dllpath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            dllpath = dllpath.Substring(8, dllpath.Length - 8);    // 8是 file:// 的长度
            char sep = System.IO.Path.DirectorySeparatorChar;
            string dir = System.IO.Path.GetDirectoryName(dllpath) + sep + GlobalVar.Folder_Config + sep + "MAPPING" + sep;
            lastfile = dir + fileName;
            if (!Directory.Exists(dir)) throw new Exception(string.Format("读取{0}的MAPPING文件路径不存在", GlobalVar.gl_str_product));
            if (INIFileValue.Product_GROUP > 0)
            {
                for (int i = 1; i <= INIFileValue.Product_GROUP; i++)//读取对应值
                {
                    string pcs_str = GlobalVar.gl_mapping[i].ToString();
                    WritePrivateProfileString("MAPPING", i.ToString(), pcs_str, lastfile);
                }

            }
            else throw new Exception(string.Format("更改{0}的MAPPING文件异常", GlobalVar.gl_str_product));
        }
        #endregion

        /// <summary>
        /// 检验输入是否合法
        /// </summary>
        /// <param name="str">检测字串</param>
        /// <param name="checkType">1：数字  2：英文字符  3：数字+英文字符 4：数字+英文字符+横杠线(用于新的条码定义)</param>
        /// <returns></returns>
        internal bool CheckStringIsLegal(string str, int checkType)
        {
            bool result = true;
            if (checkType == 1)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    char c = str[i];
                    result &= ((c >= 48) && (c <= 57));
                }
            }
            else if (checkType == 2)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    char c = str[i];
                    result &= (((c >= 65) && (c <= 90))
                        || ((c >= 97) && (c <= 122)));
                }
            }
            else if (checkType == 3)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    char c = str[i];
                    result &= ((((c >= 65) && (c <= 90)) || ((c >= 97) && (c <= 122)))
                        || ((c >= 48) && (c <= 57)));
                }
            }
            else if (checkType == 4)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    char c = str[i];
                    result &= ((((c >= 65) && (c <= 90)) || ((c >= 97) && (c <= 122)))
                        || ((c >= 48) && (c <= 57)) || (c == '-'));
                }
            }
            return result;
        }

        /// <summary>
        /// 读取U盘的文件【返回是否读取成功】
        /// </summary>
        /// <param name="UFiles">读取的文件</param>
        /// <param name="ErrStr">异常的原因</param>
        /// <returns></returns>
        internal bool Read_UDisk(ref string[] UFiles, ref string ErrStr)
        {
            DriveInfo[] s = DriveInfo.GetDrives();
            foreach (DriveInfo i in s)
            {
                if (i.DriveType == DriveType.Removable && i.IsReady)
                {
                    string[] files = Directory.GetFiles(i.RootDirectory.ToString());
                    List<string> txt_list = new List<string>();
                    foreach (string str in files)
                    {
                        if (str.ToUpper().Contains(".TXT") || str.ToUpper().Contains(".INI")) txt_list.Add(str);
                    }

                    if (txt_list.Count == 0)
                    {
                        ErrStr = "移动存储盘读取文件失败,移动存储盘内不存在文件";
                        return false;
                    }
                    UFiles = txt_list.ToArray();
                    return true;
                }
            }
            ErrStr = "移动存储盘读取文件失败,移动存储盘不存在";
            return false;
        }

        /// <summary>
        /// 获得像素与长度的比例
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        internal double GetRate(IntPtr handle)
        {
            System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(handle);
            IntPtr hdc = g.GetHdc();
            int width = GetDeviceCaps(hdc, 4);     // HORZRES 
            int pixels = GetDeviceCaps(hdc, 8);     // BITSPIXEL
            g.ReleaseHdc(hdc);
            return (double)pixels / (double)width;
        }

        /// <summary>
        /// 字符串反转
        /// </summary>
        /// <param name="str">需要反转的字符串</param>
        /// <returns></returns>
        internal static string StrReverse(string str)
        {
            char[] c = str.ToCharArray();
            Array.Reverse(c);
            return new string(c);
        }

        /// <summary>
        /// 调整模式下，调整完毕后治具的偏差值 写CSV文件【后缀是INF】
        /// </summary>
        /// <param name="FileName">文件名</param>
        /// <param name="deviation">偏差值</param>
        internal void WriteINF(string FileName, List<double> deviation)
        {
            try
            {
                string filename = FileName;//文件名称
                string dirName = "LOG";

                if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);

                bool CsvCreate = false;
                string _logfile = dirName + filename + ".INF";// ".csv";
                CsvCreate = File.Exists(_logfile);

                using (FileStream FS = new FileStream(_logfile, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    using (StreamWriter SW = new StreamWriter(FS, Encoding.Default))
                    {

                        if (!CsvCreate)
                        {
                            string Title = string.Format("时间,制品名称,治具偏差值,");//标题
                            SW.WriteLine(Title);
                        }

                        StringBuilder writestr = new StringBuilder(string.Format("{0},{1},", DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss"), INIFileValue.Product_NAME));
                        foreach (double item in deviation)
                        {
                            writestr.Append(item + ",");
                        }

                        SW.WriteLine(writestr);
                        SW.Close();
                        SW.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "保存CSV Error:");
                log.AddERRORLOG("！！！保存CSV Error\r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 写CSV文件
        /// </summary>
        /// <param name="Product">产品名称</param>
        /// <param name="FixtureID">治具芯片编号</param>
        /// <param name="Lot">Lot号</param>
        /// <param name="Barcode">条码</param>
        /// <param name="Result">所有Punch的测试结果</param>
        /// <param name="NewTitle">新建标题</param>
        internal void WriteCSV(string Product, string FixtureID, string Lot, string Barcode, List<EC_TestResultItem[]> Result, bool NewTitle = false)
        {
            try
            {
                string filename = string.Format("{0}_{1}", Barcode, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));//文件名称
                string dirName = Application.StartupPath + string.Format(@"\Log\TestResult\");

                if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);

                bool NeedTitle = false;
                string _logfile = string.Format("{0}{1}.CSV", dirName, filename);
                NeedTitle = !File.Exists(_logfile) || NewTitle;

                using (FileStream FS = new FileStream(_logfile, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    StreamWriter SW = new StreamWriter(FS, Encoding.GetEncoding("GB2312"));

                    StringBuilder str = new StringBuilder();

                    if (NeedTitle)
                    {
                        str.Append(Barcode + "\r\n");
                        str.Append("2DID,Date,Time,ModelName,FixtureID,LotNumber,PieceIndex,Judge\r\n");
                    }

                    DateTime Time = DateTime.Now;
                    int Index = 0;
                    foreach (EC_TestResultItem[] item in Result)
                    {
                        foreach (EC_TestResultItem result in item)
                        {
                            str.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}\r\n",
                                                        Barcode,
                                                        Time.ToString("yyyy/MM/dd"),
                                                        Time.ToString("HH:mm:ss"),
                                                        Product,
                                                        FixtureID,
                                                        Lot,
                                                        ++Index,
                                                        result.Item));
                        }
                    }
                    SW.WriteLine(str.ToString());
                    SW.Close();
                    SW.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "WriteCSV Error:");
            }
        }

        /// <summary>
        /// 写CSV文件
        /// </summary>
        internal void WriteTestCSV(string testname, float score, double offsetX, double offsetY, bool NewTitle = false)
        {
            try
            {
                string filename = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHH"));//文件名称
                string dirName = Application.StartupPath + string.Format(@"\Log\TestData\");

                if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);

                bool NeedTitle = false;
                string _logfile = string.Format("{0}{1}.CSV", dirName, filename);
                NeedTitle = !File.Exists(_logfile) || NewTitle;

                using (FileStream FS = new FileStream(_logfile, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    StreamWriter SW = new StreamWriter(FS, Encoding.GetEncoding("GB2312"));

                    StringBuilder str = new StringBuilder();

                    if (NeedTitle)
                    {
                        ;
                        str.Append("Time,Name,Score,X_Offest,Y_Offset\r\n");
                    }

                    DateTime Time = DateTime.Now;

                    str.Append(string.Format("{0},{1},{2},{3},{4}",
                                                Time.ToString("HH:mm:ss"),
                                                testname,
                                                score.ToString(),
                                                offsetX,
                                                offsetY));

                    SW.WriteLine(str.ToString());
                    SW.Close();
                    SW.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "WriteCSV Error:");
            }
        }

        /// <summary>
        /// 保存SHEET统计信息文件
        /// </summary>
        internal void SaveSheet()
        {
            string SaveData = Security.EncryptDES(string.Format("制品检测张数:{0},合格制品数:{1},不合格制品数:{2},断路:{3},短路:{4},M:{5},N:{6},BY:{7},压合次数:{8},打标器更换时间:{9},已经打标次数:{10},允许打标的总数:{11},报警启动时间(小时):{12},报警启动时间(分):{13},报警时间间隔:{14}",
                                                              INIFileValue.ProductTestNum,
                                                              INIFileValue.ProductQualifiedNum,
                                                              INIFileValue.ProductUnQualifidNUm,
                                                              INIFileValue.ProductOpen,
                                                              INIFileValue.ProductShort,
                                                              INIFileValue.ProductOffsetM,
                                                              INIFileValue.ProductOffsetN,
                                                              INIFileValue.ProductForgetPaste,
                                                              INIFileValue.ProductPressCount,
                                                              GetTimeStamp(INIFileValue.MarkChangeTime),
                                                              INIFileValue.MarkCount,
                                                              INIFileValue.MarkTotalCount,
                                                              INIFileValue.AlarmHour,
                                                              INIFileValue.AlarmMinute,
                                                              INIFileValue.AlarmIntervalDay),
                                                   Security.KeyCode);

            File.WriteAllText(string.Format("{0}\\{1}", GlobalVar.Folder_Config, GlobalVar.File_Sheet), SaveData, Encoding.Default);
        }

        /// <summary>
        /// DateTime转换为Unix时间戳
        /// </summary>
        /// <param name="Time">时间</param>
        /// <returns></returns>
        private long GetTimeStamp(DateTime Time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(Time - startTime).TotalSeconds; // 相差秒数
            return timeStamp;
        }

        /// <summary>
        /// 时间戳转换为DateTime
        /// </summary>
        /// <param name="Stamp">时间戳</param>
        /// <returns></returns>
        private DateTime GetTimeFromStamp(long Stamp)
        {
            //Use To Test  long unixTimeStamp = 1478162177;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            return startTime.AddSeconds(Stamp);
        }

        /// <summary>
        /// 读取SHEET统计信息文件
        /// </summary>
        internal void ReadSheet()
        {
            try
            {
                string SheetFile = string.Format("{0}\\{1}", GlobalVar.Folder_Config, GlobalVar.File_Sheet);
                if (!File.Exists(SheetFile))
                {
                    throw new Exception("配置文件不存在");
                }
                string Text = File.ReadAllText(SheetFile);
                Text = Security.DecryptDES(Text, Security.KeyCode);
                string[] str = Text.Split(',');

                INIFileValue.ProductTestNum = ResolveSheetText(str, 0);
                INIFileValue.ProductQualifiedNum = ResolveSheetText(str, 1);
                INIFileValue.ProductUnQualifidNUm = ResolveSheetText(str, 2);
                INIFileValue.ProductOpen = ResolveSheetText(str, 3);
                INIFileValue.ProductShort = ResolveSheetText(str, 4);
                INIFileValue.ProductOffsetM = ResolveSheetText(str, 5);
                INIFileValue.ProductOffsetN = ResolveSheetText(str, 6);
                INIFileValue.ProductForgetPaste = ResolveSheetText(str, 7);
                INIFileValue.ProductPressCount = ResolveSheetText(str, 8);
                INIFileValue.MarkChangeTime = GetTimeFromStamp((long)ResolveSheetText(str, 9));
                INIFileValue.MarkCount = ResolveSheetText(str, 10);
                INIFileValue.MarkTotalCount = ResolveSheetText(str, 11);
                INIFileValue.AlarmHour = ResolveSheetText(str, 12);
                INIFileValue.AlarmMinute = ResolveSheetText(str, 13);
                INIFileValue.AlarmIntervalDay = ResolveSheetText(str, 14);
            }
            catch (Exception ex)
            {
                log.AddERRORLOG(ex.Message);
            }
        }

        /// <summary>
        /// 读取PLC异常的XML文档
        /// </summary>
        internal void ReadPLCErrXML()
        {
            try
            {
                string xml_path = string.Format("{0}\\{1}", GlobalVar.Folder_Config, GlobalVar.File_PLCErr);
                if (!File.Exists(xml_path))
                {
                    throw new Exception("PLC异常文件不存在");
                }

                OpXML xml = new OpXML(xml_path);
                string Value = "PLCErr";
                List<XmlNode> NodeList = OpXML.GetChildNodes(xml_path, Value);

                foreach (XmlNode item in NodeList)
                {
                    int Index = Convert.ToInt32(((XmlElement)item).GetAttribute("Index"));
                    string ErrStr = ((XmlElement)item).GetAttribute("ErrStr");
                    GlobalVar.PLCErrDir.Add(Index, new PLCErr(ErrStr));
                }
            }
            catch (Exception ex)
            {
                log.AddERRORLOG("读取PLC异常:" + ex.Message);
            }
        }

        #region 快捷方式
        /// <summary>
        /// 重命名桌面快捷方式  (0:重命名成功；1:不需要重命名（文件名已经是需要重命名的名称）；2:不存在，新建快捷方式;3:异常)
        /// </summary>
        /// <param name="NewLnkName">新的快捷方式名称</param>
        /// <returns></returns>
        internal int RenameDesktopLnk(string NewLnkName)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string desktop_dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            Console.WriteLine("逻辑桌面:" + desktop);
            Console.WriteLine("物理桌面:" + desktop_dir);

            return RenameLnk(desktop_dir, NewLnkName);
        }

        /// <summary>
        /// 重命名快捷启动快捷方式  (0:重命名成功；1:不需要重命名（文件名已经是需要重命名的名称）；2:不存在，新建快捷方式;3:异常)
        /// </summary>
        /// <param name="NewLnkName">新的快捷方式名称</param>
        /// <returns></returns>
        internal int RenameStartupLnk(string NewLnkName)
        {
            string startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            Console.WriteLine("启动:" + startup);

            return RenameLnk(startup, NewLnkName);
        }

        /// <summary>
        /// 重命名快捷方式，不存在则创建
        /// </summary>
        /// <param name="path"></param>
        /// <param name="NewLnkName"></param>
        /// <returns></returns>
        private int RenameLnk(string path, string NewLnkName)
        {
            try
            {
                if (NewLnkName.ToLower().IndexOf(".lnk") == -1) NewLnkName += ".lnk";

                string CurProgram = System.Diagnostics.Process.GetCurrentProcess().ProcessName;//当前程序名称

                if (CurProgram.IndexOf(".") > 0) CurProgram = CurProgram.Substring(0, CurProgram.IndexOf("."));

                string[] files = Directory.GetFiles(path);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Contains(".lnk"))
                    {
                        ShellLink slLinkObject = new ShellLink(files[i]);
                        if (slLinkObject.ExecuteFile.ToUpper() == Application.ExecutablePath.Replace('/', '\\').ToUpper())
                        {
                            NewLnkName = Path.GetDirectoryName(files[i]) + "\\" + NewLnkName;
                            if (slLinkObject.CurrentShortcutFile == NewLnkName) return 1;

                            File.Move(files[i], NewLnkName);
                            return 0;
                        }
                    }
                }
                AddNewLnk(path, NewLnkName);
                return 2;
            }
            catch (Exception ex)
            {
                log.AddERRORLOG("重命名快捷方式异常:" + ex.Message);
                return 3;
            }
        }

        private void AddNewLnk(string path, string NewLnkName)
        {
            ShellLink slLinkObject = new ShellLink();
            slLinkObject.WorkPath = Application.StartupPath;
            slLinkObject.IconLocation = ",0";   // 0 为图示文件的 Index  
            slLinkObject.ExecuteFile = Application.ExecutablePath;
            slLinkObject.Save(path + "\\" + NewLnkName);
            slLinkObject.Dispose();
        }
        #endregion

        /// <summary>
        /// 解析Sheet文本
        /// </summary>
        /// <param name="Text">文本数组</param>
        /// <param name="Index">序号</param>
        /// <returns></returns>
        private int ResolveSheetText(string[] Text, int Index)
        {
            try
            {
                if (Index >= Text.Length) throw new Exception("序号超出范围");
                string str = Text[Index];
                str = str.Substring(str.IndexOf(":") + 1);
                return Convert.ToInt32(str); ;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("解析Sheet文本异常序号:{0}\r\n{1}", Index, ex.Message));
            }
        }

        #region 获取软件版本号
        public string GetVersion()
        {
            string NowVersion = "V1.0";
            object[] attributes = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), false);
            if (attributes.Length > 0)
            {
                if (attributes.Length > 0)
                {
                    NowVersion = ((System.Reflection.AssemblyFileVersionAttribute)attributes[0]).Version;
                }
            }
            return NowVersion;
        }
        #endregion

        public Bitmap copyImage(Bitmap sourceBmp, int startX, int startY, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            if ((startX + width >= sourceBmp.Width)
                || (startY + height >= sourceBmp.Height))
            {
                return bitmap;
            }
            try
            {
                Graphics g = Graphics.FromImage(bitmap);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(sourceBmp, new Rectangle(0, 0, bitmap.Width, bitmap.Height)
                    , startX, startY, width, height, GraphicsUnit.Pixel);
                g.Save();
                return bitmap;
            }
            catch { return bitmap; }
            finally { }
        }

        /// <summary>
        /// 射频卡 校验的字符串
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static byte[] CheckString(byte[] array)
        {
            byte Sum, XOR;
            List<byte> CheckStr = new List<byte>();

            #region 算术和

            #endregion


            #region 异或和

            #endregion

            return CheckStr.ToArray();
        }
    }
}
