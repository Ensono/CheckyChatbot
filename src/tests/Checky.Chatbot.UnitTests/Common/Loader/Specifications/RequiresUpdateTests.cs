using Checky.Common.Loader.Specifications;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using NUnit.Framework;

namespace Checky.Chatbot.UnitTests.Common.Loader.Specifications {
    [TestFixture]
    public class RequiresUpdateTests {
        [Test]
        public void RequiresUpdate() {
            var properties = new BlobProperties {ContentMD5 = "5vw74Qzy/b1nn94lYraLnw=="};
            var mock = new Mock<ICloudBlob>();
            mock.Setup(x => x.Properties).Returns(properties);
            var spec = new RequiresUpdate(properties.ContentMD5);
            spec.IsSatisfiedBy(mock.Object);
        }
    }
}