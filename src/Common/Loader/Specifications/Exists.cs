using Microsoft.WindowsAzure.Storage.Blob;
using Specifications;

namespace Loader.Specifications {
    internal class Exists : SpecificationBase<CloudBlob> {
        public override bool IsSatisfiedBy(CloudBlob instance) {
            return instance.Exists();
        }
    }
}