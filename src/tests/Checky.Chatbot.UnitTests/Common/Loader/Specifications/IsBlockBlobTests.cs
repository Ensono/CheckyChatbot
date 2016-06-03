using Checky.Common.Loader.Specifications;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using NUnit.Framework;
using Should;

namespace Checky.Chatbot.UnitTests.Common.Loader.Specifications {
    [TestFixture]
    public class IsBlockBlobTests {
        [Test]
        public void IsBlockBlob() {
            var mock = new Mock<ICloudBlob>();
            mock.Setup(x => x.BlobType).Returns(BlobType.BlockBlob);
            var spec = new IsBlockBlob();
            spec.IsSatisfiedBy(mock.Object).ShouldBeTrue();
        }

        [Test]
        [TestCase(BlobType.AppendBlob)]
        [TestCase(BlobType.PageBlob)]
        [TestCase(BlobType.Unspecified)]
        public void IsNotBlockBlob(BlobType blobType) {
            var mock = new Mock<ICloudBlob>();
            mock.Setup(x => x.BlobType).Returns(blobType);
            var spec = new IsBlockBlob();
            spec.IsSatisfiedBy(mock.Object).ShouldBeFalse();
        }
    }
}