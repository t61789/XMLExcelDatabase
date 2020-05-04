using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using System.IO;

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
            doc.Root.Element("unsaved-file").Element("path").Value = path;
            doc.Root.Element("unsaved-file").Element("hash").Value = hash;
            doc.Save(configPath);
        }

        public bool HaveUnsavedFile()
        {
            //return doc.Root.Element("pre-path").Value != "";

            return doc.Root.Element("unsaved-file").Element("path").Value != "";
        }

        //public string GetUnsavedFile()
        //{
        //    return doc.Root.Element("pre-path").Value;
        //}

        //public string GetUnsavedFileHash()
        //{
        //    return doc.Root.Element("pre-hash").Value;
        //}

        public void ClearUnsavedFile()
        {
            //doc.Root.Element("pre-path").Value = "";
            //doc.Root.Element("pre-hash").Value = "";
            //doc.Save(configPath);

            doc.Root.Element("unsaved-file").Element("path").Value = "";
            doc.Root.Element("unsaved-file").Element("hash").Value = "";
            doc.Save(configPath);
        }

        public (string path,string hash) GetUnsavedFile()
        {
            XElement x = doc.Root.Element("unsaved-file");
            return (x.Element("path").Value, x.Element("hash").Value);
        }

        public string GetBuffFile(string path)
        {
            return doc.Root.Element("buff-file").Elements().Where(x => x.Element("path").Value == path).FirstOrDefault()?.Element("buff").Value;
        }

        public string RecordBuffFile(string path)
        {
            string buffName = Guid.NewGuid().ToString("N")+".xlsx";
            XElement newNode = new XElement("file");
            newNode.Add(new XElement("path") { Value = path});
            newNode.Add(new XElement("buff") { Value = buffName });
            doc.Root.Element("buff-file").Add(newNode);
            doc.Save(configPath);
            return buffName;
        }

        public void ClearBuff()
        {
            foreach (var item in Directory.GetFiles(Database.instance.buffDirect))
                Database.instance.DeleteBuffFile(Path.GetFileName(item));
            doc.Root.Element("buff-file").RemoveAll();
            doc.Save(configPath);
        }
    }
}
