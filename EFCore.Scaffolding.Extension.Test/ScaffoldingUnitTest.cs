namespace EFCore.Scaffolding.Extension.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Dapper;
    using EFCore.Scaffolding.Extension.Entity.Dapper;
    using EFCore.Scaffolding.Extension.Entity.Enums;
    using Entities;
    using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
    using WeCantSpell.Hunspell;
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Extensions.Ordering;

    public class ScaffoldingUnitTest
    {
        private static WordList wordList;
        private readonly ITestOutputHelper log;

        public ScaffoldingUnitTest(ITestOutputHelper outputHelper)
        {
            this.log = outputHelper;
        }

        [Fact]
        [Order(1)]
        public void Run_database_script()
        {
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
            var scaffoldingFile = di.Parent.Parent.Parent.Parent.GetFiles(".sql", SearchOption.AllDirectories).FirstOrDefault();
            string sql = File.ReadAllText(scaffoldingFile.FullName);
            if (!string.IsNullOrEmpty(sql))
            {
                int count = DapperExtension.Connection.Execute(sql);
                Assert.Equal(count, -1);
            }
        }

        [Fact]
        [Order(2)]
        public void Generate_entities_and_DBContext()
        {
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
            var scaffoldingFile = di.Parent.Parent.Parent.Parent.GetFiles(".Scaffolding.xml", SearchOption.AllDirectories).FirstOrDefault();
            var list = ScaffoldingHelper.Scaffolding("Entities", "ScaffoldingDbContext", scaffoldingFile.Directory.FullName);

            this.log.WriteLine(string.Join(Environment.NewLine, list));
        }

        [Fact]
        public void Database_check_all_table_must_has_primary_key()
        {
            foreach (var table in DbContextGenerator.DatabaseModel.Tables)
            {
                if (table.GetType() != typeof(DatabaseView))
                {
                    Assert.NotEmpty(table.PrimaryKey.Columns);
                }
            }
        }

        [Fact]
        public void Database_check_all_table_primary_key_at_the_top()
        {
            foreach (var table in DbContextGenerator.DatabaseModel.Tables)
            {
                if (table.GetType() != typeof(DatabaseView))
                {
                    foreach (var column in table.Columns)
                    {
                        var index = table.Columns.IndexOf(column);
                        if (index < table.PrimaryKey.Columns.Count)
                        {
                            Assert.Contains(column, table.PrimaryKey.Columns);
                        }
                        else
                        {
                            Assert.DoesNotContain(column, table.PrimaryKey.Columns);
                        }
                    }
                }
            }
        }

        [Fact]
        public void Database_check_typo()
        {
            bool isSuccess = true;
            foreach (var table in DbContextGenerator.DatabaseModel.Tables)
            {
                var tableNameSuggests = this.FieldSpellCheckAndReturnSuggestionsWhenHasTypo(table.Name);
                isSuccess &= tableNameSuggests.Count == 0;
                if (tableNameSuggests.Count != 0)
                {
                    this.log.WriteLine($"Typo: Table Name: {table.Name}.");
                    this.log.WriteLine("Suggestions:");
                    this.log.WriteLine($"{string.Join(Environment.NewLine, tableNameSuggests.Where(o => !o.Contains("-")).Select(o => "     " + o.Replace(" ", "_").ToLower()))}");
                    this.log.WriteLine(new string('-', 30));
                }

                foreach (var column in table.Columns)
                {
                    var columnSuggests = this.FieldSpellCheckAndReturnSuggestionsWhenHasTypo(column.Name);
                    isSuccess &= columnSuggests.Count == 0;
                    if (columnSuggests.Count != 0)
                    {
                        this.log.WriteLine($"Typo: Table Name: {table.Name}, Column: {column.Name}.");
                        this.log.WriteLine("Suggestions:");
                        this.log.WriteLine($"{string.Join(Environment.NewLine, columnSuggests.Where(o => !o.Contains("-")).Select(o => "     " + o.Replace(" ", "_").ToLower()))}");
                        this.log.WriteLine(new string('-', 30));
                    }
                }

                Assert.True(isSuccess);
            }
        }

        [Fact]
        public void Test_insert_entity()
        {
            using var testContext = new ScaffoldingDbContext();
            var entity = new Student
            {
                Name = "test",
                Sex = SexEnum.Male,
                Mobile = "123456789",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
            };
            testContext.Student.Add(entity);
            int count = testContext.SaveChanges();
            Assert.Equal(1, count);
        }

        private WordList GetWordList()
        {
            if (wordList == null)
            {
                var directory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent;
                var file = directory.GetFiles("en_US.dic").First();
                wordList = WordList.CreateFromFiles(file.FullName);
            }

            return wordList;
        }

        private List<string> FieldSpellCheckAndReturnSuggestionsWhenHasTypo(string filed)
        {
            var dictionary = this.GetWordList();
            var valuesToCheck = filed.Split('_');
            bool ok = true;
            var suggests = new List<string>();
            foreach (var item in valuesToCheck)
            {
                var isItemOk = dictionary.Check(item);
                ok &= isItemOk;
                if (!isItemOk)
                {
                    suggests = suggests.Concat(dictionary.Suggest(item)).ToList();
                }
            }

            return suggests;
        }
    }
}
