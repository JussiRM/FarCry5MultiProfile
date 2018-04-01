using FarCry5MultiProfile.Properties;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace FarCry5MultiProfile
{
    /// <summary>
    /// Interaction logic for PathSettingsWindow.xaml
    /// </summary>
    public partial class PathSettingsWindow : Window
    {
        public event Action OnPathSettingsWindowClose;

        public PathSettingsWindow()
        {
            InitializeComponent();

            SaveGameFolderInput.Text = Settings.Default.SaveGamePath;
            GameIdInput.Text = Settings.Default.GameId;
            GameExeInput.Text = Settings.Default.GameExe;
        }

        private void ExitWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var result = SaveValues();
            if (result)
            {
                this.Close();
            }
        }

        private bool SaveValues()
        {
            Settings.Default.SaveGamePath = SaveGameFolderInput.Text;
            Settings.Default.GameId = GameIdInput.Text;
            Settings.Default.GameExe = GameExeInput.Text;

            if (!Directory.Exists(Settings.Default.SaveGamePath))
            {
                MessageBox.Show("´Savegames Path´ doesn't seem to exist.", "NANI?! ╯°□°）╯︵ ┻━┻");
                return false;
            }
            else
            {
                if (!Directory.Exists(System.IO.Path.Combine(Settings.Default.SaveGamePath, Settings.Default.GameId)))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(Settings.Default.SaveGamePath, Settings.Default.GameId));
                }
            }

            if (!Directory.Exists(Settings.Default.GameExe))
            {
                MessageBox.Show("´Game Exe Path´ doesn't seem to exist.", "NANI?! ╯°□°）╯︵ ┻━┻");
                return false;
            }

            Settings.Default.Save();
            return true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnPathSettingsWindowClose.Invoke();
        }
    }
}
