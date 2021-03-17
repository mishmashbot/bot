using System;
using System.IO;

namespace Ollio.Plugin
{
    public class UploadFile : Ollio.Common.Models.UploadFile {
        public UploadFile(Stream content) : base(content) { }
        public UploadFile(Uri uri) : base(uri) { }
        public UploadFile(string value) : base(value) { }
    }
}