using Microsoft.WindowsAzure.Storage.Blob;
using Specifications;
using Specifications.Extensions;

namespace Checky.Common.Loader.Specifications {
    public class IsLeasable : SpecificationBase<ICloudBlob> {
        public override bool IsSatisfiedBy(ICloudBlob instance) {
            return instance.Properties.LeaseStatus.Satisfies<IsLeasableEnum>();
        }
    }

    public class IsLeasableEnum : SpecificationBase<LeaseStatus> {
        public override bool IsSatisfiedBy(LeaseStatus instance) {
            return instance == LeaseStatus.Unlocked;
        }
    }
}