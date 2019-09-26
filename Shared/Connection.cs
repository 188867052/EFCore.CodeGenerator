namespace EFCore.Scaffolding.Extension
{
    public static class Connection
    {
        private const bool IsDevelop = false;
        private const string AliCloud = "Data Source=47.105.214.235;Initial Catalog=Scaffolding;Persist Security Info=True;User ID=sa;Password=931592457czA";
        private const string Local = @"Data Source=HCHENG\SQLEXPRESS;Initial Catalog=Scaffolding;Integrated Security=True";

        public static string ConnectionString
        {
            get
            {
                return IsDevelop ? Local : AliCloud;
            }
        }
    }
}
