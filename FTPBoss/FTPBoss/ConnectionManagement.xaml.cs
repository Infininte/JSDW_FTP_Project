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
        private void PopulateProfiles()
        {
            listbox_credentialprofiles.Items.Clear();

            string[] profiles = Program2.credProfiles.GetProfiles();

            string profileList = "";
            for (int i = 0; i < profiles.Count(); ++i)
            {
                profileList += profiles[i] + "\r\n";
                listbox_credentialprofiles.Items.Add(profiles[i]);
            }
        }

        public ConnectionManagement()
        {
            InitializeComponent();

            PopulateProfiles();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (listbox_credentialprofiles.SelectedIndex >= 0)
            {
                int index = listbox_credentialprofiles.SelectedIndex;
                string profile = listbox_credentialprofiles.Items[index].ToString();

                Program2.credProfiles.Delete(profile);
                PopulateProfiles();
            }

           
        }
    }
}
