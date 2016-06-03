using Microsoft.WindowsAzure.Storage.Blob;
using Specifications;

namespace Checky.Common.Loader.Specifications {
    public class IsBlockBlob : SpecificationBase<ICloudBlob> {
        public override bool IsSatisfiedBy(ICloudBlob instance) {
            return instance.BlobType == BlobType.BlockBlob;
        }
    }
}