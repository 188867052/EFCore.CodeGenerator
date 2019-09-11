using ReleaseManage.ControllerHelper.Scaffolding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestNamespace;
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
            var list = ScaffoldingHelper.Scaffolding("TestNamespace", "TestContextName", _Scaffolding.Directory.FullName);
        }


        [Fact]
        public void Test2()
        {
            TestContextName testContext = new TestContextName();
            testContext.Person.Add(new Person { Name = "test", Sex = "ÄÐ" });
            int count = testContext.SaveChanges();
            Assert.Equal(1, count);
        }
    }
}
