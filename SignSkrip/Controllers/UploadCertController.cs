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
    //[EnableCorsAttribute("http://localhost:8080", "*", "GET, POST, PUT, DELETE")]
    public class UploadCertController : ApiController
    {
        public HttpResponseMessage Get(string memberId)
        {
            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, 
                        entities.Certifys
                        .Where(e => e.memberId.ToLower() == memberId)
                        .ToList());
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

            }
        }

        public HttpResponseMessage Get(int Id)
        {
            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entities.Certifys.Where(e => e.id == Id).ToList());
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

            }
        }

        // asynchronous function 
        [Mime]
        public async Task<FileUploadDetails> Post(string memberId)
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
                Certify certi = new Certify();
                certi.certificate = nameFile;
                certi.memberId = memberId;

                entities.Certifys.Add(certi);
                entities.SaveChanges();
                int idSign = certi.id;
                
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


             
