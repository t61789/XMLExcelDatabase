using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace XMLExcelDatabase
{
    public class Database
    {
        public static Database instance;

        public readonly string buffDirect = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\buff\\";

        static Database()
        {
            instance = new Database();
        }

        public void CreateBuff(string path, string buffName)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet iSheet= workbook.CreateSheet();
            XDocument loadDoc = XDocument.Load(path);
            
            int rowCount = 0;
            IRow row = iSheet.CreateRow(rowCount);
            int count = 0;
            foreach (var item in loadDoc.Root.Element("fields").Elements())
            {
                row.CreateCell(count).SetCellValue(item.Name.ToString());
                count++;
            }

            foreach (var item in loadDoc.Root.Element("rows").Elements())
            {
                rowCount++;
                row = iSheet.CreateRow(rowCount);
                count = 0;
                foreach (var i in item.Elements())
                {
                    row.CreateCell(count).SetCellValue(i.Value);
                    count++;
                }
            }

            using(FileStream fs = new FileStream(buffDirect+buffName, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }

            workbook.Close();
        }

        public void Save(string xmlPath,string buffName)
        {
            XSSFWorkbook workbook = null;
            using (FileStream fs = new FileStream(buffDirect+ buffName, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fs);
            }
            ISheet sheet = workbook.GetSheetAt(0);
            List<string> columns = new List<string>();
            XDocument newDoc = new XDocument();
            newDoc.Add(new XElement("root"));

            XElement curEle = new XElement("fields");
            if (sheet.GetRowEnumerator().MoveNext())
                foreach (var item in sheet.GetRow(0))
                {
                    string temp = item.StringCellValue;
                    columns.Add(temp);
                    curEle.Add(new XElement(temp));
                }
            newDoc.Root.Add(curEle);

            curEle = new XElement("rows");

            bool jump = true;
            if (sheet.LastRowNum != 0)
                foreach (IRow item in sheet)
                {
                    if (jump)
                    {
                        jump = false;
                        continue;
                    }

                    XElement newRow = new XElement("row");
                    for (int i = 0; i < columns.Count; i++)
                        newRow.Add(new XElement(columns[i], item.GetCell(i)?.ToString()));
                    curEle.Add(newRow);
                }
            newDoc.Root.Add(curEle);

            workbook.Close();
            newDoc.Save(xmlPath);
        }

        public void CreateEmpty(string path)
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("root"));
            doc.Root.Add(new XElement("fields"));
            doc.Root.Add(new XElement("rows"));
            doc.Save(path);
        }
        public string GetHash(string buffName)
        {
            //string sha256;
            //SHA256Managed s = new SHA256Managed();
            //using (FileStream fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read))
            //{
            //    sha256 = BitConverter.ToString(s.ComputeHash(fs));
            //}
            //return BitConverter.ToString(s.ComputeHash(Encoding.ASCII.GetBytes(sha256 + xmlPath)));
            string sha256;
            SHA256Managed s = new SHA256Managed();
            using (FileStream fs = new FileStream(buffDirect+buffName, FileMode.Open, FileAccess.Read))
            {
                sha256 = BitConverter.ToString(s.ComputeHash(fs));
            }
            return sha256;
        }

        public void DeleteBuffFile(string buffName)
        {
            File.Delete(buffDirect+buffName);
        }
    }
}
