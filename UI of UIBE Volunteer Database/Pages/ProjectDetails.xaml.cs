﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using VolunteerDatabase.Desktop.Pages.InPutVolunteerInBatch;
using VolunteerDatabase.Entity;
using VolunteerDatabase.Helper;
using VolunteerDatabase.Interface;
using FirstFloor.ModernUI.Windows.Controls;

namespace VolunteerDatabase.Desktop.Pages
{
    /// <summary>
    /// AddManager.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectInformation : Window
    {
        private Project Pro;
        private AppUserIdentityClaims Claims;
        public void sendClaimsEventHandler(AppUserIdentityClaims claim)
        {
            IsEnabled = true;
            this.Claims = claim;
            IdentityPage identitypage = IdentityPage.GetInstance(claim);
        }

        public void logOutEventHandler()
        {
            Claims = null;
            Close();
        }

        public ProjectInformation(AppUserIdentityClaims Claim, Project pro)
        {
            if (Claim == null)
            {
                Login.GetClaims(sendClaimsEventHandler, logOutEventHandler);
                IsEnabled = false;
            }
            else
            {
                this.Claims = Claim;
                Pro = pro;
            }
            InitializeComponent();
            Auth();        
            ProInfoShow();
            List<AppUser> users=Pro.Managers.ToList();
            List<Volunteer> vols=Pro.Volunteers.ToList();
            project_manager_list.ItemsSource = users;
            volunteer_list.ItemsSource = vols;
        }
        private void Auth()
        {
            if (Pro.ScoreCondition != ProjectScoreCondition.Scored)
            {
                endproject.IsEnabled = false;
            }
            if (Claims.Roles.Count() == 0||Pro.Condition==ProjectCondition.Finished)
            {
                AddManager_btn.IsEnabled = false;
                deleteproject_btn.IsEnabled = false;
                endproject.IsEnabled = false;
                piliang.IsEnabled = false;
                AddVolunteer_btn.IsEnabled = false;
            }
            if (Claims.Roles.Count() == 1 && Claims.IsInRole(AppRoleEnum.OrgnizationMember))
            {
                AddManager_btn.IsEnabled = false;
                deleteproject_btn.IsEnabled = false;
                project_manager_list.IsEnabled = false;
                AddManager.IsEnabled = false;
                AddManager_btn.IsEnabled = false;
            }
            if (Claims.Roles.Count() == 1 && Claims.IsInRole(AppRoleEnum.OrgnizationAdministrator))
            {
                endproject.IsEnabled = false;
                piliang.IsEnabled = false;
                AddVolunteer_btn.IsEnabled = false;
            }
        }
        private void ProInfoShow()
        {
            if (Pro != null)
            {
                //org.Text = Pro.Organization.Name;
                Title = Pro.Name;
                project_id.Text = Pro.Id.ToString();
                project_place.Text = Pro.Place;
                project_status.Text = Pro.Condition.ToString();
                project_time.Text = Pro.Time.ToString();
                project_accomodation.Text = Pro.Volunteers.Count() + "/" + Pro.Maximum.ToString();
            }         
        }

        private void piliang_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Csv文件|*.csv";
            List<Volunteer> list = new List<Volunteer>();
            if (op.ShowDialog() == true)
            {
                var ch = CsvHelper.GetInstance();
                list = ch.PrepareAddInBatch(op, Pro);
                if (ch.informingMessage.Count() != 0)
                {
                    foreach (string item in ch.informingMessage)
                    {
                        //if(item)
                        MessageBox.Show(item);
                    }//此处应建立窗口提示informingMessage,即有改动的信息，然后传多个学号，再调用ch中方法确定新信息
                }
                if(ch.errorList.Count()!=0)
                {
                    foreach (string item in ch.errorList)
                    {
                        MessageBox.Show(item);
                    }
                }
                Window window = new Window();

                window.Height = 650;
                window.Width = 470;
                DealWithConflict dealer = new DealWithConflict(Pro, list, window);

                window.Content = dealer;
                window.Owner = this;
                window.Show();
                //MessageBox.Show("导入的信息与志愿者库中不一致的条目已被红色高亮标记,请确认保留项目.");
            }

          
           
        }

        private void endproject_Click(object sender, RoutedEventArgs e)
        {
            {
                var pph = ProjectProgressHelper.GetInstance();
                MessageBox.Show("未单独评分的志愿者将默认全部评分为：4");
                if(Pro!=null&&Pro.Condition==ProjectCondition.Ongoing)
                {
                    var result=pph.ScoringDefaultForVolunteers(Pro, 4);
                    if(!result.Succeeded)
                    {
                        MessageBox.Show("评分失败");
                    }
                    
                }          
                if (Pro != null)
                {
                    var result = pph.FinishProject(Pro);
                    if (!result.Succeeded)
                    {
                        MessageBox.Show("结项失败");
                    }
                }                        
            }

        }
        private void deleteproject_btn_Click(object sender, RoutedEventArgs e)
        {
            var pmh = ProjectManageHelper.GetInstance();
            MessageBoxResult result=MessageBox.Show("确定要删除该项目?", "删除提醒", MessageBoxButton.YesNo, MessageBoxImage.Information);
            switch(result)
            {
                case MessageBoxResult.Yes:
                    pmh.ProjectDelete(Pro);
         
                    this.Close();
                    break;
                case MessageBoxResult.No:
                    break;
            }

        }

        private void AddManager_btn_Click(object sender, RoutedEventArgs e)
        {
            if(AddManager.Text=="")
            {
                MessageBox.Show("请输入管理者学号!");
            }
            else
            { 
                var pmh = ProjectManageHelper.GetInstance();
                try
                {
                    var result = pmh.AddManager(int.Parse(AddManager.Text), Pro);
                    if(result.Succeeded)
                    {
                        MessageBox.Show("学号为[" + AddManager.Text + "]的用户已经被添加为项目["+Pro.Name+"]的项目管理者.");
                        project_manager_list.ItemsSource = null;
                        project_manager_list.ItemsSource = Pro.Managers.ToList();
                    }
                    if (!result.Succeeded)
                    {
                        MessageBox.Show("导入失败,错误信息:"+string.Join(",",result.Errors));
                        AddManager.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void AddVolunteer_btn_Click(object sender, RoutedEventArgs e)
        {
            if(AddVolunteer.Text=="")
            {
                MessageBox.Show("请输入管理者学号!");
            }
            else
            {
                var pph = ProjectProgressHelper.GetInstance();
                var result = pph.SingleVolunteerInputById(int.Parse(AddVolunteer.Text), Pro);
                if (result.Succeeded)
                {
                    MessageBox.Show("学号为[" + AddVolunteer.Text + "]的志愿者已经被添加入项目[" + Pro.Name + "]的志愿者列表.");
                    volunteer_list.ItemsSource = null;
                    volunteer_list.ItemsSource = Pro.Volunteers.ToList();
                }
                if (!result.Succeeded)
                {
                    MessageBox.Show("导入失败");
                    AddVolunteer.Clear();
                }
            }          
        }

        private void AddManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)))
            {
                e.Handled = true;
            }
        }

        private void AddVolunteer_KeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)))
            {
                e.Handled = true;
            }
        }

        private void rate_btn_Click(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            if (senderButton.DataContext is Volunteer)
            {
                Volunteer Vol = (Volunteer)senderButton.DataContext;
                if(Vol!=null&&Pro.ScoreCondition!=ProjectScoreCondition.UnScored)
                {
                    var rp = new Rating(Vol,Pro);
                    rp.Owner = this;
                }
            }          
        }

        private void deleteprojectmanager_btn_Click(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            if (senderButton.DataContext is AppUser)
            {
                AppUser Man = (AppUser)senderButton.DataContext;
                if(Man!=null)
                {
                    var pmh = ProjectManageHelper.GetInstance();
                    pmh.DeletManager(Man.StudentNum, Pro);
                    project_manager_list.ItemsSource = null;
                    project_manager_list.ItemsSource = Pro.Managers.ToList();
                }
            }
        }

        private void deletevolunteer_btn_Click(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            if (senderButton.DataContext is Volunteer)
            {
                Volunteer Vol = (Volunteer)senderButton.DataContext;
                if (Vol != null)
                {
                    var pph = ProjectProgressHelper.GetInstance();
                    pph.DeleteVolunteerFromProject(Vol, Pro);
                    volunteer_list.ItemsSource = null;
                    volunteer_list.ItemsSource = Pro.Volunteers.ToList();
                }
            }
        }

        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            var templateName = "导入模板.csv";
            var sfd = new SaveFileDialog
            {
                Filter = "逗号分隔的列表文件（*.csv）|*.csv",
                DefaultExt = "csv",
                FileName = "导入模板",
                AddExtension = true
            };
            if (sfd.ShowDialog() == true)
            {
                var pathName = sfd.FileName;
                var templatePathName = Environment.CurrentDirectory + @"\" + templateName;
                try
                {
                    File.Copy(templatePathName, pathName, true);
                }
                catch (IOException exception)
                {
                    //TODO: LOG
                }

                MessageBox.Show("导出模板文件成功。");
            }
            else
            {
                MessageBox.Show("用户取消操作。");
            }
        }
    }
}
