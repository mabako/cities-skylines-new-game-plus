﻿using ColossalFramework.IO;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace NewGamePlus
{
    public class Configuration
    {
        public bool AllRoads = false;
        public bool AllAreas = false;

        // Unused.
        public bool FreeAreas = false;

        public bool Ships = false;
        public bool Trains = false;
        public bool Airplanes = false;
        public bool Buses = false;
        public bool Subways = false;

        public long StartMoney = 70000;

        private static string GetConfigPath()
        {
            // base it on the path Cities: Skylines uses
            string path = string.Format("{0}/{1}/", DataLocation.localApplicationData, "ModConfig");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += "new-game-plus.xm";

            return path;
        }

        public static void Serialize(Configuration config)
        {
            var serializer = new XmlSerializer(typeof(Configuration));

            using (var writer = new StreamWriter(GetConfigPath()))
            {
                serializer.Serialize(writer, config);
            }
        }

        public static Configuration Deserialize()
        {
            var serializer = new XmlSerializer(typeof(Configuration));

            try
            {
                using (var reader = new StreamReader(GetConfigPath()))
                {
                    return (Configuration)serializer.Deserialize(reader);
                }
            }
            catch { }

            return null;
        }
    }
}
