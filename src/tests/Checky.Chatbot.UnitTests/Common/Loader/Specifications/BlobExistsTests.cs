using Checky.Common.Loader.Specifications;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using NUnit.Framework;
using Should;

namespace Checky.Chatbot.UnitTests.Common.Loader.Specifications {
    [TestFixture]
    public class BlobExistsTests {
        [Test]
        public void BlobDoesNotExist() {
            var mock = new Mock<ICloudBlob>();
            mock.Setup(x => x.Exists(null, null)).Returns(false);
            var spec = new Exists();
            spec.IsSatisfiedBy(mock.Object).ShouldBeFalse();
        }

        [Test]
        public void BlobExists() {
            var mock = new Mock<ICloudBlob>();
            mock.Setup(x => x.Exists(null, null)).Returns(true);
            var spec = new Exists();
            spec.IsSatisfiedBy(mock.Object).ShouldBeTrue();
        }
    }
}