﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECInspect
{
    partial class INIFileValue
    {
        #region SANTEC.ini文件读取的值
        /// <summary>
        /// 连接EC的串口名称
        /// </summary>
        public static string EC_COM = "COM5";
        /// <summary>
        /// 连接PLC的串口名称
        /// </summary>
        public static string PLC_COM = "COM4";
        /// <summary>
        /// 连接读卡器的串口名称
        /// </summary>
        public static string ReadCard_COM = "COM3";
        /// <summary>
        /// 连接康耐视条码枪的串口名称
        /// </summary>
        public static string Scan_COM = "COM6";
        /// <summary>
        /// 打点轴最大值和最小值
        /// </summary>
        public static Range_Int DotAxisRange = new Range_Int(300, -55);
        /// <summary>
        /// X轴最大值和最小值
        /// </summary>
        public static Range_Int XAxisRange = new Range_Int(-1, -2);
        /// <summary>
        /// Y轴最大值和最小值
        /// </summary>
        public static Range_Int YAxisRange = new Range_Int(430, -40);
        /// <summary>
        /// 搬运轴最大值和最小值
        /// </summary>
        public static Range_Int CarryAxisRange = new Range_Int(480, -2);
        /// <summary>
        /// 相机轴最大最小值
        /// </summary>
        public static Range_Int CCDAxisRange = new Range_Int(420,0);
        #region 考虑删除    2017.11.09确定不用，长期不用后删除ini的相关代码及配置文件
        ///// <summary>
        ///// 保存模版文件时，X轴的值
        ///// </summary>
        //public static double AxisX_Std = 0d;
        ///// <summary>
        ///// 保存模版文件时，Y轴的值
        ///// </summary>
        //public static double AxisY_Std = 0d;
        #endregion
        /// <summary>
        /// 保存模版文件时，Mark点X的值
        /// </summary>
        public static double MarkX_Std = 0d;
        /// <summary>
        /// 保存模版文件时，Mark点Y的值
        /// </summary>
        public static double MarkY_Std = 0d;
        /// <summary>
        /// 定位的模版文件
        /// </summary>
        public static string FindModelFile = string.Empty;

        /// <summary>
        /// CCD的SN号码
        /// </summary>
        public static string CCDSN = string.Empty;

        /// <summary>
        /// 是否启用条码枪
        /// </summary>
        public static bool BarcodeGunEnable = false;

        /// <summary>
        /// 是否启用康耐视条码枪--[2018.3.28 lqz]
        /// </summary>
        public static bool BarcodeScanEnable = false;
        /// <summary>
        /// 数据上传flowid
        /// </summary>
        public static string FlowID = "33";
        #endregion
    }
}
