using System;
using System.Diagnostics;
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

        //public void Start(string[] args)
        //{
        //    if (!Config.instance.CheckUnsavedFile("", ""))
        //    {
        //        Console.Write("检测到未保存的文件，是否保存？[Y/N]");
        //        string command = Console.ReadLine();
        //        if (command == "" || command == "Y" || command == "y")
        //        {
        //            try 
        //            {
        //                Database.instance.Save(Config.instance.UnsavedFile());
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e);
        //                Console.WriteLine("保存失败，按任意键继续");
        //                Console.ReadKey();
        //                return;
        //            }

        //            Console.WriteLine("保存成功");
        //        }
        //    }
        //    string path = null;
        //    if (args.Length == 1)
        //    {
        //        path = args[0];
        //    }
        //    else
        //    {
        //        while (true)
        //        {
        //            Console.Write("打开或创建文件？[o/c]");
        //            string command = Console.ReadLine();
        //            if (command == "o")
        //            {
        //                OpenFileDialog openFileDialog = new OpenFileDialog
        //                {
        //                    Filter = "XML数据库|*.xml",
        //                    Title = "打开XML文件"
        //                };

        //                DialogResult d = openFileDialog.ShowDialog();
        //                if (d == DialogResult.OK)
        //                    path = openFileDialog.FileName;
        //                else if (d == DialogResult.Cancel)
        //                    return;
        //                break;
        //            }
        //            else if(command=="c")
        //            {
        //                SaveFileDialog saveFileDialog = new SaveFileDialog
        //                {
        //                    Filter = "XML数据库|*.xml",
        //                    Title = "保存XML文件"
        //                };
        //                DialogResult d = saveFileDialog.ShowDialog();
        //                if (d == DialogResult.OK)
        //                    path = saveFileDialog.FileName;
        //                else if (d == DialogResult.Cancel)
        //                    return;
        //                Database.instance.CreateEmpty(path);
        //                break;
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
        //        Console.WriteLine("读取失败，按任意键退出……");
        //        Console.ReadKey();
        //        return;
        //    }

        //    Console.WriteLine("等待编辑完成……");

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

        //    Console.Write("是否应用修改？[Y/N]");
        //    string command = Console.ReadLine();
        //    if (!(command == "" || command == "Y" || command == "y"))
        //        return;

        //    try 
        //    {
        //        Database.instance.Save(Config.instance.UnsavedFile());
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        Console.WriteLine("保存失败，按任意键退出……");
        //        Console.ReadKey();
        //        return;
        //    }

        //    Console.WriteLine("保存完成，按任意键退出……");
        //    Console.ReadKey();
        //}

        public void Start(string[] args)
        {
            while (true)
            {
                BuffCheck();
                EditFile(ref args);
            }
        }

        public void EditFile(ref string[] args)
        {
            string path;
            if (args.Length != 0)
            {
                path = args[0];
                args = new string[0];
            }
            else
            {
                while (true)
                {
                    int command = Command("打开或新建xml数据库文件？", "打开", "新建","退出");
                    if (command == 1)
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog
                        {
                            Filter = "XML数据库|*.xml",
                            Title = "打开XML文件"
                        };

                        DialogResult d = openFileDialog.ShowDialog();
                        if (d == DialogResult.OK)
                        {
                            path = openFileDialog.FileName;
                            break;
                        }
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
                            break;
                        }
                    }else if (command == 3)
                    {
                        System.Environment.Exit(0);
                    }
                }
            }

            try
            {
                Database.instance.Read(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("读取失败");
                return;
            }

            Config.instance.SetUnsavedFile(path, Database.instance.GetHash(path));

            Console.WriteLine("\n等待编辑完成…………");
            Process p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Database.instance.excelPath
                }
            };
            p.Start();
            p.WaitForExit();
            p.Close();
        }

        public void BuffCheck()
        {
            if (!Config.instance.HaveUnsavedFile())
                return;
            if (Database.instance.GetHash(Config.instance.GetUnsavedFile()) == Config.instance.GetUnsavedFileHash())
            {
                Config.instance.ClearUnsavedFile();
                return;
            }

            int command = Command("检测到未保存的文件，是否保存？", "保存", "取消");
            if (command == 1)
            {
                Database.instance.Save(Config.instance.GetUnsavedFile());
                Config.instance.ClearUnsavedFile();
            }
            else if (command == 2)
            {
                command = Command("是否清空缓存？", "是", "否");
                if (command == 1)
                {
                    Config.instance.ClearUnsavedFile();
                }
                else if (command == 2)
                {
                    command = Command("是否编辑缓存文件？", "是", "否");
                    if (command == 1)
                    {
                        Console.WriteLine("\n等待编辑完成…………");

                        Process p = new Process()
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = Database.instance.excelPath
                            }
                        };
                        p.Start();
                        p.WaitForExit();
                        p.Close();

                        BuffCheck();
                    }
                }
            }
        }

        public int Command(string discripe, params string[] selection)
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


    }
}
