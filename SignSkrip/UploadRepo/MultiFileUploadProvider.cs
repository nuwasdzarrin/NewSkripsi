using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http.Headers;
using System.Net.Http;

namespace SignSkrip.UploadRepo
{
    public class MultiFileUploadProvider : MultipartFormDataStreamProvider
    {
        public MultiFileUploadProvider(string rootPath) : base(rootPath) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            if (headers != null && headers.ContentDisposition != null)
            {
                return headers
                    .ContentDisposition
                    .FileName.TrimEnd('"').TrimStart('"');
            }

            return base.GetLocalFileName(headers);
        }
    }
}