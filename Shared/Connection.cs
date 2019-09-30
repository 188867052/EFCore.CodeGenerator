using System;
using System.IO;
using System.Linq;

namespace EFCore.Scaffolding.Extension
{
    public static class Connection
    {
        private const bool IsDevelop = false;
        private static string aliCloud;
        private const string Local = @"Data Source=HCHENG\SQLEXPRESS;Initial Catalog=Scaffolding;Integrated Security=True";

        public static string ConnectionString
        {
            get
            {
                return IsDevelop ? Local : AliCloud;
            }
        }

        private static string AliCloud
        {
            get
            {
                if (string.IsNullOrEmpty(aliCloud))
                {
                    var di = new DirectoryInfo(Environment.CurrentDirectory);
                    string file = Directory.GetFiles(di.Parent.Parent.Parent.Parent.FullName, "ConnectionString.txt", SearchOption.AllDirectories).FirstOrDefault();
                    aliCloud = File.ReadAllText(file);
                }

                return aliCloud;
            }
        }
    }
}
