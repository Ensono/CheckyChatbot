using Microsoft.WindowsAzure.Storage.Blob;
using Specifications;

namespace Checky.Common.Loader.Specifications {
    public class RequiresUpdate : SpecificationBase<ICloudBlob> {
        private readonly string _hash;

        public RequiresUpdate(string hash) {
            _hash = hash;
        }

        public override bool IsSatisfiedBy(ICloudBlob instance) {
            return instance.Properties.ContentMD5 != _hash;
        }
    }
}