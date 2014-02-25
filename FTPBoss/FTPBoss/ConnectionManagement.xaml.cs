using System;
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

namespace FTPBoss
{
    /// <summary>
    /// Interaction logic for ConnectionManagement.xaml
    /// </summary>
    public partial class ConnectionManagement
    {
        public ConnectionManagement()
        {
            InitializeComponent();

            string[] profiles = Program2.credProfiles.GetProfiles();

            string profileList = "";
            for (int i = 0; i < profiles.Count(); ++i)
            {
                profileList += profiles[i] + "\r\n";
                listbox_credentialprofiles.Items.Add(profiles[i]);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
