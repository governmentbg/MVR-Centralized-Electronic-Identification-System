using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using eID.PJS.Services.Verification;

namespace eID.PJS.Services.Tests
{
    public class ExclusionsTests
    {

        private const string GUID1 = "00000000-0000-0000-0000-000000000001";
        private const string GUID2 = "00000000-0000-0000-0000-000000000002";
        private const string GUID3 = "00000000-0000-0000-0000-000000000003";
        private const string GUID4 = "00000000-0000-0000-0000-000000000004";

        public ExclusionsTests()
        { 

        }
        private Exclusions CreateSampleData()
        {
            var p = new List<VerificationExclusion>();

            p.Add(new FileORFolderExclusion
            {
                CreatedBy = "test",
                DateCreated = DateTime.UtcNow,
                ExcludedPath = "audit",
                ReasonForExclusion = "reason",
                Id = Guid.NewGuid(),
            });

            p.Add(new FileORFolderExclusion
            {
                CreatedBy = "test",
                DateCreated = DateTime.UtcNow,
                ExcludedPath = "audit\\sub-test",
                ReasonForExclusion = "reason",
                Id = Guid.NewGuid(),
            });

            p.Add(new FileORFolderExclusion
            {
                CreatedBy = "test",
                DateCreated = DateTime.UtcNow,
                ExcludedPath = "audit\\sub-test\\sub-sub-test",
                ReasonForExclusion = "reason",
                Id = Guid.NewGuid(),
            });

            p.Add(new DateRangeExclusion
            {
                CreatedBy = "test",
                DateCreated = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ReasonForExclusion = "reason"
            });

            p.Add(new DateRangeExclusion
            {
                CreatedBy = "test",
                DateCreated = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ReasonForExclusion = "reason"
            });

            p.Add(new DateRangeExclusion
            {
                CreatedBy = "test",
                DateCreated = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ReasonForExclusion = "reason"
            });

            p.Add(new FileORFolderExclusion
            {
                CreatedBy = "test",
                DateCreated = DateTime.UtcNow,
                ExcludedPath = "audit\\test",
                Id = Guid.Parse(GUID1),
                ReasonForExclusion = "reason"
            });

            return new Exclusions(p);
        }

        private Exclusions CreateRandomData(int numRecords)
        {

            var p = new List<VerificationExclusion>();

            for (int i = 0; i < numRecords; i++)
            {
                p.Add(new DateRangeExclusion
                {
                    CreatedBy = "test",
                    DateCreated = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    ReasonForExclusion = "reason",
                });

                p.Add(new FileORFolderExclusion
                {
                    CreatedBy = "test",
                    DateCreated = DateTime.UtcNow,
                    ExcludedPath = "test\\sub-test\\sub-sub-test",
                    Id = Guid.NewGuid(),
                    ReasonForExclusion = "reason",
                });
            }

            return new Exclusions(p);
        }

        //[Fact]
        public void GetByIdTest()
        {

            CreateSampleData();

            var _provider = CreateSampleData();

            var item = _provider.Get(Guid.Parse(GUID1));

            Assert.NotNull(item);
        }

        //[Fact]
        public void IsDateExcluded()
        {
            var _provider = CreateSampleData();

            var excluded = _provider.IsExcluded(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3)));
            var notExcluded = _provider.IsExcluded(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)));


            Assert.True(excluded);
            Assert.False(notExcluded);

        }

        //[Fact]
        public void IsPathExcludedTest()
        {
            var _provider = CreateSampleData();

            Assert.True(_provider.IsExcluded("audit\\test"));
            Assert.True(_provider.IsExcluded("audit\\test.audit"));
            Assert.True(_provider.IsExcluded("audit\\test\\test.audit"));
            Assert.True(_provider.IsExcluded("audit"));
            Assert.True(_provider.IsExcluded("audit\\test\\test2\\test3"));

            Assert.False(_provider.IsExcluded("audit2"));
            Assert.False(_provider.IsExcluded("audit2\\test.audit"));
            Assert.False(_provider.IsExcluded("audit2\\test"));
            Assert.False(_provider.IsExcluded("audit2\\test\\test.audit"));
            Assert.False(_provider.IsExcluded("root\\audit"));


        }
    }
}
