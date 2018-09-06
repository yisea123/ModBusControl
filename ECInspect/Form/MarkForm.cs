﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ECInspect
{
    public partial class MarkForm : Frame
    {
        public MarkForm()
        {
            InitializeComponent();
            this.WindowRefresh.Tick += new EventHandler(WindowRefresh_Tick);
            WindowRefresh_Tick(null, EventArgs.Empty);//立刻刷新一次
        }

        private void WindowRefresh_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("Refresh");
            this.either_DownJig.LeftPress = !GlobalVar.c_Modbus.Coils.YSignLocation.Value;         //下治具 位置
            this.either_Mark1.LeftPress = !GlobalVar.c_Modbus.Coils.XWaitMarkLocation.Value;     //打标器 位置
            this.either_Mark2.LeftPress = !GlobalVar.c_Modbus.Coils.Cylinder_Dot.Value;          //打标器 上升

            this.Invalidate();
        }

        private void btn_MarkTest_Click(object sender, EventArgs e)
        {
            if (!GlobalVar.c_Modbus.Coils.XWaitMarkLocation.Value)
            {
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.XWaitMarkLocation, true);
                GlobalVar.c_Modbus.CoilMsgSync(GlobalVar.c_Modbus.Coils.Cylinder_Dot, false);
                Thread.Sleep(1300);
                //while (!GlobalVar.c_Modbus.Coils.XWaitMarkLocation.Value) Thread.Sleep(100);

                GlobalVar.c_Modbus.CoilMsgSync(GlobalVar.c_Modbus.Coils.Cylinder_Dot, true);
                Thread.Sleep(500);
                while (!GlobalVar.c_Modbus.Coils.Cylinder_Dot.Value) Thread.Sleep(100);

                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.XWaitMarkLocation, false);
            }
        }

        private void btn_ChangeMarkDevice_Click(object sender, EventArgs e)
        {
            if (!GlobalVar.c_Modbus.Coils.XWaitMarkLocation.Value)
            {
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.XWaitMarkLocation, true);
            }
            else
            {
                GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.XWaitMarkLocation, false);
            }
        }

        private void btn_ChangeMarkSet_Click(object sender, EventArgs e)
        {
            MarkChangeSetForm form = new MarkChangeSetForm();
            form.TopLevel = false;
            form.Parent = this;
            form.MdiParent = this.MdiParent;
            form.ShowDialog();
        }

        private void either_DownJig_Event_BtnClick(object sender, LeftRightSide lr)
        {
            switch (lr)
            {
                case LeftRightSide.Left:
                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.YSignLocation, false);
                    break;
                case LeftRightSide.Right:
                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.YSignLocation, true);
                    break;
            }
        }

        private void either_Mark1_Event_BtnClick(object sender, LeftRightSide lr)
        {
            switch (lr)
            {
                case LeftRightSide.Left:
                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.XWaitMarkLocation, false);
                    break;
                case LeftRightSide.Right:
                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.XWaitMarkLocation, true);
                    break;
            }
        }

        private void either_Mark2_Event_BtnClick(object sender, LeftRightSide lr)
        {
            switch (lr)
            {
                case LeftRightSide.Left:
                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.Cylinder_Dot, false);
                    break;
                case LeftRightSide.Right:
                    GlobalVar.c_Modbus.AddMsgList(GlobalVar.c_Modbus.Coils.Cylinder_Dot, true);
                    break;
            }
        }

    }
}
