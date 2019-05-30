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
using System.Web.Http.Cors;
using SignData;

namespace SignSkrip.Controllers
{
    [EnableCorsAttribute("http://localhost:8080", "*", "GET, POST, PUT, DELETE")]
    public class UploadCertController : ApiController
    {
        // asynchronous function 
        [Mime]
        public async Task<FileUploadDetails> Put(int id)
        {
            // file path
            var fileuploadPath = HttpContext.Current.Server.MapPath("~/UploadFile/sertifikat");

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
                var entity = entities.SignatureTables.FirstOrDefault(e => e.id == id);
                if (entity == null)
                {
                    System.Diagnostics.Debug.WriteLine("Id tidak ditemukan di table");
                } else
                {
                    entity.certName = nameFile;
                    entities.SaveChanges();
                    System.Diagnostics.Debug.WriteLine("Sukses input cert");
                }

                return new FileUploadDetails
                {
                    FilePath = uploadingFileName,

                    FileName = Path.GetFileName(uploadingFileName),

                    FileLength = new FileInfo(uploadingFileName).Length,

                    FileCreatedTime = DateTime.Now.ToLongDateString(),

                    IdSign = id
                };
            }
        }
    }
}


             
