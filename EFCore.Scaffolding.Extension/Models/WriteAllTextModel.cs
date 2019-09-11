namespace EFCore.Scaffolding.Extension.Models
{
    internal class WriteAllTextModel
    {
        public WriteAllTextModel(string code, string path)
        {
            Code = code;
            Path = path;
        }

        public string Code { get; set; }

        public string Path { get; set; }
    }
}
