using FarCry5MultiProfile.Properties;
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
using System.Windows.Shapes;

namespace FarCry5MultiProfile
{
    /// <summary>
    /// Interaction logic for ProfileManagerWindow.xaml
    /// </summary>
    public partial class ProfileManagerWindow : Window
    {
        public event Action OnProfileManagerWindowClosed;

        public ProfileManagerWindow()
        {
            InitializeComponent();
            RefreshCombobox();
            CurrentActiveProfileTextBlock.Text = Settings.Default.CurrentProfile;

            if (ProfileManager.Instance.Profiles.Count > 0)
            {
                NoProfilesMessage.Visibility = Visibility.Collapsed;
            }
        }

        private void RefreshCombobox()
        {
            ProfileManager.Instance.RefreshCombobox(ProfileList);
        }

        private void AddProfileButton_Click(object sender, RoutedEventArgs e)
        {
            string profileName = ProfileNameInput.Text;
            ProfileNameInput.Text = "";
            MessageBox.Show("Profile Added!", "Wow much hype");
            ProfileManager.Instance.AddProfile(profileName);
            RefreshCombobox();
        }

        private void RemoveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var listItem = ProfileList.Text;

            if (Settings.Default.CurrentProfile == listItem)
            {
                MessageBox.Show(
                    "Can't delete profile.\nThis is the current active profile. Please play with some other profile before deleting this one.",
                    "NANI?! ╯°□°）╯︵ ┻━┻", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Exclamation
                );
                return;
            }

            if (MessageBox.Show("Really remove profile? (Saves won't be deleted)", "NANI?! ╯°□°）╯︵ ┻━┻", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ProfileManager.Instance.RemoveProfile(listItem);
                RefreshCombobox();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnProfileManagerWindowClosed.Invoke();
        }

        private void ForceSetActiveProfileInputButton_Click(object sender, RoutedEventArgs e)
        {
            string profile = ForceSetActiveProfileInput.Text;
            bool result = ProfileManager.Instance.ForcefullySetActiveProfile(profile);

            if (result)
            {
                MessageBox.Show("Active profile is now " + profile, "( ͡° ͜ʖ ͡°)");
                ForceSetActiveProfileInput.Text = "";
                CurrentActiveProfileTextBlock.Text = profile;
            }
        }
    }
}
