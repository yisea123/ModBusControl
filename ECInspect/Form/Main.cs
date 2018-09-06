﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Euresys.Open_eVision_1_2;

namespace ECInspect
{
    public partial class Main : Form
    {
        #region 窗体参数
        private float X; private float Y;
        #endregion

        /// <summary>
        /// 当前的动作模式
        /// </summary>
        private MovementPattern MovePattern = MovementPattern.Normal;
        /// <summary>
        /// 当前的按钮状态
        /// </summary>
        private BtnStatus CurBtnStatus = BtnStatus.Reset;
        /// <summary>
        /// 调试模式是否完成
        /// </summary>
        private bool WaitAdjustEnd = false;
        /// <summary>
        /// 开始测试信号
        /// </summary>
        private AutoResetEvent StartTest = new AutoResetEvent(false);
        /// <summary>
        /// 测试时间
        /// </summary>
        private Stopwatch TestTime = new Stopwatch();

        /// <summary>
        /// Lot号
        /// </summary>
        private string LotNum = string.Empty;
        /// <summary>
        /// 条码枪输入框
        /// </summary>
        private RevBarcodeGunScan BarcodeGunForm;

        myFunction myfunction = new myFunction();
        private Logs log = Logs.LogsT();
        private TextSpeech speaker = TextSpeech.GetSpeaker();
        private System.Collections.Concurrent.ConcurrentQueue<TextInfo> ShowLog = new System.Collections.Concurrent.ConcurrentQueue<TextInfo>();//软件界面右下方的日志
        private readonly Color DefaultTextColor = SystemColors.ControlLightLight;//文字默认颜色
        private readonly Color btn_SystemBackColor, btn_ManualBackColor, btn_PrepareBackColor, btn_ResetColor;//按钮的背景色

        private List<TestPanel> TestStation = new List<TestPanel>();//所有的测试工位，用以显示测试状态
        private List<EC_TestResultItem[]> TotalPunchTestResult = new List<EC_TestResultItem[]>();//所有Punch的测试结果 

        public Main()
        {
            InitializeComponent();
            BarcodeGunForm = new RevBarcodeGunScan();

            this.Resize += new EventHandler(Form_Resize);

            X = this.Width;
            Y = this.Height;
            setTag(this);

            #region 初始化值
            this.btn_SystemBackColor = this.btn_System.BackColor;
            this.btn_ManualBackColor = this.btn_Manual.BackColor;
            this.btn_PrepareBackColor = this.btn_Prepare.BackColor;
            this.btn_ResetColor = this.btn_Reset.BackColor;
            #endregion
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                MovePattern = MovementPattern.Normal;//初始化为通常模式
                ShowMovePattern();
                #region 是否启用条码显示框
                if (INIFileValue.BarcodeScanEnable)
                {
                    label2.Visible = true;
                    textBox_barcode.Visible = true;
                    label__NotStatistics.Location = new Point(5, 116);
                    label_CCDWorkPattern.Location = new Point(3, 522);
                    richTextBox_Process.Size = new Size(274, 400);
                    label_update.Text = "开启";
                }
                else
                {
                    label2.Visible = false;
                    textBox_barcode.Visible = false;
                    label__NotStatistics.Location = new Point(5, 60);
                    label_CCDWorkPattern.Location = new Point(3, 465);
                    richTextBox_Process.Size = new Size(274, 457);
                    label_update.Text = "关闭";
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrMsgBox(ex.Message, "初始化异常");
            }
            finally
            {
                Open_Thread();
                Add_Event();

                this.BringToFront();

                Application.ThreadException +=
       new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException +=
        new System.UnhandledExceptionEventHandler(AppDomain_UnHandledException);
            }
        }

        #region 异常处理
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            log.AddERRORLOG("Application 异常：" + e.Exception.Message + "\r\n" + e.Exception.StackTrace);
#if DEBUG
            MessageBox.Show("Application Error:" + e.Exception.Message, DateTime.Now.ToString("HH:mm:ss"));
#endif
        }
        private void AppDomain_UnHandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is System.Exception)
            {
                log.AddERRORLOG(string.Format("AppDomain 异常：{0}\r\n{1}", e.ExceptionObject.ToString()));
#if DEBUG
                MessageBox.Show("AppDomain Error:" + e.ExceptionObject.ToString(), DateTime.Now.ToString("HH:mm:ss"));
#endif
                //System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);  //重新开启当前程序
                //this.Close();
            }
        }
        #endregion

        /// <summary>
        /// 窗体等待【等待时，界面继续刷新】
        /// </summary>
        /// <param name="MillSecond">等待多少毫秒</param>
        /// <param name="IsResetPLC">是否为复位PLC</param>
        private void FormWait(int MillSecond, bool IsResetPLC = false)
        {
            Sleep(MillSecond, IsResetPLC);//延时等待日志线程更新
            Application.DoEvents();//更新界面
        }

        /// <summary>
        /// 画线---在picturebox里绘制线，避免窗体重绘后线消失
        /// </summary>
        private void DrawLine()
        {
            PictureBox p = this.pictureBox_Display;
            Bitmap b = new Bitmap(p.Width, p.Height);

            Point top = new Point(p.Width / 2, 0);
            Point bottom = new Point(p.Width / 2, p.Height);
            Point left = new Point(0, p.Height / 2);
            Point right = new Point(p.Width, p.Height / 2);

            Graphics g = Graphics.FromImage(b);
            Pen pen = new Pen(Color.White);
            g.DrawLine(pen, top.X, top.Y, bottom.X, bottom.Y);
            g.DrawLine(pen, left.X, left.Y, right.X, right.Y);

            this.pictureBox_Display.Image = b;
        }

        /// <summary>
        /// 读取配置文件后更新测试框
        /// </summary>
        /// <param name="SelectFile">选中的文件</param>
        private void UpdateTestPanel()
        {
            INIFileValue.BlockPoint.Clear();
            INIFileValue.MarkPoint.Clear();
            myfunction.ReadProductIni();
            if (INIFileValue.BlockPoint.Count != INIFileValue.Product_GROUP)
            {
                ErrMsgBox(INIFileValue.Product_NAME + string.Format("文档读取的区块数量{0}与测试数量{1}不一致\r\n请重新开启软件", INIFileValue.BlockPoint.Count, INIFileValue.Product_GROUP), "显示数量不一致");
                return;
            }
            foreach (KeyValuePair<int, Point[]> item in INIFileValue.MarkPoint)
            {
                if (item.Value.Length != INIFileValue.Product_STAMP_TON)
                {
                    ErrMsgBox(INIFileValue.Product_NAME +
                        string.Format("文档读取的第{0}区块的打标坐标数量{1}与对应的打点数量{2}不一致\r\n请重新开启软件",
                                        item.Key,
                                        item.Value.Length,
                                        INIFileValue.Product_STAMP_TON), "打点数量不一致");
                    return;
                }
            }
            //GlobalVar.m_ECTest.SetTRL();
            DrawTestPanel();
        }

        /// <summary>
        /// 绘制测试小框
        /// </summary>
        private void DrawTestPanel()
        {
            Point[] location = INIFileValue.BlockPoint.ToArray();
            #region 重新计算坐标
            List<int> x_spectrum = new List<int>();
            List<int> y_spectrum = new List<int>();
            foreach (Point p in INIFileValue.BlockPoint)
            {
                x_spectrum.Add(Math.Abs(p.X));
                y_spectrum.Add(Math.Abs(p.Y));
            }
            //获取坐标的范围值
            int x_Max = x_spectrum.Max();
            int y_Max = y_spectrum.Max();
            int pic_x = Convert.ToInt32(this.pictureBox_Display.Width * 0.4);
            int pic_y = Convert.ToInt32(this.pictureBox_Display.Height * 0.4);
            List<Point> location_list = new List<Point>();
            foreach (Point p in INIFileValue.BlockPoint)
            {
                double x = 0;
                double y = 0;
                double r = 0;
                if (x_Max != 0) r = Math.Round(((double)p.X / (double)x_Max), 3);
                else r = 0;
                x = r * pic_x;
                if (y_Max != 0) r = Math.Round(((double)p.Y / (double)y_Max), 3);
                else r = 0;
                y = r * pic_y;
                x =  Convert.ToInt32(x);
                y =  Convert.ToInt32(y/3);
                Point point = new Point((int)x, (int)y);
                location_list.Add(point);
            }
            #endregion
            int Width = INIFileValue.Product_VIEW_MODE == 0 ? 30 : INIFileValue.Product_VIEW_MARK_X;
            int Height = INIFileValue.Product_VIEW_MODE == 0 ? 20 : INIFileValue.Product_VIEW_MARK_Y;

            PictureBox picbox = this.pictureBox_Display;

            Point Origin = new Point(picbox.Width / 2, picbox.Height / 2);//原点X、Y

            picbox.Controls.Clear();//foreach 不能修改，只读

            TestStation.Clear();

            int rate = 2;//Convert.ToInt32(myfunction.GetRate(picbox.Handle));//比例
            int SizeRate = 2;//尺寸扩大倍数
            int PanelWidth = Width * rate * SizeRate;//绘制小框的宽度
            if (PanelWidth < 80) PanelWidth = 80;
            int PanelHeight = Height * rate * SizeRate;//绘制小框的高度
            int PunchX, PunchY;//每Punch的坐标，Punch内的小框在此基础上修改
            //增加测试小框
            for (int i = 1; i <= INIFileValue.Product_OKURI; i++)
            {
                PunchX = Origin.X;
                if (i < INIFileValue.Product_OKURI / 2)
                    PunchY = Origin.Y + (PanelHeight + (int)INIFileValue.Product_PITCH / rate) * (INIFileValue.Product_OKURI / 2 - i);
                else
                    PunchY = Origin.Y - (PanelHeight + (int)INIFileValue.Product_PITCH / rate) * (i - INIFileValue.Product_OKURI / 2);
                //PunchY = Origin.Y + (PanelHeight + (int)INIFileValue.Product_PITCH / rate) * (INIFileValue.Product_OKURI/2 - i);
                location = location_list.ToArray();
                //for (int j = 0; j < location.Length; j++)
                //{
                //    TestPanel tp = new TestPanel(PanelWidth, PanelHeight);
                //    tp.ShowText = string.Format("{0}-", i * location.Length - location.Length + j + 1);//小框显示的序号
                //    int X = location[j].X / 100;
                //    X *= rate;
                //    int Y = -location[j].Y / 100;
                //    Y *= rate;
                //    tp.Location = new Point(PunchX + X, PunchY + Y);
                //    picbox.Controls.Add(tp);
                //    TestStation.Add(tp);
                //}
                for (int j = 0; j < location.Length; j++)
                {
                    TestPanel tp = new TestPanel(PanelWidth, PanelHeight);
                    tp.ShowText = string.Format("{0}-", i * location.Length - location.Length + j + 1);//小框显示的序号
                    int X = location[j].X;
                    //X *= rate;
                    int Y = -location[j].Y;
                    //Y *= rate;
                    tp.Location = new Point(PunchX + X, PunchY + Y);
                    picbox.Controls.Add(tp);
                    TestStation.Add(tp);
                }
            }
            picbox.Update();

            this.label_TuTi.Text = string.Format("塗替 {0}", INIFileValue.Product_NK == 1 ? "打开" : "关闭");
            this.linkLabel_Product.Text = INIFileValue.Product_NAME;
            //移动LinkLabel_Product至合适的位置
            this.linkLabel_Product.Location = new Point(this.linkLabel_Product.Parent.Width - this.linkLabel_Product.Width - 5, this.linkLabel_Product.Location.Y);

            if (GlobalVar.CCDEnable)
            {
                if (GlobalVar.CCD == null)
                {
                    GlobalVar.CCD = new BaslerCCD(INIFileValue.CCDSN);
                    GlobalVar.CCD.Event_ImageGrab += new BaslerCCD.dele_ImageGrab(CCD_Event_ImageGrab);//勿动，避免从无相机品目切换至有相机的品目
                }
                AddLogStr(string.Format("CCD 状态:{0}", GlobalVar.CCD.Status));
                this.label_CCDWorkPattern.Text = string.Format("相机工作模式\r\n测试{0}次", INIFileValue.Product_CM_MODE);
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.CCDUse, true);//通知PLC 相机使用
            }
            else
            {
                this.label_CCDWorkPattern.Text = string.Format("相机不启用");
                this.label_CCDWorkPattern.BackColor = Color.Gray;
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.CCDUse, false);//通知PLC 相机不使用
            }
        }

        private int GrabCount = 0;//抓取的次数
        private double MarkX, MarkY;//CCD纠正的坐标

        private void CCD_Event_ImageGrab(Bitmap e)
        {
            try
            {
                GrabCount++;
                AddLogStr(string.Format("第{0}次拍照", GrabCount));
                GlobalVar.FindModel.ShapeFind(ref e);
                EFoundPattern[] info = GlobalVar.FindModel.m_FindResult;
                if (info.Length > 0 &&
                    info[0].Score > 0.55)//0.9分以上才算匹配成功
                {
                    double X_Offset = info[0].Center.X - INIFileValue.MarkX_Std;
                    double Y_Offset = info[0].Center.Y - INIFileValue.MarkY_Std;
                    double rate = 0.002;//像素与坐标的换算关系
                    X_Offset *= rate;
                    Y_Offset *= rate;
                    log.AddCommLOG(string.Format("抓取坐标X:{0}\tY:{1}\r\n偏移量X:{2}\tY:{3}", info[0].Center.X.ToString("#0.000"), info[0].Center.Y.ToString("#0.000"), X_Offset.ToString("#0.000"), Y_Offset.ToString("#0.000")));
                    AddLogStr(string.Format("Score:{0}\r\n偏移量X:{1}\tY:{2}", (info[0].Score * 100).ToString("#0.0"), X_Offset.ToString("#0.000"), Y_Offset.ToString("#0.000")), false, Color.Purple);

                    #region 记录测试数据
                    myfunction.WriteTestCSV(this.textBox_LOT.Text, info[0].Score, X_Offset, Y_Offset);
                    #endregion

                    //纠正Y轴   纠正X轴
                    MarkX = (double)GlobalVar.c_Modbus.HoldingRegisters.AxisX_AssemblyLocation.Value / GlobalVar.ConverRate;
                    MarkY = (double)GlobalVar.c_Modbus.HoldingRegisters.AxisY_JigPoint.Value / GlobalVar.ConverRate;
                    AddLogStr(string.Format("原始值X:{0}\tY:{1}", MarkX.ToString("#0.000"), MarkY.ToString("#0.000")), false, Color.Purple);
                    MarkX += X_Offset;
                    MarkY -= Y_Offset;
                    AddLogStr(string.Format("纠正值X:{0}\tY:{1}", MarkX.ToString("#0.000"), MarkY.ToString("#0.000")), false, Color.Purple);
                    GlobalVar.c_Modbus.XYAxisMove(MarkX, MarkY);

                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.MatchResult, 1);//拍照成功
                    //GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.MatchResult, 2);//临时 拍照失败
                }
                else
                {
                    //用线程再次启动
                    if (GrabCount < INIFileValue.Product_CM_RETRY)
                    {
                        Thread thd = new Thread(new ThreadStart(delegate
                        {
                            Thread.Sleep((int)INIFileValue.Product_CM_DELAY);
                            GlobalVar.CCD.OneShot();
                            Label_Show("重复拍照");//收到拍照信号
                        }));
                        thd.IsBackground = true;
                        thd.Name = "线程拍照";
                        thd.Start();
                    }
                    else
                    {
                        Punch = 0;
                        //e.Save(@"LOG\\Pic\" + DateTime.Now.ToString("HHmmssfff"));
                        AddLogStr("拍照失败，请确认相机拍照结果", true, Color.Red);
                        GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.MatchResult, 2);//拍照失败
                        MsgBoxShow("请确认制品Mark点在相机视野内，并且没有脏污", "拍照失败", Color.OrangeRed, MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                log.AddERRORLOG("主界面抓取图像异常:" + ex.Message);
            }
        }

        /// <summary>
        /// 更新CT
        /// </summary>
        private void UpdateCT()
        {
            if (this.label_SheetCheckTime.InvokeRequired)
            {
                this.label_SheetCheckTime.Invoke(new Action(delegate { UpdateCT(); }));
            }
            else
            {
                this.label_SheetCheckTime.Text = TestTime.Elapsed.TotalSeconds.ToString("#0.0");//CT            
            }
        }

        /// <summary>
        /// 更新统计信息 并保存文件
        /// </summary>
        private void UpdateSheet()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => { UpdateSheet(); }));
            }
            else
            {
                try
                {
                    #region Sheet统计信息
                    this.label_SheetProdcutTestNum.Text = INIFileValue.ProductTestNum.ToString();
                    this.label_SheetProductQualifieNum.Text = INIFileValue.ProductQualifiedNum.ToString();
                    this.label_SheetProductUnQualifieNum.Text = INIFileValue.ProductUnQualifidNUm.ToString();
                    this.label_SheetProdutOpen.Text = INIFileValue.ProductOpen.ToString();
                    this.label_SheetProductShort.Text = INIFileValue.ProductShort.ToString();
                    this.label_SheetOffsetM.Text = INIFileValue.ProductOffsetM.ToString();
                    this.label_SheetOffsetN.Text = INIFileValue.ProductOffsetN.ToString();
                    this.label_SheetForgetPaste.Text = INIFileValue.ProductForgetPaste.ToString();
                    this.label_SheetPressCount.Text = INIFileValue.ProductPressCount.ToString();
                    #endregion

                    this.label__NotStatistics.Text = string.Format("不良率: {0}%", (((double)INIFileValue.ProductUnQualifidNUm / (INIFileValue.ProductQualifiedNum + INIFileValue.ProductUnQualifidNUm)) * 100).ToString("#0.0")); ;//不良率

                    //保存数据
                    myfunction.SaveSheet();
                }
                catch (Exception ex)
                {
                    AddLogStr("保存统计数据失败", true, Color.Red);
                }
            }
        }

        /// <summary>
        /// 开启线程
        /// </summary>
        private void Open_Thread()
        {
            Thread Thd_Test = new Thread(TestProcedure);
            Thd_Test.IsBackground = true;
            Thd_Test.Name = "测试线程";
            Thd_Test.Start();

            Thread Thd_RefreshLog = new Thread(UpdateLog);
            Thd_RefreshLog.IsBackground = true;
            Thd_RefreshLog.Name = "刷新软件界面日志线程";
            Thd_RefreshLog.Start();

            Thread Thd_ShowWaitTest = new Thread(ShowWaitTest);
            Thd_ShowWaitTest.IsBackground = true;
            Thd_ShowWaitTest.Name = "显示等待测试中";
            Thd_ShowWaitTest.Start();

            Thread Thd_CoilChange = new Thread(CoilChange);
            Thd_CoilChange.IsBackground = true;
            Thd_CoilChange.Name = "线圈变化线程";
            Thd_CoilChange.Start();

            Thread Thd_MarkAlarm = new Thread(MarkAlarm);
            Thd_MarkAlarm.IsBackground = true;
            Thd_MarkAlarm.Name = "打点笔报警线程";
            Thd_MarkAlarm.Start();

            Thread Thd_Other = new Thread(OtherOperation);
            Thd_Other.IsBackground = true;
            Thd_Other.Name = "其他操作线程";
            Thd_Other.Start();

            Thread updateResultToDataBase_th = new Thread(th_updateResultToDataBase);//增加数据上传线程 [2018/03/03 lqz]
            updateResultToDataBase_th.IsBackground = true;
            updateResultToDataBase_th.Name = "上传数据线程";
            updateResultToDataBase_th.Start();
        }

        /// <summary>
        /// 增加委托事件
        /// </summary>
        private void Add_Event()
        {
            if (GlobalVar.m_ECTest != null)
            {
                GlobalVar.m_ECTest.Event_RunStatus += new ECTest.dele_RunStatus(m_ECTest_Event_RunStatus);
            }
            if (GlobalVar.c_Modbus != null)
            {
                GlobalVar.c_Modbus.Coils.Event_CoilValueChanged += new AllCoil.dele_CoilValueChanged(Coils_Event_CoilValueChanged);
                GlobalVar.c_Modbus.event_AxisMove += new CModbus.dele_AxisMove(c_Modbus_event_AxisMove);
            }
            if (GlobalVar.PCSoftware != null)
            {
                GlobalVar.PCSoftware.EventShowStr += new IISUpdate.dele_ShowStr(PCSoftware_EventShowStr);
            }
            if (GlobalVar.CCD != null)
            {
                GlobalVar.CCD.Event_ShowException += new BaslerCCD.dele_ShowException(CCD_Event_ShowException);
                GlobalVar.CCD.Event_CCDStatusChanged += new BaslerCCD.dele_CCDStatusChanged(CCD_Event_CCDStatusChanged);
            }
        }

        private void textBox_LOT_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalVar.c_Modbus == null || !GlobalVar.c_Modbus.Coils.StartLeft.Value) return;

                using (Keyboard form = new Keyboard(false))
                {
                    form.WindowState = FormWindowState.Maximized;
                    form.TopLevel = false;
                    form.Parent = this;
                    form.MdiParent = this.MdiParent;
                    if (form.ShowDialog() != DialogResult.OK) return;
                    this.textBox_LOT.Text = this.LotNum = form.RealValue.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrMsgBox(ex.Message);
            }
        }

        /// <summary>
        /// PLC空闲，并且非测试中，则显示测试等待
        /// </summary>
        private void ShowWaitTest()
        {
            do
            {
                Thread.Sleep(1000);

                //PLC 未开始计时，测试空闲，急停 未按下
                if (GlobalVar.c_Modbus.Coils.CT.Value || !Idle || GlobalVar.c_Modbus.Coils.EMCStop.Value)
                {
                    //Label_Show(string.Empty);
                    continue;
                }

                Label_Show("测试等待中...");

                Thread.Sleep(1000);//避免刚开启软件时，显示“复位”，又立刻显示条码输入框
                while (MsgBox.IsShow) Thread.Sleep(100);//有弹框时，暂时不显示输入条码的框

                if (INIFileValue.BarcodeGunEnable && this.MovePattern == MovementPattern.Normal) ShowBarcodeGunForm(true);//显示条码输入框
                else ShowBarcodeGunForm(false);//隐藏条码输入框
            }
            while (!GlobalVar.SoftWareShutDown);
        }

        /// <summary>
        /// 显示\隐藏 条码输入框
        /// </summary>
        private void ShowBarcodeGunForm(bool Show)
        {
            if (Show) BarcodeGunShow();
            else BarcodeGunHide();
        }
        /// <summary>
        /// 显示条码输入框
        /// </summary>
        private void BarcodeGunShow()
        {
            if (this.pictureBox_Display.Controls.Contains(BarcodeGunForm) && this.BarcodeGunForm.Visible) return;
            if (!string.IsNullOrEmpty(this.BarcodeGunForm.Barcode)) return;
            if (this.pictureBox_Display.InvokeRequired)
            {
                this.pictureBox_Display.Invoke(new Action(delegate
                {
                    this.pictureBox_Display.Controls.Add(BarcodeGunForm);
                }));
            }
            else this.pictureBox_Display.Controls.Add(BarcodeGunForm);
            if (this.BarcodeGunForm.InvokeRequired)
            {
                this.BarcodeGunForm.Invoke(new Action(delegate
                {
                    BarcodeGunShowInvoke();
                }));
            }
            else BarcodeGunShowInvoke();
        }
        private void BarcodeGunShowInvoke()
        {
            BarcodeGunForm.Location = new Point((this.pictureBox_Display.Width - this.BarcodeGunForm.Width) / 2, (this.pictureBox_Display.Height - this.BarcodeGunForm.Height) / 2);
            BarcodeGunForm.BringToFront();
            BarcodeGunForm.Visible = true;
            BarcodeGunForm.Focus();
        }

        /// <summary>
        /// 隐藏条码输入框
        /// </summary>
        private void BarcodeGunHide()
        {
            if (this.BarcodeGunForm.InvokeRequired)
            {
                this.BarcodeGunForm.Invoke(new Action(delegate
                {
                    BarocodeGunHideInvoke();
                }));
            }
            else BarocodeGunHideInvoke();
        }
        private void BarocodeGunHideInvoke()
        {
            if (!this.pictureBox_Display.Controls.Contains(BarcodeGunForm) || !BarcodeGunForm.Visible) return;
            this.pictureBox_Display.Controls.Remove(BarcodeGunForm);
            BarcodeGunForm.Visible = false;
        }

        #region 
        /// <summary>
        /// 上传测试结果的线程[2018/03/03 lqz]
        /// </summary>
        private void th_updateResultToDataBase()
        {
            while (true)
            {
                if (GlobalVar.gl_UpSql)
                {
                    updateResultToDataBase();
                }
            }
        }

        /// <summary>
        /// 数据上传线程 [2018/03/03 lqz]
        /// </summary>
        private void updateResultToDataBase()
        {
            int WaitTime = 10000;//10秒
            try
            {
                string dir_path = Application.StartupPath + GlobalVar.gl_TestResultPath;
                if (!Directory.Exists(dir_path)) Directory.CreateDirectory(dir_path);
                DirectoryInfo TheFolder = new DirectoryInfo(Application.StartupPath + GlobalVar.gl_TestResultPath);  //获取文件名称
                FileInfo[] files = TheFolder.GetFiles();
                if (files.Length > 0)
                {
                    InsertTestResult(files);
                    Thread.Sleep(1000);
                }
                else Thread.Sleep(10000);
            }
            catch (DirectoryNotFoundException dnfe) { Thread.Sleep(WaitTime); }
            catch (Exception ex)
            {
                AddLogStr("新增测试结果到数据库 线程异常:" + ex.Message);
            }
        }
        /// <summary>
        ///     读取文件  新增测试结果数据到数据库[2018/03/03 lqz]
        /// </summary>
        private void InsertTestResult(FileInfo[] Files)
        {
            for (int i = 0; i < Files.Length; i++)
            {
                try
                {
                    string content = File.ReadAllLines(Files[i].FullName)[0]; //File.ReadAllText(Files[i].FullName);
                    if (string.IsNullOrEmpty(content.Trim('\0')))
                    {
                        File.Delete(Files[i].FullName);//内容为空 直接删除该文件
                        return;
                    }

                    string[] str = content.Split(',');
                    string filename = Files[i].Name.Substring(0, Files[i].Name.IndexOf("."));

                    int j = 0;
                    TestResult tr = new TestResult();
                    tr.ShtBarcode = str[j++].Trim();
                    tr.PcsIndex = Convert.ToInt32(str[j++]);
                    tr.Result = Convert.ToInt32(str[j++]);
                    tr.FlowId = Convert.ToInt32(str[j++]);
                    tr.CreateUser=str[j++].Trim();//CreateUser
                    tr.CreateDate = DateTime.ParseExact(str[j], GlobalVar.TimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                    string database_name = ConfirmDataBaseName(filename);
                    if (string.IsNullOrEmpty(database_name)) continue;
                    DBQuery db = new DBQuery();
                    if (db.InsertTestResult(database_name, tr) && db.InsertNGResultInBM(tr))
                    {
                        //File.Copy(Files[i].FullName,Application.StartupPath+@"\temp_Result\"+Files[i].Name,true);
                        File.Delete(Files[i].FullName);//写入数据库成功后删除该文件
                    }
                }
                catch (Exception ex)
                {
                    log.AddERRORLOG("写测试结果异常：" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 写本地日志
        /// </summary>
        /// <param name="tr"></param>
        private void WriteSimpleResult(string sheet, string result, string date)
        {
            try
            {
                string dirpath = Application.StartupPath + @"\LOG\Simple\";
                string filepath = dirpath + DateTime.Now.ToString("yyyyMMddHH");
                if (!Directory.Exists(dirpath)) Directory.CreateDirectory(dirpath);
                bool CsvCreate = false;
                string _logfile = filepath + ".csv";
                CsvCreate = File.Exists(_logfile);

                FileStream FS = new FileStream(_logfile, FileMode.Append, FileAccess.Write, FileShare.Write);
                StreamWriter SW = new StreamWriter(FS, Encoding.Default);

                if (!CsvCreate)
                    SW.Write(string.Format("SheetBarcode,结果,日期\r\n"));
                string writestr = string.Format("{0},{1},{2}\r\n",
                sheet,
                result,
                date
                );
                SW.Write(writestr);
                SW.Close();
                SW.Dispose();
            }
            catch (Exception ex)
            {
                log.AddERRORLOG("WriteCSV Error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 确定机种名称  (数据库名称)
        /// </summary>
        /// <param name="str">待识别的名称</param>
        /// <returns></returns>
        private string ConfirmDataBaseName(string str)
        {
            string database = string.Empty;

            int length = str.IndexOf("TFLEX");
            if (length > 0) database = str.Substring(0, str.IndexOf('-'));

            return database;
        }
        #endregion



        /// <summary>
        /// 其他操作（以线程方式执行）
        /// </summary>
        private void OtherOperation()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => { OtherOperation(); }));
            }
            else
            {
                #region 2018.1.5启用 上线后更新一版本后删除
                if (Directory.Exists(Application.StartupPath + string.Format(@"\TestResult\"))) Directory.Delete(Application.StartupPath + string.Format(@"\TestResult\"), true);
                #endregion

                DrawLine();                     //画十字线
                DrawTestPanel();                //绘制小框
                UpdateSheet();                  //更新统计信息
                Init();

                ErgodicPLCErr(true);

#if !DEBUG
                log.AddCommLOG(string.Format("重命名桌面快捷方式结果:{0}\t重命名启动快捷方式结果:{1}",
                         myfunction.RenameDesktopLnk("EC电测上位机(含相机)"),
                         myfunction.RenameStartupLnk("EC电测上位机(含相机)")));
#endif

                ResetPLC(false);//软件开启，通知系统复位
            }
        }

        /// <summary>
        /// 程序初始化
        /// </summary>
        private void Init()
        {
            Label_Show(string.Empty);
        }

        /// <summary>
        /// 上一次显示的字符串，相同则不更新
        /// </summary>
        private string LastLabelShow = string.Empty;
        /// <summary>
        /// 更改显示的字符串
        /// </summary>
        /// <param name="str">显示的字符串</param>
        private void Label_Show(string str)
        {
            Color forecolor = Color.White;
            Font font = new Font("Microsoft YaHei", 18F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(134)));
            Point location = new Point(9, 1040);//位置
            if (GlobalVar.c_Modbus.Coils.EMCStop.Value || GlobalVar.c_Modbus.Coils.EMCStop.Value)
            {
                str = "急停按下";
                forecolor = Color.Red;
                font = new Font("Microsoft YaHei", 188F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(134)));
                location = new Point(156, 550);
            }

            if (string.Equals(LastLabelShow, str)) return;//字符串相同则不更新
            LastLabelShow = str;
            if (this.label_Show.InvokeRequired)
            {
                this.label_Show.BeginInvoke(new Action(() =>
                {
                    SetLabel_Show(str, forecolor, font, location);
                }));
            }
            else
            {
                SetLabel_Show(str, forecolor, font, location);
            }
        }

        /// <summary>
        /// 设置label【不能调用】
        /// </summary>
        /// <param name="Text">文本</param>
        /// <param name="ForeColor">前景色</param>
        /// <param name="font">字体</param>
        /// <param name="location">位置</param>
        private void SetLabel_Show(string Text, Color ForeColor, Font font, Point location)
        {
            this.label_Show.Text = Text;
            this.label_Show.ForeColor = ForeColor;
            this.label_Show.Font = font;
            this.label_Show.Location = location;
        }

        private void m_ECTest_Event_RunStatus(string str, EC_RunType type)
        {
            switch (type)
            {
                case EC_RunType.Normal:
                    AddLogStr(str, false, Color.LightSkyBlue);
                    break;
                case EC_RunType.CommInfo:
                    log.AddCommLOG(str);
                    break;
                case EC_RunType.Error:
                    MsgBoxShow(str, "测试主机", Color.Red, MessageBoxButtons.OK);
                    AddLogStr(str, true, Color.MediumVioletRed);
                    ResetPLC(false);
                    break;
            }
        }
        DateTime LastPhoto;//上一次拍照的时间
        /// <summary>
        /// 监测线圈变化
        /// </summary>
        private void CoilChange()
        {
            while (!GlobalVar.SoftWareShutDown)
            {
                //CoilChangeSet.WaitOne();
                if (CoilChangeList.Count == 0)
                {
                    Thread.Sleep(50);
                    continue;
                }
                int Addr = CoilChangeList[0];
                CoilChangeList.RemoveAt(0);
                Console.WriteLine("{0}\tChanged Coil Addr\t{1}", DateTime.Now.ToString("HH:mm:ss:fff"), Addr);
                if (CheckCoil(Addr, GlobalVar.c_Modbus.Coils.StartGrab) == 1)
                {
                    if ((DateTime.Now - LastPhoto).TotalMilliseconds < 500) continue;//忽略时间间隔小于500毫秒的通知拍照
                    LastPhoto = DateTime.Now;
                    Thread.Sleep((int)INIFileValue.Product_CM_DELAY);
                    GrabCount = 0;
                    //Label_Show("通知拍照");//收到拍照信号
                    GlobalVar.CCD.OneShot();
                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.StartGrab, false);
                    continue;
                }
                if (CheckCoil(Addr, GlobalVar.c_Modbus.Coils.StartTest) == 1)
                {
                    INIFileValue.ProductPressCount++;//认为启动一次测试，压合一次
                    StartTest.Set();
                    //Label_Show(string.Empty, Color.Black);//收到启动信号，清空显示信息
                    continue;
                }
                if (CheckCoil(Addr, GlobalVar.c_Modbus.Coils.BarocodeReady) == 1)
                {
                    if (INIFileValue.BarcodeScanEnable)//接收到扫描信号--[2018.3.29 lqz]
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            textBox_barcode.Text = "";
                        }));
                        GlobalVar.gl_Barcode = "";//清空条码信息
                        if (GlobalVar.gl_Scan.StartScan())
                        {
                            GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.MatchResult, 3);//扫描条码成功
                            this.BeginInvoke(new Action(() =>
                            {
                                textBox_barcode.Text = GlobalVar.gl_Barcode;
                            }));
                        }
                        else
                        {
                            GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.MatchResult, 4);//扫描条码失败
                            this.BeginInvoke(new Action(() =>
                            {
                                textBox_barcode.Text = "扫描条码失败";
                            }));
                            MsgBoxShow("扫描条码失败！", "扫码", Color.Red, MessageBoxButtons.OK);
                        }
                        GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.BarocodeReady, false);//置0
                    }
                }
                //if (RunningMode == RunMode.Prepare)
                //{
                //    if (CheckCoil(Addr, GlobalVar.c_Modbus.Coils.SensorUpJog) == 1) ;
                //    {
                //        AddLogStr("上治具取出", false, Color.LightSkyBlue);
                //        return;
                //    }
                //}

                int ct = CheckCoil(Addr, GlobalVar.c_Modbus.Coils.CT);
                if (ct == 1) TestTime.Restart();
                else if (ct == 0)
                {
                    TestTime.Stop();
                    //更新CT
                    UpdateCT();
                }
                int EMC = CheckCoil(Addr, GlobalVar.c_Modbus.Coils.EMCStop);
                if (EMC == 1)
                {
                    Label_Show(string.Empty);
                    continue;
                }
                else if (EMC == 0)
                {
                    Label_Show(string.Empty);

                    Thread thd = new Thread(new ThreadStart(delegate
                    {
                        this.Invoke(new Action(delegate
                        {
                            ResetPLC(false);//松开急停，通知系统复位
                        }));
                    }));
                    thd.Name = "异步系统复位";
                    thd.IsBackground = true;
                    thd.Start();
                    continue;
                }
                int PrepareEMC = CheckCoil(Addr, GlobalVar.c_Modbus.Coils.EMCStop);
                if (PrepareEMC == 1)
                {
                    Label_Show(string.Empty);
                    continue;
                }
                else if (PrepareEMC == 0)
                {
                    Label_Show(string.Empty);
                    continue;
                }
                ShowPLCErr(Addr);//如果有PLC的异常信息，则弹框显示
            }
        }

        //private AutoResetEvent CoilChangeSet = new AutoResetEvent(false);
        private List<int> CoilChangeList = new List<int>();//改变的线圈的队列

        private void Coils_Event_CoilValueChanged(int Addr)
        {
            //CoilChangeSet.Set();
            CoilChangeList.Add(Addr);
        }

        /// <summary>
        /// 检查线圈的值 【-1：非该线圈；0：False；1：True】
        /// </summary>
        /// <param name="AddrChange">变动的地址</param>
        /// <param name="coil">判断的线圈</param>
        private int CheckCoil(int AddrChange, Coil coil)
        {
            if (AddrChange == coil.Addr) return coil.Value ? 1 : 0;
            else return -1;
        }

        private bool PLCErrBOXShow = false;
        /// <summary>
        /// 显示PLC的异常
        /// </summary>
        private void ShowPLCErr(int Addr)
        {
            try
            {
                if (Addr < GlobalVar.c_Modbus.Coils.PLCErr2.Addr) return;

                ErgodicPLCErr();
            }
            catch (Exception ex)
            {
                log.AddERRORLOG("ShowPLCErr:" + ex.Message);
            }
        }

        /// <summary>
        /// 遍历PLC的异常
        /// </summary>
        /// <param name="Init">是否初始化</param>
        private void ErgodicPLCErr(bool Init = false)
        {
            if (GlobalVar.c_Modbus == null) return;
            for (int i = GlobalVar.c_Modbus.Coils.PLCErr2.Addr; i <= GlobalVar.c_Modbus.Coils.BitDatas[GlobalVar.c_Modbus.Coils.BitDatas.Length - 1].Addr; i++)
            {
                PLCErrShowNot(i, GlobalVar.c_Modbus.Coils.BitDatas[i - GlobalVar.c_Modbus.Coils.PLCErr2.Addr + GlobalVar.c_Modbus.Coils.PLCErr2.Index]);
            }

            while (MsgBox.IsShow) FormWait(100);
            PLCErrBOX(Init);
        }

        private void PLCErrShowNot(int addr, Coil coil)
        {
            int Index = addr - GlobalVar.c_Modbus.Coils.PLCErr2.Addr;
            int res = CheckCoil(addr, coil);
            if (res == 1)
            {
                if (Index < GlobalVar.PLCErrDir.Count) GlobalVar.PLCErrDir[Index].Show = true;
                else log.AddERRORLOG("PLC的异常线圈大于读取XML的异常数量");
            }
            else if (res == 0)
            {
                if (Index < GlobalVar.PLCErrDir.Count) GlobalVar.PLCErrDir[Index].Show = false;
                else log.AddERRORLOG("PLC的异常线圈大于读取XML的异常数量");
            }
        }

        /// <summary>
        /// PLC异常弹框
        /// </summary>
        /// <param name="Init">是否初始化</param>
        private void PLCErrBOX(bool Init = false)
        {
            StringBuilder str = new StringBuilder();
            foreach (KeyValuePair<int, PLCErr> item in GlobalVar.PLCErrDir)
            {
                if (item.Value.Show) str.AppendLine(item.Value.ErrStr);
            }
            if (string.IsNullOrEmpty(str.ToString()) || PLCErrBOXShow)
            {
                if (!Init) AddLogStr("报警已经解除！", false, Color.BlueViolet);
                return;//无异常或者异常框已经显示，则不显示PLC的异常框
            }

            Thread thd = new Thread(new ThreadStart(delegate
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(delegate
                    {
                        PLCErrBOXShow = true;
                        ErrPLCMsgBox(str.ToString());
                        GlobalVar.c_Modbus.CoilMsgSync(GlobalVar.c_Modbus.Coils.AlarmRelease, true);//同步方法  解除报警
                        PLCErrBOXShow = false;

                        if (!GlobalVar.c_Modbus.Coils.AlarmNeedReset.Value)//判断是否需要复位                      
                            ResetPLC(false);
                        else Thread.Sleep(100);

                        if (!MsgBox.IsShow) ErgodicPLCErr();
                    }));
                }
                else log.AddERRORLOG("线程 更新 界面 异常,Main thread  ");
            }));
            thd.Name = "PLC异常框";
            thd.IsBackground = true;
            thd.Start();
        }

        /// <summary>
        /// 打点笔报警的线程
        /// </summary>
        private void MarkAlarm()
        {
            Thread.Sleep(30 * 1000);//延时30秒，等待软件完全开启后
            while (!GlobalVar.SoftWareShutDown)
            {
                try
                {
                    if (INIFileValue.ProductMarkLeftTime <= 0)
                    {
                        if (INIFileValue.AlarmHour == DateTime.Now.Hour && INIFileValue.AlarmMinute == DateTime.Now.Minute)
                        {
                            while (MsgBox.IsShow) Thread.Sleep(100);
                            if (this.InvokeRequired) this.BeginInvoke(new MethodInvoker(delegate { MsgBoxShow("打点笔达到使用寿命", "打点笔", Color.Red, MessageBoxButtons.OK); }));
                            else MsgBoxShow("打点笔达到使用寿命", "打点笔", Color.Red, MessageBoxButtons.OK);
                            Thread.Sleep(60 * 1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.AddERRORLOG("打点笔报警线程异常:" + ex.Message);
                }
                finally
                {
                    Thread.Sleep(30 * 1000);
                }
            }
        }

        private void c_Modbus_event_AxisMove(string str, bool iserr)
        {
            Label_Show(str);
            AddLogStr(str, false, !iserr ? Color.LightSeaGreen : Color.Red);
        }

        private void PCSoftware_EventShowStr(string msg)
        {
            AddLogStr(msg, false, Color.LightGreen);
        }

        private void CCD_Event_ShowException(string err)
        {
            AddLogStr(string.Format(err), true, Color.Red);
        }

        private void CCD_Event_CCDStatusChanged(CCDStatus status)
        {
            AddLogStr(string.Format("CCD 状态:{0}", status), false, Color.DeepPink);
        }

        #region 字体大小随窗体变化
        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    setTag(con);
            }
        }
        private void setControls(float newx, float newy, Control cons)
        {
            try
            {
                foreach (Control con in cons.Controls)
                {
                    string[] mytag = con.Tag.ToString().Split(new char[] { ':' });
                    float a = Convert.ToSingle(mytag[0]) * newx;
                    con.Width = (int)a;
                    a = Convert.ToSingle(mytag[1]) * newy;
                    con.Height = (int)(a);
                    a = Convert.ToSingle(mytag[2]) * newx;
                    con.Left = (int)(a);
                    a = Convert.ToSingle(mytag[3]) * newy;
                    con.Top = (int)(a);
                    Single currentSize = Convert.ToSingle(mytag[4]) * newy;
                    con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                    if (con.Controls.Count > 0)
                    {
                        setControls(newx, newy, con);
                    }
                }
            }
            catch { }
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            // throw new Exception("The method or operation is not implemented.");
            float newx = (this.Width) / X;
            //  float newy = (this.Height - this.statusStrip1.Height) / (Y - y);
            float newy = this.Height / Y;
            setControls(newx, newy, this);
            //this.Text = this.Width.ToString() +" "+ this.Height.ToString();

        }
        #endregion

        private void btn_System_Click(object sender, EventArgs e)
        {
            ShowFrame(ClickBtn.System);
        }

        private void btn_Manual_Click(object sender, EventArgs e)
        {
            ShowFrame(ClickBtn.Manual);
        }

        private void btn_Prepare_Click(object sender, EventArgs e)
        {
            ShowFrame(ClickBtn.Prepare);
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            ShowFrame(ClickBtn.Reset);
        }

        private void tableLayoutPanel_Statistics_Click(object sender, EventArgs e)
        {
            ShowFrame(ClickBtn.BadStatistics);
            UpdateSheet();//如果不良统计窗口清零，则需要重新显示统计信息
        }

        private void ShowFrame(ClickBtn cb)
        {
            if (GlobalVar.c_Modbus == null || !GlobalVar.c_Modbus.Coils.StartLeft.Value) return;
            if (GlobalVar.SystemReset) GlobalVar.SystemReset = false;//如果是复位状态，则停止复位
            RunMode temp_Mode = GlobalVar.RunningMode;//临时转换
            try
            {
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.Mode, false);
                GlobalVar.RunningMode = RunMode.NotMain;
                Frame form = null;
                this.label_MovementPattern.Enabled = false;

                if (GlobalVar.CCD != null) GlobalVar.CCD.Event_ImageGrab -= CCD_Event_ImageGrab;
                switch (cb)
                {
                    case ClickBtn.System:
                        form = new SystemForm();
                        break;
                    case ClickBtn.Manual:
                        form = new ManualForm();
                        break;
                    case ClickBtn.Prepare:
                        Prepare();
                        return;
                    case ClickBtn.Reset:
                        switch (CurBtnStatus)
                        {
                            case BtnStatus.Reset:
                                ResetPLC(true);
                                break;
                            case BtnStatus.Stop:
                                GlobalVar.m_ECTest.TestPause = true;
                                MsgBox box = new MsgBox("继续测试", "系统复位");
                                box.Title = "提示";
                                box.ShowText = string.Empty;
                                if (box.ShowDialog() == DialogResult.OK)
                                {
                                    GlobalVar.m_ECTest.TestPause = false;//继续测试
                                    return;
                                }

                                GlobalVar.m_ECTest.TestPause = false;//触发中断测试

                                GlobalVar.m_ECTest.AbortTest();
                                Thread.Sleep(500);
                                ResetPLC(false);
                                break;
                        }
                        return;
                    case ClickBtn.BadStatistics:
                        form = new BadStatisticsForm();
                        break;
                    case ClickBtn.CCD:
                        if (GlobalVar.CCD == null) return;
                        form = new CCDForm();
                        break;
                    default:
                        //默认窗口，测试用···
                        form = new Frame();
                        break;
                }
                form.TopLevel = false;
                form.Parent = this;
                form.MdiParent = this.MdiParent;
                form.ShowDialog();

            }
            catch (Exception ex)
            {
                Console.WriteLine("ShowFrame Err:" + ex.Message);
            }
            finally
            {
                if (GlobalVar.CCD != null) GlobalVar.CCD.Event_ImageGrab += new BaslerCCD.dele_ImageGrab(CCD_Event_ImageGrab);
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.Mode, true);
                this.label_MovementPattern.Enabled = true;
                GlobalVar.RunningMode = temp_Mode;//还原当前的运行模式
            }
        }

        private void label_MovementPattern_Click(object sender, EventArgs e)
        {
            if (!GlobalVar.c_Modbus.Coils.StartLeft.Value) return;

            byte Index = (byte)MovePattern;
            if (Index + 1 < Enum.GetNames(typeof(MovementPattern)).Length)
            {
                Index++;
            }
            else Index = 0;
            MovePattern = (MovementPattern)Index;

            ShowMovePattern();//显示当前的动作模式
            AddLogStr(this.label_MovementPattern.Text);
        }

        /// <summary>
        /// 显示当前动作模式
        /// </summary>
        private void ShowMovePattern()
        {
            if (this.label_MovementPattern.InvokeRequired)
            {
                this.label_MovementPattern.Invoke(new Action(delegate { ShowMovePattern(); }));
            }
            else
            {
                string Title = "动作模式\r\n";
                //改变显示的字符串
                switch (MovePattern)
                {
                    case MovementPattern.Normal:
                        this.label_MovementPattern.Text = Title + "通常";
                        break;
                    case MovementPattern.ProductSample:
                        this.label_MovementPattern.Text = Title + "制品样本";
                        break;
                    case MovementPattern.SinglePress:
                        this.label_MovementPattern.Text = Title + "单次压合测试";
                        break;
                    case MovementPattern.Adjustment:
                        this.label_MovementPattern.Text = Title + "调整";
                        break;
                }
                //改变背景色和前景色
                switch (MovePattern)
                {
                    case MovementPattern.Normal:
                    case MovementPattern.ProductSample:
                    case MovementPattern.SinglePress:
                        this.label_MovementPattern.BackColor = Color.MediumBlue;
                        this.label_MovementPattern.ForeColor = Color.White;
                        break;
                    case MovementPattern.Adjustment:
                        this.label_MovementPattern.BackColor = Color.Yellow;
                        this.label_MovementPattern.ForeColor = SystemColors.ControlText;
                        break;
                }

                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.MovementPattern, (byte)MovePattern);
            }
        }

        /// <summary>
        /// 段取的异常信息
        /// </summary>
        private string ErrPrepare = string.Empty;
        /// <summary>
        /// 段取计数
        /// </summary>
        private int PrepareCount = 0;
        /// <summary>
        /// 段取流程
        /// </summary>
        private void Prepare()
        {
            dele_ChangeStatus changstr = new dele_ChangeStatus(ChangeStatus);
            try
            {
                ErrPrepare = string.Empty;

                if (PrepareMsgBox("开始段取?", MessageBoxButtons.YesNo) != DialogResult.OK) return;
                /***正式进入段取流程***/
                log.AddCommLOG(string.Format("&&第{0}次段取 正式进入&&", ++PrepareCount));
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.Prepare, 1);//通知PLC进入段取模式D180

                if (!PrepareCheck("按下紧急停止键，交换【治具】", GlobalVar.c_Modbus.Coils.PrepareEMCStop)) throw new PrepareCancle("取消段取");
                if (!PrepareCheck("交换完了后，解除紧急停止状态", GlobalVar.c_Modbus.Coils.PrepareEMCStop, false)) throw new PrepareCancle("取消段取");
                if (GlobalVar.c_Modbus.Coils.SensorUpJog.Value)
                {
                    if (!PrepareCheck("上治具未推到最里面，确认段取用的隔离销是否装到治具上", GlobalVar.c_Modbus.Coils.SensorUpJog, false))
                        throw new PrepareCancle("取消段取"); ;
                }
                if (!PrepareCheck("放置段取用的隔离销，将段取用的隔离销装到下治具上", GlobalVar.c_Modbus.Coils.LsolationPin)) throw new PrepareCancle("取消段取");
                if (!PrepareCheck("插入移动存储盘，按【右开始键】，段取开始")) throw new PrepareCancle("取消段取");
                if (!GlobalVar.c_Modbus.Coils.StartRight.Value)
                {
                    AddLogStr("等待按下右启动键");
                    FormWait(100);
                }
                while (!GlobalVar.c_Modbus.Coils.StartRight.Value)
                {
                    Sleep(100);
                }
                AddLogStr("右启动键按下，开始读取移动存储盘");
                ReadUDisk();
                AddLogStr("读取移动存储盘完成");
                #region U盘退出
                UsbEject.VolumeDeviceClass volumeDeviceClass = new UsbEject.VolumeDeviceClass();
                foreach (UsbEject.Volume device in volumeDeviceClass.Devices)
                {
                    // is this volume on USB disks?  
                    if (!device.IsUsb)
                        continue;

                    // is this volume a logical disk?  
                    if ((device.LogicalDrive == null) || (device.LogicalDrive.Length == 0))
                        continue;

                    device.Eject(true); // allow Windows to display any relevant UI  
                }

                AddLogStr("移动存储盘退出，可以拔出移动存储盘", false, Color.LimeGreen);
                #endregion
                this.Invoke(changstr, BtnStatus.Stop);//显示“停止”
                if (!PrepareCheck("确认段取用的隔离销是否装到治具上")) throw new PrepareCancle("取消段取");   //无法判断
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.Prepare, 7);//通知PLC读取U盘结束

                PrepareCheck("请同时按下左右启动键，运行至压合位置", "运动完成", 8, true);
                if (GlobalVar.c_Modbus.Coils.SensorUpJog.Value)
                {
                    if (!PrepareCheck("将上治具推到最里面", GlobalVar.c_Modbus.Coils.SensorUpJog, false)) throw new PrepareCancle("取消段取");
                }
                PrepareCheck("请同时按下左右启动键，开始压合", "压合完成", 9, true);
                PrepareCheck("请同时按下左右启动键，抬起汽缸", "抬起完成", 10, true);
                PrepareCheck("请确认隔离销没有卡在上治具，然后同时按下左右启动键");
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.Prepare, 100);
                PrepareCheck("开始轴复位", "轴复位完成", 11);
                if (!PrepareCheck("从下治具取出段取用的隔离销", GlobalVar.c_Modbus.Coils.LsolationPin, false)) throw new PrepareCancle("取消段取"); ;//隔离销放回
                PrepareMsgBox("请放入制品\r\n同时按下左右启动键，开始调整", MessageBoxButtons.OK);// PrepareCheck("请同时按下左右启动键，开始调整", string.Empty, 12, true);
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.Prepare, 122);

                MovePattern = MovementPattern.Adjustment;
                ShowMovePattern();//更新为调整模式

                WaitAdjustEnd = false;
                while (!WaitAdjustEnd)//等待调整结束
                {
                    FormWait(100);
                }

                if (!string.IsNullOrEmpty(ErrPrepare)) return;

                if (GlobalVar.CCDEnable)
                {
                    PrepareMsgBox("准备调整相机位置", MessageBoxButtons.OK);
                    ShowFrame(ClickBtn.CCD);
                    PrepareMsgBox("关闭调整相机位置用的门", MessageBoxButtons.OK);
                }
                PrepareMsgBox("段取结束，手动调整吸附装置", MessageBoxButtons.OK);
                PrepareMsgBox("请在手动模式下关闭低黏着装置后在更换", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                if (ex is PrepareCancle)
                {
                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.Prepare, 0);//通知PLC段取取消
                    AddLogStr("用户取消段取");
                    return;
                }
                if (ex is ResetPLC)
                {
                    log.AddCommLOG("段取流程中，PLC复位");
                    return;
                }

                ErrMsgBox(ex.Message, "段取异常");
            }
            finally
            {
                this.Invoke(changstr, BtnStatus.Reset);//显示“返回原点”
                log.AddCommLOG(string.Format("&&第{0}次段取 完全结束&&", PrepareCount));
            }
        }

        /// <summary>
        /// 段取过程中的检查与判断
        /// </summary>
        /// <param name="Msg">提示操作的信息</param>
        private bool PrepareCheck(string Msg)
        {
            if (PrepareMsgBox(Msg) == DialogResult.Cancel) return false;

            return true;
        }

        /// <summary>
        /// 检查与判断
        /// </summary>
        /// <param name="Msg">提示操作的信息</param>
        /// <param name="coil">需要判断的线圈</param>
        /// <param name="Press">判断是否按下 【0:False；1:True】</param>
        private bool PrepareCheck(string Msg, Coil coil, bool Press = true)
        {
            if (CoilMsgBox(Msg, coil, Press, "段取流程", MessageBoxButtons.OK) == DialogResult.Cancel) return false;

            return true;
        }

        /// <summary>
        /// 检查保持寄存器的值是否满足
        /// </summary>
        /// <param name="TipMsg">提示消息</param>
        /// <param name="WaitMsg">等待时的消息</param>
        /// <param name="Value">判断的值</param>
        /// <param name="NeedBothStart">是否需要双手启动</param>
        private void PrepareCheck(string TipMsg, string WaitMsg, int Value, bool NeedBothStart = false)
        {
            if (!string.IsNullOrEmpty(TipMsg)) AddLogStr(TipMsg, false, Color.LightGreen);
            if (NeedBothStart)//需要双手启动
            {
                while (!GlobalVar.c_Modbus.Coils.StartLeft.Value || !GlobalVar.c_Modbus.Coils.StartRight.Value)
                {
                    FormWait(100);
                }
            }
            if (!string.IsNullOrEmpty(WaitMsg))
            {
                if (GlobalVar.c_Modbus.HoldingRegisters.Prepare.Value != Value)
                {
                    AddLogStr("等待" + WaitMsg, false, Color.LightGreen);
                    FormWait(100);
                }
            }
            while (GlobalVar.c_Modbus.HoldingRegisters.Prepare.Value != Value)
            {
                FormWait(100);
            }
            AddLogStr(WaitMsg, false, Color.LightGreen);
        }

        /// <summary>
        /// 从U盘读取内容
        /// </summary>
        private void ReadUDisk()
        {
            try
            {
                myFunction myfunction = new myFunction();
                string[] files = new string[] { };
                string err = string.Empty;//异常信息                
                string TxtFile = string.Empty;//确定的文件
                if (!myfunction.Read_UDisk(ref files, ref err))
                {
                    AddLogStr(err, true, Color.Red);
                    FilesSelect SelectForm = new FilesSelect();
                    SelectForm.TopLevel = false;
                    SelectForm.Parent = this;
                    SelectForm.MdiParent = this.MdiParent;
                    if (SelectForm.ShowDialog() != DialogResult.OK) throw new Exception("移动存储盘读取文件失败,取消读取");
                    TxtFile = SelectForm.SelectedFile;
                }
                else if (files.Length > 1)
                {
                    FilesSelect SelectForm = new FilesSelect(files);
                    SelectForm.TopLevel = false;
                    SelectForm.Parent = this;
                    SelectForm.MdiParent = this.MdiParent;
                    if (SelectForm.ShowDialog() != DialogResult.OK) throw new Exception("移动存储盘读取文件失败,取消读取");
                    TxtFile = SelectForm.SelectedFile;
                }
                else if (files.Length == 1) TxtFile = files[0];

                Console.WriteLine("选择的文件:" + TxtFile);

                File.Copy(TxtFile,
                    Application.StartupPath + string.Format(@"\{0}\{1}\", GlobalVar.Folder_Config, GlobalVar.Folder_Product) + Path.GetFileName(TxtFile), true);//备份品目文件 复制文件至Product目录下，存在则替换
                File.Copy(TxtFile,
                   Application.StartupPath + string.Format(@"\{0}\{1}", GlobalVar.Folder_Config, GlobalVar.File_LOAD), true);//复制文件至load.ini

                UpdateTestPanel();
            }
            catch (Exception ex)
            {
                throw new Exception("读取文件异常 " + ex.Message);
            }
        }

        private const int MoveUpper = 12;//调整模式，移动的次数的上限
        private int ContinuedAdjustPASS = 0;//调整模式时，持续Pass的数量，连续测试PASS大于5次则停止
        /// <summary>
        /// 调整模式【返回调整结果-成功/失败】
        /// </summary>
        private bool AdjustmentMode()
        {
            try
            {
                double MoveDistance = 0.02;
                int OriginPosition = INIFileValue.Product_MAC_XY == 0 ? GlobalVar.c_Modbus.HoldingRegisters.AxisY_RealLocation.Value : GlobalVar.c_Modbus.HoldingRegisters.AxisX_RealLocation.Value;
                int Center = 0;//等待计算的中心位置
                Center = AdjustmentMode_Search(MoveDistance, OriginPosition);

                if (!GlobalVar.CCDEnable)
                    AddLogStr("中心位置=" + (double)Center / GlobalVar.ConverRate, false, Color.PowderBlue);
                else
                {
                    AddLogStr(string.Format("以中心位置:{0}\t进一步搜索", (double)Center / GlobalVar.ConverRate), false, Color.DodgerBlue);
                    MoveDistance = 0.01;
                    Center = AdjustmentMode_Search(MoveDistance, Center);
                    AddLogStr("中心位置=" + (double)Center / GlobalVar.ConverRate, false, Color.PowderBlue);
                }

                if (Math.Abs((double)GlobalVar.c_Modbus.HoldingRegisters.AxisY_PrepareLocation.Value / GlobalVar.ConverRate) - Math.Abs((double)Center / GlobalVar.ConverRate) > INIFileValue.Product_AL_FINE * MoveUpper)
                    throw new Exception(string.Format("程序计算错误，请联系开发人员\r\n进入位置:{0}\t中心位置:{1}\r\n差距上限:{2}",
                                                        (double)GlobalVar.c_Modbus.HoldingRegisters.AxisY_PrepareLocation.Value / GlobalVar.ConverRate,
                                                        (double)Center / GlobalVar.ConverRate,
                                                        INIFileValue.Product_AL_FINE * MoveUpper));

                if (INIFileValue.Product_MAC_XY == 0)
                {
                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.AxisY_JigPoint, Center);
                    AddLogStr("Y轴治具位置修改");
                }
                else
                {
                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.AxisX_AssemblyLocation, Center);
                    AddLogStr("X轴合模位置修改");
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrPrepare = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 搜索【调整模式】
        /// </summary>
        /// <param name="MoveDistance">移动距离</param>
        /// <param name="OriginPosition">搜索的原点位置</param>
        /// <returns></returns>
        private int AdjustmentMode_Search(double MoveDistance, int OriginPosition)
        {
            ContinuedAdjustPASS = 0;//置零
            string ErrStr = string.Empty;
            ValueConfirm Num1 = new ValueConfirm();
            ValueConfirm Num2 = new ValueConfirm();
            //①间隔0.02(和0.01)mm负方向搜索
            MoveDistance *= -1;
            AddLogStr(string.Format("{0} 负方向搜索中,间距{1}mm", INIFileValue.Product_MAC_XY == 0 ? "Y轴" : "X轴", MoveDistance), false, Color.PowderBlue);
            //第一次调整结果
            if (!AdjustmentMode_Test(MoveDistance, OriginPosition, ref Num1, ref Num2, ref ErrStr)) AddLogStr(ErrStr, true, Color.Red);

            if (Num1.IsConfirm && Num1.IsOrigin)
            {
                Num1.Value = Num2.Value;//如果起始位置是原点，则将结束位置交给起始位置，然后重新寻找结束位置
                Num2.IsConfirm = false;
            }

            if (!Num2.IsConfirm)//结束位置未确认
            {
                //②间隔0.02(和0.01)正方向搜索
                MoveDistance = INIFileValue.Product_AL_FINE;
                AddLogStr(string.Format("{0}正方向搜索中,间距{1}mm", INIFileValue.Product_MAC_XY == 0 ? "Y轴" : "X轴", MoveDistance), false, Color.PowderBlue);
                log.AddCommLOG(string.Format("Num1:{0}{1}\r\nNum2:{2}{3}", Num1.IsConfirm, Num1.Value, Num2.IsConfirm, Num2.Value));
                if (!AdjustmentMode_Test(MoveDistance, OriginPosition + (int)(MoveDistance * GlobalVar.ConverRate), ref Num1, ref Num2, ref ErrStr))
                {
                    log.AddERRORLOG("搜索异常:" + ErrStr);
                    throw new Exception("尽管搜索了，但是测试没通过");
                }
            }

            //③计算①②中心位置
            log.AddCommLOG(string.Format("Num1:{0}\tNum2:{1}", Num1.Value, Num2.Value));
            int Center = (Num1.Value + Num2.Value) / 2;
            return Center;
        }

        /// <summary>
        /// 调整模式测试【返回值 调整是否正常,终点确认即认为正常 】
        /// </summary>
        /// <param name="MoveDistance">移动距离</param>
        /// <param name="DatumPosition">基准位置【以保持寄存器的值为准，实际值的一千倍】</param>
        /// <param name="PassStartPosition">PASS起始位置</param>
        /// <param name="PassEndPosition">PASS结束位置</param>
        /// <param name="ErrStr">异常信息</param>
        private bool AdjustmentMode_Test(double MoveDistance, int DatumPosition, ref ValueConfirm PassStartPosition, ref ValueConfirm PassEndPosition, ref string ErrStr)
        {
            ErrStr = string.Empty;
            int reallocation = INIFileValue.Product_MAC_XY == 0 ? GlobalVar.c_Modbus.HoldingRegisters.AxisY_RealLocation.Value : GlobalVar.c_Modbus.HoldingRegisters.AxisX_RealLocation.Value;
            double x, y;//x和y的坐标，用于定位
            x = (double)GlobalVar.c_Modbus.HoldingRegisters.AxisX_RealLocation.Value / GlobalVar.ConverRate;
            y = (double)GlobalVar.c_Modbus.HoldingRegisters.AxisY_RealLocation.Value / GlobalVar.ConverRate;
            if (DatumPosition != reallocation)//基准位置非当前位置，则移动到基准位置
            {
                if (INIFileValue.Product_MAC_XY == 0) y = (double)DatumPosition / GlobalVar.ConverRate;
                else x = (double)DatumPosition / GlobalVar.ConverRate;

                GlobalVar.c_Modbus.XYAxisMove(x, y);//一直等，直到可以写入坐标

                while (GlobalVar.c_Modbus.Coils.PLCStatus.Value) Sleep(50);//等待移动到位
            }

            int MoveCount = 0;//当前移动的次数
            for (; MoveCount < MoveUpper;)
            {
                while (!GlobalVar.c_Modbus.Coils.StartTest.Value) Sleep(50);//等待启动测试信号

                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.StartTest, false);//收到启动信号，置0启动信号
                AddLogStr("调整模式-开始测试", false, Color.LightSkyBlue);
                UpdateTestStationResult(null);//清空界面的测试结果
                GlobalVar.m_ECTest.ClearAllTestResult();//清除之前的测试结果
                EC_TestResult TR = GlobalVar.m_ECTest.StartTest();
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.TestResult, 1);//测试完成

                if (TR != EC_TestResult.ERROR) UpdateTestStationResult(GlobalVar.m_ECTest.GetTestResult(), 1);//更新界面上的测试工位
                else throw new Exception("主机测试异常，停止调整！");

                if (TR == EC_TestResult.OK)
                {
                    if (++ContinuedAdjustPASS >= 5)
                    {
                        PassEndPosition.Value = INIFileValue.Product_MAC_XY == 0 ? GlobalVar.c_Modbus.HoldingRegisters.AxisY_RealLocation.Value : GlobalVar.c_Modbus.HoldingRegisters.AxisX_RealLocation.Value;//结束位置
                        PassEndPosition.IsConfirm = true;
                        return true;
                    }
                }
                else ContinuedAdjustPASS = 0;

                if (TR == EC_TestResult.OK && !PassStartPosition.IsConfirm)
                {
                    PassStartPosition.Value = INIFileValue.Product_MAC_XY == 0 ? GlobalVar.c_Modbus.HoldingRegisters.AxisY_RealLocation.Value : GlobalVar.c_Modbus.HoldingRegisters.AxisX_RealLocation.Value;//起始位置
                    PassStartPosition.IsConfirm = true;

                    if (MoveCount == 0)
                    {
                        PassStartPosition.IsOrigin = true;//是否为原点
                        log.AddCommLOG("原点位置测试OK");
                    }
                }
                if (PassStartPosition.IsConfirm && TR == EC_TestResult.FAIL)
                {
                    PassEndPosition.Value = INIFileValue.Product_MAC_XY == 0 ? GlobalVar.c_Modbus.HoldingRegisters.AxisY_RealLocation.Value : GlobalVar.c_Modbus.HoldingRegisters.AxisX_RealLocation.Value;//结束位置
                    PassEndPosition.IsConfirm = true;
                    return true;
                }

                if (++MoveCount >= MoveUpper)//达到上限，不再移动轴
                {
                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.TestResult, 1);//测试完成
                    ErrStr = string.Format("移动次数超过上限 {0}次", MoveUpper);

                    log.AddCommLOG(string.Format("PassStartPosition IsConfirm:{0}\tValue:{1}\tIsOrigin{2}", PassStartPosition.IsConfirm, PassStartPosition.Value, PassStartPosition.IsOrigin));
                    if (PassStartPosition.IsConfirm)
                    {
                        PassEndPosition.Value = INIFileValue.Product_MAC_XY == 0 ? GlobalVar.c_Modbus.HoldingRegisters.AxisY_RealLocation.Value : GlobalVar.c_Modbus.HoldingRegisters.AxisX_RealLocation.Value;//结束位置
                        PassEndPosition.IsConfirm = true;
                        return true;
                    }

                    return false;
                }
                else
                {
                    int RealLocation = INIFileValue.Product_MAC_XY == 0 ? GlobalVar.c_Modbus.HoldingRegisters.AxisY_RealLocation.Value : GlobalVar.c_Modbus.HoldingRegisters.AxisX_RealLocation.Value;//当前位置
                    int Target = RealLocation + Convert.ToInt32(MoveDistance * GlobalVar.ConverRate);//移动的距离扩大一千倍
                    log.AddCommLOG(string.Format("当前位置:{0}\t目标位置:{1}", RealLocation, Target));
                    if (INIFileValue.Product_MAC_XY == 0) y = (double)Target / GlobalVar.ConverRate;
                    else x = (double)Target / GlobalVar.ConverRate;

                    GlobalVar.c_Modbus.XYAxisMove(x, y);
                }
            }
            ErrStr = "此行日志不应该出现";
            return false;//只是的返回，无其他
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="MillSecond">毫秒</param>
        /// <param name="IsResetPLC">是否复位</param> 
        private void Sleep(int MillSecond, bool IsResetPLC = false)
        {
            Thread.Sleep(MillSecond);
            if (!IsResetPLC && GlobalVar.SystemReset) throw new ResetPLC("系统复位");
            if (GlobalVar.SoftWareShutDown) throw new Exception("程序关闭");
        }

        /// <summary>
        /// 通知PLC复位
        /// </summary>
        /// <param name="Press">是否为点击屏幕“返回原点” 通知复位</param>
        private void ResetPLC(bool Press)
        {
            try
            {
                if (Press && !GlobalVar.c_Modbus.Coils.StartLeft.Value) return;

                GlobalVar.SystemReset = true;
                if (!Press) GlobalVar.c_Modbus.CoilMsgSync(GlobalVar.c_Modbus.Coils.Mode, false);//不是点击的返回原点，切换为手动状态
                UpdateTestStationResult(null);//清空界面的测试结果
                GlobalVar.c_Modbus.CoilMsgSync(GlobalVar.c_Modbus.Coils.ResetUp, true);
                DateTime WaitUp = DateTime.Now;
                if (GlobalVar.c_Modbus.Coils.ResetUp.Value)//通知下压汽缸抬起
                {
                    if ((DateTime.Now - WaitUp).TotalMilliseconds > 1000)
                    {
                        AddLogStr("下压汽缸抬起超时，无法复位！", true, Color.Red);
                        return;
                    }
                    FormWait(100, true);
                }
                if (GlobalVar.c_Modbus.Coils.LsolationPin.Value)//判断隔离销是否在位置上
                {
                    MsgBoxShow("请确认隔离销没有卡在上治具？", "提示", Color.LightPink, MessageBoxButtons.OK);
                }

                if (MsgBoxShow("是否系统复位？", "提示", Color.LimeGreen, MessageBoxButtons.OK) == DialogResult.OK)
                {
                    GlobalVar.c_Modbus.CoilMsgSync(GlobalVar.c_Modbus.Coils.SystemReset, true);
                    AddLogStr("系统复位！", false, Color.LightGreen);
                    GlobalVar.m_ECTest.Reset();//测试主机复位
                    Thread.Sleep(500);
                    DateTime beg = DateTime.Now;
                    while (GlobalVar.c_Modbus.Coils.SystemReset.Value)
                    {
                        FormWait(100, true);
                        if ((DateTime.Now - beg).TotalSeconds > 25)
                        {
                            AddLogStr("系统复位超时", true, Color.Red);
                            GlobalVar.c_Modbus.CoilMsgSync(GlobalVar.c_Modbus.Coils.SystemReset, false);//复位超时后，关闭此信号
                            return;
                        }
                    }
                    AddLogStr("系统复位完成");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Punch = 0;
                TotalPunchTestResult.Clear();//复位，清空所有的测试结果
                if (!string.IsNullOrEmpty(this.BarcodeGunForm.Barcode)) this.BarcodeGunForm.ClearAll();//复位，清空扫描的条码

                Thread thd = new Thread(new ThreadStart(delegate
                {
                    Thread.Sleep(500);//延时500毫秒，关闭复位的信号
                    GlobalVar.SystemReset = false;
                }));
                thd.IsBackground = true;
                thd.Start();

                if (!Press &&
                        GlobalVar.RunningMode == RunMode.Normal) //只有在主界面，才切回自动状态
                    GlobalVar.c_Modbus.CoilMsgSync(GlobalVar.c_Modbus.Coils.Mode, true);//不是点击的返回原点，切换为自动状态
            }
        }

        private delegate void dele_ChangeStatus(BtnStatus s);
        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="s"></param>
        private void ChangeStatus(BtnStatus s)
        {
            string str = string.Empty;
            switch (s)
            {
                case BtnStatus.Reset:
                    str = "返回原点";
                    this.btn_Manual.Enabled = true;
                    this.btn_Manual.BackColor = this.btn_ManualBackColor;
                    this.btn_Prepare.Enabled = true;
                    this.btn_Prepare.BackColor = this.btn_PrepareBackColor;
                    this.btn_Reset.BackColor = this.btn_ResetColor;
                    break;
                case BtnStatus.Stop:
                    str = "停止";
                    this.btn_Manual.Enabled = false;
                    this.btn_Manual.BackColor = Color.Gray;
                    this.btn_Prepare.Enabled = false;
                    this.btn_Prepare.BackColor = Color.Gray;
                    this.btn_Reset.BackColor = Color.Red;
                    break;
                default:
                    return;
            }
            CurBtnStatus = s;
            this.btn_Reset.Text = str;
        }

        /// <summary>
        /// 改变Picturebox背景色
        /// </summary>
        /// <param name="color"></param>
        private void ChangeShowBackColor(Color color)
        {
            if (this.pictureBox_Display.InvokeRequired)
            {
                this.pictureBox_Display.BeginInvoke(new Action(delegate { ChangeShowBackColor(color); }));
            }
            else
            {
                this.pictureBox_Display.BackColor = color;
                this.splitContainer_Inner.Panel1.BackColor = color;
            }
        }

        bool Idle = true;//主机是否空闲
        int Punch = 0;//当前第几Punch测试
        /// <summary>
        /// 测试流程
        /// </summary>
        private void TestProcedure()
        {
            dele_ChangeStatus changstr = new dele_ChangeStatus(ChangeStatus);
            while (!GlobalVar.SoftWareShutDown)
            {
                StartTest.Reset();//重置多余的启动信号
                if (StartTest.WaitOne())
                {
                    try
                    {
                        GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.PunchTest, false);//置默认值，非多Punch测试
                        Label_Show(string.Format("开始测试 X:{0}  Y:{1}",
                            (double)GlobalVar.c_Modbus.HoldingRegisters.AxisX_RealLocation.Value / GlobalVar.ConverRate,
                            (double)GlobalVar.c_Modbus.HoldingRegisters.AxisY_RealLocation.Value / GlobalVar.ConverRate));
                        AddLogStr(string.Format(" X:{0}  Y:{1}",
                            (double)GlobalVar.c_Modbus.HoldingRegisters.AxisX_RealLocation.Value / GlobalVar.ConverRate,
                            (double)GlobalVar.c_Modbus.HoldingRegisters.AxisY_RealLocation.Value / GlobalVar.ConverRate), false, Color.LightSteelBlue);
                        Idle = false;
                        if (GlobalVar.SystemReset) GlobalVar.SystemReset = false;

                        if (GlobalVar.m_ECTest.InTest) throw new Exception("测试，不启动测试"); ;//测试中，不启动测试
                        this.Invoke(changstr, BtnStatus.Stop);//显示“停止”
                        GlobalVar.m_ECTest.ClearAllTestResult();//清除上一轮的测试结果
                        switch (MovePattern)
                        {
                            case MovementPattern.Normal:
                                ++Punch;
                                int PcsNo = 0;//不良位置[2018/03/03 lqz]
                                if (INIFileValue.Product_OKURI > 1) AddLogStr(string.Format("第【{0}】Punch测试", Punch));
                                EC_TestResult tr = Test(Punch);//测试
                                TotalPunchTestResult.Add(GlobalVar.m_ECTest.GetTestResult());//每一Punch加入测试结果
                                if (INIFileValue.Product_OKURI > 1 &&
                                    Punch < INIFileValue.Product_OKURI)
                                {
                                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.PunchTest, true);//分为多Punch测试
                                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.TestResult, 1);//测试完成，移动位置
                                    GlobalVar.c_Modbus.XYAxisMove(MarkX,
                                                                  (double)GlobalVar.c_Modbus.HoldingRegisters.AxisY_ReversePlacePoint.Value / GlobalVar.ConverRate - Punch * INIFileValue.Product_PITCH);
                                    continue;
                                }

                                INIFileValue.ProductTestNum++;                                //制品张数总数自增            
                                Punch = 0;

                                bool TotalResult = true;//本张总结果
                                foreach (EC_TestResultItem[] item in TotalPunchTestResult)
                                {
                                    foreach (EC_TestResultItem testitem in item)
                                    {
                                        if (testitem.isOnceOK || testitem.Item == EC_NGItem.PASS) continue;//曾经有过OK，或者最后一次OK
                                        TotalResult = false;
                                    }
                                }

                                if (MovePattern == MovementPattern.Normal //通常模式
                                    && INIFileValue.BarcodeGunEnable      //启用条码枪(忽略条码是否扫描成功)
                                    && TotalPunchTestResult.Count > 0)      //有测试结果
                                {
                                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.BarocodeReady, false);
                                    myfunction.WriteCSV(INIFileValue.Product_NAME, "Unknown", LotNum, this.BarcodeGunForm.Barcode, TotalPunchTestResult);
                                    this.BarcodeGunForm.ClearAll();
                                }
                                if (!TotalResult)
                                {
                                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.UpperJig, false);//确保处于未按下状态
                                    ChangeShowBackColor(Color.FromArgb(186, 193, 69));
                                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.TestResult, 3);
                                    Mark();//开始打标

                                    if (INIFileValue.MarkTotalCount - INIFileValue.MarkCount <= 0) ErrMsgBox(string.Format("打标器已经使用{0}次，需要更换", INIFileValue.MarkCount), "更换打标器");
                                }
                                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.TestResult, 2);
                                if (!TotalResult)
                                {
                                    //CoilMsgBox("确认不良位置，按复位键!", GlobalVar.c_Modbus.Coils.UpperJig, true, "需要复位", MessageBoxButtons.OK);
                                    Label_Show("确认不良位置，按复位键!");
                                    while (!GlobalVar.c_Modbus.Coils.UpperJig.Value) FormWait(100);//等待按下
                                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.UpperJig, false);//收到按下，改变为未按下状态
                                }
                                if (GlobalVar.gl_UpSql)
                                {
                                    #region 将结果写入本地 [2018/03/03 lqz]
                                    if (INIFileValue.Product_OKURI > 1)
                                    {
                                        int test_result = 0;
                                        // log.AddCommLOG("PUNCH:"+TotalPunchTestResult.Count.ToString());
                                        foreach (EC_TestResultItem[] item in TotalPunchTestResult)
                                        {
                                            foreach (EC_TestResultItem testitem in item)
                                            {
                                                PcsNo++;
                                                if (testitem.isOnceOK || testitem.Item == EC_NGItem.PASS)
                                                    test_result = 0;//曾经有过OK，或者最后一次OK
                                                else
                                                    test_result = 1;
                                                string pcsIndex = GlobalVar.gl_mapping[PcsNo].ToString();
                                                string testResult = GlobalVar.gl_Barcode + "," + pcsIndex + "," + test_result.ToString() + ","+INIFileValue.FlowID+",TOEC," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                                                //log.AddCommLOG(PcsNo.ToString()+"pcs测试结果:"+testResult);
                                                string filename = GlobalVar.gl_str_product + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                                                if (GlobalVar.gl_UpSql && INIFileValue.BarcodeScanEnable)
                                                {
                                                    writeResultTolocal(filename, testResult);
                                                }
                                            }
                                        }
                                        PcsNo = 0;
                                        log.AddCommLOG("测试结果写入本地,条码" + GlobalVar.gl_Barcode);
                                    }
                                    else
                                    {
                                        myfunction.WriteCSV(INIFileValue.Product_NAME, "Unknown", LotNum, GlobalVar.gl_Barcode, TotalPunchTestResult);
                                        int test_result = 0;
                                        foreach (EC_TestResultItem testitem in GlobalVar.m_ECTest.GetTestResult())
                                        {
                                            PcsNo++;
                                            if (testitem.isOnceOK || testitem.Item == EC_NGItem.PASS)
                                                test_result = 0;//曾经有过OK，或者最后一次OK
                                            else
                                                test_result = 1;
                                            string pcsIndex = GlobalVar.gl_mapping[PcsNo].ToString();
                                            
                                            string testResult = GlobalVar.gl_Barcode + "," + pcsIndex + "," + test_result.ToString() + ","+ INIFileValue.FlowID + ","+ INIFileValue.MType + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                                            // log.AddCommLOG(PcsNo.ToString() + "pcs测试结果:" + testResult);
                                            string filename = GlobalVar.gl_str_product + "-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                                            if (GlobalVar.gl_UpSql && INIFileValue.BarcodeScanEnable)
                                            {
                                                writeResultTolocal(filename, testResult);
                                            }
                                        }
                                        PcsNo = 0;
                                        log.AddCommLOG("测试结果写入本地,条码" + GlobalVar.gl_Barcode);
                                    }
                                    //WriteSimpleResult(GlobalVar.gl_Barcode,TotalResult.ToString(),DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                                    #endregion
                                }
                                break;
                            case MovementPattern.SinglePress:
                            case MovementPattern.ProductSample:
                                UpdateTestStationResult(null);//清空界面的测试结果
                                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.StartTest, false);//收到启动信号，置0启动信号
                                if (GlobalVar.m_ECTest.StartTest() != EC_TestResult.ERROR)
                                {
                                    EC_TestResultItem[] Result = GlobalVar.m_ECTest.GetTestResult();
                                    UpdateTestStationResult(Result, 1);//更新界面上的测试工位，多Punch的认为测试第一Punch
                                }
                                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.TestResult, 4);
                                break;
                            case MovementPattern.Adjustment:
                                AddLogStr(string.Format("开始调整模式"), false, Color.LightSteelBlue);
                                ErrPrepare = string.Empty;
                                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.AdjustComplete, false);//调整模式中
                                bool result = AdjustmentMode();//执行调整模式 
                                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.AdjustResult, result);//通知PLC调整的结果
                                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.StartTest, false);//再次 置0启动信号 避免启动信号未关闭
                                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.AdjustComplete, true);//通知PLC调整模式结束
                                WaitAdjustEnd = true;//调整模式结束

                                if (!string.IsNullOrEmpty(ErrPrepare))
                                {
                                    while (!GlobalVar.c_Modbus.Coils.AdjustComplete.Value) FormWait(10);//等待调整模式结束消息发送完成
                                    ErrMsgBox(ErrPrepare, "调整异常");
                                    if (GlobalVar.CCDEnable)
                                    {
                                        AddLogStr("同时按下左右开始键，退出下治具", false, Color.SeaGreen);
                                        while (!GlobalVar.c_Modbus.Coils.StartLeft.Value || !GlobalVar.c_Modbus.Coils.StartRight.Value)
                                        {
                                            Sleep(100);//等待按下左右启动键
                                            if (GlobalVar.SystemReset) throw new Exception("系统复位，不再等待按下左右开始键退出下治具");
                                        }
                                    }
                                }

                                MovePattern = MovementPattern.Normal;
                                ShowMovePattern();//还原为当前的模式
                                continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.AddERRORLOG("测试异常:"+ex.Message+"\r\n"+ex.StackTrace);
                        if (!GlobalVar.SystemReset)
                            AddLogStr(string.Format("测试异常:{0}\r\n请手动复位", ex.Message), true, Color.Red);
                    }
                    finally
                    {
                        if (MovePattern != MovementPattern.Adjustment)//非调整模式，更新测试数据
                        {
                            UpdateDate(GlobalVar.m_ECTest.GetTestResult());//更新测试结果的数据，界面显示统计信息
                            UpdateSheet();
                        }
                        ChangeShowBackColor(Color.Black);
                        if (!GlobalVar.SoftWareShutDown) //程序未关闭时才显示，避免出错
                            this.Invoke(changstr, BtnStatus.Reset);//显示“返回原点”
                        GlobalVar.SystemReset = false;
                        Idle = true;

                        if (MovePattern != MovementPattern.Adjustment)
                        {
                            TotalPunchTestResult.Clear();//清空测试结果
                            GlobalVar.m_ECTest.ClearAllTestResult();//清除上一轮的测试结果  
                            GlobalVar.gl_Barcode = "";//清空条码
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  将测试结果写入到本地文件[2018/03/03 lqz]
        /// </summary>
        private void writeResultTolocal(string FileName, string testResult)
        {
            try
            {
                FileName = string.IsNullOrEmpty(FileName) ? DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") : FileName;
                string exten = ".Txt";
                if (!Directory.Exists(Application.StartupPath + "\\" + GlobalVar.gl_TestResultPath)) Directory.CreateDirectory(Application.StartupPath + "\\" + GlobalVar.gl_TestResultPath);
                string Old_FileName = Application.StartupPath + "\\" + GlobalVar.gl_TestResultPath + @"\" + FileName;
                string NewFileName = Old_FileName + exten;
                if (File.Exists(Old_FileName + exten))
                {
                    int i = 0;
                    while (File.Exists(Old_FileName + "." + i.ToString("0000") + exten))
                    {
                        i++;
                    }
                    NewFileName = Old_FileName + "." + i.ToString("0000") + exten;
                }
                if (testResult != "")
                {
                    using (FileStream fs = new FileStream(NewFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        StreamWriter sw = new StreamWriter(fs);
                        sw.WriteLine(testResult);
                        sw.Close();
                        sw.Close();
                    }
                }
                // log.AddCommLOG("保存测试结果文件成功:" + NewFileName);
            }
            catch (Exception ex)
            {
                string err = "保存测试结果文件异常：" + ex.Message + "\r\n" + ex.StackTrace;
                log.AddERRORLOG("保存测试结果文件异常：" + err);
            }
        }

        /// <summary>
        /// 测试 【返回本轮测试的结果】
        /// </summary>
        /// <param name="PunchIndex">第几Punch【序号从零开始】</param>
        /// <returns></returns>
        private EC_TestResult Test(int PunchIndex)
        {
            EC_TestResult ALLOK = EC_TestResult.OK;//是否全部Pass
            int Pos = 0;//正向
            int Neg = 0;//反向

            /*******************测试NG,重复测试******************/
            for (int i = 0; i < INIFileValue.Product_AL_NUM; i++)
            {
                if (INIFileValue.Product_AL_MODE == 0)
                {
                    log.AddCommLOG("同一个位置重复测试，认为收到启动信号,暂停1秒，用于显示上次的测试结果");
                    Sleep(1000);//同一个位置重复测试，认为收到启动信号,暂停1秒，用于显示上次的测试结果
                }
                else
                {
                    log.AddCommLOG("等待启动测试的信号");
                    do
                    {
                        Sleep(50);
                    }
                    while (!GlobalVar.c_Modbus.Coils.StartTest.Value);//等待启动测试信号
                    log.AddCommLOG("收到启动测试的信号");
                }
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.StartTest, false);//收到启动信号，置0启动信号

                if (GlobalVar.SystemReset) throw new Exception("系统复位");

                UpdateTestStationResult(null, PunchIndex);//清空界面的测试结果
                if (GlobalVar.m_ECTest.StartTest() == EC_TestResult.ERROR)
                {
                    AddLogStr(string.Format("测试异常"),
                     true, Color.DarkRed);
                    throw new Exception("测试异常不更新数据"); ;//测试异常不更新数据
                }

                EC_TestResultItem[] Result = GlobalVar.m_ECTest.GetTestResult();

                ALLOK = EC_TestResult.OK;//初始化为全部OK
                foreach (EC_TestResultItem item in Result)
                {
                    if (item.Item != EC_NGItem.PASS)
                    {
                        ALLOK = EC_TestResult.FAIL;
                        break;
                    }
                }

                UpdateTestStationResult(Result, PunchIndex);//更新界面上的测试工位

                if (ALLOK == EC_TestResult.OK) break;
                else if (ALLOK == EC_TestResult.FAIL)
                {
                    if (INIFileValue.Product_AL_MODE == 0) continue;//同一个位置重复测试
                    else
                    {
                        if (i + 1 >= INIFileValue.Product_AL_NUM) break;
                        GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.HoldingRegisters.TestResult, 1);

                        double TargetX = (double)GlobalVar.c_Modbus.HoldingRegisters.AxisX_RealLocation.Value / GlobalVar.ConverRate;
                        double TargetY = (double)GlobalVar.c_Modbus.HoldingRegisters.AxisY_JigPoint.Value / GlobalVar.ConverRate;
                        if (INIFileValue.Product_MAC_XY == 0)
                        {
                            // TargetY = MarkY;
                            TargetY += (((i % 2) == 0) ? (++Pos) * INIFileValue.Product_AL_FINE ://偶数向前移动
                                                                  (--Neg) * INIFileValue.Product_AL_FINE);//奇数向后移动
                        }
                        else
                        {
                            // TargetX = MarkX;
                            TargetX += (((i % 2) == 0) ? (++Pos) * INIFileValue.Product_AL_FINE ://偶数向前移动
                                                                 (--Neg) * INIFileValue.Product_AL_FINE);//奇数向后移动
                        }

                        //通知PLC移动到指定位置
                        //GlobalVar.c_Modbus.YAxisMove(Target, true);//只移动Y轴，已经放弃
                        GlobalVar.c_Modbus.XYAxisMove(TargetX, TargetY);
                        while (GlobalVar.c_Modbus.Coils.PLCStatus.Value) FormWait(100);
                        AddLogStr("轴移动完成！");
                    }
                }
            }
            return ALLOK;
        }

        /// <summary>
        /// 开始打标
        /// </summary>
        private void Mark()
        {
            GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.MarkEnd, false);//打标进行中

            int PunchIndex = 0;//第几Punch，正方向的
            foreach (EC_TestResultItem[] item in TotalPunchTestResult)
            {
                PunchMark(item,//每一Punch的测试结果
                          PunchIndex++,//每一Punch打点
                          PunchIndex % 2 == 0);//偶数行，打点位置反转，走蛇形路线，便于快速打点
            }
            TotalPunchTestResult.Clear();//打完点，清空所有的测试结果

            GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.MarkEnd, true);//打标完成
            //GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.XWaitMarkLocation, true);//打标器归位
            while (GlobalVar.c_Modbus.Coils.PLCStatus.Value ||                                                                              //下位机忙
                Math.Abs(GlobalVar.c_Modbus.HoldingRegisters.AxisDot_RealLocation.Value - GlobalVar.c_Modbus.HoldingRegisters.AxisDot_WaitMarkPoint.Value) > 1)   //轴移动到打标等待位置
            {
                Sleep(100);
            }
            //GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.Cylinder_Dot, true);//放下打标器
        }

        /// <summary>
        /// Punch打点
        /// </summary>
        /// <param name="PunchTestResult">当前Punch的测试结果</param>
        /// <param name="PunchIndex">当前Punch的序号【从零开始】</param>
        /// <param name="ReverseDotIndex">打点序号反转</param>
        private void PunchMark(EC_TestResultItem[] PunchTestResult, int PunchIndex, bool ReverseDotIndex)
        {
            //初始定义，当前Punch的Y轴坐标
            double PunchY = (double)GlobalVar.c_Modbus.HoldingRegisters.AxisY_MarkPoint.Value / GlobalVar.ConverRate -
                                    (INIFileValue.Product_OKURI - 1) * INIFileValue.Product_PITCH;
            PunchY += PunchIndex * INIFileValue.Product_PITCH;//当前Punch在打点笔下的Y轴坐标
            List<int> markorder = new List<int>();//当前Punch打点顺序
            for (int i = 0; i < PunchTestResult.Length; i++)
            {
                if (PunchTestResult[i].Item == EC_NGItem.PASS) continue;
                else markorder.Add(i);
            }
            if (ReverseDotIndex) markorder.Reverse();//打点序号反转
            foreach (int item in markorder)
            {
                BlockMark(PunchIndex, item, PunchY);//按照区块来打点，一个区块对应一个或者多PCS打点
            }
        }

        /// <summary>
        /// 按照区块来打点
        /// </summary>
        /// <param name="PunchIndex">第几Punch【从零开始】</param>
        /// <param name="BlockIndex">第几个区块</param>
        /// <param name="PunchY">当前Punch的Y轴坐标</param>
        private void BlockMark(int PunchIndex, int BlockIndex, double PunchY)
        {
            Point[] markpoint = INIFileValue.MarkPoint[BlockIndex];
            for (int i = 0; i < markpoint.Length; i++)
            {
                DateTime beg = DateTime.Now;
           Write:
                GlobalVar.c_Modbus.CoilMsgSync(GlobalVar.c_Modbus.Coils.DotComplete, GlobalVar.c_Modbus.Coils.DotComplete.Value = false);//清除单次打点完成信号
                if (GlobalVar.c_Modbus.Coils.DotComplete.Value) goto Write;
                Console.WriteLine("单次打点完成修改用时:{0}", (DateTime.Now - beg).TotalMilliseconds);

                double X = markpoint[i].X / 1000;//微米转换成毫米
                double Y = markpoint[i].Y / 1000;//微米转换成毫米
                //Y *= -1;//Y取反，因为轴是相对的

                //打点坐标，转换前
                double dot_x = (double)GlobalVar.c_Modbus.HoldingRegisters.AxisDot_MarkPoint.Value / GlobalVar.ConverRate + X;
                double dot_y = PunchY + Y;

                if (GlobalVar.Dot_Coordinate_Radian != 0)//计算偏移坐标[2018/03/08 lqz]
                {
                    double a, b;
                    CalcRectPositon(dot_x, dot_y, (double)GlobalVar.c_Modbus.HoldingRegisters.AxisDot_MarkPoint.Value / GlobalVar.ConverRate, PunchY,
                        GlobalVar.Dot_Coordinate_Radian, out a, out b);

                    AddLogStr("校正打点坐标后移动,弧度:" + GlobalVar.Dot_Coordinate_Radian.ToString("F3"), false, Color.CadetBlue);
                    GlobalVar.c_Modbus.DotYAxisMove(a, b);
                }
                else GlobalVar.c_Modbus.DotYAxisMove(dot_x, dot_y);

                do
                {
                    Sleep(1);
                }
                while (!GlobalVar.c_Modbus.Coils.DotComplete.Value);//等待单次打点完成

                string showstr = string.Empty;
                if (INIFileValue.Product_OKURI > 1) showstr = string.Format("第【{0}】 Punch--", PunchIndex + 1);
                if (INIFileValue.Product_STAMP_TON == 1) AddLogStr(string.Format("{0}第{1}PCS制品打点完成", showstr, BlockIndex + 1), false, Color.CadetBlue);
                else AddLogStr(string.Format("{0}第{1}区块  第{2}PCS制品打点完成", showstr, BlockIndex + 1, i + 1), false, Color.CadetBlue);
                INIFileValue.MarkCount++;
            }
        }

        /// <summary>
        /// 矩形 绕 矩形中心 旋转
        /// </summary>
        /// <param name="X1">已知矩形点X</param>
        /// <param name="Y1">已知矩形点Y</param>
        /// <param name="X_Center">已知矩形中心点X</param>
        /// <param name="Y_Center">已知矩形中心点Y</param>
        /// <param name="radian">旋转的弧度</param>
        /// <param name="X">圆上点X旋转后的值</param>
        /// <param name="Y">圆上点Y旋转后的值</param>
        private void CalcRectPositon(double X1, double Y1, double X_Center, double Y_Center, double radian, out double X, out double Y)
        {
            X = (X1 - X_Center) * Math.Cos(radian) - (Y1 - Y_Center) * Math.Sin(radian) + X_Center;
            Y = (X1 - X_Center) * Math.Sin(radian) + (Y1 - Y_Center) * Math.Cos(radian) + Y_Center;
        }

        /// <summary>
        /// 更新测试结果数据
        /// </summary>
        /// <param name="Result">测试结果</param>
        private void UpdateDate(EC_TestResultItem[] TestResult)
        {
            try
            {
                //.Show(TestResult.Length.ToString());
                EC_NGItem[] Result = new EC_NGItem[TestResult.Length];
                for (int i = 0; i < TestResult.Length; i++)
                {
                    Result[i] = TestResult[i].Item;
                }

                foreach (EC_NGItem res in Result)
                {
                    if (res == EC_NGItem.PASS) INIFileValue.ProductQualifiedNum++;   //合格数量自增
                    else INIFileValue.ProductUnQualifidNUm++;      //不合格数量自增

                    switch (res)
                    {
                        case EC_NGItem.OPEN:
                            INIFileValue.ProductOpen++;
                            break;
                        case EC_NGItem.SHORT:
                            INIFileValue.ProductShort++;
                            break;
                        case EC_NGItem.OFFSET_M:
                            INIFileValue.ProductOffsetM++;
                            break;
                        case EC_NGItem.OFFSET_N:
                            INIFileValue.ProductOffsetN++;
                            break;
                        case EC_NGItem.ForgetPaste:
                            INIFileValue.ProductForgetPaste++;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                AddLogStr("更新界面异常:" + ex.Message, true, Color.Red);
            }
        }

        /// <summary>
        /// 更新测试工位界面
        /// </summary>
        /// <param name="TestResult">测试结果</param>
        /// <param name="PunchIndex">第几Punch【序号从一开始，默认值是零，零不会对序号产生影响】</param>
        private void UpdateTestStationResult(EC_TestResultItem[] TestResult, int PunchIndex = 0)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => { UpdateTestStationResult(TestResult, PunchIndex); }));
                }
                else
                {
                    if (PunchIndex == 0 || PunchIndex == 1 &&  //非多Punch测试 或者 多Punch测试的第一Punch
                        (TestResult == null || TestResult.Length == 0))
                    {
                        //清空测试结果
                        this.TestStation.ForEach(X =>
                        {
                            //X.TextFont.Style = FontStyle.Bold;
                            X.ShowText = X.ShowText.Substring(0, X.ShowText.IndexOf("-") + 1);
                            X.PanelBackColor = Color.Black;
                        });
                        //log.AddCommLOG("还原界面所有小块");
                        return;
                    }
                    else if (TestResult == null) return;//空结果不修改界面

                    for (int i = 0; i < TestResult.Length; i++)
                    {
                        EC_TestResultItem result = TestResult[i];
                        string explain = string.Empty;
                        Color backcolor = Color.Black; ;
                        GetResultExplain(result.Item, ref explain, ref backcolor);
                        int index = PunchIndex * TestResult.Length - TestResult.Length + i;//序号
                        this.TestStation[index].ShowText = string.Format("{0}-{1}", (index + 1), explain);
                        this.TestStation[index].PanelBackColor = backcolor;
                        //至少有一次OK，则添加下划线
                        if (result.isOnceOK) this.TestStation[index].TextFont = new Font(this.TestStation[index].TextFont, FontStyle.Bold | FontStyle.Underline);
                        else this.TestStation[index].TextFont = new Font(this.TestStation[index].TextFont, FontStyle.Bold);
                        //log.AddCommLOG(string.Format("更新界面小块 {0}\t结果:{1}\t解释:{2}\t颜色:{3}", (i + 1), result.Item, explain, backcolor));
                    }
                }
            }
            catch (Exception ex)
            {
                log.AddERRORLOG("更新 显示测试结果 异常:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取  结果字符串  颜色
        /// </summary>
        private void GetResultExplain(EC_NGItem ResultType, ref string str, ref Color backcolor)
        {
            switch (ResultType)
            {
                case EC_NGItem.PASS:
                    str = "OK";
                    backcolor = Color.LimeGreen;
                    break;
                case EC_NGItem.OPEN:
                    str = "O";
                    backcolor = Color.Red;
                    break;
                case EC_NGItem.SHORT:
                    str = "S";
                    backcolor = Color.Orange;
                    break;
                case EC_NGItem.LEAK:
                    str = "L";
                    backcolor = Color.YellowGreen;
                    break;
                case EC_NGItem.OFFSET_M:
                    str = "M";
                    backcolor = Color.Peru;
                    break;
                case EC_NGItem.OFFSET_N:
                    str = "N";
                    backcolor = Color.Peru;
                    break;
                case EC_NGItem.ForgetPaste:
                    str = "B";
                    backcolor = Color.MediumOrchid;
                    break;
                case EC_NGItem.NG:
                    str = "";
                    backcolor = Color.BlueViolet;
                    break;
            }
        }

        bool tt = false;
        private EC_TestResultItem[] punchTestResult;

        private void btn_Test_Click(object sender, EventArgs e)
        {
            Thread thd = new Thread(new ThreadStart(delegate
            {
                while (!GlobalVar.SoftWareShutDown)
                {
                    string msg = GlobalVar.m_CardReader.ReadOnce();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        AddLogStr("读卡器:" + msg);
                        Console.WriteLine("ReadCard:" + msg);
                    }
                    else Console.WriteLine("读取失败");
                    Thread.Sleep(200);
                }
            }));
            thd.Start();

            //if (tt)
            //{
            //    Thread thd = new Thread(new ThreadStart(delegate
            //    {
            //        while (true)
            //        {
            //            Label_Show(DateTime.Now.ToLongTimeString());
            //            //StartTest.Set();
            //            Thread.Sleep(1000);
            //        }
            //    }));
            //    thd.Start();
            //}
            //else
            //{
            //    COMM_EC form = new COMM_EC();
            //    form.TopLevel = false;
            //    form.Parent = this;
            //    form.MdiParent = this.MdiParent;
            //    form.ShowDialog();
            //}
            //tt = !tt;
        }

        /// <summary>
        /// 弹框【确认或者取消】
        /// </summary>
        /// <param name="text">内容</param>
        /// <param name="title">标题</param>
        /// <param name="backcolor">背景色</param>
        /// <returns></returns>
        private DialogResult MsgBoxShow(string text, string title, Color backcolor, MessageBoxButtons btn)
        {
            MsgBox box = new MsgBox(btn);
            box.Title = title;
            box.ShowText = text;
            box.BackColor = backcolor;
            return box.ShowDialog();
        }

        /// <summary>
        /// 弹框【只有确认】
        /// </summary>
        /// <param name="text">内容</param>
        /// <param name="title">标题</param>
        /// <param name="backcolor">背景色</param>
        /// <returns></returns>
        private DialogResult MsgBoxNoConfirm(string text, string title, Color backcolor)
        {
            MsgBox box = new MsgBox();
            box.Title = title;
            box.ShowText = text;
            box.BackColor = backcolor;
            return box.ShowDialog();
        }

        /// <summary>
        /// PLC异常弹框
        /// </summary>
        /// <param name="errmsg">异常内容</param>
        /// <returns></returns>
        private DialogResult ErrPLCMsgBox(string errmsg)
        {
            AddLogStr(errmsg, true, Color.Red);
            MsgBox box = new MsgBox("确认");
            box.Title = "PLC异常";
            box.ShowText = errmsg;
            return box.ShowDialog();
        }

        /// <summary>
        /// 异常弹框
        /// </summary>
        /// <param name="errmsg">异常内容</param>
        /// <param name="title">异常标题</param>
        /// <returns></returns>
        private DialogResult ErrMsgBox(string errmsg, string title = "异常")
        {
            AddLogStr(errmsg, true, Color.Red);
            MsgBox box = new MsgBox();
            box.Title = title;
            box.ShowText = errmsg;
            return box.ShowDialog();
        }

        /// <summary>
        /// 段取流程 弹框
        /// </summary>
        /// <param name="Msg">消息</param>
        /// <returns></returns>
        private DialogResult PrepareMsgBox(string Msg, MessageBoxButtons btn = MessageBoxButtons.YesNo)
        {
            speaker.AddSpeech(Msg);
            AddLogStr("段取流程 " + Msg, false, Color.LimeGreen);
            MsgBox box;
            switch (btn)
            {
                case MessageBoxButtons.YesNo:
                case MessageBoxButtons.OK:
                    box = new MsgBox(btn);
                    break;
                default:
                    return DialogResult.Cancel;
            }
            box.Title = "段取流程";
            box.ShowText = Msg;
            box.BackColor = Color.LimeGreen;
            return box.ShowDialog();
        }

        /// <summary>
        /// 段取流程 弹框
        /// </summary>
        /// <param name="Msg">消息</param>
        /// <param name="coil">判断的线圈</param>
        /// <param name="value">判断线圈的值</param>
        /// <param name="Title">标题</param>
        /// <param name="btn">按钮</param>
        /// <returns></returns>
        private DialogResult CoilMsgBox(string Msg, Coil coil, bool value, string Title = "提示", MessageBoxButtons btn = MessageBoxButtons.YesNo)
        {
            speaker.AddSpeech(Msg);
            AddLogStr(Title + Msg, false, Color.LimeGreen);
            MsgBox box;
            switch (btn)
            {
                case MessageBoxButtons.YesNo:
                case MessageBoxButtons.OK:
                    box = new MsgBox(coil, value, btn);
                    break;
                default:
                    return DialogResult.Cancel;
            }
            box.Title = Title;
            box.ShowText = Msg;
            box.BackColor = Color.LimeGreen;
            return box.ShowDialog();
        }

        private void linkLabel_Product_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!GlobalVar.c_Modbus.Coils.StartLeft.Value) return;

        }

        #region 显示日志部分
        /// <summary>
        /// 添加软件右下方日志
        /// </summary>
        /// <param name="str">信息</param>
        /// <param name="isError">是否为异常信息</param>
        /// <param name="color">右下方显示的字体的颜色</param>
        private void AddLogStr(string str, bool isError = false, Color? color = null)
        {
            if (!isError) log.AddCommLOG(str);
            else log.AddERRORLOG(str);
            str = "\r\n" + DateTime.Now.ToString("HH:mm:ss:fff") + "\t" + str;
            color = color ?? DefaultTextColor;
            Color m_color = (Color)color;
            ShowLog.Enqueue(new TextInfo(str, m_color));
        }

        /// <summary>
        /// 更新显示动作界面
        /// </summary>
        private void UpdateLog()
        {
            while (true)
            {
                if (ShowLog.Count > 0)
                {
                    StringBuilder logstr = new StringBuilder();
                    Color color = DefaultTextColor;
                    int addcount = ShowLog.Count;
                    for (int i = 0; i < addcount; i++)
                    {
                        TextInfo ti;
                        if (!ShowLog.TryDequeue(out ti)) continue;
                        if (ti.TextColor == DefaultTextColor)
                        {
                            logstr.Append(ti.Text);
                        }
                        else
                        {
                            if (logstr.Length == 0)
                            {
                                logstr.Append(ti.Text);
                                color = ti.TextColor;
                            }
                            break;
                        }
                    }
                    if (this.richTextBox_Process.InvokeRequired)
                    {
                        this.richTextBox_Process.Invoke(new Action(() => { ShowStr(logstr.ToString(), color); }));
                    }
                    else ShowStr(logstr.ToString(), color);
                }
                Thread.Sleep(10);
            }
        }

        private void ShowStr(string str, Color color)
        {
            const int ClearLength = 280;//删除判断条件
            const int RemoveLength = 40;//删除的前多少行
            if (richTextBox_Process.Lines.Length > ClearLength)
            {
                string[] lines = richTextBox_Process.Lines;
                string[] NewLines;
                int length = 0;
                for (int i = RemoveLength; i >= 0; i--)
                {
                    DateTime time;
                    if (lines[i].Length >= 8 && DateTime.TryParse(lines[i].Substring(0, 8), out time))
                    {
                        length = i;
                        break;
                    }
                }
                NewLines = new string[lines.Length - length];
                Array.Copy(lines, length, NewLines, 0, lines.Length - length);
                this.richTextBox_Process.Lines = NewLines;
            }

            int nSelectStart = richTextBox_Process.TextLength;

            this.richTextBox_Process.AppendText(str);

            int nLength = richTextBox_Process.TextLength - 1;
            richTextBox_Process.Select(nSelectStart, nLength);
            richTextBox_Process.SelectionColor = color;

            this.richTextBox_Process.ScrollToCaret();
            richTextBox_Process.Select(richTextBox_Process.TextLength, 0);
        }
        #endregion

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (GlobalVar.CCD != null)
                {
                    GlobalVar.CCD.Event_CCDStatusChanged -= CCD_Event_CCDStatusChanged;
                    GlobalVar.CCD.Stop();
                }
                if (GlobalVar.m_CardReader != null) GlobalVar.m_CardReader.Dispose();
                GlobalVar.c_Modbus.CoilMsgSync(GlobalVar.c_Modbus.Coils.Mode, false);//软件关闭，切换为手动状态
            }
            catch { }
            finally
            {
                GlobalVar.SoftWareShutDown = true;
                Application.ExitThread();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_VOLUME
        {
            public int dbcv_size;
            public int dbcv_devicetype;
            public int dbcv_reserved;
            public int dbcv_unitmask;
        }
        protected override void WndProc(ref Message m)
        {
            // 发生设备变动
            const int WM_DEVICECHANGE = 0x0219;
            // 系统检测到一个新设备
            const int DBT_DEVICEARRIVAL = 0x8000;
            // 系统完成移除一个设备
            const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
            // 逻辑卷标
            const int DBT_DEVTYP_VOLUME = 0x00000002;

            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    switch (m.WParam.ToInt32())
                    {
                        case DBT_DEVICEARRIVAL://插入U盘
                            int devType = Marshal.ReadInt32(m.LParam, 4);
                            if (devType == DBT_DEVTYP_VOLUME)
                            {
                                DEV_BROADCAST_VOLUME vol;
                                vol = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(
                                    m.LParam, typeof(DEV_BROADCAST_VOLUME));

                                UpdateTool.Main TestUDrive = new UpdateTool.Main();
                                string sourceFile = TestUDrive.U_DriveAlreadyIN(); ;
                                if (sourceFile != string.Empty)//判断U盘插入，是否为含有更新程序的U盘
                                {
                                    UpdateTool.MsgBox msgbox = new UpdateTool.MsgBox();
                                    msgbox.isStart_CountDown = true;
                                    msgbox.ShowStr = "检测到升级U盘插入，是否升级";
                                    this.TopMost = false;
                                    msgbox.ShowDialog();
                                    this.TopMost = true;
                                    if (msgbox.DialogResult == DialogResult.OK)
                                    {
                                        msgbox.Dispose();
                                        #region 先复制Update.exe，后面使用Update复制时，不复制Update.exe
                                        //try//先复制Update.exe，后面使用Update复制时，不复制Update.exe
                                        //{
                                        //    string app = @"\Update.exe";
                                        //    sourceFile += app;
                                        //    string destFile = Application.StartupPath + app;
                                        //    System.Diagnostics.Process cmd = new System.Diagnostics.Process();
                                        //    cmd.StartInfo.CreateNoWindow = false;
                                        //    cmd.StartInfo.UseShellExecute = false;
                                        //    cmd.StartInfo.RedirectStandardInput = true;
                                        //    cmd.StartInfo.RedirectStandardOutput = true;
                                        //    cmd.StartInfo.RedirectStandardError = true;
                                        //    cmd.StartInfo.FileName = "cmd.exe";
                                        //    cmd.Start();
                                        //    cmd.StandardInput.WriteLine("copy /y {0} {1}", sourceFile, destFile);
                                        //    cmd.StandardInput.WriteLine("exit");
                                        //    cmd.WaitForExit();
                                        //    string error = cmd.StandardError.ReadToEnd();
                                        //    if (error != string.Empty) throw new Exception(error);
                                        //}
                                        //catch (Exception copyex)
                                        //{
                                        //    if (MessageBox.Show("Update：" + copyex.Message + "\r\n是否继续", "出现异常", MessageBoxButtons.YesNo) == DialogResult.No) return;
                                        //}
                                        #endregion
                                        while (!msgbox.IsDisposed) Thread.Sleep(50);//未释放继续等待
                                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                                        p.StartInfo.FileName = Application.StartupPath + @"\Update";
                                        this.Hide();
                                        p.Start();
                                        Environment.Exit(0);
                                    }
                                }
                            }
                            break;
                        case DBT_DEVICEREMOVECOMPLETE://U盘退出
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

    }
}
