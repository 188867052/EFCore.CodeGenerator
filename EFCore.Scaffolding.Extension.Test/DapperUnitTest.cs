namespace EFCore.Scaffolding.Extension.Test
{
    using System;
    using EFCore.Scaffolding.Extension.Entity.Dapper;
    using EFCore.Scaffolding.Extension.Entity.Enums;
    using Entities;
    using Xunit;
    using Xunit.Abstractions;

    public class DapperUnitTest
    {
        private readonly ITestOutputHelper output;

        public DapperUnitTest(ITestOutputHelper outputHelper)
        {
            this.output = outputHelper;
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

            int count = DapperHelper.Insert(log);
            log = DapperHelper.Find<Log>(id);

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

            int count = DapperHelper.Insert(log);
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

            int count = DapperHelper.Insert(log);
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

            DapperHelper.Insert(log);
            log.UpdateTimeTicks = DateTime.Now;
            log.Message = nameof(this.Test_update_entity);
            int count = DapperHelper.Update(log);
            Assert.Equal(1, count);
        }

        [Fact]

        public void Test_update_entity_with_self_increase_PK()
        {
            var student = DapperHelper.FirstOrDefault<Student>();
            if (student != null)
            {
                student.Name = "update";
                int count = DapperHelper.Update(student);
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

            int count = DapperHelper.Insert(student);
            Assert.Equal(1, count);
        }

        [Fact]
        public void Test_delete_by_PK()
        {
            using (var dbContext = new ScaffoldingDbContext())
            {
                var student = new Student
                {
                    Name = "insert",
                    UpdateTime = DateTime.Now,
                };

                dbContext.Add(student);
                dbContext.SaveChanges();
                int count = DapperHelper.Delete<Student>(student.Id);
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public void Test_delete_by_entity()
        {
            using (var dbContext = new ScaffoldingDbContext())
            {
                var student = new Student
                {
                    Name = "insert",
                    UpdateTime = DateTime.Now,
                };

                dbContext.Add(student);
                dbContext.SaveChanges();
                int count = DapperHelper.Delete(student);
                Assert.Equal(1, count);
            }
        }
    }
}
