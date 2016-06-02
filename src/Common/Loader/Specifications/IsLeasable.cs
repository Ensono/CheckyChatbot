using Microsoft.WindowsAzure.Storage.Blob;
using Specifications;

namespace Checky.Common.Loader.Specifications {
    internal class IsLeasable : SpecificationBase<ICloudBlob> {
        public override bool IsSatisfiedBy(ICloudBlob instance) {
            return instance.Properties.LeaseStatus == LeaseStatus.Unlocked;
        }
    }
}