﻿using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class FormClient : System.Windows.Forms.Form
    {
        private  string mysqlConnectionString;//保存MySQL连接信息的字符串

        public int UserType = 0;        //用户类型 0学生 1教授 2管理员

        private String UserTypeName = String.Empty;

        public string id;

        public Boolean Selecting = true;

        public bool IsCourseRegistrationOpen = true;
        public FormClient()
        {
            InitializeComponent();
            InitializeConnection();
            //在此添加更多的其他初始化函数
            this.tabControl1.TabPages.Clear();
            this.tabControl1.TabPages.Add(this.LoginTabpage);
            this.UserTypeComboBox.SelectedIndex = 0;
        }


        private void FormClient_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if(Selecting == false)
            {
                e.Cancel = true;
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (log())
            {
                this.UserType = this.UserTypeComboBox.SelectedIndex;
                this.tabControl1.TabPages.Clear();
                switch (this.UserType)
                {
                    case 0:
                        this.tabControl1.TabPages.Add(this.PersonalInformationTabPage);
                        this.ShowTabPage1.Text = "查看已选课程";
                        this.tabControl1.TabPages.Add(this.ShowTabPage1);
                        this.labelShowTabPage1.Text = "已选课程:";
                        this.ShowTabPage2.Text = "查看课程信息";
                        this.tabControl1.TabPages.Add(this.ShowTabPage2);
                        this.labelShowTabPage2.Text = "课程详细信息:";
                        this.tabControl1.TabPages.Add(this.NotificationTabPage);
                        //this.tabControl1.TabPages.Add(this.RegisterCoursesTabPage);
                        this.tabControl1.TabPages.Add(this.ViewReportCardTabPage);
                        this.tabControl1.TabPages.Add(this.ScheduleTabPage);
                        InitializeDataTable课程表();
                        InitializeStudentPart();
                        Show_personal_info();
                        break;
                    case 1:
                        this.tabControl1.TabPages.Add(this.PersonalInformationTabPage);
                        if (IsCourseRegistrationOpen)
                            this.tabControl1.TabPages.Add(this.TeachCourseManageTabPage);
                        this.ShowTabPage1.Text = "本学期所教授课程";
                        this.labelShowTabPage1.Text = "以下为您本学期所教授的课程：";
                        this.tabControl1.TabPages.Add(this.ShowTabPage1);
                        this.tabControl1.TabPages.Add(this.SubmitGradesTabPage);
                        InitializeProfessorPart();
                        InitializeDataTable课程表();
                        Show_personal_info();
                        this.label双击课程提示.Visible = false;
                        break;
                    case 2:
                        this.tabControl1.TabPages.Add(this.ProfessorInformationTabPage);
                        this.tabControl1.TabPages.Add(this.StudentInformationTabPage);
                        this.tabControl1.TabPages.Add(this.SystemManageTabPage);
                        Show_pro_info();
                        Show_stu_info();
                        break;
                }
            }
            else
            {
                if (ReceiveMessage.Length == 0)
                    ReceiveMessage = "登录失败";
                MessageBox.Show(ReceiveMessage, "无效登录", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool log()
        {
            this.id = textBoxLoginName.Text;
            string message = String.Format("login@{0},{1},{2}", UserTypeName, textBoxLoginName.Text, textBoxLoginPassword.Text);
            GetServerMessage(message);
            string receivedMessage = ReceiveMessage;
            if (receivedMessage.Length > 0)
            {
                string[] receivedMessages = receivedMessage.Split('@');
                this.mysqlConnectionString = receivedMessages[1];
                if (receivedMessages[0].Equals("True"))
                {
                    MySqlConnection conn = new MySqlConnection(this.mysqlConnectionString);
                    try
                    {
                        string mes = "服务器数据连接建立成功\n";
                        if (receivedMessages[2].Equals("CourseRegistrationOpen"))
                        {
                            this.IsCourseRegistrationOpen = true;
                            mes += "课程注册开放";
                        }
                        else if (receivedMessages[2].Equals("CourseRegistrationClose"))
                        {
                            this.IsCourseRegistrationOpen = false;
                            mes += "课程注册关闭";
                        }
                        conn.Open();
                        MessageBox.Show(mes, "数据连接测试", MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("服务器数据连接建立失败" + ex.ToString(), "数据连接测试", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                    conn.Close();
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        private void UserTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.UserTypeComboBox.SelectedIndex)
            {
                case 0:
                    this.UserTypeName = "student";
                    break;
                case 1:
                    this.UserTypeName = "professor";
                    break;
                case 2:
                    this.UserTypeName = "registrar";
                    break;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
