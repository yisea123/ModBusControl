﻿namespace ECInspect
{
    partial class PLCInterface
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PLCInterface));
            this.tabControl_PLC = new System.Windows.Forms.TabControl();
            this.tabPage_Axis = new System.Windows.Forms.TabPage();
            this.tabControlNF_Axis = new ECInspect.TabControlNF();
            this.tabPage_CCD = new System.Windows.Forms.TabPage();
            this.Axis_CCD = new ECInspect.MyControl.AxisCCD();
            this.tabPage_Dot = new System.Windows.Forms.TabPage();
            this.Axis_Dot = new ECInspect.AxisDot();
            this.tabPage_X = new System.Windows.Forms.TabPage();
            this.Axis_X = new ECInspect.AxisX();
            this.tabPage_Y = new System.Windows.Forms.TabPage();
            this.Axis_Y = new ECInspect.AxisY();
            this.tabPage_Carry = new System.Windows.Forms.TabPage();
            this.Axis_Carry = new ECInspect.MyControl.AxisCarry();
            this.tabPage_Peel = new System.Windows.Forms.TabPage();
            this.Axis_Peel = new ECInspect.AxisPeel();
            this.tabPage_Location = new System.Windows.Forms.TabPage();
            this.groupBoxEx4 = new ECInspect.GroupBoxEx();
            this.textBox_YCCD = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_XCCD = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBoxEx3 = new ECInspect.GroupBoxEx();
            this.textBox_YScan = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox_XScan = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBoxEx1 = new ECInspect.GroupBoxEx();
            this.textBox_YMark = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_XMark = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tabPage_Other = new System.Windows.Forms.TabPage();
            this.groupBoxEx2 = new ECInspect.GroupBoxEx();
            this.textBox_ShortTimeDelay = new System.Windows.Forms.TextBox();
            this.textBox_MediumTimeDelay = new System.Windows.Forms.TextBox();
            this.textBox_LongTimeDelay = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.either_Rotate = new ECInspect.Either();
            this.either_FeedIsPress = new ECInspect.Either();
            this.tabPage_Comm = new System.Windows.Forms.TabPage();
            this.btn_Pause = new ECInspect.ImageButton();
            this.richbox_CommLog = new System.Windows.Forms.RichTextBox();
            this.groupBoxEx5 = new ECInspect.GroupBoxEx();
            this.textBox_Yorigion = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_Xorigin = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tabControl_PLC.SuspendLayout();
            this.tabPage_Axis.SuspendLayout();
            this.tabControlNF_Axis.SuspendLayout();
            this.tabPage_CCD.SuspendLayout();
            this.tabPage_Dot.SuspendLayout();
            this.tabPage_X.SuspendLayout();
            this.tabPage_Y.SuspendLayout();
            this.tabPage_Carry.SuspendLayout();
            this.tabPage_Peel.SuspendLayout();
            this.tabPage_Location.SuspendLayout();
            this.groupBoxEx4.SuspendLayout();
            this.groupBoxEx3.SuspendLayout();
            this.groupBoxEx1.SuspendLayout();
            this.tabPage_Other.SuspendLayout();
            this.groupBoxEx2.SuspendLayout();
            this.tabPage_Comm.SuspendLayout();
            this.groupBoxEx5.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tabControl_PLC);
            // 
            // tabControl_PLC
            // 
            this.tabControl_PLC.Controls.Add(this.tabPage_Axis);
            this.tabControl_PLC.Controls.Add(this.tabPage_Other);
            this.tabControl_PLC.Controls.Add(this.tabPage_Comm);
            this.tabControl_PLC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_PLC.Location = new System.Drawing.Point(0, 0);
            this.tabControl_PLC.Name = "tabControl_PLC";
            this.tabControl_PLC.SelectedIndex = 0;
            this.tabControl_PLC.Size = new System.Drawing.Size(1920, 942);
            this.tabControl_PLC.TabIndex = 0;
            this.tabControl_PLC.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl_PLC_Selected);
            // 
            // tabPage_Axis
            // 
            this.tabPage_Axis.Controls.Add(this.tabControlNF_Axis);
            this.tabPage_Axis.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Axis.Name = "tabPage_Axis";
            this.tabPage_Axis.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Axis.Size = new System.Drawing.Size(1912, 916);
            this.tabPage_Axis.TabIndex = 0;
            this.tabPage_Axis.Text = "轴";
            this.tabPage_Axis.UseVisualStyleBackColor = true;
            // 
            // tabControlNF_Axis
            // 
            this.tabControlNF_Axis.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControlNF_Axis.Controls.Add(this.tabPage_CCD);
            this.tabControlNF_Axis.Controls.Add(this.tabPage_Dot);
            this.tabControlNF_Axis.Controls.Add(this.tabPage_X);
            this.tabControlNF_Axis.Controls.Add(this.tabPage_Y);
            this.tabControlNF_Axis.Controls.Add(this.tabPage_Carry);
            this.tabControlNF_Axis.Controls.Add(this.tabPage_Peel);
            this.tabControlNF_Axis.Controls.Add(this.tabPage_Location);
            this.tabControlNF_Axis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlNF_Axis.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlNF_Axis.ItemSize = new System.Drawing.Size(100, 100);
            this.tabControlNF_Axis.Location = new System.Drawing.Point(3, 3);
            this.tabControlNF_Axis.Multiline = true;
            this.tabControlNF_Axis.Name = "tabControlNF_Axis";
            this.tabControlNF_Axis.SelectedIndex = 0;
            this.tabControlNF_Axis.Size = new System.Drawing.Size(1906, 910);
            this.tabControlNF_Axis.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlNF_Axis.TabIndex = 2;
            this.tabControlNF_Axis.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControlNF_Axis_Selected);
            // 
            // tabPage_CCD
            // 
            this.tabPage_CCD.Controls.Add(this.Axis_CCD);
            this.tabPage_CCD.Location = new System.Drawing.Point(104, 4);
            this.tabPage_CCD.Name = "tabPage_CCD";
            this.tabPage_CCD.Size = new System.Drawing.Size(1798, 902);
            this.tabPage_CCD.TabIndex = 6;
            this.tabPage_CCD.Text = "相机轴";
            this.tabPage_CCD.UseVisualStyleBackColor = true;
            // 
            // Axis_CCD
            // 
            this.Axis_CCD.AxisName = ECInspect.Axis.CCD;
            this.Axis_CCD.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Axis_CCD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Axis_CCD.Location = new System.Drawing.Point(0, 0);
            this.Axis_CCD.Name = "Axis_CCD";
            this.Axis_CCD.Size = new System.Drawing.Size(1798, 902);
            this.Axis_CCD.TabIndex = 0;
            // 
            // tabPage_Dot
            // 
            this.tabPage_Dot.Controls.Add(this.Axis_Dot);
            this.tabPage_Dot.Location = new System.Drawing.Point(104, 4);
            this.tabPage_Dot.Name = "tabPage_Dot";
            this.tabPage_Dot.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Dot.Size = new System.Drawing.Size(1798, 902);
            this.tabPage_Dot.TabIndex = 0;
            this.tabPage_Dot.Text = "打点轴";
            this.tabPage_Dot.UseVisualStyleBackColor = true;
            // 
            // Axis_Dot
            // 
            this.Axis_Dot.AxisName = ECInspect.Axis.Dot;
            this.Axis_Dot.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Axis_Dot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Axis_Dot.Location = new System.Drawing.Point(3, 3);
            this.Axis_Dot.Name = "Axis_Dot";
            this.Axis_Dot.Size = new System.Drawing.Size(1792, 896);
            this.Axis_Dot.TabIndex = 0;
            // 
            // tabPage_X
            // 
            this.tabPage_X.Controls.Add(this.Axis_X);
            this.tabPage_X.Location = new System.Drawing.Point(104, 4);
            this.tabPage_X.Name = "tabPage_X";
            this.tabPage_X.Size = new System.Drawing.Size(1798, 902);
            this.tabPage_X.TabIndex = 4;
            this.tabPage_X.Text = "  X轴";
            this.tabPage_X.UseVisualStyleBackColor = true;
            // 
            // Axis_X
            // 
            this.Axis_X.AxisName = ECInspect.Axis.X;
            this.Axis_X.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Axis_X.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Axis_X.Location = new System.Drawing.Point(0, 0);
            this.Axis_X.Name = "Axis_X";
            this.Axis_X.Size = new System.Drawing.Size(1798, 902);
            this.Axis_X.TabIndex = 0;
            // 
            // tabPage_Y
            // 
            this.tabPage_Y.Controls.Add(this.Axis_Y);
            this.tabPage_Y.Location = new System.Drawing.Point(104, 4);
            this.tabPage_Y.Name = "tabPage_Y";
            this.tabPage_Y.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Y.Size = new System.Drawing.Size(1798, 902);
            this.tabPage_Y.TabIndex = 1;
            this.tabPage_Y.Text = "  Y轴";
            this.tabPage_Y.UseVisualStyleBackColor = true;
            // 
            // Axis_Y
            // 
            this.Axis_Y.AxisName = ECInspect.Axis.Y;
            this.Axis_Y.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Axis_Y.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Axis_Y.Location = new System.Drawing.Point(3, 3);
            this.Axis_Y.Name = "Axis_Y";
            this.Axis_Y.Size = new System.Drawing.Size(1792, 896);
            this.Axis_Y.TabIndex = 1;
            // 
            // tabPage_Carry
            // 
            this.tabPage_Carry.Controls.Add(this.Axis_Carry);
            this.tabPage_Carry.Location = new System.Drawing.Point(104, 4);
            this.tabPage_Carry.Name = "tabPage_Carry";
            this.tabPage_Carry.Size = new System.Drawing.Size(1798, 902);
            this.tabPage_Carry.TabIndex = 5;
            this.tabPage_Carry.Text = "搬运轴";
            this.tabPage_Carry.UseVisualStyleBackColor = true;
            // 
            // Axis_Carry
            // 
            this.Axis_Carry.AxisName = ECInspect.Axis.Carry;
            this.Axis_Carry.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Axis_Carry.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Axis_Carry.Location = new System.Drawing.Point(0, 0);
            this.Axis_Carry.Name = "Axis_Carry";
            this.Axis_Carry.Size = new System.Drawing.Size(1798, 902);
            this.Axis_Carry.TabIndex = 0;
            // 
            // tabPage_Peel
            // 
            this.tabPage_Peel.Controls.Add(this.Axis_Peel);
            this.tabPage_Peel.Location = new System.Drawing.Point(104, 4);
            this.tabPage_Peel.Name = "tabPage_Peel";
            this.tabPage_Peel.Size = new System.Drawing.Size(1798, 902);
            this.tabPage_Peel.TabIndex = 2;
            this.tabPage_Peel.Text = "剥料轴";
            this.tabPage_Peel.UseVisualStyleBackColor = true;
            // 
            // Axis_Peel
            // 
            this.Axis_Peel.AxisName = ECInspect.Axis.Peel;
            this.Axis_Peel.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Axis_Peel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Axis_Peel.Location = new System.Drawing.Point(0, 0);
            this.Axis_Peel.Name = "Axis_Peel";
            this.Axis_Peel.Size = new System.Drawing.Size(1798, 902);
            this.Axis_Peel.TabIndex = 0;
            // 
            // tabPage_Location
            // 
            this.tabPage_Location.Controls.Add(this.groupBoxEx5);
            this.tabPage_Location.Controls.Add(this.groupBoxEx4);
            this.tabPage_Location.Controls.Add(this.groupBoxEx3);
            this.tabPage_Location.Controls.Add(this.groupBoxEx1);
            this.tabPage_Location.Location = new System.Drawing.Point(104, 4);
            this.tabPage_Location.Name = "tabPage_Location";
            this.tabPage_Location.Size = new System.Drawing.Size(1798, 902);
            this.tabPage_Location.TabIndex = 3;
            this.tabPage_Location.Text = "  坐标";
            this.tabPage_Location.UseVisualStyleBackColor = true;
            // 
            // groupBoxEx4
            // 
            this.groupBoxEx4.BackColor = System.Drawing.Color.White;
            this.groupBoxEx4.Controls.Add(this.textBox_YCCD);
            this.groupBoxEx4.Controls.Add(this.label12);
            this.groupBoxEx4.Controls.Add(this.textBox_XCCD);
            this.groupBoxEx4.Controls.Add(this.label13);
            this.groupBoxEx4.Location = new System.Drawing.Point(10, 222);
            this.groupBoxEx4.Name = "groupBoxEx4";
            this.groupBoxEx4.Radius = 10;
            this.groupBoxEx4.Size = new System.Drawing.Size(400, 100);
            this.groupBoxEx4.TabIndex = 6;
            this.groupBoxEx4.TabStop = false;
            this.groupBoxEx4.Text = "拍照位置";
            this.groupBoxEx4.TitleFont = new System.Drawing.Font("SimSun", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // textBox_YCCD
            // 
            this.textBox_YCCD.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
            this.textBox_YCCD.Location = new System.Drawing.Point(237, 46);
            this.textBox_YCCD.Name = "textBox_YCCD";
            this.textBox_YCCD.ReadOnly = true;
            this.textBox_YCCD.Size = new System.Drawing.Size(100, 29);
            this.textBox_YCCD.TabIndex = 5;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft YaHei", 15.75F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(208, 46);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 28);
            this.label12.TabIndex = 4;
            this.label12.Text = "Y:";
            // 
            // textBox_XCCD
            // 
            this.textBox_XCCD.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
            this.textBox_XCCD.Location = new System.Drawing.Point(75, 46);
            this.textBox_XCCD.Name = "textBox_XCCD";
            this.textBox_XCCD.ReadOnly = true;
            this.textBox_XCCD.Size = new System.Drawing.Size(100, 29);
            this.textBox_XCCD.TabIndex = 3;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft YaHei", 15.75F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(7, 38);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(60, 28);
            this.label13.TabIndex = 2;
            this.label13.Text = "拍照:";
            // 
            // groupBoxEx3
            // 
            this.groupBoxEx3.BackColor = System.Drawing.Color.White;
            this.groupBoxEx3.Controls.Add(this.textBox_YScan);
            this.groupBoxEx3.Controls.Add(this.label10);
            this.groupBoxEx3.Controls.Add(this.textBox_XScan);
            this.groupBoxEx3.Controls.Add(this.label11);
            this.groupBoxEx3.Location = new System.Drawing.Point(10, 116);
            this.groupBoxEx3.Name = "groupBoxEx3";
            this.groupBoxEx3.Radius = 10;
            this.groupBoxEx3.Size = new System.Drawing.Size(400, 100);
            this.groupBoxEx3.TabIndex = 5;
            this.groupBoxEx3.TabStop = false;
            this.groupBoxEx3.Text = "扫码位置";
            this.groupBoxEx3.TitleFont = new System.Drawing.Font("SimSun", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // textBox_YScan
            // 
            this.textBox_YScan.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
            this.textBox_YScan.Location = new System.Drawing.Point(237, 46);
            this.textBox_YScan.Name = "textBox_YScan";
            this.textBox_YScan.ReadOnly = true;
            this.textBox_YScan.Size = new System.Drawing.Size(100, 29);
            this.textBox_YScan.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft YaHei", 15.75F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(208, 46);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 28);
            this.label10.TabIndex = 4;
            this.label10.Text = "Y:";
            // 
            // textBox_XScan
            // 
            this.textBox_XScan.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
            this.textBox_XScan.Location = new System.Drawing.Point(75, 46);
            this.textBox_XScan.Name = "textBox_XScan";
            this.textBox_XScan.ReadOnly = true;
            this.textBox_XScan.Size = new System.Drawing.Size(100, 29);
            this.textBox_XScan.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft YaHei", 15.75F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(9, 46);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 28);
            this.label11.TabIndex = 2;
            this.label11.Text = "扫码:";
            // 
            // groupBoxEx1
            // 
            this.groupBoxEx1.BackColor = System.Drawing.Color.White;
            this.groupBoxEx1.Controls.Add(this.textBox_YMark);
            this.groupBoxEx1.Controls.Add(this.label5);
            this.groupBoxEx1.Controls.Add(this.textBox_XMark);
            this.groupBoxEx1.Controls.Add(this.label9);
            this.groupBoxEx1.Location = new System.Drawing.Point(10, 10);
            this.groupBoxEx1.Name = "groupBoxEx1";
            this.groupBoxEx1.Radius = 10;
            this.groupBoxEx1.Size = new System.Drawing.Size(400, 100);
            this.groupBoxEx1.TabIndex = 4;
            this.groupBoxEx1.TabStop = false;
            this.groupBoxEx1.Text = "打标位置";
            this.groupBoxEx1.TitleFont = new System.Drawing.Font("SimSun", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // textBox_YMark
            // 
            this.textBox_YMark.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
            this.textBox_YMark.Location = new System.Drawing.Point(237, 46);
            this.textBox_YMark.Name = "textBox_YMark";
            this.textBox_YMark.ReadOnly = true;
            this.textBox_YMark.Size = new System.Drawing.Size(100, 29);
            this.textBox_YMark.TabIndex = 5;
            this.textBox_YMark.Click += new System.EventHandler(this.textBox_YMark_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft YaHei", 15.75F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(208, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 28);
            this.label5.TabIndex = 4;
            this.label5.Text = "Y:";
            // 
            // textBox_XMark
            // 
            this.textBox_XMark.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
            this.textBox_XMark.Location = new System.Drawing.Point(75, 46);
            this.textBox_XMark.Name = "textBox_XMark";
            this.textBox_XMark.ReadOnly = true;
            this.textBox_XMark.Size = new System.Drawing.Size(100, 29);
            this.textBox_XMark.TabIndex = 3;
            this.textBox_XMark.Click += new System.EventHandler(this.textBox_XMark_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft YaHei", 15.75F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(9, 46);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 28);
            this.label9.TabIndex = 2;
            this.label9.Text = "打点:";
            // 
            // tabPage_Other
            // 
            this.tabPage_Other.BackColor = System.Drawing.Color.DarkTurquoise;
            this.tabPage_Other.Controls.Add(this.groupBoxEx2);
            this.tabPage_Other.Controls.Add(this.either_Rotate);
            this.tabPage_Other.Controls.Add(this.either_FeedIsPress);
            this.tabPage_Other.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Other.Name = "tabPage_Other";
            this.tabPage_Other.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Other.Size = new System.Drawing.Size(1912, 916);
            this.tabPage_Other.TabIndex = 1;
            this.tabPage_Other.Text = "其他";
            // 
            // groupBoxEx2
            // 
            this.groupBoxEx2.Controls.Add(this.textBox_ShortTimeDelay);
            this.groupBoxEx2.Controls.Add(this.textBox_MediumTimeDelay);
            this.groupBoxEx2.Controls.Add(this.textBox_LongTimeDelay);
            this.groupBoxEx2.Controls.Add(this.label8);
            this.groupBoxEx2.Controls.Add(this.label7);
            this.groupBoxEx2.Controls.Add(this.label6);
            this.groupBoxEx2.Font = new System.Drawing.Font("Microsoft YaHei", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBoxEx2.Location = new System.Drawing.Point(901, 80);
            this.groupBoxEx2.Name = "groupBoxEx2";
            this.groupBoxEx2.Radius = 10;
            this.groupBoxEx2.Size = new System.Drawing.Size(500, 350);
            this.groupBoxEx2.TabIndex = 1;
            this.groupBoxEx2.TabStop = false;
            this.groupBoxEx2.Text = "时间";
            this.groupBoxEx2.TitleFont = new System.Drawing.Font("Microsoft YaHei", 36F);
            // 
            // textBox_ShortTimeDelay
            // 
            this.textBox_ShortTimeDelay.Location = new System.Drawing.Point(211, 233);
            this.textBox_ShortTimeDelay.Name = "textBox_ShortTimeDelay";
            this.textBox_ShortTimeDelay.Size = new System.Drawing.Size(250, 71);
            this.textBox_ShortTimeDelay.TabIndex = 1;
            this.textBox_ShortTimeDelay.Click += new System.EventHandler(this.textBox_ShortTimeDelay_Click);
            // 
            // textBox_MediumTimeDelay
            // 
            this.textBox_MediumTimeDelay.Location = new System.Drawing.Point(211, 156);
            this.textBox_MediumTimeDelay.Name = "textBox_MediumTimeDelay";
            this.textBox_MediumTimeDelay.Size = new System.Drawing.Size(250, 71);
            this.textBox_MediumTimeDelay.TabIndex = 1;
            this.textBox_MediumTimeDelay.Click += new System.EventHandler(this.textBox_MediumTimeDelay_Click);
            // 
            // textBox_LongTimeDelay
            // 
            this.textBox_LongTimeDelay.Location = new System.Drawing.Point(209, 79);
            this.textBox_LongTimeDelay.Name = "textBox_LongTimeDelay";
            this.textBox_LongTimeDelay.Size = new System.Drawing.Size(250, 71);
            this.textBox_LongTimeDelay.TabIndex = 1;
            this.textBox_LongTimeDelay.Click += new System.EventHandler(this.textBox_LongTimeDelay_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 237);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(183, 62);
            this.label8.TabIndex = 0;
            this.label8.Text = "短延时:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 160);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(183, 62);
            this.label7.TabIndex = 0;
            this.label7.Text = "中延时:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(183, 62);
            this.label6.TabIndex = 0;
            this.label6.Text = "长延时:";
            // 
            // either_Rotate
            // 
            this.either_Rotate.BackColor = System.Drawing.SystemColors.Control;
            this.either_Rotate.BtnLeftText = "不旋转";
            this.either_Rotate.BtnRightText = "旋转";
            this.either_Rotate.LeftPress = true;
            this.either_Rotate.Location = new System.Drawing.Point(53, 264);
            this.either_Rotate.Name = "either_Rotate";
            this.either_Rotate.Size = new System.Drawing.Size(717, 113);
            this.either_Rotate.TabIndex = 0;
            this.either_Rotate.Title = "旋转汽缸";
            this.either_Rotate.Event_BtnClick += new ECInspect.Either.dele_LeftRight(this.either_Rotate_Event_BtnClick);
            // 
            // either_FeedIsPress
            // 
            this.either_FeedIsPress.BackColor = System.Drawing.SystemColors.Control;
            this.either_FeedIsPress.BtnLeftText = "下压";
            this.either_FeedIsPress.BtnRightText = "不下压";
            this.either_FeedIsPress.LeftPress = true;
            this.either_FeedIsPress.Location = new System.Drawing.Point(53, 57);
            this.either_FeedIsPress.Name = "either_FeedIsPress";
            this.either_FeedIsPress.Size = new System.Drawing.Size(717, 113);
            this.either_FeedIsPress.TabIndex = 0;
            this.either_FeedIsPress.Title = "下料-是否下压";
            this.either_FeedIsPress.Event_BtnClick += new ECInspect.Either.dele_LeftRight(this.either_FeedIsPress_Event_BtnClick);
            // 
            // tabPage_Comm
            // 
            this.tabPage_Comm.Controls.Add(this.btn_Pause);
            this.tabPage_Comm.Controls.Add(this.richbox_CommLog);
            this.tabPage_Comm.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Comm.Name = "tabPage_Comm";
            this.tabPage_Comm.Size = new System.Drawing.Size(1912, 916);
            this.tabPage_Comm.TabIndex = 2;
            this.tabPage_Comm.Text = "通讯[调试]";
            this.tabPage_Comm.UseVisualStyleBackColor = true;
            // 
            // btn_Pause
            // 
            this.btn_Pause.Dock = System.Windows.Forms.DockStyle.Top;
            this.btn_Pause.Font = new System.Drawing.Font("Microsoft YaHei", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Pause.Location = new System.Drawing.Point(0, 0);
            this.btn_Pause.Name = "btn_Pause";
            this.btn_Pause.Size = new System.Drawing.Size(1912, 60);
            this.btn_Pause.TabIndex = 1;
            this.btn_Pause.Text = "Pause";
            this.btn_Pause.UseVisualStyleBackColor = true;
            this.btn_Pause.Click += new System.EventHandler(this.btn_Pause_Click);
            // 
            // richbox_CommLog
            // 
            this.richbox_CommLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richbox_CommLog.Location = new System.Drawing.Point(0, 0);
            this.richbox_CommLog.Name = "richbox_CommLog";
            this.richbox_CommLog.Size = new System.Drawing.Size(1912, 916);
            this.richbox_CommLog.TabIndex = 0;
            this.richbox_CommLog.Text = "";
            // 
            // groupBoxEx5
            // 
            this.groupBoxEx5.BackColor = System.Drawing.Color.White;
            this.groupBoxEx5.Controls.Add(this.textBox_Yorigion);
            this.groupBoxEx5.Controls.Add(this.label14);
            this.groupBoxEx5.Controls.Add(this.textBox_Xorigin);
            this.groupBoxEx5.Controls.Add(this.label15);
            this.groupBoxEx5.Location = new System.Drawing.Point(10, 328);
            this.groupBoxEx5.Name = "groupBoxEx5";
            this.groupBoxEx5.Radius = 10;
            this.groupBoxEx5.Size = new System.Drawing.Size(400, 100);
            this.groupBoxEx5.TabIndex = 7;
            this.groupBoxEx5.TabStop = false;
            this.groupBoxEx5.Text = "拍照原点";
            this.groupBoxEx5.TitleFont = new System.Drawing.Font("SimSun", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // textBox_Yorigion
            // 
            this.textBox_Yorigion.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
            this.textBox_Yorigion.Location = new System.Drawing.Point(237, 46);
            this.textBox_Yorigion.Name = "textBox_Yorigion";
            this.textBox_Yorigion.ReadOnly = true;
            this.textBox_Yorigion.Size = new System.Drawing.Size(100, 29);
            this.textBox_Yorigion.TabIndex = 5;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft YaHei", 15.75F, System.Drawing.FontStyle.Bold);
            this.label14.Location = new System.Drawing.Point(208, 46);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 28);
            this.label14.TabIndex = 4;
            this.label14.Text = "Y:";
            // 
            // textBox_Xorigin
            // 
            this.textBox_Xorigin.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold);
            this.textBox_Xorigin.Location = new System.Drawing.Point(75, 46);
            this.textBox_Xorigin.Name = "textBox_Xorigin";
            this.textBox_Xorigin.ReadOnly = true;
            this.textBox_Xorigin.Size = new System.Drawing.Size(100, 29);
            this.textBox_Xorigin.TabIndex = 3;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft YaHei", 15.75F, System.Drawing.FontStyle.Bold);
            this.label15.Location = new System.Drawing.Point(9, 46);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(60, 28);
            this.label15.TabIndex = 2;
            this.label15.Text = "拍照:";
            // 
            // PLCInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "PLCInterface";
            this.Text = "PLCInterface";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PLCInterface_FormClosing);
            this.Load += new System.EventHandler(this.PLCInterface_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.tabControl_PLC.ResumeLayout(false);
            this.tabPage_Axis.ResumeLayout(false);
            this.tabControlNF_Axis.ResumeLayout(false);
            this.tabPage_CCD.ResumeLayout(false);
            this.tabPage_Dot.ResumeLayout(false);
            this.tabPage_X.ResumeLayout(false);
            this.tabPage_Y.ResumeLayout(false);
            this.tabPage_Carry.ResumeLayout(false);
            this.tabPage_Peel.ResumeLayout(false);
            this.tabPage_Location.ResumeLayout(false);
            this.groupBoxEx4.ResumeLayout(false);
            this.groupBoxEx4.PerformLayout();
            this.groupBoxEx3.ResumeLayout(false);
            this.groupBoxEx3.PerformLayout();
            this.groupBoxEx1.ResumeLayout(false);
            this.groupBoxEx1.PerformLayout();
            this.tabPage_Other.ResumeLayout(false);
            this.groupBoxEx2.ResumeLayout(false);
            this.groupBoxEx2.PerformLayout();
            this.tabPage_Comm.ResumeLayout(false);
            this.groupBoxEx5.ResumeLayout(false);
            this.groupBoxEx5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl_PLC;
        private System.Windows.Forms.TabPage tabPage_Axis;
        private TabControlNF tabControlNF_Axis;
        private System.Windows.Forms.TabPage tabPage_Dot;
        private AxisDot Axis_Dot;
        private System.Windows.Forms.TabPage tabPage_Y;
        private AxisY Axis_Y;
        private System.Windows.Forms.TabPage tabPage_Peel;
        private System.Windows.Forms.TabPage tabPage_Other;
        private System.Windows.Forms.TabPage tabPage_Comm;
        private System.Windows.Forms.RichTextBox richbox_CommLog;
        private ImageButton btn_Pause;
        private AxisPeel Axis_Peel;
        private System.Windows.Forms.TabPage tabPage_Location;
        private System.Windows.Forms.TextBox textBox_XMark;
        private System.Windows.Forms.Label label9;
        private GroupBoxEx groupBoxEx1;
        private System.Windows.Forms.TextBox textBox_YMark;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage_X;
        private AxisX Axis_X;
        private System.Windows.Forms.TabPage tabPage_Carry;
        private MyControl.AxisCarry Axis_Carry;
        private Either either_FeedIsPress;
        private Either either_Rotate;
        private GroupBoxEx groupBoxEx2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_ShortTimeDelay;
        private System.Windows.Forms.TextBox textBox_MediumTimeDelay;
        private System.Windows.Forms.TextBox textBox_LongTimeDelay;
        private GroupBoxEx groupBoxEx3;
        private System.Windows.Forms.TextBox textBox_YScan;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox_XScan;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabPage tabPage_CCD;
        private GroupBoxEx groupBoxEx4;
        private System.Windows.Forms.TextBox textBox_YCCD;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox_XCCD;
        private System.Windows.Forms.Label label13;
        private MyControl.AxisCCD Axis_CCD;
        private GroupBoxEx groupBoxEx5;
        private System.Windows.Forms.TextBox textBox_Yorigion;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox_Xorigin;
        private System.Windows.Forms.Label label15;
    }
}