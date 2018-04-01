using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarCry5MultiProfile
{
    public class ConfigurationManager
    {
        private string _configPath;
        private static ConfigurationManager _instance;
        public static ConfigurationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigurationManager();
                }
                return _instance;
            }
        }

        private Dictionary<string, string> _configDictionary;

        public string CurrentProfile { get { return _configDictionary["CurrentProfile"]; } set { _configDictionary["CurrentProfile"] = value; } }
        public string Profiles { get { return _configDictionary["Profiles"]; } set { _configDictionary["Profiles"] = value; } }
        public string SaveGamePath { get { return _configDictionary["SaveGamePath"]; } set { _configDictionary["SaveGamePath"] = value; } }
        public string GameId { get { return _configDictionary["GameId"]; } set { _configDictionary["GameId"] = value; } }
        public string GameExe { get { return _configDictionary["GameExe"]; } set { _configDictionary["GameExe"] = value; } }

        private ConfigurationManager()
        {
            _configDictionary = new Dictionary<string, string>();
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
            var configRaw = File.ReadAllLines(_configPath);

            foreach(var line in configRaw)
            {
                if (line.Trim().Length == 0)
                {
                    continue;
                }

                if (line[0] == '#')
                {
                    continue;
                }

                var lineData = line.Split('=');
                _configDictionary.Add(lineData[0], lineData[1]);
            }
        }

        public void Save()
        {
            var saveData = new List<string>();
            foreach(var key in _configDictionary.Keys)
            {
                saveData.Add(key + "=" + _configDictionary[key]);
            }
            File.WriteAllLines(_configPath, saveData, Encoding.UTF8);
        }
    }

    public static class Settings
    {
        private static ConfigurationManager _default;
        public static ConfigurationManager Default
        {
            get
            {
                if (_default == null)
                {
                    _default = ConfigurationManager.Instance;
                }
                return _default;
            }
        }
    }
}
