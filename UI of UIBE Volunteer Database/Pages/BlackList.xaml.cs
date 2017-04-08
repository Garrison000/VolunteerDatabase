﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VolunteerDatabase.Entity;

namespace VolunteerDatabase.Desktop.Pages
{
    /// <summary>
    /// Interaction logic for BlackList.xaml
    /// </summary>
    public partial class BlackList : UserControl
    {
        private Volunteer Vol;
        List<BlackListRecord> sourceList;
        public BlackList(Volunteer vol)
        {
            Vol = vol;
            InitializeComponent();
            sourceList = Vol.BlackListRecords;
            Blacklistdata.ItemsSource = sourceList;
        }      
    }
}
