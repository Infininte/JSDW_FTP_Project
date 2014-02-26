﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F343EA336C3B18169A523E852472B0CC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using FTPBoss;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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


namespace FTPBoss {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 66 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_getCredentials;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_connect;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView localDirectoryBrowser;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ServerDirectoryBrowser;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MahApps.Metro.Controls.MetroProgressBar progressBar;
        
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
            System.Uri resourceLocater = new System.Uri("/FTPBoss;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
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
            this.button_getCredentials = ((System.Windows.Controls.Button)(target));
            
            #line 66 "..\..\MainWindow.xaml"
            this.button_getCredentials.Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.button_connect = ((System.Windows.Controls.Button)(target));
            
            #line 71 "..\..\MainWindow.xaml"
            this.button_connect.Click += new System.Windows.RoutedEventHandler(this.Button_Connect);
            
            #line default
            #line hidden
            return;
            case 3:
            this.localDirectoryBrowser = ((System.Windows.Controls.TreeView)(target));
            
            #line 79 "..\..\MainWindow.xaml"
            this.localDirectoryBrowser.MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.OnPreviewMouseRightButtonDown);
            
            #line default
            #line hidden
            
            #line 79 "..\..\MainWindow.xaml"
            this.localDirectoryBrowser.AddHandler(System.Windows.Controls.TreeViewItem.ExpandedEvent, new System.Windows.RoutedEventHandler(this.TreeViewItemExpanded));
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 82 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.upload_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ServerDirectoryBrowser = ((System.Windows.Controls.ListBox)(target));
            
            #line 123 "..\..\MainWindow.xaml"
            this.ServerDirectoryBrowser.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.remoteDirBrowser_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 126 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.createDirectoryRemote_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 127 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.delete_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 128 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.download_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 129 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.permissions_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.progressBar = ((MahApps.Metro.Controls.MetroProgressBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

