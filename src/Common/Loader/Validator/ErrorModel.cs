using System.Collections.Generic;
using System.Linq;

namespace Checky.Common.Loader.Validator {
    public class ErrorModel {
        public bool IsValid { get; set; }
        public IEnumerable<string> Errors { get; set; }

        public static ErrorModel FromErrorMessage(string message) {
            return new ErrorModel {
                IsValid = false,
                Errors = new[] {message}
            };
        }

        public static ErrorModel FromErrorModels(IEnumerable<ErrorModel> errorRecords) {
            var errorModels = errorRecords as ErrorModel[] ?? errorRecords.ToArray();
            return new ErrorModel {
                IsValid = errorModels.All(x => x.IsValid),
                Errors = errorModels.Where(x => !x.IsValid).SelectMany(x => x.Errors)
            };
        }

        internal static ErrorModel FromErrorModels(ErrorModel lhs, ErrorModel rhs) {
            return FromErrorModels(new[] {rhs, lhs});
        }

        public static ErrorModel Valid() {
            return new ErrorModel {
                IsValid = true
            };
        }
    }
}