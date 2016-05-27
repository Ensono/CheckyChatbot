using Microsoft.WindowsAzure.Storage.Blob;
using Specifications;

namespace Loader.Specifications {
    internal class IsBlockBlob : SpecificationBase<CloudBlob> {
        public override bool IsSatisfiedBy(CloudBlob instance) {
            return instance.BlobType == BlobType.BlockBlob;
        }
    }
}