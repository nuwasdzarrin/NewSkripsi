using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using SignSkrip.UploadRepo;
using SignData;
using System.Web.Http.Cors;

namespace SignSkrip.Controllers
{
    [EnableCorsAttribute("http://localhost:8080", "*", "GET, POST, PUT, DELETE")]
    public class UploadPDFController : ApiController
    {
        /*private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }*/

        /*public static string ResolveServerUrl(string serverUrl, bool forceHttps)
        {
            if (serverUrl.IndexOf("://") > -1)
                return serverUrl;

            string newUrl = serverUrl;
            Uri originalUri = System.Web.HttpContext.Current.Request.Url;
            newUrl = (forceHttps ? "https" : originalUri.Scheme) +
                "://" + originalUri.Authority + newUrl;
            return newUrl;
        }

        public string get()
        {
            var url = ResolveServerUrl(VirtualPathUtility.ToAbsolute("~/UploadFile/input/ForCekTandaTangan.pdf"),false);
            return url;
        }*/

        // asynchronous function 
        [Mime]
        public async Task<FileUploadDetails> Post()
        {
            // file path
            var fileuploadPath = HttpContext.Current.Server.MapPath("~/UploadFile/input");

            // 
            var multiFormDataStreamProvider = new MultiFileUploadProvider(fileuploadPath);

            // Read the MIME multipart asynchronously 
            await Request.Content.ReadAsMultipartAsync(multiFormDataStreamProvider);

            string uploadingFileName = multiFormDataStreamProvider
                .FileData.Select(x => x.LocalFileName).FirstOrDefault();

            var nameFile = Path.GetFileName(uploadingFileName);
            // var random = RandomString(10);
            // System.Diagnostics.Debug.WriteLine(nameFile);

            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                SignatureTable signa = new SignatureTable();
                signa.pdfName = nameFile;
                
                entities.SignatureTables.Add(signa);
                entities.SaveChanges();

                int idSign = signa.id;

                return new FileUploadDetails
                {
                    FilePath = uploadingFileName,

                    FileName = Path.GetFileName(uploadingFileName),

                    FileLength = new FileInfo(uploadingFileName).Length,

                    FileCreatedTime = DateTime.Now.ToLongDateString(),

                    IdSign = idSign
                };
            }          
            
        }
    }
}
