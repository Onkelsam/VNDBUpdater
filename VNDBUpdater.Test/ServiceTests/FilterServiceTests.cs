using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.Database.Interfaces;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.Services.Filters;

namespace VNDBUpdater.Test.ServiceTests
{
    [TestFixture]
    public class FilterServiceTests
    {
        private Mock<IFilterRepository> _FilterRepositoryMock;
        private FilterService _ServiceUnderTest;

        private static List<TagModel> _TestTagModels = new List<TagModel>
        {
            { new TagModel(new TagEntity { ID = 1 }) },
            { new TagModel(new TagEntity { ID = 3 }) },
        };

        public static IEnumerable<TestCaseData> VNShouldBeFilteredOutTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new Dictionary<FilterModel.BooleanOperations, List<TagModel>>
                    {
                        { FilterModel.BooleanOperations.AND, _TestTagModels }
                    },
                    new List<int>
                    {
                        1, 3
                    },
                    false
                );
                yield return new TestCaseData(
                    new Dictionary<FilterModel.BooleanOperations, List<TagModel>>
                    {
                        { FilterModel.BooleanOperations.OR, _TestTagModels }
                    },
                    new List<int>
                    {
                        1
                    },
                    false
                );
                yield return new TestCaseData(
                    new Dictionary<FilterModel.BooleanOperations, List<TagModel>>
                    {
                        { FilterModel.BooleanOperations.NOT, _TestTagModels }
                    },
                    new List<int>
                    {
                        3
                    },
                    true
               );                    
            }
        }
        
        [TestCaseSource(nameof(VNShouldBeFilteredOutTestCases))]
        public void VNShouldBeFilteredOut_FiltersCorrectly(Dictionary<FilterModel.BooleanOperations, List<TagModel>> filter, List<int> tags, bool expectedResult)
        {
            _FilterRepositoryMock = new Mock<IFilterRepository>(MockBehavior.Strict);

            _ServiceUnderTest = new FilterService(_FilterRepositoryMock.Object);

            var result = _ServiceUnderTest.VNShouldBeFilteredOut(filter, tags);

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
