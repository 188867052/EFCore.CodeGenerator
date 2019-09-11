using Entities;
using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace EFCore.Scaffolding.Extension.Test
{
    public class ScaffoldingUnitTest
    {
        [Fact]
        public void Test1()
        {
            DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
            var _Scaffolding = di.Parent.Parent.Parent.Parent.GetFiles("_Scaffolding.xml", SearchOption.AllDirectories).FirstOrDefault();
            var list = ScaffoldingHelper.Scaffolding("Entities", "TestDbContext", _Scaffolding.Directory.FullName);
        }

        [Fact]
        public void database_fields_spell_check()
        {
        }

        [Fact]
        public void Test2()
        {
            using (TestDbContext testContext = new TestDbContext())
            {
                testContext.Student.Add(new Student { Name = "test", Sex = "ÄÐ" });
                int count = testContext.SaveChanges();
                Assert.Equal(1, count);
            }
        }
    }
}
