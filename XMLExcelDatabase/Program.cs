using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace XMLExcelDatabase
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Program().Start(args);
        }

        private bool cancel = false;

        public void Start(string[] args)
        {
            //while (true)
            //{
            //    BuffCheck();
            //    EditFile(ref args);
            //}

            if (Config.instance.HaveUnsavedFile())
                SaveFile();

            if (args.Length != 0)
            {
                string path = args[0];
                OpenFile(path);
                SaveFile();
            }

            while (true)
            {
                int command = Command("请选择操作", "选择文件", "创建文件", "清除缓存", "退出");
                string path = null;
                if (command == 1)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Filter = "XML数据库|*.xml",
                        Title = "打开XML文件"
                    };

                    DialogResult d = openFileDialog.ShowDialog();
                    if (d == DialogResult.OK)
                        path = openFileDialog.FileName;
                }
                else if (command == 2)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "XML数据库|*.xml",
                        Title = "保存XML文件"
                    };
                    DialogResult d = saveFileDialog.ShowDialog();
                    if (d == DialogResult.OK)
                    {
                        path = saveFileDialog.FileName;
                        Database.instance.CreateEmpty(path);
                    }
                }

                if (command == 1 || command == 2)
                {
                    if (path == null)
                        continue;

                    OpenFile(path);
                    SaveFile();
                }

                if (command == 3)
                {
                    Config.instance.ClearBuff();
                }
                else if (command == 4)
                {
                    return;
                }
            }
        }

        public void OpenFile(string path)
        {
            string buffName = Config.instance.GetBuffFile(path);
            if (buffName == null)
            {
                buffName = Config.instance.RecordBuffFile(path);
                Database.instance.CreateBuff(path, buffName);
            }
            Config.instance.SetUnsavedFile(path, Database.instance.GetHash(buffName));

            OpenBuff(buffName);
        }

        public void SaveFile()
        {
            (string path, string hash) = Config.instance.GetUnsavedFile();
            string buffName = Config.instance.GetBuffFile(path);
            string curHash = Database.instance.GetHash(buffName);
            if (!hash.Equals(curHash))
            {
                int command = Command("发现未保存的文件，是否保存？", "是", "否");
                if (command == 1)
                {
                    try
                    {
                        Database.instance.Save(path, buffName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.Write("\n保存失败，按任意键继续");
                        return;
                    }
                }
                else if (command == 2)
                {
                    command = Command("是否放弃未保存文件？", "是", "否");
                    if (command == 1)
                    {
                        Database.instance.CreateBuff(path, buffName);
                    }
                    else if (command == 1)
                    {
                        return;
                    }
                }
            }
            Config.instance.ClearUnsavedFile();
        }

        #region shit
        //public void EditFile(ref string[] args)
        //{
        //    string path;
        //    if (args.Length != 0)
        //    {
        //        path = args[0];
        //        args = new string[0];
        //    }
        //    else
        //    {
        //        while (true)
        //        {
        //            int command = Command("打开或新建xml数据库文件？", "打开", "新建", "退出");
        //            if (command == 1)
        //            {
        //                OpenFileDialog openFileDialog = new OpenFileDialog
        //                {
        //                    Filter = "XML数据库|*.xml",
        //                    Title = "打开XML文件"
        //                };

        //                DialogResult d = openFileDialog.ShowDialog();
        //                if (d == DialogResult.OK)
        //                {
        //                    path = openFileDialog.FileName;
        //                    break;
        //                }
        //            }
        //            else if (command == 2)
        //            {
        //                SaveFileDialog saveFileDialog = new SaveFileDialog
        //                {
        //                    Filter = "XML数据库|*.xml",
        //                    Title = "保存XML文件"
        //                };
        //                DialogResult d = saveFileDialog.ShowDialog();
        //                if (d == DialogResult.OK)
        //                {
        //                    path = saveFileDialog.FileName;
        //                    Database.instance.CreateEmpty(path);
        //                    break;
        //                }
        //            }
        //            else if (command == 3)
        //            {
        //                System.Environment.Exit(0);
        //            }
        //        }
        //    }

        //    try
        //    {
        //        Database.instance.Read(path);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        Console.WriteLine("读取失败");
        //        return;
        //    }

        //    Config.instance.SetUnsavedFile(path, Database.instance.GetHash(path));

        //    Console.WriteLine("\n等待编辑完成…………");
        //    Process p = new Process()
        //    {
        //        StartInfo = new ProcessStartInfo()
        //        {
        //            FileName = Database.instance.excelPath
        //        }
        //    };
        //    p.Start();
        //    p.WaitForExit();
        //    p.Close();
        //}

        //public void BuffCheck()
        //{
        //    if (!Config.instance.HaveUnsavedFile())
        //        return;
        //    if (Database.instance.GetHash(Config.instance.GetUnsavedFile()) == Config.instance.GetUnsavedFileHash())
        //    {
        //        Config.instance.ClearUnsavedFile();
        //        return;
        //    }

        //    int command = Command("检测到未保存的文件，是否保存？", "保存", "取消");
        //    if (command == 1)
        //    {
        //        Database.instance.Save(Config.instance.GetUnsavedFile());
        //        Config.instance.ClearUnsavedFile();
        //    }
        //    else if (command == 2)
        //    {
        //        command = Command("是否清空缓存？", "是", "否");
        //        if (command == 1)
        //        {
        //            Config.instance.ClearUnsavedFile();
        //        }
        //        else if (command == 2)
        //        {
        //            command = Command("是否编辑缓存文件？", "是", "否");
        //            if (command == 1)
        //            {
        //                Console.WriteLine("\n等待编辑完成…………");

        //                Process p = new Process()
        //                {
        //                    StartInfo = new ProcessStartInfo()
        //                    {
        //                        FileName = Database.instance.excelPath
        //                    }
        //                };
        //                p.Start();
        //                p.WaitForExit();
        //                p.Close();

        //                BuffCheck();
        //            }
        //        }
        //    }
        //}
        #endregion

        public static int Command(string discripe, params string[] selection)
        {
            while (true)
            {
                Console.WriteLine('\n' + discripe);
                for (int i = 1; i <= selection.Length; i++)
                    Console.WriteLine(i.ToString() + '.' + selection[i - 1]);
                Console.Write("command:>");
                if (int.TryParse(Console.ReadLine(), out int result))
                {
                    if (result >= 1 && result <= selection.Length)
                        return result;
                }
                Console.WriteLine();
            }
        }

        public void OpenBuff(string buffName)
        {
            buffName = Database.instance.buffDirect + buffName;
            Console.WriteLine("\n等待编辑进程结束…………");
            Process p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = buffName
                }
            };
            p.Start();
            p.WaitForExit();
            p.Close();
        }
    }
}
