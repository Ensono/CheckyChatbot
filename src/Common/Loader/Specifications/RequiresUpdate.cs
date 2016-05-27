using Microsoft.WindowsAzure.Storage.Blob;
using Specifications;

namespace Loader.Specifications {
    internal class RequiresUpdate : SpecificationBase<CloudBlob> {
        private readonly string _hash;

        public RequiresUpdate(string hash) {
            _hash = hash;
        }

        public override bool IsSatisfiedBy(CloudBlob instance) {
            return instance.Properties.ContentMD5 != _hash;
        }
    }
}