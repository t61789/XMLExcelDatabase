using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace XMLExcelDatabase
{
    public class Config
    {
        public static Config instance;

        private readonly string configPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase+"config.xml";

        private XDocument doc;

        static Config()
        {
            instance = new Config();
        }

        public Config()
        {
            doc = XDocument.Load(configPath);
        }

        public void SetUnsavedFile(string path,string hash)
        {
            doc.Root.Element("pre-path").Value = path;
            doc.Root.Element("pre-hash").Value = hash;
            doc.Save(configPath);
        }

        public bool HaveUnsavedFile()
        {
            return doc.Root.Element("pre-path").Value != "";
        }

        public string GetUnsavedFile()
        {
            return doc.Root.Element("pre-path").Value;
        }

        public string GetUnsavedFileHash()
        {
            return doc.Root.Element("pre-hash").Value;
        }

        public void ClearUnsavedFile()
        {
            doc.Root.Element("pre-path").Value = "";
            doc.Root.Element("pre-hash").Value = "";
            doc.Save(configPath);
        }
    }
}
