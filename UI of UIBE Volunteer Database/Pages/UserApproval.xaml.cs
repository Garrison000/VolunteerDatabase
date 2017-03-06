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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VolunteerDatabase.Helper;

namespace Desktop.Pages
{
    /// <summary>
    /// Interaction logic for UserApproval.xaml
    /// </summary>
    public partial class UserApproval : UserControl
    {
        private IdentityPage identitypage = IdentityPage.GetInstance();
        private AppUserIdentityClaims Claims { get; set; }
        public UserApproval()
        {
            Claims = identitypage.Claims;
            InitializeComponent();
        }
        private void sendClaimsEventHandler(AppUserIdentityClaims claims)
        {
            this.Claims = claims;
            MessageBox.Show("用户审批模块收到令牌.");
        }
    }
}
