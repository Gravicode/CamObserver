using IniFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamObserver.Display.Helpers
{
    public class AppConfig
    {
        public Ini MyConfig { get; set; }
        public string ConfigPath { get; set; }
        public AppConfig()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);

            ConfigPath = System.IO.Path.Join(path, "/cam-observer");
            if (!Directory.Exists(ConfigPath))
                Directory.CreateDirectory(ConfigPath);
            ConfigPath = System.IO.Path.Join(ConfigPath, "/config.ini");
            if(!File.Exists(ConfigPath))
            {
                MyConfig = new Ini
                {
                    new Section("Coords")
                };
                MyConfig["Coords"].Add(new Property("SelectionArea", "0,0,0,0"));
                MyConfig.SaveTo(ConfigPath);
            }
            else
            {
                var StrIni = File.ReadAllText(ConfigPath);
                MyConfig = Ini.Load(StrIni);
            }
        }

        public void Save()
        {
            MyConfig.SaveTo(ConfigPath);
        }
    }
}
