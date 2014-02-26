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
            if(button_addprofile.Content == "Add")
            {
                Program2.credProfiles.Add(textbox_addprofile.Text, textbox_addhost.Text, textbox_adduser.Text, textbox_addpass.Text, textbox_addport.Text);
            }
            else if (button_addprofile.Content == "Save")
            {
                int index = listbox_credentialprofiles.SelectedIndex;

                if (index >= 0)
                {
                    string profile = listbox_credentialprofiles.Items[index].ToString();
                    Program2.credProfiles.Edit(profile, textbox_addprofile.Text, textbox_addhost.Text, textbox_adduser.Text, textbox_addpass.Text, textbox_addport.Text);

                    // Restore button look and fields
                    button_addprofile.Content = "Add";
                    textbox_addprofile.Text = "";
                    textbox_addhost.Text = "";
                    textbox_adduser.Text = "";
                    textbox_addpass.Text = "";
                    textbox_addport.Text = "";
                }
            }
            
            PopulateProfiles();
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

        private void button_savecredstofile_Click(object sender, RoutedEventArgs e)
        {
            Program2.credProfiles.SaveToFile(Program2.CredentialFile);
        }

        private void button_load_Click(object sender, RoutedEventArgs e)
        {
            int index = listbox_credentialprofiles.SelectedIndex;

            if (index >= 0)
            {
                string profile = listbox_credentialprofiles.Items[index].ToString();
                Program2.credProfiles.SelectProfile(profile);

                Program2.Host = Program2.credProfiles.GetHost();
                Program2.User = Program2.credProfiles.GetUser();
                Program2.Pass = Program2.credProfiles.GetPass();
                Program2.Port = Program2.credProfiles.GetPort();
                this.Close();
            }
        }

        private void button_edit_Click(object sender, RoutedEventArgs e)
        {
            int index = listbox_credentialprofiles.SelectedIndex;

            if (index >= 0)
            {
                button_addprofile.Content = "Save";

                string profile = listbox_credentialprofiles.Items[index].ToString();
                Program2.credProfiles.SelectProfile(profile);

                textbox_addprofile.Text = profile;
                textbox_addhost.Text = Program2.credProfiles.GetHost();
                textbox_adduser.Text = Program2.credProfiles.GetUser();
                textbox_addpass.Text = Program2.credProfiles.GetPass();
                textbox_addport.Text = Program2.credProfiles.GetPort();
            }
        }
    }
}
