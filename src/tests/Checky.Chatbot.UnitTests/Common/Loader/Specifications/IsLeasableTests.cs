using Checky.Common.Loader.Specifications;
using Microsoft.WindowsAzure.Storage.Blob;
using NUnit.Framework;
using Should;

namespace Checky.Chatbot.UnitTests.Common.Loader.Specifications {
    [TestFixture]
    public class IsLeasableTests {
        [Test]
        public void IsLeasable() {
            var spec = new IsLeasableEnum();
            spec.IsSatisfiedBy(LeaseStatus.Unlocked).ShouldBeTrue();
        }

        [Test]
        [TestCase(LeaseStatus.Locked)]
        [TestCase(LeaseStatus.Unspecified)]
        public void IsNotLeasable(LeaseStatus leaseStatus) {
            var spec = new IsLeasableEnum();
            spec.IsSatisfiedBy(leaseStatus).ShouldBeFalse();
        }
    }
}