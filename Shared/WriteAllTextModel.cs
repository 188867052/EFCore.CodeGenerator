namespace EFCore.CodeGenerator
{
    internal class WriteAllTextModel
    {
        public WriteAllTextModel(string code, string path)
        {
            this.Code = code;
            this.Path = path;
        }

        public string Code { get; set; }

        public string Path { get; set; }
    }
}
