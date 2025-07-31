//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using eID.PJS.Services.Entities;
//using eID.PJS.Services.Verification;

//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;

//using Moq;

//using OpenSearch.Client;

//namespace eID.PJS.Services.Tests
//{
//    public class ExclusionProviderTests : ServiceTestBase
//    {

//        private const string GUID1 = "00000000-0000-0000-0000-000000000001";
//        private const string GUID2 = "00000000-0000-0000-0000-000000000002";
//        private const string GUID3 = "00000000-0000-0000-0000-000000000003";
//        private const string GUID4 = "00000000-0000-0000-0000-000000000004";
//        public ExclusionProviderTests() : base()
//        {

//        }

//        private IVerificationExclusionProvider NewProvider()
//        {
//            var builder = new DbContextOptionsBuilder<VerificationExclusionsDbContext>();
//            builder.UseNpgsql(_config!.GetConnectionString("DefaultConnection")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

//            var context = new VerificationExclusionsDbContext(builder.Options);
            
//            return new PostgresExclusionProvider(context);
//        }



//        private void CreateSampleData()
//        {
//            var p = NewProvider();

//            p.RemoveAll();

//            p.Add(new FileORFolderExclusion
//            {
//                CreatedBy = "test",
//                DateCreated = DateTime.UtcNow,
//                ExcludedPath = "audit",
//                Id = Guid.NewGuid(),
//            });

//            p.Add(new FileORFolderExclusion
//            {
//                CreatedBy = "test",
//                DateCreated = DateTime.UtcNow,
//                ExcludedPath = "audit\\sub-test",
//                Id = Guid.NewGuid(),
//            });

//            p.Add(new FileORFolderExclusion
//            {
//                CreatedBy = "test",
//                DateCreated = DateTime.UtcNow,
//                ExcludedPath = "audit\\sub-test\\sub-sub-test",
//                Id = Guid.NewGuid(),
//            });

//            p.Add(new DateRangeExclusion
//            {
//                CreatedBy = "test",
//                DateCreated = DateTime.UtcNow,
//                Id = Guid.NewGuid(),
//                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
//                EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
//            });

//            p.Add(new DateRangeExclusion
//            {
//                CreatedBy = "test",
//                DateCreated = DateTime.UtcNow,
//                Id = Guid.NewGuid(),
//                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
//                EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
//            });

//            p.Add(new DateRangeExclusion
//            {
//                CreatedBy = "test",
//                DateCreated = DateTime.UtcNow,
//                Id = Guid.NewGuid(),
//                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
//                EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
//            });

//            p.Add(new FileORFolderExclusion
//            {
//                CreatedBy = "test",
//                DateCreated = DateTime.UtcNow,
//                ExcludedPath = "audit\\test",
//                Id = Guid.Parse(GUID1),
//            });

//        }

//        private void CreateRandomData(int numRecords)
//        {

//            var p = NewProvider();

//            p.RemoveAll();

//            for (int i = 0; i < numRecords; i++)
//            {
//                p.Add(new DateRangeExclusion
//                {
//                    CreatedBy = "test",
//                    DateCreated = DateTime.UtcNow,
//                    Id = Guid.NewGuid(),
//                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
//                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
//                });

//                p.Add(new FileORFolderExclusion
//                {
//                    CreatedBy = "test",
//                    DateCreated = DateTime.UtcNow,
//                    ExcludedPath = "test\\sub-test\\sub-sub-test",
//                    Id = Guid.NewGuid(),
//                });
//            }

//        }

     
//        //[Fact]
//        public void AddAndGetTest()
//        {
//            var _provider = NewProvider();

//            var exFile = new FileORFolderExclusion
//            {
//                CreatedBy = "test",
//                DateCreated = DateTime.UtcNow,
//                ExcludedPath = "test",
//                Id = Guid.NewGuid(),
//            };

//            var exDate = new DateRangeExclusion
//            {
//                CreatedBy = "test",
//                DateCreated = DateTime.UtcNow,
//                Id = Guid.NewGuid(),
//                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
//                EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
//            };

//            var result1 = _provider.Add(exFile);
//            var result2 = _provider.Add(exDate);

//            Assert.Null(result1);
//            Assert.Null(result2);

//            Assert.NotNull(_provider.Get(exFile.Id));
//            Assert.NotNull(_provider.Get(exDate.Id));
//        }

//        //[Fact]
//        public void CountTest()
//        {
//            CreateSampleData();

//            var _provider = NewProvider();

//            Assert.True(_provider.Count() == 7);
//        }

//        //[Fact]
//        public void PerformanceLoadTest()
//        {
//            var metric = new SimplePerformanceMetrics();

//            SystemExtensions.MeasureCodeExecution(() =>
//            {
//                CreateRandomData(1000);

//                var _provider = NewProvider();

//                Assert.True(_provider.Count() == 2000);

//            }, metric);


//        }

//        //[Fact]
//        public void GetByIdTest()
//        {

//            CreateSampleData();

//            var _provider = NewProvider();

//            var item = _provider.Get(Guid.Parse(GUID1));

//            Assert.NotNull(item);
//        }

//        //[Fact]
//        public void GetAllTest()
//        {
//            CreateRandomData(1000);
//            var _provider = NewProvider();

//            var items = _provider.GetAll();

//            Assert.NotNull(items);
//            Assert.Equal(2000, items.Count());

//        }


//        //[Fact]
//        public void IsDateExcluded()
//        {
//            CreateSampleData();
//            var _provider = NewProvider();

//            var excluded = _provider.IsExcluded(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3)));
//            var notExcluded = _provider.IsExcluded(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)));


//            Assert.True(excluded);
//            Assert.False(notExcluded);

//        }

//        //[Fact]
//        public void IsPathExcludedTest()
//        {
//            CreateSampleData();

//            var _provider = NewProvider();

//            Assert.True(_provider.IsExcluded("audit\\test"));
//            Assert.True(_provider.IsExcluded("audit"));
//            Assert.True(_provider.IsExcluded("audit\\test\\test2\\test3"));

//            Assert.False(_provider.IsExcluded("audit2"));
//            Assert.False(_provider.IsExcluded("audit2\\test"));
//            Assert.False(_provider.IsExcluded("root\\audit"));


//        }



//    }
//}
