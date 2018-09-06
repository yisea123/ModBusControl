﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECInspect
{
    /// <summary>
    /// 线圈
    /// </summary>
    class Coil
    {
        /// <summary>
        /// PLC起始地址
        /// </summary>
        public const int PLCStartAddr = 0x0320;
        /// <summary>
        /// 地址
        /// </summary>
        public readonly int Addr; 
        /// <summary>
        /// 序号，通讯协议里的第几位
        /// </summary>
        internal readonly int Index;
        /// <summary>
        /// 长度
        /// </summary>
        public const int Size = 1;
        private bool m_Value;
        /// <summary>
        /// 值【0:False;1:True】
        /// </summary>
        public bool Value
        {
            get { return m_Value; }
            set
            {
                if (this.m_Value != value && this.Event_BitDataValueChanged != null)
                {
                    this.m_Value = value;
                    this.Event_BitDataValueChanged(this.Addr);//更改值后触发
                }
                else this.m_Value = value;
            }
        }

        public delegate void dele_BitDataValueChanged(int Addr);//委托-位 值
        public event dele_BitDataValueChanged Event_BitDataValueChanged;

        public Coil(int addr)
        {
            this.Index = addr - 1;

            this.Addr = this.Index + PLCStartAddr;//加上PLC的modbus的起始地址.

            m_Value = false;
        }
    }

    /// <summary>
    /// 保持寄存器
    /// </summary>
    public struct HoldingRegister  //结构struct是值类型
    {
        /// <summary>
        /// PLC起始地址
        /// </summary>
        public const int PLCStartAddr = 0x00B4;
        /// <summary>
        /// 一次读取寄存器的长度的最大值
        /// </summary>
        public const int MAXLength = 125;

        /// <summary>
        /// 地址
        /// </summary>
        public readonly int Addr;
        /// <summary>
        /// 序号，供读取本地值使用
        /// </summary>
        internal readonly int Index;
        /// <summary>
        /// 长度
        /// </summary>
        public readonly int Size;
        /// <summary>
        /// 值
        /// </summary>
        public int Value
        {
            get { return ModbusTool.WordToInt(AllHoldingRegister.RegisterArray, this.Index, this.Size); }
        }

        private static int m_Length = 0;
        /// <summary>
        /// 保持寄存器的总长度
        /// </summary>
        public static int TotalLength { get { return m_Length; } }

        //public delegate void dele_HoldingValue(double RegisterValue);//委托-寄存器 值
        //public event dele_HoldingValue EventHoldingValue;

        public HoldingRegister(int addr, int size)
        {
            this.Index = addr;

            this.Addr = this.Index + PLCStartAddr;//加上PLC的modbus的起始地址

            this.Size = size;

            m_Length += this.Size;//修改总长度
        }

        /// <summary>
        /// 获得原始byte数据
        /// </summary>
        /// <returns></returns>
        internal byte[] GetByte()
        {
            int m_Size = this.Size * 2;
            int m_Addr = this.Index * 2;
            byte[] byCopy = new byte[m_Size];
            for (int i = 0; i < m_Size; i++)
            {
                byCopy[i] = AllHoldingRegister.RegisterArray[m_Addr + i];
            }
            //switch (m_Size)
            //{
            //    case 2:
            //        byCopy[0] = AllHoldingRegister.RegisterArray[m_Addr];
            //        byCopy[1] = AllHoldingRegister.RegisterArray[m_Addr + 1];
            //        break;
            //    case 4:
            //        byCopy[0] = AllHoldingRegister.RegisterArray[m_Addr];
            //        byCopy[1] = AllHoldingRegister.RegisterArray[m_Addr + 1];
            //        byCopy[2] = AllHoldingRegister.RegisterArray[m_Addr + 2];
            //        byCopy[3] = AllHoldingRegister.RegisterArray[m_Addr + 3];
            //        break;
            //    default: throw new ArgumentOutOfRangeException("解析Modbus，超出范围");
            //}
            return byCopy;
        }
    }

    /// <summary>
    /// 所有的线圈
    /// </summary>
    class AllCoil
    {
        #region 位 值
        private Coil m_BitData1 = new Coil(1);
        /// <summary>
        /// 打点轴JOG+
        /// </summary>
        internal Coil DotPositive { get { return m_BitData1; } }

        private Coil m_BitData2 = new Coil(2);
        /// <summary>
        /// 打点轴JOG-
        /// </summary>
        internal Coil DotNegative { get { return m_BitData2; } }

        private Coil m_BitData3 = new Coil(3);
        /// <summary>
        /// 打点轴手动定位
        /// </summary>
        internal Coil DotManualPosition { get { return m_BitData3; } }

        private Coil m_BitData4 = new Coil(4);
        /// <summary>
        /// 打点轴HOME
        /// </summary>
        internal Coil DotHome { get { return m_BitData4; } }

        private Coil m_BitData5 = new Coil(5);
        /// <summary>
        /// 打点轴报警解除
        /// </summary>
        internal Coil DotAlarmRelease { get { return m_BitData5; } }

        private Coil m_BitData6 = new Coil(6);
        /// <summary>
        /// Y轴JOG+
        /// </summary>
        internal Coil YPositive { get { return m_BitData6; } }

        private Coil m_BitData7 = new Coil(7);
        /// <summary>
        /// Y轴JOG-
        /// </summary>
        internal Coil YNegative { get { return m_BitData7; } }

        private Coil m_BitData8 = new Coil(8);
        /// <summary>
        /// Y轴手动定位
        /// </summary>
        internal Coil YManualPosition { get { return m_BitData8; } }

        private Coil m_BitData9 = new Coil(9);
        /// <summary>
        /// Y轴HOME
        /// </summary>
        internal Coil YHome { get { return m_BitData9; } }

        private Coil m_BitData10 = new Coil(10);
        /// <summary>
        /// Y轴报警解除
        /// </summary>
        internal Coil YAlarmRelease { get { return m_BitData10; } }

        private Coil m_BitData11 = new Coil(11);
        /// <summary>
        /// 启动拍照
        /// </summary>
        internal Coil StartGrab { get { return m_BitData11; } }

        private Coil m_BitData12 = new Coil(12);
        /// <summary>
        /// 压平汽缸
        /// </summary>
        internal Coil Cylinder_Flat { get { return m_BitData12; } }

        private Coil m_BitData13 = new Coil(13);
        /// <summary>
        /// 下压汽缸
        /// </summary>
        internal Coil Cylinder_DownPressure { get { return m_BitData13; } }

        private Coil m_BitData14 = new Coil(14);
        /// <summary>
        /// 左右夹取汽缸
        /// </summary>
        internal Coil Cylinder_LeftRightClamp { get { return m_BitData14; } }

        private Coil m_BitData15 = new Coil(15);
        /// <summary>
        /// 前后夹取汽缸
        /// </summary>
        internal Coil Cylinder_BeforeBack { get { return m_BitData15; } }

        private Coil m_BitData16 = new Coil(16);
        /// <summary>
        /// 上下夹取汽缸
        /// </summary>
        internal Coil Cylinder_UpDown { get { return m_BitData16; } }

        private Coil m_BitData17 = new Coil(17);
        /// <summary>
        /// 托料汽缸
        /// </summary>
        internal Coil Cylinder_HoldMater { get { return m_BitData17; } }

        private Coil m_BitData18 = new Coil(18);
        /// <summary>
        /// 吸取汽缸
        /// </summary>
        internal Coil Cylinder_Absorb { get { return m_BitData18; } }

        private Coil m_BitData19 = new Coil(19);
        /// <summary>
        /// 旋转汽缸
        /// </summary>
        internal Coil Cylinder_Rotate { get { return m_BitData19; } }

        private Coil m_BitData20 = new Coil(20);
        /// <summary>
        /// 打点汽缸
        /// </summary>
        internal Coil Cylinder_Dot { get { return m_BitData20; } }

        private Coil m_BitData21 = new Coil(21);
        /// <summary>
        /// 吸取真空
        /// </summary>
        internal Coil Vacuum_Absorb { get { return m_BitData21; } }

        private Coil m_BitData22 = new Coil(22);
        /// <summary>
        /// 吸取破真空
        /// </summary>
        internal Coil Vacuum_AbsorbBreak { get { return m_BitData22; } }

        private Coil m_BitData23 = new Coil(23);
        /// <summary>
        /// 汽缸11
        /// </summary>
        internal Coil Cylinder11 { get { return m_BitData23; } }

        private Coil m_BitData24 = new Coil(24);
        /// <summary>
        /// 相机汽缸12
        /// </summary>
        internal Coil Cylinder12 { get { return m_BitData24; } }

        private Coil m_BitData25 = new Coil(25);
        /// <summary>
        /// 自动/手动
        /// </summary>
        internal Coil Mode { get { return m_BitData25; } }

        private Coil m_BitData26 = new Coil(26);
        /// <summary>
        /// 下位机状态
        /// </summary>
        internal Coil PLCStatus { get { return m_BitData26; } }

        private Coil m_BitData27 = new Coil(27);
        /// <summary>
        /// 系统复位
        /// </summary>
        internal Coil SystemReset { get { return m_BitData27; } }

        private Coil m_BitData28 = new Coil(28);
        /// <summary>
        /// 系统报警解除
        /// </summary>
        internal Coil AlarmRelease { get { return m_BitData28; } }

        private Coil m_BitData29 = new Coil(29);
        /// <summary>
        /// Y轴治具位置
        /// </summary>
        internal Coil YJigLocation { get { return m_BitData29; } }

        private Coil m_BitData30 = new Coil(30);
        /// <summary>
        /// Y轴逆向放置位置
        /// </summary>
        internal Coil YReverseLocation { get { return m_BitData30; } }

        private Coil m_BitData31 = new Coil(31);
        /// <summary>
        /// Y轴放置位置
        /// </summary>
        internal Coil YPlaceLocation { get { return m_BitData31; } }

        private Coil m_BitData32 = new Coil(32);
        /// <summary>
        /// Y轴标记位置
        /// </summary>
        internal Coil YSignLocation { get { return m_BitData32; } }

        private Coil m_BitData33 = new Coil(33);
        /// <summary>
        /// X轴打标等待位置
        /// </summary>
        internal Coil XWaitMarkLocation { get { return m_BitData33; } }

        private Coil m_BitData34 = new Coil(34);
        /// <summary>
        /// 左键启动
        /// </summary>
        internal Coil StartLeft { get { return m_BitData34; } }

        private Coil m_BitData35 = new Coil(35);
        /// <summary>
        /// 右键启动
        /// </summary>
        internal Coil StartRight { get { return m_BitData35; } }

        private Coil m_BitData36 = new Coil(36);
        /// <summary>
        /// 夹具状态
        /// </summary>
        internal Coil FixtureStatus { get { return m_BitData36; } }

        private Coil m_BitData37 = new Coil(37);
        /// <summary>
        /// 治具位置状态
        /// </summary>
        internal Coil JigLocation { get { return m_BitData37; } }

        private Coil m_BitData38 = new Coil(38);
        /// <summary>
        /// 治具锁紧状态
        /// </summary>
        internal Coil JigLock { get { return m_BitData38; } }

        private Coil m_BitData39 = new Coil(39);
        /// <summary>
        /// 隔离销
        /// </summary>
        internal Coil LsolationPin { get { return m_BitData39; } }

        private Coil m_BitData40 = new Coil(40);
        /// <summary>
        /// 压合状态
        /// </summary>
        internal Coil PressStatus { get { return m_BitData40; } }

        private Coil m_BitData41 = new Coil(41);
        /// <summary>
        /// 感应上治具
        /// </summary>
        internal Coil SensorUpJog { get { return m_BitData41; } }

        private Coil m_BitData42 = new Coil(42);
        /// <summary>
        /// 轴复位完成
        /// </summary>
        internal Coil AxisReset { get { return m_BitData42; } }

        private Coil m_BitData43 = new Coil(43);
        /// <summary>
        /// 打标完成
        /// </summary>
        internal Coil MarkEnd { get { return m_BitData43; } }

        private Coil m_BitData44 = new Coil(44);        
        /// <summary>
        /// 启动测试
        /// </summary>
        internal Coil StartTest { get { return m_BitData44; } }

        private Coil m_BitData45 = new Coil(45);
        /// <summary>
        /// 是否可以写入坐标
        /// </summary>
        internal Coil CanWriteCoordinate { get { return m_BitData45; } }

        private Coil m_BitData46 = new Coil(46);
        /// <summary>
        /// 写入坐标完成
        /// </summary>
        internal Coil WriteCoordinateComplete { get { return m_BitData46; } }

        private Coil m_BitData47 = new Coil(47);
        /// <summary>
        /// 调整模式结束（段取流程）
        /// </summary>
        internal Coil AdjustComplete { get { return m_BitData47; } }


        private Coil m_BitData48 = new Coil(48);
        /// <summary>
        /// 单次打点完成
        /// </summary>
        internal Coil DotComplete { get { return m_BitData48; } }

        private Coil m_BitData49 = new Coil(49);
        /// <summary>
        /// 复位键【抬起】
        /// </summary>
        internal Coil UpperJig { get { return m_BitData49; } }

        private Coil m_BitData50 = new Coil(50);
        /// <summary>
        /// CT计时【0：停止计时，1：开始计时】
        /// </summary>
        internal Coil CT { get { return m_BitData50; } }

        private Coil m_BitData51 = new Coil(51);
        /// <summary>
        /// 低粘着【0：打开，1：关闭】
        /// </summary>
        internal Coil DiNianZhe { get { return m_BitData51; } }


        private Coil m_BitData52 = new Coil(52);
        /// <summary>
        /// X轴JOG+
        /// </summary>
        internal Coil XPositive { get { return m_BitData52; } }

        private Coil m_BitData53 = new Coil(53);
        /// <summary>
        /// X轴JOG-
        /// </summary>
        internal Coil XNegative { get { return m_BitData53; } }

        private Coil m_BitData54 = new Coil(54);
        /// <summary>
        /// X轴手动定位
        /// </summary>
        internal Coil XManualPosition { get { return m_BitData54; } }

        private Coil m_BitData55 = new Coil(55);
        /// <summary>
        /// X轴HOME
        /// </summary>
        internal Coil XHome { get { return m_BitData55; } }

        private Coil m_BitData56 = new Coil(56);
        /// <summary>
        /// X轴报警解除
        /// </summary>
        internal Coil XAlarmRelease { get { return m_BitData56; } }

        private Coil m_BitData57 = new Coil(57);
        /// <summary>
        /// 搬运轴JOG+
        /// </summary>
        internal Coil CarryPositive { get { return m_BitData57; } }

        private Coil m_BitData58 = new Coil(58);
        /// <summary>
        /// 搬运轴JOG-
        /// </summary>
        internal Coil CarryNegative { get { return m_BitData58; } }

        private Coil m_BitData59 = new Coil(59);
        /// <summary>
        /// 搬运轴手动定位
        /// </summary>
        internal Coil CarryManualPosition { get { return m_BitData59; } }

        private Coil m_BitData60 = new Coil(60);
        /// <summary>
        /// 搬运轴HOME
        /// </summary>
        internal Coil CarryHome { get { return m_BitData60; } }

        private Coil m_BitData61 = new Coil(61);
        /// <summary>
        /// 搬运轴报警解除
        /// </summary>
        internal Coil CarryAlarmRelease { get { return m_BitData61; } }

        private Coil m_BitData62 = new Coil(62);
        /// <summary>
        /// 剥料轴正转
        /// </summary>
        internal Coil PeelPositive { get { return m_BitData62; } }

        private Coil m_BitData63 = new Coil(63);
        /// <summary>
        /// 剥料轴反转
        /// </summary>
        internal Coil PeelNegative { get { return m_BitData63; } }
        
        private Coil m_BitData64 = new Coil(64);
        /// <summary>
        /// 搬运轴上料位置
        /// </summary>
        internal Coil CarryFeedPosition { get { return m_BitData64; } }

        private Coil m_BitData65 = new Coil(65);
        /// <summary>
        /// 搬运轴下料位置
        /// </summary>
        internal Coil CarryBlankPositon { get { return m_BitData65; } }

        private Coil m_BitData66 = new Coil(66);
        /// <summary>
        /// 定位相机拍照位置
        /// </summary>
        internal Coil TakePhotoPosition { get { return m_BitData66; } }

        private Coil m_BitData67 = new Coil(67);
        /// <summary>
        /// 是否使用相机
        /// </summary>
        internal Coil CCDUse { get { return m_BitData67; } }

        private Coil m_BitData68 = new Coil(68);
        /// <summary>
        /// 相机光源控制
        /// </summary>
        internal Coil CCDLightStatus { get { return m_BitData68; } }

        private Coil m_BitData69 = new Coil(69);
        /// <summary>
        /// 复位前，先通知抬起下压汽缸
        /// </summary>
        internal Coil ResetUp { get { return m_BitData69; } }

        private Coil m_BitData70 = new Coil(70);
        /// <summary>
        /// 下料时，是否下压
        /// </summary>
        internal Coil FeedIsPress { get { return m_BitData70; } }

        private Coil m_BitData71 = new Coil(71);
        /// <summary>
        /// 多次测试，需要重新设定
        /// </summary>
        internal Coil PunchTest { get { return m_BitData71; } }

        private Coil m_BitData72 = new Coil(72);
        /// <summary>
        /// 旋转汽缸，是否旋转
        /// </summary>
        internal Coil Rotate { get { return m_BitData72; } }

        private Coil m_BitData73 = new Coil(73);
        /// <summary>
        /// 调整模式的结果
        /// </summary>
        internal Coil AdjustResult { get { return m_BitData73; } }

        private Coil m_BitData74 = new Coil(74);
        /// <summary>
        /// 下治具电磁阀
        /// </summary>
        internal Coil DownJigCylinder { get { return m_BitData74; } }

        private Coil m_BitData75 = new Coil(75);
        /// <summary>
        /// 条码准备好/通知扫描条码--[2018.3.28 lqz]
        /// </summary>
        internal Coil BarocodeReady { get { return m_BitData75; } }
        
        private Coil m_BitData76 = new Coil(76);
        /// <summary>
        /// 条码枪是否启用--[2018.3.28 lqz]
        /// </summary>
        internal Coil BarcodeScanUesd { get { return m_BitData76; } }

        private Coil m_BitData77 = new Coil(77);
        /// <summary>
        /// 相机轴JOG+
        /// </summary>
        internal Coil CCD_Positive { get { return m_BitData77; } }

        private Coil m_BitData78 = new Coil(78);
        /// <summary>
        /// 相机轴JOG-
        /// </summary>
        internal Coil CCD_Negative { get { return m_BitData78; } }

        private Coil m_BitData79 = new Coil(79);
        /// <summary>
        /// 相机轴手动定位
        /// </summary>
        internal Coil CCD_ManualMark { get { return m_BitData79; } }

        private Coil m_BitData80 = new Coil(80);
        /// <summary>
        /// 相机轴HOME
        /// </summary>
        internal Coil CCD_Home { get { return m_BitData80; } }

        private Coil m_BitData81 = new Coil(81);
        /// <summary>
        /// 相机轴拍照位置
        /// </summary>
        internal Coil CCD_PhotoPoint { get { return m_BitData81; } }

        private Coil m_BitData82= new Coil(82);
        /// <summary>
        /// 扫描条码位置--[2018.3.29 lqz]
        /// </summary>
        internal Coil ScanPosition { get { return m_BitData82; } }

        private Coil m_BitData83 = new Coil(83);
        /// <summary>
        /// 报警是否需要复位
        /// </summary>
        internal Coil AlarmNeedReset { get { return m_BitData83; } }
        #region 预留16位

        private Coil m_BitData84 = new Coil(84);
        private Coil m_BitData85 = new Coil(85);
        private Coil m_BitData86 = new Coil(86);
        private Coil m_BitData87 = new Coil(87);
        private Coil m_BitData88 = new Coil(88);
        private Coil m_BitData89 = new Coil(89);
        private Coil m_BitData90 = new Coil(90);
        private Coil m_BitData91 = new Coil(91);
        private Coil m_BitData92 = new Coil(92);
        private Coil m_BitData93 = new Coil(93);
        private Coil m_BitData94 = new Coil(94);
        private Coil m_BitData95 = new Coil(95);
        private Coil m_BitData96 = new Coil(96);
        private Coil m_BitData97 = new Coil(97);
        private Coil m_BitData98 = new Coil(98);
        private Coil m_BitData99 = new Coil(99);
        #endregion  
     
        private Coil m_BitData100 = new Coil(100);
        /// <summary>
        /// 段取  紧急停止键
        /// </summary>
        internal Coil PrepareEMCStop { get { return m_BitData100; } }

        private Coil m_BitData101 = new Coil(101);
        /// <summary>
        /// 紧急停止键
        /// </summary>
        internal Coil EMCStop { get { return m_BitData101; } }
    
        #region PLC报警
        private Coil m_BitData102 = new Coil(102);
        /// <summary>
        /// 机台警报2
        /// </summary>
        internal Coil PLCErr2 { get { return m_BitData102; } }

        private Coil m_BitData103 = new Coil(103);
        /// <summary>
        /// 机台警报3
        /// </summary>
        internal Coil PLCErr3 { get { return m_BitData103; } }

        private Coil m_BitData104 = new Coil(104);
        /// <summary>
        /// 机台警报4
        /// </summary>
        internal Coil PLCErr4 { get { return m_BitData104; } }

        private Coil m_BitData105 = new Coil(105);
        /// <summary>
        /// 机台警报5
        /// </summary>
        internal Coil PLCErr5 { get { return m_BitData105; } }

        private Coil m_BitData106 = new Coil(106);
        /// <summary>
        /// 机台警报6
        /// </summary>
        internal Coil PLCErr6 { get { return m_BitData106; } }

        private Coil m_BitData107 = new Coil(107);
        /// <summary>
        /// 机台警报7
        /// </summary>
        internal Coil PLCErr7 { get { return m_BitData107; } }

        private Coil m_BitData108 = new Coil(108);
        /// <summary>
        /// 机台警报8
        /// </summary>
        internal Coil PLCErr8 { get { return m_BitData108; } }

        private Coil m_BitData109 = new Coil(109);
        /// <summary>
        /// 机台警报9
        /// </summary>
        internal Coil PLCErr9 { get { return m_BitData109; } }

        private Coil m_BitData110 = new Coil(110);
        /// <summary>
        /// 机台警报10
        /// </summary>
        internal Coil PLCErr10 { get { return m_BitData110; } }

        private Coil m_BitData111 = new Coil(111);
        /// <summary>
        /// 机台警报11
        /// </summary>
        internal Coil PLCErr11 { get { return m_BitData111; } }

        private Coil m_BitData112 = new Coil(112);
        /// <summary>
        /// 机台警报12
        /// </summary>
        internal Coil PLCErr12 { get { return m_BitData112; } }

        private Coil m_BitData113 = new Coil(113);
        /// <summary>
        /// 机台警报13
        /// </summary>
        internal Coil PLCErr13 { get { return m_BitData113; } }

        private Coil m_BitData114 = new Coil(114);
        /// <summary>
        /// 机台警报14
        /// </summary>
        internal Coil PLCErr14 { get { return m_BitData114; } }

        private Coil m_BitData115 = new Coil(115);
        /// <summary>
        /// 机台警报15
        /// </summary>
        internal Coil PLCErr15 { get { return m_BitData115; } }

        private Coil m_BitData116 = new Coil(116);
        /// <summary>
        /// 机台警报16
        /// </summary>
        internal Coil PLCErr16 { get { return m_BitData116; } }

        private Coil m_BitData117 = new Coil(117);
        /// <summary>
        /// 机台警报17
        /// </summary>
        internal Coil PLCErr17 { get { return m_BitData117; } }

        private Coil m_BitData118 = new Coil(118);
        /// <summary>
        /// 机台警报18
        /// </summary>
        internal Coil PLCErr18 { get { return m_BitData118; } }

        private Coil m_BitData119 = new Coil(119);
        /// <summary>
        /// 机台警报19
        /// </summary>
        internal Coil PLCErr19 { get { return m_BitData119; } }

        private Coil m_BitData120 = new Coil(120);
        /// <summary>
        /// 机台警报20
        /// </summary>
        internal Coil PLCErr20 { get { return m_BitData120; } }

        private Coil m_BitData121 = new Coil(121);
        /// <summary>
        /// 机台警报21
        /// </summary>
        internal Coil PLCErr21 { get { return m_BitData121; } }

        private Coil m_BitData122 = new Coil(122);
        /// <summary>
        /// 机台警报22
        /// </summary>
        internal Coil PLCErr22 { get { return m_BitData122; } }

        private Coil m_BitData123 = new Coil(123);
        /// <summary>
        /// 机台警报23
        /// </summary>
        internal Coil PLCErr23 { get { return m_BitData123; } }

        private Coil m_BitData124 = new Coil(124);
        /// <summary>
        /// 机台警报24
        /// </summary>
        internal Coil PLCErr24 { get { return m_BitData124; } }

        private Coil m_BitData125 = new Coil(125);
        /// <summary>
        /// 机台警报25
        /// </summary>
        internal Coil PLCErr25 { get { return m_BitData125; } }

        private Coil m_BitData126 = new Coil(126); 
        /// <summary>
        ///【机台警报26】
        /// </summary>
        internal Coil PLCErr26 { get { return m_BitData126; } }

        private Coil m_BitData127 = new Coil(127);
        /// <summary>
        /// 机台警报27
        /// </summary>
        internal Coil PLCErr27 { get { return m_BitData127; } }

        private Coil m_BitData128 = new Coil(128);
        /// <summary>
        /// 机台警报28
        /// </summary>
        internal Coil PLCErr28 { get { return m_BitData128; } }

        private Coil m_BitData129 = new Coil(129);
        /// <summary>
        /// 机台警报29
        /// </summary>
        internal Coil PLCErr29 { get { return m_BitData129; } }

        private Coil m_BitData130 = new Coil(130);
        /// <summary>
        /// 机台警报30
        /// </summary>
        internal Coil PLCErr30 { get { return m_BitData130; } }

        private Coil m_BitData131 = new Coil(131);
        /// <summary>
        /// 机台警报31
        /// </summary>
        internal Coil PLCErr31 { get { return m_BitData131; } }

        private Coil m_BitData132= new Coil(132);
        /// <summary>
        /// 机台警报32
        /// </summary>
        internal Coil PLCErr32 { get { return m_BitData132; } }

        private Coil m_BitData133 = new Coil(133);
        /// <summary>
        /// 机台警报33
        /// </summary>
        internal Coil PLCErr33 { get { return m_BitData133; } }

        private Coil m_BitData134 = new Coil(134);
        /// <summary>
        /// 机台警报34
        /// </summary>
        internal Coil PLCErr34 { get { return m_BitData134; } }

        private Coil m_BitData135 = new Coil(135);
        /// <summary>
        /// 机台警报35
        /// </summary>
        internal Coil PLCErr35 { get { return m_BitData135; } }

        private Coil m_BitData136 = new Coil(136);
        /// <summary>
        /// 机台警报36
        /// </summary>
        internal Coil PLCErr36 { get { return m_BitData136; } }

        private Coil m_BitData137 = new Coil(137);
        /// <summary>
        /// 机台警报37
        /// </summary>
        internal Coil PLCErr37 { get { return m_BitData137; } }

        private Coil m_BitData138 = new Coil(138);
        /// <summary>
        /// 机台警报38
        /// </summary>
        internal Coil PLCErr38 { get { return m_BitData138; } }

        private Coil m_BitData139 = new Coil(139);
        /// <summary>
        /// 机台警报39
        /// </summary>
        internal Coil PLCErr39 { get { return m_BitData139; } }

        private Coil m_BitData140 = new Coil(140);
        /// <summary>
        /// 机台警报40
        /// </summary>
        internal Coil PLCErr40 { get { return m_BitData140; } }

        private Coil m_BitData141 = new Coil(141);
        /// <summary>
        /// 机台警报41
        /// </summary>
        internal Coil PLCErr41 { get { return m_BitData141; } }
        
        private Coil m_BitData142 = new Coil(142);
        /// <summary>
        /// 机台警报42
        /// </summary>
        internal Coil PLCErr42 { get { return m_BitData142; } }

        private Coil m_BitData143 = new Coil(143);
        /// <summary>
        /// 机台警报43
        /// </summary>
        internal Coil PLCErr43 { get { return m_BitData143; } }

        private Coil m_BitData144 = new Coil(144);
        /// <summary>
        /// 机台警报44
        /// </summary>
        internal Coil PLCErr44 { get { return m_BitData144; } }

        private Coil m_BitData145 = new Coil(145);
        /// <summary>
        /// 机台警报45
        /// </summary>
        internal Coil PLCErr45 { get { return m_BitData145; } }
        #endregion

        #endregion

        /// <summary>
        /// 所有的线圈
        /// </summary>
        internal readonly Coil[] BitDatas;
        /// <summary>
        /// 线圈的长度
        /// </summary>
        internal int Count { get { return BitDatas.Length; } }

        public delegate void dele_CoilValueChanged(int Addr);
        public event dele_CoilValueChanged Event_CoilValueChanged;//值改变时 触发

        public AllCoil()
        {
            BitDatas = new Coil[] { 
                m_BitData1,                m_BitData2,                m_BitData3,                m_BitData4,                m_BitData5,
                m_BitData6,                m_BitData7,                m_BitData8,                m_BitData9,                m_BitData10,
                m_BitData11,               m_BitData12,               m_BitData13,               m_BitData14,               m_BitData15,
                m_BitData16,               m_BitData17,               m_BitData18,               m_BitData19,               m_BitData20,
                m_BitData21,               m_BitData22,               m_BitData23,               m_BitData24,               m_BitData25,
                m_BitData26,               m_BitData27,               m_BitData28,               m_BitData29,               m_BitData30,
                m_BitData31,               m_BitData32,               m_BitData33,               m_BitData34,               m_BitData35,
                m_BitData36,               m_BitData37,               m_BitData38,               m_BitData39,               m_BitData40,
                m_BitData41,               m_BitData42,               m_BitData43,               m_BitData44,               m_BitData45,
                m_BitData46,               m_BitData47,               m_BitData48,               m_BitData49,               m_BitData50,
                m_BitData51,               m_BitData52,               m_BitData53,               m_BitData54,               m_BitData55,
                m_BitData56,               m_BitData57,               m_BitData58,               m_BitData59,               m_BitData60,
                m_BitData61,               m_BitData62,               m_BitData63,               m_BitData64,               m_BitData65,
                m_BitData66,               m_BitData67,               m_BitData68,               m_BitData69,               m_BitData70,
                m_BitData71,               m_BitData72,               m_BitData73,               m_BitData74,               m_BitData75,
                m_BitData76,               m_BitData77,               m_BitData78,               m_BitData79,               m_BitData80,
                m_BitData81,               m_BitData82,               m_BitData83,               m_BitData84,               m_BitData85,
                m_BitData86,               m_BitData87,               m_BitData88,               m_BitData89,               m_BitData90,
                m_BitData91,               m_BitData92,               m_BitData93,               m_BitData94,               m_BitData95,
                m_BitData96,               m_BitData97,               m_BitData98,               m_BitData99,               m_BitData100,
                m_BitData101,              m_BitData102,              m_BitData103,              m_BitData104,              m_BitData105,
                m_BitData106,              m_BitData107,              m_BitData108,              m_BitData109,              m_BitData110,
                m_BitData111,              m_BitData112,              m_BitData113,              m_BitData114,              m_BitData115,
                m_BitData116,              m_BitData117,              m_BitData118,              m_BitData119,              m_BitData120,
                m_BitData121,              m_BitData122,              m_BitData123,              m_BitData124,              m_BitData125,
                m_BitData126,              m_BitData127,              m_BitData128,              m_BitData129,              m_BitData130,
                m_BitData131,              m_BitData132,              m_BitData133,              m_BitData134,              m_BitData135,
                m_BitData136,              m_BitData137,              m_BitData138,              m_BitData139,              m_BitData140,
                m_BitData141,              m_BitData142,              m_BitData143,              m_BitData144,              m_BitData145
            };

            foreach (Coil coil in BitDatas)
            {
                coil.Event_BitDataValueChanged += new Coil.dele_BitDataValueChanged(coil_Event_BitDataValueChanged);
            }
        }

        private void coil_Event_BitDataValueChanged(int Addr)
        {
            if (this.Event_CoilValueChanged != null) this.Event_CoilValueChanged(Addr);
        }

        /// <summary>
        /// 设置线圈的值
        /// </summary>
        /// <param name="binary">根据该字符串判断线圈的值</param>
        internal void SetBitDatasValue(string binary)
        {
            char Zero = '0';

            for (int i = 0; i < BitDatas.Length; i++)
            {
                BitDatas[i].Value = (binary[i] != Zero);
            }
        }
    }

    /// <summary>
    /// 所有的保持寄存器
    /// </summary>
    class AllHoldingRegister
    {
        #region 寄存器 值
        #region 寄存器范围【0-124】
        private HoldingRegister m_Register0 = new HoldingRegister(0, 1);
        /// <summary>
        /// 段取流程
        /// </summary>
        internal HoldingRegister Prepare { get { return m_Register0; } }

        #region 预留的保持寄存器
        private HoldingRegister m_Register1 = new HoldingRegister(1, 9);
        #endregion

        private HoldingRegister m_Register2 = new HoldingRegister(10, 2);
        /// <summary>
        /// 打点轴坐标
        /// </summary>
        internal HoldingRegister AxisDot_Point { get { return m_Register2; } }

        private HoldingRegister m_Register3 = new HoldingRegister(12, 2);
        /// <summary>
        /// Y轴坐标
        /// </summary>
        internal HoldingRegister AxisY_Point { get { return m_Register3; } }

        private HoldingRegister m_Register4 = new HoldingRegister(14, 1);
        /// <summary>
        /// 测试完成
        /// </summary>
        internal HoldingRegister TestResult  { get { return m_Register4; } }//奇怪？？？{ get { return m_Register6; } }

        private HoldingRegister m_Register5 = new HoldingRegister(15, 1);
        /// <summary>
        /// 拍照结果
        /// </summary>
        internal HoldingRegister MatchResult { get { return m_Register5; } }
        
        private HoldingRegister m_Register6 = new HoldingRegister(16, 2);
        /// <summary>
        /// 相机校正X轴坐标
        /// </summary>
        internal HoldingRegister AxisX_Point { get { return m_Register6; } }

        #region 预留的保持寄存器

        private HoldingRegister m_Register7 = new HoldingRegister(18, 1);
      
        private HoldingRegister m_Register8 = new HoldingRegister(19, 1);
     
        //private HoldingRegister m_Register9 = new HoldingRegister(19, 1);
        #endregion

        #region 打点轴
        private HoldingRegister m_Register10 = new HoldingRegister(20, 2);
        /// <summary>
        /// 打点轴定位速度
        /// </summary>
        internal HoldingRegister AxisDot_MoveSpeed { get { return m_Register10; } }

        private HoldingRegister m_Register11 = new HoldingRegister(22, 2);
        /// <summary>
        /// 打点轴归原点速度
        /// </summary>
        internal HoldingRegister AxisDot_MoveOriginSpeed { get { return m_Register11; } }

        private HoldingRegister m_Register12 = new HoldingRegister(24, 2);
        /// <summary>
        /// 打点轴归原点爬行速度
        /// </summary>
        internal HoldingRegister AxisDot_MoveOriginCrawlSpeed { get { return m_Register12; } }

        private HoldingRegister m_Register13 = new HoldingRegister(26, 2);
        /// <summary>
        /// 打点轴JOG速度
        /// </summary>
        internal HoldingRegister AxisDot_JOGSpeed { get { return m_Register13; } }

        private HoldingRegister m_Register14 = new HoldingRegister(28, 2);
        /// <summary>
        /// 打点轴加速度
        /// </summary>
        internal HoldingRegister AxisDot_AcceleratSpeed { get { return m_Register14; } }

        private HoldingRegister m_Register15 = new HoldingRegister(30, 2);
        /// <summary>
        /// 打点轴减速度
        /// </summary>
        internal HoldingRegister AxisDot_DeceleratSpeed { get { return m_Register15; } }

        private HoldingRegister m_Register16 = new HoldingRegister(32, 2);
        /// <summary>
        /// 打点轴目标坐标
        /// </summary>
        internal HoldingRegister AxisDot_TargetPoint { get { return m_Register16; } }
        #endregion

        #region Y轴
        private HoldingRegister m_Register17 = new HoldingRegister(34, 2);
        /// <summary>
        /// Y轴定位速度
        /// </summary>
        internal HoldingRegister AxisY_MoveSpeed { get { return m_Register17; } }

        private HoldingRegister m_Register18 = new HoldingRegister(36, 2);
        /// <summary>
        /// Y轴归原点速度
        /// </summary>
        internal HoldingRegister AxisY_MoveOriginSpeed { get { return m_Register18; } }

        private HoldingRegister m_Register19 = new HoldingRegister(38, 2);
        /// <summary>
        /// Y轴归原点爬行速度
        /// </summary>
        internal HoldingRegister AxisY_MoveOriginCrawlSpeed { get { return m_Register19; } }

        private HoldingRegister m_Reigster20 = new HoldingRegister(40, 2);
        /// <summary>
        /// Y轴JOG速度
        /// </summary>
        internal HoldingRegister AxisY_JOGSpeed { get { return m_Reigster20; } }

        private HoldingRegister m_Register21 = new HoldingRegister(42, 2);
        /// <summary>
        /// Y轴加速度
        /// </summary>
        internal HoldingRegister AxisY_AcceleratSpeed { get { return m_Register21; } }

        private HoldingRegister m_Register22 = new HoldingRegister(44, 2);
        /// <summary>
        /// Y轴减速度
        /// </summary>
        internal HoldingRegister AxisY_DeceleratSpeed { get { return m_Register22; } }

        private HoldingRegister m_Register23 = new HoldingRegister(46, 2);
        /// <summary>
        /// Y轴目标坐标
        /// </summary>
        internal HoldingRegister AxisY_TargetPoint { get { return m_Register23; } }
        #endregion

        #region 剥料轴
        private HoldingRegister m_Register24 = new HoldingRegister(48, 2);
        /// <summary>
        /// 剥料轴速度
        /// </summary>
        internal HoldingRegister AxisPeel_Speed { get { return m_Register24; } }

        private HoldingRegister m_Register25 = new HoldingRegister(50, 2);
        /// <summary>
        /// 剥料轴JOG速度
        /// </summary>
        internal HoldingRegister AxisPeel_JOGSpeed { get { return m_Register25; } }

        private HoldingRegister m_Register26 = new HoldingRegister(52, 2);
        /// <summary>
        /// 剥料轴加速度
        /// </summary>
        internal HoldingRegister AxisPeel_AcceleratSpeed { get { return m_Register26; } }

        private HoldingRegister m_Register27 = new HoldingRegister(54, 2);
        /// <summary>
        /// 剥料轴减速度
        /// </summary>
        internal HoldingRegister AxisPeel_DeceleratSpeed { get { return m_Register27; } }

        private HoldingRegister m_Register28 = new HoldingRegister(56, 2);
        /// <summary>
        /// 剥料轴完成后运动的距离
        /// </summary>
        internal HoldingRegister AxisPeel_EndDistance { get { return m_Register28; } }
        #endregion

        private HoldingRegister m_Register29 = new HoldingRegister(58, 2);
        /// <summary>
        /// 长延时
        /// </summary>
        internal HoldingRegister LongTimeDelay { get { return m_Register29; } }

        private HoldingRegister m_Register30 = new HoldingRegister(60, 2);
        /// <summary>
        /// 中延时
        /// </summary>
        internal HoldingRegister MediumTimeDelay { get { return m_Register30; } }

        private HoldingRegister m_Register31 = new HoldingRegister(62, 2);
        /// <summary>
        /// 短延时 汽缸动作延时
        /// </summary>
        internal HoldingRegister ShortTimeDelay { get { return m_Register31; } }

        private HoldingRegister m_Register32 = new HoldingRegister(64, 2);
        /// <summary>
        /// Y轴治具位置
        /// </summary>
        internal HoldingRegister AxisY_JigPoint { get { return m_Register32; } }

        private HoldingRegister m_Register33 = new HoldingRegister(66, 2);
        /// <summary>
        /// Y轴逆向放置位置
        /// </summary>
        internal HoldingRegister AxisY_ReversePlacePoint { get { return m_Register33; } }

        private HoldingRegister m_Register34 = new HoldingRegister(68, 2);
        /// <summary>
        /// Y轴放置位置
        /// </summary>
        internal HoldingRegister AxisY_PlacePoint { get { return m_Register34; } }

        private HoldingRegister m_Register35 = new HoldingRegister(70, 2);
        /// <summary>
        /// Y轴标记位置
        /// </summary>
        internal HoldingRegister AxisY_MarkPoint { get { return m_Register35; } }

        private HoldingRegister m_Register36 = new HoldingRegister(72, 2);
        /// <summary>
        /// 打点轴打标等待位置
        /// </summary>
        internal HoldingRegister AxisDot_WaitMarkPoint { get { return m_Register36; } }

        private HoldingRegister m_Register37 = new HoldingRegister(74, 2);
        /// <summary>
        /// 打点轴当前位置
        /// </summary>
        internal HoldingRegister AxisDot_RealLocation { get { return m_Register37; } }

        private HoldingRegister m_Register38 = new HoldingRegister(76, 2);
        /// <summary>
        /// Y轴当前位置
        /// </summary>
        internal HoldingRegister AxisY_RealLocation { get { return m_Register38; } }

        private HoldingRegister m_Register39 = new HoldingRegister(78, 2);
        /// <summary>
        /// Y轴段取 调整模式 初次进入位置
        /// </summary>
        internal HoldingRegister AxisY_PrepareLocation { get { return m_Register39; } }

        private HoldingRegister m_Register40 = new HoldingRegister(80, 2);
        /// <summary>
        /// 打点轴标记位置
        /// </summary>
        internal HoldingRegister AxisDot_MarkPoint { get { return m_Register40; } }

        private HoldingRegister m_Register41 = new HoldingRegister(82, 1);
        /// <summary>
        /// 测试模式
        /// </summary>
        internal HoldingRegister MovementPattern { get { return m_Register41; } }

        #region 搬运轴
        private HoldingRegister m_Register42 = new HoldingRegister(84, 2);
        /// <summary>
        /// 搬运轴定位速度
        /// </summary>
        internal HoldingRegister AxisCarry_MoveSpeed { get { return m_Register42; } }

        private HoldingRegister m_Register43 = new HoldingRegister(86, 2);
        /// <summary>
        /// 搬运轴归原点速度
        /// </summary>
        internal HoldingRegister AxisCarry_MoveOriginSpeed { get { return m_Register43; } }

        private HoldingRegister m_Register44 = new HoldingRegister(88, 2);
        /// <summary>
        /// 搬运轴归原点爬行速度
        /// </summary>
        internal HoldingRegister AxisCarry_MoveOriginCrawlSpeed { get { return m_Register44; } }

        private HoldingRegister m_Register45 = new HoldingRegister(90, 2);
        /// <summary>
        /// 搬运轴JOG速度
        /// </summary>
        internal HoldingRegister AxisCarry_JOGSpeed { get { return m_Register45; } }

        private HoldingRegister m_Register46 = new HoldingRegister(92, 2);
        /// <summary>
        /// 搬运轴加速度
        /// </summary>
        internal HoldingRegister AxisCarry_AcceleratSpeed { get { return m_Register46; } }

        private HoldingRegister m_Register47 = new HoldingRegister(94, 2);
        /// <summary>
        /// 搬运轴减速度
        /// </summary>
        internal HoldingRegister AxisCarry_DeceleratSpeed { get { return m_Register47; } }

        private HoldingRegister m_Register48 = new HoldingRegister(96, 2);
        /// <summary>
        /// 搬运轴目标坐标
        /// </summary>
        internal HoldingRegister AxisCarry_TargetPoint { get { return m_Register48; } }
        #endregion

        #region X轴
        private HoldingRegister m_Register49 = new HoldingRegister(98, 2);
        /// <summary>
        /// X轴定位速度
        /// </summary>
        internal HoldingRegister AxisX_MoveSpeed { get { return m_Register49; } }

        private HoldingRegister m_Register50 = new HoldingRegister(100, 2);
        /// <summary>
        /// X轴归原点速度
        /// </summary>
        internal HoldingRegister AxisX_MoveOriginSpeed { get { return m_Register50; } }

        private HoldingRegister m_Register51 = new HoldingRegister(102, 2);
        /// <summary>
        /// X轴归原点爬行速度
        /// </summary>
        internal HoldingRegister AxisX_MoveOriginCrawlSpeed { get { return m_Register51; } }

        private HoldingRegister m_Register52 = new HoldingRegister(104, 2);
        /// <summary>
        /// X轴JOG速度
        /// </summary>
        internal HoldingRegister AxisX_JOGSpeed { get { return m_Register52; } }

        private HoldingRegister m_Register53 = new HoldingRegister(106, 2);
        /// <summary>
        /// X轴加速度
        /// </summary>
        internal HoldingRegister AxisX_AcceleratSpeed { get { return m_Register53; } }

        private HoldingRegister m_Register54 = new HoldingRegister(108, 2);
        /// <summary>
        /// X轴减速度
        /// </summary>
        internal HoldingRegister AxisX_DeceleratSpeed { get { return m_Register54; } }

        private HoldingRegister m_Register55 = new HoldingRegister(110, 2);
        /// <summary>
        /// X轴目标坐标
        /// </summary>
        internal HoldingRegister AxisX_TargetPoint { get { return m_Register55; } }
        #endregion

        private HoldingRegister m_Register56 = new HoldingRegister(112, 2);
        /// <summary>
        /// 当前搬运轴的坐标位置
        /// </summary>
        internal HoldingRegister AxisCarry_RealLocation { get { return m_Register56; } }

        private HoldingRegister m_Register57 = new HoldingRegister(114, 2);
        /// <summary>
        /// 当前X轴的坐标位置
        /// </summary>
        internal HoldingRegister AxisX_RealLocation { get { return m_Register57; } }

        private HoldingRegister m_Register58 = new HoldingRegister(116, 2);
        /// <summary>
        /// 搬运轴上料的坐标位置
        /// </summary>
        internal HoldingRegister AxisCarry_FeedLocation { get { return m_Register58; } }

        private HoldingRegister m_Register59 = new HoldingRegister(118, 2);
        /// <summary>
        /// 搬运轴下料的坐标位置
        /// </summary>
        internal HoldingRegister AxisCarry_BlankLocation { get { return m_Register59; } }

        private HoldingRegister m_Register60 = new HoldingRegister(120, 2);
        /// <summary>
        /// X轴合模的坐标位置
        /// </summary>
        internal HoldingRegister AxisX_AssemblyLocation { get { return m_Register60; } }
        
        private HoldingRegister m_Register61 = new HoldingRegister(122, 2);
        /// <summary>
        /// Y轴相机拍照的坐标位置
        /// </summary>
        internal HoldingRegister AxisY_PhotoLocation { get { return m_Register61; } }
        #endregion

        #region 寄存器范围【125+】
        private HoldingRegister m_Register62 = new HoldingRegister(124, 4);
        /// <summary>
        /// PLC程序版本
        /// </summary>
        internal HoldingRegister PLCVer { get { return m_Register62; } }

        private HoldingRegister m_Register63 = new HoldingRegister(128,2);
        /// <summary>
        /// Y轴扫描条码位置--[2018.3.28 lqz]
        /// </summary>
        internal HoldingRegister AxisY_ScanPoint { get { return m_Register63; } }

        private HoldingRegister m_Register64 = new HoldingRegister(130,2);
        /// <summary>
        /// 打点轴扫码位置--[2018.3.28 lqz]
        /// </summary>
        internal HoldingRegister AxisDot_ScanPoint { get { return m_Register64; } }

        private HoldingRegister m_Register65 = new HoldingRegister(132, 2);
        /// <summary>
        /// 相机轴定位速度
        /// </summary>
        internal HoldingRegister CCD_MoveSpeed { get { return m_Register65; } }

        private HoldingRegister m_Register66 = new HoldingRegister(134, 2);
        /// <summary>
        /// 相机轴归原点速度        
        /// </summary>
        internal HoldingRegister CCD_MoveOriginSpeed { get { return m_Register66; } }

        private HoldingRegister m_Register67 = new HoldingRegister(136, 2);
        /// <summary>
        /// 相机轴归原点爬行速度
        /// </summary>
        internal HoldingRegister CCD_MoveOriginCrawlSpeed { get { return m_Register67; } }

        private HoldingRegister m_Register68 = new HoldingRegister(138, 2);
        /// <summary>
        ///相机轴JOG速度        
        /// </summary>
        internal HoldingRegister CCD_JogSpeed { get { return m_Register68; } }

        private HoldingRegister m_Register69 = new HoldingRegister(140, 1);
        /// <summary>
        ///相机轴加速度
        /// </summary>
        internal HoldingRegister CCD_AccelerateSpeed { get { return m_Register69; } }

        private HoldingRegister m_Register70 = new HoldingRegister(141, 1);
        /// <summary>
        ///相机轴减速度        
        /// </summary>
        internal HoldingRegister CCD_DecelerateSpeed { get { return m_Register70; } }

        private HoldingRegister m_Register71 = new HoldingRegister(142, 2);
        /// <summary>
        ///相机轴手动定位目标坐标        
        /// </summary>
        internal HoldingRegister CCD_ManualPoint { get { return m_Register71; } }

        private HoldingRegister m_Register72 = new HoldingRegister(144, 2);
        /// <summary>
        ///相机轴拍照位置     
        /// </summary>
        internal HoldingRegister CCD_TargetPoint { get { return m_Register72; } }

        private HoldingRegister m_Register73 = new HoldingRegister(146,2);
        /// <summary>
        /// 当前相机轴的坐标位置
        /// </summary>
        internal HoldingRegister CCD_RealLocation { get { return m_Register73; } }
        #endregion

        #endregion

        /// <summary>
        /// 保持寄存器数组，先修改该数组，读取保持寄存器时再转换
        /// </summary>
        private static List<byte> m_RegisterList = new List<byte>();
        private static byte[] m_RegisterArray;
        /// <summary>
        /// 获取保持寄存器数组
        /// </summary>
        internal static byte[] RegisterArray 
        {
            get {
                try
                {
                    return m_RegisterList.Count < HoldingRegister.TotalLength * 2 ? m_RegisterArray : m_RegisterArray = m_RegisterList.ToArray();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new byte[] { 0 };
                }
            } 
        }

        public AllHoldingRegister()
        {
            m_RegisterArray = new byte[HoldingRegister.TotalLength * 2];
        }

        /// <summary>
        /// 设置保持寄存器的数组  读取时再转换
        /// </summary>
        /// <param name="Recv">接收的数据</param>
        /// <param name="PlusAddr">追加的地址</param>
        internal void SetRegisterArray(byte[] Recv, int PlusAddr)
        {
            byte[] TempArray = new byte[Recv.Length - 3 - 2];//有效数据，前三位是头，后两位是校验位
            int RecvLenght = Recv[2];
            Array.Copy(Recv, 3, TempArray, 0, TempArray.Length);

            if (PlusAddr == 0) m_RegisterList.Clear();//起始位置，先清空，再添加
            m_RegisterList.AddRange(TempArray);
        }

    }
}