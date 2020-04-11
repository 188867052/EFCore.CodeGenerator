namespace EFCore.CodeGenerator.Test
{
    using System;
    using EFCore.CodeGenerator.Entity.Dapper;
    using Entities;
    using Xunit;
    using Xunit.Abstractions;

    public class DapperUnitTest
    {
        private readonly ITestOutputHelper log;

        public DapperUnitTest(ITestOutputHelper outputHelper)
        {
            log = outputHelper;
        }

        [Fact]
        public void Value_converter()
        {
            var id = Guid.NewGuid();
            var log = new Log()
            {
                Identifier = id,
                CreateTime = DateTime.Now,
                UpdateTimeTicks = DateTime.Now,
                Message = nameof(this.Value_converter),
            };

            int count = DapperExtension.Insert(log);
            log = DapperExtension.Find<Log>(id);

            Assert.Equal(1, count);
            Assert.Equal(id, log.Identifier);
        }

        [Fact]
        public void Test_insert_default_value_sql_PK_is_default()
        {
            var log = new Log()
            {
                CreateTime = DateTime.Now,
                UpdateTimeTicks = DateTime.Now,
                Message = nameof(this.Test_insert_default_value_sql_PK_is_default),
            };

            int count = DapperExtension.Insert(log);
            Assert.Equal(1, count);
        }

        [Fact]
        public void Test_insert_default_value_sql_PK_has_value()
        {
            var log = new Log()
            {
                Identifier = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                UpdateTimeTicks = DateTime.Now,
                Message = nameof(this.Test_insert_default_value_sql_PK_has_value),
            };

            int count = DapperExtension.Insert(log);
            Assert.Equal(1, count);
        }

        [Fact]
        public void Test_update_entity()
        {
            var log = new Log()
            {
                Identifier = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                UpdateTimeTicks = DateTime.Now,
                Message = nameof(this.Test_insert_default_value_sql_PK_has_value),
            };

            DapperExtension.Insert(log);
            log.UpdateTimeTicks = DateTime.Now;
            log.Message = nameof(this.Test_update_entity);
            int count = DapperExtension.Update(log);
            Assert.Equal(1, count);
        }

        [Fact]

        public void Test_update_entity_with_self_increase_PK()
        {
            var student = DapperExtension.FirstOrDefault<Student>();
            if (student != null)
            {
                student.Name = "update";
                int count = DapperExtension.Update(student);
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public void Test_entity_has_insert_self_increase_PK()
        {
            var student = new Student
            {
                Name = "test",
                Sex = SexEnum.Male,
                Mobile = "123456789",
                IsDeleted = true,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
            };

            int count = DapperExtension.Insert(student);
            Assert.Equal(1, count);
        }

        [Fact]
        public void Test_delete_by_PK()
        {
            using var dbContext = new ScaffoldingDbContext();
            var student = new Student
            {
                Name = "insert",
                UpdateTime = DateTime.Now,
            };

            dbContext.Add(student);
            dbContext.SaveChanges();
            int count = DapperExtension.Delete<Student>(student.Id);
            Assert.Equal(1, count);
        }

        [Fact]
        public void Test_delete_by_entity()
        {
            using var dbContext = new ScaffoldingDbContext();
            var student = new Student
            {
                Name = "insert",
                UpdateTime = DateTime.Now,
            };

            dbContext.Add(student);
            dbContext.SaveChanges();
            int count = DapperExtension.Delete(student);
            Assert.Equal(1, count);
        }

        [Fact]
        public void Test_BoolToZeroOneConverter()
        {
            var course = new Course
            {
                Name = "insert",
                UpdateTime = DateTime.Now,
                CreateTime = DateTime.Now,
                IsDeleted = true,
            };

            int count = DapperExtension.Insert(course);
            Assert.Equal(1, count);

            course = DapperExtension.FirstOrDefault<Course>();
        }

        [Fact]
        public void Test_DateTimeToTicksConverter()
        {
            var log = new Log
            {
                Message = nameof(this.Test_DateTimeToTicksConverter),
                UpdateTimeTicks = DateTime.Now,
            };

            int count = DapperExtension.Insert(log);
            Assert.Equal(1, count);

            log = DapperExtension.FirstOrDefault<Log>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("http://www.google.com/")]
        [InlineData("https://www.google.com/")]
        public void Test_UriToStringConverter(string url)
        {
            var log = new Log
            {
                Message = url,
                UpdateTimeTicks = DateTime.Now,
                Url = url == null ? null : new Uri(url),
            };

            int count = DapperExtension.Insert(log);
            Assert.Equal(1, count);

            log = DapperExtension.FirstOrDefault<Log>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("Fake Uri")]
        [InlineData("www.google.com")]
        public void Test_UriToStringConverter_throws(string url)
        {
            Assert.Throws<UriFormatException>(() =>
           {
               var log = new Log
               {
                   Message = url,
                   UpdateTimeTicks = DateTime.Now,
                   Url = new Uri(url),
               };
               DapperExtension.Insert(log);
           });
        }

        [Fact]
        public void Test_select_from_view()
        {
            var log = DapperExtension.FirstOrDefault<VLog>();
        }
    }
}
