using System;
using System.Collections.Generic;
using System.IO;

// Type Defines
using SettingPair = System.Collections.Generic.KeyValuePair<string, string>;

namespace EB_Utility
{
    public static class Settings
    {
        public static string settings_file = "settings.eb";
        public static List<SettingPair> default_settings = new List<SettingPair> { };
        private static List<SettingPair> settings = new List<SettingPair>();
		
        public static void load_settings()
        {
            if(!File.Exists(settings_file))
            {
                File.Create(settings_file).Close();

                if(default_settings != null)
                {
                    for(int i=0; i < default_settings.Count; i++)
                        set_setting(default_settings[i].Key, default_settings[i].Value, true);
                }

                return;
            }

            settings.Clear();

            string[] settings_content = File.ReadAllLines(settings_file);

            for(int i=0; i < settings_content.Length; i++)
            {
                string   setting = settings_content[i];
                string[] setting_split = setting.Split(new char[] { '=' }, 2);

                SettingPair pair = new SettingPair(setting_split[0], setting_split[1]);

                settings.Add(pair);
            }
        }

        public static T get_setting<T>(string key)
        {
            for(int i=0; i < settings.Count; i++)
            {
                SettingPair pair = settings[i];

                if(pair.Key   == key
                && pair.Value != "")
                {
                    try
                    {
                        return (T) Convert.ChangeType(pair.Value, typeof(T));
                    }
                    catch(InvalidCastException)
                    {
                        return default;
                    }
                }
            }

            return default;
        }

        public static void set_setting(string key, object value, bool add_if_not_exist=false)
        {
            if(value == null) value = "";
            else value = value.ToString();

            List<string> settings_content = new List<string>(File.ReadAllLines(settings_file));

            for(int i=0; i < settings.Count; i++)
            {
                SettingPair setting = settings[i];

                if(setting.Key == key)
                {
                    settings_content[i] = key + "=" + value;
                    File.WriteAllLines(settings_file, settings_content.ToArray());
                    
                    load_settings();
                    return;
                }
            }

            if(add_if_not_exist)
            {
                settings_content.Add(key + "=" + value);
                File.WriteAllLines(settings_file, settings_content.ToArray());
                    
                load_settings();
                return;
            }
        }
    }
}
