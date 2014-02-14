using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FTP_Boss
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public async void Connect(object sender, RoutedEventArgs e)
        {
            if(validateConnectForm())
            {
                //Code to connect to the FTP server
                this.Frame.Navigate(typeof(FileTransferPage), "File Transfer Page");
            }
            else
            {
                //Validation failed display some error
                var messageDialog = new MessageDialog("An error has occurred");
                await messageDialog.ShowAsync();
            }
        }

        private bool validateConnectForm()
        {
            return true;
            string hostText = host.Text;
            string portNum = port.Text;
            string usernameText = username.Text;
            string passwordText = password.Password;

            if(hostText != "" && usernameText != "" && passwordText != "")
            {
                return true;
            }
            return false;
        }
    }
}
