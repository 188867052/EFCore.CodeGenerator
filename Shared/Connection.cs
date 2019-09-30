using System;
using System.IO;
using System.Linq;

namespace EFCore.Scaffolding.Extension
{
    public static class Connection
    {
        private static string aliCloud;
        private const string LocalConnectionString = @"Data Source=HCHENG\SQLEXPRESS;Initial Catalog=Scaffolding;Integrated Security=True";

        public static string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(aliCloud))
                {
                    return aliCloud;
                }

                string filePath = GetPipelineFile();
                bool isAzure = string.IsNullOrEmpty(filePath);
                if (isAzure)
                {
                    if (string.IsNullOrEmpty(aliCloud))
                    {
                        aliCloud = File.ReadAllText(filePath);
                    }
                }
                else
                {
                    return LocalConnectionString;
                }

                return aliCloud;
            }
        }

        private static string GetPipelineFile()
        {
            var di = new DirectoryInfo(Environment.CurrentDirectory);
            return Directory.GetFiles(di.Parent.Parent.Parent.Parent.FullName, "ConnectionString.txt", SearchOption.AllDirectories).FirstOrDefault();
        }
    }
}
