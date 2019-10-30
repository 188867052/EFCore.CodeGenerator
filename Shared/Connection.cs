using System;
using System.IO;
using System.Linq;

namespace EFCore.Scaffolding.Extension
{
    public static class Connection
    {
        private static string aliCloud;
        private const string LocalConnectionString = @"Data Source=.;Initial Catalog=Scaffolding;Integrated Security=True";

        public static string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(aliCloud))
                {
                    return aliCloud;
                }

                string filePath = GetPipelineFile();
                bool isAzure = !string.IsNullOrEmpty(filePath);
                if (isAzure)
                {
                    aliCloud = File.ReadAllText(filePath);
                    return aliCloud;
                }
                else
                {
                    return LocalConnectionString;
                }
            }
        }

        private static string GetPipelineFile()
        {
            var di = new DirectoryInfo(Environment.CurrentDirectory);
            return Directory.GetFiles(di.Parent.Parent.Parent.Parent.Parent.FullName, "ConnectionString.txt", SearchOption.AllDirectories).FirstOrDefault();
        }
    }
}
