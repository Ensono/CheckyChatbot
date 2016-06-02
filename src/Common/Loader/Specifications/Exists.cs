using Microsoft.WindowsAzure.Storage.Blob;
using Specifications;

namespace Checky.Common.Loader.Specifications {
    public class Exists : SpecificationBase<ICloudBlob> {
        public override bool IsSatisfiedBy(ICloudBlob instance) {
            return instance.Exists();
        }
    }
}