using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Aotenjo
{
    /// <summary>
    /// 模组实例类
    /// </summary>
    [Serializable]
    public class Mod
    {
        public string modID;
        public string name; //模组名称
        public string version; //模组版本
        public string author; //模组作者
        public string description; //模组描述
        public string modDir;
        public bool isFromWorkshop = false;
        public string itemUrl = "";
        public string workshopId = "";
        public Mod(string name, string version, string author, string description)
        {
            this.name = name;
            this.version = version;
            this.author = author;
            this.description = description;
        }
        
        public string GetModInfo()
        {
            return $"{name} v{version} by {author}";
        }
        
        public string GetRootDir()
        {
            return System.IO.Path.Combine(Application.persistentDataPath, "mods", name);
        }

        public static Mod LoadFromDirectory(string modDir)
        {
            string modInfoPath = System.IO.Path.Combine(modDir, "modinfo.json");
            if (!System.IO.File.Exists(modInfoPath))
            {
                Debug.LogError($"Mod info file not found in directory: {modDir}");
                return null;
            }
            string lines = System.IO.File.ReadAllText(modInfoPath);
            Mod mod = JsonConvert.DeserializeObject<Mod>(lines);
            return mod;
        }
    }
}