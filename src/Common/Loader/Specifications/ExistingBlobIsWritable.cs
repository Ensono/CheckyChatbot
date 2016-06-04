using Microsoft.WindowsAzure.Storage.Blob;
using Specifications;

namespace Checky.Common.Loader.Specifications {
    public class ExistingBlobIsWritable : SpecificationBase<ICloudBlob> {
        private readonly ISpecification<ICloudBlob> _blockBlob;
        private readonly ISpecification<ICloudBlob> _exists;
        private readonly ISpecification<ICloudBlob> _leasable;

        public ExistingBlobIsWritable(ISpecification<ICloudBlob> exists = null,
                                      ISpecification<ICloudBlob> blockBlob = null,
                                      ISpecification<ICloudBlob> leasable = null) {
            _exists = exists ?? new Exists();
            _blockBlob = blockBlob ?? new IsBlockBlob();
            _leasable = leasable ?? new IsLeasable();
        }

        public override bool IsSatisfiedBy(ICloudBlob instance) {
            return _exists.And(_blockBlob).And(_leasable).IsSatisfiedBy(instance);
        }
    }
}