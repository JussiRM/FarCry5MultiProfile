using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FarCry5MultiProfile.Properties;
using System.Windows.Controls;
using System.Windows;

namespace FarCry5MultiProfile
{
    public class ProfileManager
    {
        public List<string> Profiles { get; private set; }
        public string SaveFilePath { get
            {
                return Settings.Default.SaveGamePath;
            }
        }
        public string FarCry5Id
        {
            get
            {
                return Settings.Default.GameId;
            }
        }

        private static ProfileManager _instance;
        public static ProfileManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProfileManager();
                }
                return _instance;
            }
        }
        
        private ProfileManager()
        {
            Profiles = new List<string>();
            var profileListFromSettings = Settings.Default.Profiles;
            if (profileListFromSettings.Length > 0)
            {
                Profiles = profileListFromSettings.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        public void AddProfile(string profileName)
        {
            if (!Profiles.Contains(profileName))
            {
                if (Profiles.Count == 0)
                {
                    Settings.Default.CurrentProfile = profileName;
                }
                Profiles.Add(profileName);
                SaveProfiles();
            }
        }

        public void RemoveProfile(string profileName)
        {
            if (Profiles.Contains(profileName))
            {
                Profiles.Remove(profileName);
                SaveProfiles();
            }
        }

        private void SaveProfiles()
        {
            string profListString = string.Join(";", Profiles);
            Settings.Default.Profiles = profListString;
            Settings.Default.Save();
        }

        public void ChangeProfile(string toProfile)
        {
            if (!IsProfileReal(toProfile))
            {
                return;
            }

            // Get current profile
            string currentProfile = Settings.Default.CurrentProfile;

            // Get Far Cry 5 Save Directory.
            // This is the "Current profile" save directory.
            var farcry5Directory = GetFarCry5Folder();

            string toProfilePath = farcry5Directory.ProfilePath(toProfile);

            // Check if the profile exists that we are switching to.
            // Copy current profile saves to new profile as default.
            CreateProfileFolder(toProfile, false);

            // Rename current save dir to current profile
            string currentProfileNewPath = farcry5Directory.ProfilePath(currentProfile);
            Directory.Move(farcry5Directory.Path, currentProfileNewPath);

            // Rename profile folder to farcry5id 
            Directory.Move(toProfilePath, farcry5Directory.Path);

            // Profit?
            Settings.Default.CurrentProfile = toProfile;
            Settings.Default.Save();
        }

        private DirObject GetFarCry5Folder()
        {
            return Directory.GetDirectories(SaveFilePath)
                .Select(x => new DirObject(x))
                .Where(x => x.Name == FarCry5Id)
                .FirstOrDefault();
        }

        private bool IsProfileReal(string toProfile, bool withMessages = true)
        {
            if (!ProfileManager.Instance.Profiles.Contains(toProfile))
            {
                if (withMessages) MessageBox.Show("No such profile.", "NANI?! ╯°□°）╯︵ ┻━┻");
                return false;
            }

            if (toProfile == Settings.Default.CurrentProfile)
            {
                if (withMessages) MessageBox.Show("This is already active profile!", "NANI?! ╯°□°）╯︵ ┻━┻");
                return false;
            }

            return true;
        }

        public bool ForcefullySetActiveProfile(string toProfile)
        {
            if (!IsProfileReal(toProfile))
            {
                return false;
            }

            string currentActiveProfile = Settings.Default.CurrentProfile;

            var farcry5Directory = GetFarCry5Folder();

            if (!Directory.Exists(farcry5Directory.ProfilePath(currentActiveProfile)))
            {
                CreateProfileFolder(toProfile);
                Directory.Move(farcry5Directory.ProfilePath(toProfile), farcry5Directory.ProfilePath(currentActiveProfile));
            }
            else
            {
                MessageBox.Show(@"App detected that current active profile " + currentActiveProfile + " already has save files stored.\n" +
                    "This should not normally happen.\n" +
                    "App only marked the active profile for " + toProfile);
            }
            Settings.Default.CurrentProfile = toProfile;
            Settings.Default.Save();
            return true;
        }

        private void CreateProfileFolder(string toProfile, bool copyFiles = true)
        {
            var farcry5Directory = GetFarCry5Folder();

            if (!Directory.Exists(farcry5Directory.ProfilePath(toProfile)))
            {
                Directory.CreateDirectory(farcry5Directory.ProfilePath(toProfile));
                if (copyFiles)
                {
                    foreach (var file in Directory.GetFiles(farcry5Directory.Path))
                    {
                        var filename = GetFileNameFromPath(file);
                        File.Copy(file, farcry5Directory.ProfilePath(toProfile) + "\\" + filename, false);
                    }
                }
            }
        }

        private string GetFileNameFromPath(string path)
        {
            var pathArr = path.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            return pathArr[pathArr.Length - 1];
        }

        public void RefreshCombobox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            foreach(var p in Profiles)
            {
                comboBox.Items.Add(p);
            }
        }
    }

    public class DirObject
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public DirObject(string path)
        {
            var pathArr = path.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            Name = pathArr[pathArr.Length-1];
            Path = path;
        }

        public string ProfilePath(string profile)
        {
            return Path + "_" + profile;
        }
    }
}
