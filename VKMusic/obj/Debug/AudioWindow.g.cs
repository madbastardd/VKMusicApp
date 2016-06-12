﻿#pragma checksum "..\..\AudioWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "8C83EDB1F5A3E4C0AED29DF9C214A749"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using VKMusic;


namespace VKMusic {
    
    
    /// <summary>
    /// AudioWindow
    /// </summary>
    public partial class AudioWindow : MahApps.Metro.Controls.MetroWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 23 "..\..\AudioWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel mainPanel;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\AudioWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid basic;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\AudioWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button playSong;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\AudioWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button downloadSong;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\AudioWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button loadMore;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\AudioWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar downloadProgress;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\AudioWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button playGlobalSong;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\AudioWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button previousRemote;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\AudioWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button forwardRemote;
        
        #line default
        #line hidden
        
        
        #line 118 "..\..\AudioWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider remoteSong;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\AudioWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock songDuration;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VKMusic;component/audiowindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\AudioWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.mainPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 2:
            this.basic = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.playSong = ((System.Windows.Controls.Button)(target));
            return;
            case 4:
            this.downloadSong = ((System.Windows.Controls.Button)(target));
            return;
            case 5:
            this.loadMore = ((System.Windows.Controls.Button)(target));
            
            #line 63 "..\..\AudioWindow.xaml"
            this.loadMore.Click += new System.Windows.RoutedEventHandler(this.loadMore_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.downloadProgress = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 7:
            this.playGlobalSong = ((System.Windows.Controls.Button)(target));
            
            #line 88 "..\..\AudioWindow.xaml"
            this.playGlobalSong.Click += new System.Windows.RoutedEventHandler(this.playSong_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.previousRemote = ((System.Windows.Controls.Button)(target));
            return;
            case 9:
            this.forwardRemote = ((System.Windows.Controls.Button)(target));
            return;
            case 10:
            this.remoteSong = ((System.Windows.Controls.Slider)(target));
            
            #line 119 "..\..\AudioWindow.xaml"
            this.remoteSong.PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(this.remoteSong_PreviewMouseUp);
            
            #line default
            #line hidden
            
            #line 120 "..\..\AudioWindow.xaml"
            this.remoteSong.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.remoteSong_PreviewMouseMove);
            
            #line default
            #line hidden
            return;
            case 11:
            this.songDuration = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

