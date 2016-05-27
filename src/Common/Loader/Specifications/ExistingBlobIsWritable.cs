using Microsoft.WindowsAzure.Storage.Blob;
using Specifications;
using Specifications.Extensions;

namespace Loader.Specifications {
    internal class ExistingBlobIsWritable : SpecificationBase<CloudBlob> {
        public override bool IsSatisfiedBy(CloudBlob instance) {
            var exists = new Exists();
            var blockBlob = new IsBlockBlob();
            var leasable = new IsLeasable();

            return instance.Satisfies(exists.And(blockBlob).And(leasable));
        }
    }
}