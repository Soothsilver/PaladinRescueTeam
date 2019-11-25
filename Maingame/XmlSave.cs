using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Cother;
using Newtonsoft.Json;

namespace Origin.Characters
{
    internal class XmlSave
    {
        static XmlSave()
        {
            settingsFileName = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppDomain.CurrentDomain.FriendlyName,
                "ApplicationWideSettings.json");
            Directory.CreateDirectory(path: Path.GetDirectoryName(settingsFileName));
        }

    
        private static readonly string settingsFileName;


        public static Treasure LoadSettings()
        {
            if (!File.Exists(settingsFileName))
                return new Treasure();
            try
            {
                return JsonConvert.DeserializeObject<Treasure>(File.ReadAllText(settingsFileName));
            }
            catch (Exception)
            {
                return new Treasure();
            }
        }
        public static void SaveSettings(Treasure container)
        {
            try
            {
                File.WriteAllText(settingsFileName, JsonConvert.SerializeObject(container));
            }
            catch (Exception)
            {
            }
        }
    }
}

