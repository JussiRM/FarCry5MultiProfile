using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace FarCry5MultiProfile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProfileManager _profileManager;

        public MainWindow()
        {
            InitializeComponent();
            _profileManager = ProfileManager.Instance;

            RefreshCombobox();

            CheckStartGameButton();
        }

        private void CheckStartGameButton()
        {
            bool isOkSettings = CheckIfSettingsAreOk();
            StartGameButton.IsEnabled = isOkSettings;
            StartGameError.Visibility = isOkSettings ? Visibility.Collapsed : Visibility.Visible;
        }

        private bool CheckIfSettingsAreOk()
        {
            var settings = Settings.Default;
            ManageProfilesButton.IsEnabled = true;

            if (!Directory.Exists(settings.GameExe) || !Directory.Exists(settings.SaveGamePath))
            {
                StartGameError.Text = "Check Path Settings";
                ManageProfilesButton.IsEnabled = false;
                return false;
            }

            if (settings.GameId.Length == 0)
            {
                StartGameError.Text = "Check Game ID in Path Settings";
                ManageProfilesButton.IsEnabled = false;
                return false;
            }

            if (ProfileManager.Instance.Profiles.Count == 0)
            {
                StartGameError.Text = "Please create one or more profiles.";
                return false;
            }

            if (settings.CurrentProfile == "")
            {
                StartGameError.Text = "CurrentProfile is empty. WTF? ╯°□°）╯︵ ┻━┻ \nFix in config file?";
                return false;
            }

            Process[] pname = Process.GetProcessesByName("Steam");
            if (pname.Length == 0)
            {
                StartGameError.Text = "Steam is not running. Please run Steam.";
                return true; // Well not sure if steam is required...?
            }

            return true;
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            var currentProfile = Settings.Default.CurrentProfile;
            var profileTo = (string)ProfileList.SelectedItem;

            if (profileTo != currentProfile)
            {
                _profileManager.ChangeProfile(profileTo);
            }

            Process.Start(Settings.Default.GameExe + "\\FarCry5.exe");
        }

        private void ManageProfilesButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileManagerWindow pmWindow = new ProfileManagerWindow();
            pmWindow.OnProfileManagerWindowClosed += () =>
            {
                RefreshCombobox();
                CheckStartGameButton();
            };
            pmWindow.ShowDialog();
        }

        private void RefreshCombobox()
        {
            ProfileManager.Instance.RefreshCombobox(ProfileList);

            ProfileList.SelectedIndex = ProfileList.Items.IndexOf(Settings.Default.CurrentProfile);
        }

        private void PathSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            PathSettingsWindow psWindow = new PathSettingsWindow();
            psWindow.OnPathSettingsWindowClose += () => CheckStartGameButton();
            psWindow.ShowDialog();
        }
    }
}
