﻿#pragma checksum "C:\Users\omer\documents\visual studio 2013\Projects\FeedReader\FeedReader\View\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "19EB4F589CA01C5018E86FDD11BD92E6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using SlideView.Library;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace FeedReader {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal SlideView.Library.SlideView SliderView;
        
        internal System.Windows.Controls.Grid leftView;
        
        internal Microsoft.Phone.Controls.LongListSelector CategoryLLS;
        
        internal System.Windows.Controls.Button addFeedButton;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.ListBox NewsListBox;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/FeedReader;component/View/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.SliderView = ((SlideView.Library.SlideView)(this.FindName("SliderView")));
            this.leftView = ((System.Windows.Controls.Grid)(this.FindName("leftView")));
            this.CategoryLLS = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("CategoryLLS")));
            this.addFeedButton = ((System.Windows.Controls.Button)(this.FindName("addFeedButton")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.NewsListBox = ((System.Windows.Controls.ListBox)(this.FindName("NewsListBox")));
        }
    }
}

