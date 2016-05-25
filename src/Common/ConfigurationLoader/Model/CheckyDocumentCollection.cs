﻿using System.Collections.Generic;
using System.IO;
using ConfigurationLoader.Validator;

namespace ConfigurationLoader.Model {
    public class CheckyDocumentCollection {
        public DirectoryInfo Directory { get; set; }
        public IEnumerable<CheckyDocument> Documents { get; set; }

        public ErrorModel Validate(string collectionName) {
            var validator = new DocumentCollectionValidator();
            return validator.Validate(collectionName, this);
        }
    }
}