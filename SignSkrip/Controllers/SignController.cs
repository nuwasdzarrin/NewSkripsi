using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SignData;
using SignSkrip.SignProcess;
using System.Web.Http.Cors;

namespace SignSkrip.Controllers
{
    [EnableCorsAttribute("http://localhost:8080", "*", "GET, POST, PUT, DELETE")]
    public class SignController : ApiController
    {
        string pathToFiles = HttpContext.Current.Server.MapPath("~/UploadFile/");
        public HttpResponseMessage Get()
        {
            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entities.SignatureTables.ToList());
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

            }
        }

        public HttpResponseMessage Put(int id, [FromBody] SignatureTable signature)
        {
            try
            {
                Cert myCert = null;
                try
                {
                    myCert = new Cert(pathToFiles + "sertifikat/" + signature.certName, signature.password);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

                //Adding Meta Datas
                MetaData MyMD = new MetaData();
                MyMD.Author = signature.author;
                MyMD.Title = signature.title;
                MyMD.Subject = signature.subject;
                MyMD.Keywords = signature.keyword;

                PDFSigner pdfs = new PDFSigner(pathToFiles + "input/" + signature.pdfName, pathToFiles + "output/sign_" + signature.pdfName, myCert, MyMD);
                pdfs.Sign(signature.reason, signature.email, signature.location, true);
                using (SignatureDBEntities entities = new SignatureDBEntities())
                {
                    var entity = entities.SignatureTables.FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Id tidak ditemukan di table");
                    } else
                    {
                        entity.author = signature.author;
                        entity.title = signature.title;
                        entity.subject = signature.subject;
                        entity.keyword = signature.keyword;
                        entity.reason = signature.reason;
                        entity.email = signature.email;
                        entity.location = signature.location;
                        entity.password = signature.password;
                        entities.SaveChanges();
                        System.Diagnostics.Debug.WriteLine("Sukses Sign File");
                    }
                    /*entities.SignatureTables.Add(signature);
                    entities.SaveChanges();*/

                    var message = Request.CreateResponse(HttpStatusCode.Created, signature);
                    message.Headers.Location = new Uri(Request.RequestUri + "/" + signature.id.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
