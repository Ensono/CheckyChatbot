using Microsoft.WindowsAzure.Storage.Blob;
using Specifications;
using Specifications.Extensions;

namespace Checky.Common.Loader.Specifications {
    internal class ExistingBlobIsWritable : SpecificationBase<ICloudBlob> {
        public override bool IsSatisfiedBy(ICloudBlob instance) {
            var exists = new Exists();
            var blockBlob = new IsBlockBlob();
            var leasable = new IsLeasable();

            return instance.Satisfies(exists.And(blockBlob).And(leasable));
        }
    }
}