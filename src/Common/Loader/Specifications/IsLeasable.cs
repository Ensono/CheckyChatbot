using Microsoft.WindowsAzure.Storage.Blob;
using Specifications;

namespace Loader.Specifications {
    internal class IsLeasable : SpecificationBase<CloudBlob> {
        public override bool IsSatisfiedBy(CloudBlob instance) {
            return instance.Properties.LeaseStatus == LeaseStatus.Unlocked;
        }
    }
}