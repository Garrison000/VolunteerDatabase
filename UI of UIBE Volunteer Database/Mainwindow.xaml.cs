﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VolunteerDatabase.Helper;
using VolunteerDatabase.Interface;

namespace WpfApplication1
{
    /// <summary>
    /// Mainwindow.xaml 的交互逻辑
    /// </summary>
    public partial class Mainwindow : Window
    {
        private AppUserIdentityClaims Claims { get; set; }
        private ProjectManageHelper ProjectManageHelper { get; set; }
        private ProjectProgressHelper PrijectProgressHelper { get; set; }
        public Mainwindow(AppUserIdentityClaims claims)
        {
            InitializeComponent();
            Claims = claims;
            on_window_Create();

        }

        private void on_window_Create()
        {
            ProjectManageHelper = ProjectManageHelper.GetInstance();
            if (Claims.IsInRole(AppRoleEnum.Administrator) || Claims.IsInRole(AppRoleEnum.TestOnly))
            {
                Project_Manage.IsEnabled = true;
                Project_Add.IsEnabled = true;
                User_Approve.IsEnabled = true;
                Volunteer_Info.IsEnabled = true;
                User_Info.IsEnabled = true;
            }
            if (Claims.IsInRole(AppRoleEnum.OrgnizationAdministrator))
            {
                Project_Manage.IsEnabled = true;
                Project_Add.IsEnabled = true;
                User_Approve.IsEnabled = true;
                Volunteer_Info.IsEnabled = true;
                User_Info.IsEnabled = true;
            }
            if (Claims.IsInRole(AppRoleEnum.OrgnizationMember))
            {
                Project_Manage.IsEnabled = true;
                Volunteer_Info.IsEnabled = true;
                User_Info.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("用户身份非法,请重新登陆后再试.");
                Main_Tabcontrol.IsEnabled = false;
            }
            if (Claims.Roles.Contains(AppRoleEnum.OrgnizationMember))
            {
                PrijectProgressHelper = ProjectProgressHelper.GetInstance();
                var list = PrijectProgressHelper.FindAuthorizedProjectsByUser(Claims.User);
                project_list.ItemsSource = list;
            }
            if (Claims.Roles.Contains(AppRoleEnum.OrgnizationAdministrator))
            {
                var list = ProjectManageHelper.ShowProjectList(Claims.Holder.Organization, true);
                project_list.ItemsSource = list;
            }
        }

        private void exit_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void create_project_button_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(project_details.Document.ContentStart, project_details.Document.ContentEnd);
            if (project_name.Text == "" || project_place.Text == "" || project_time.DisplayDate == null || project_time.DisplayDate < DateTime.Now.AddYears(-20) || project_place.Text == "" || project_maximum.Text == "" || textRange.Text == "")
            {
                MessageBox.Show("请完整输入所有项目.");
            }
            else
            {
                //可以在这里对RichTextBox做美化               
                ProgressResult result = ProjectManageHelper.CreatNewProject(Claims.Holder.Organization, project_time.DisplayDate, project_name.Text, project_place.Text, textRange.Text, int.Parse(project_maximum.Text));
                //这里缺少对最大值是否为数值的检验，看看前端能否实现
                if (result.Succeeded)
                {
                    MessageBox.Show("项目创建成功!");
                    project_name.Clear();
                    project_place.Clear();
                    project_maximum.Clear();
                    project_details.Document.Blocks.Clear();
                }
                else
                {
                    MessageBox.Show("项目创建失败!错误信息" + string.Join(",", result.Errors));
                }
            }
        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            //删除按键直接写出来感觉有点怪，看看能不能集成在单个project点击进去之后
        }

        private void project_maximum_KeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)))
            {
                e.Handled = true;
            }
        }

        private void Label_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
