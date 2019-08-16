using SignData;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SignSkrip.SignProcess;
using SignSkrip.Models;

namespace SignSkrip.Controllers
{
    public class ReqSignController : ApiController
    {
        [Route("api/requestor/sign")]
        public HttpResponseMessage Get(string memberId)
        {
            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        entities.SignatureTables
                        .Where(e => e.requestorId.ToLower() == memberId)
                        .OrderByDescending(f => f.id)
                        .ToList());
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

            }
        }

        [Route("api/requestor/sign/detail")]
        public HttpResponseMessage Get(int id)
        {
            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        entities.SignatureTables
                        .FirstOrDefault(e => e.id == id));
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

            }
        }

        [Route("api/requestor/sign")]
        public HttpResponseMessage Put(int id, [FromBody] SignatureTable signature)
        {
            try
            {   
                using (SignatureDBEntities entities = new SignatureDBEntities())
                {
                    var entity = entities.SignatureTables.FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Id tidak ditemukan di table");
                    }
                    else
                    {
                        entity.author = signature.author;
                        entity.title = signature.title;
                        entity.subject = signature.subject;
                        entity.keyword = signature.keyword;
                        entity.reason = signature.reason;
                        entity.email = signature.email;
                        entity.requestorId = signature.requestorId;
                        entity.issuerId = signature.issuerId;
                        entity.status = "waiting";
                        entities.SaveChanges();
                        System.Diagnostics.Debug.WriteLine("Success request signature");
                    }

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

        [Route("api/requestor/signSelf")]
        public HttpResponseMessage Put(int id, [FromBody] m_signature signature)
        {
            string pathToFiles = HttpContext.Current.Server.MapPath("~/UploadFile/");
            try
            {
                PictureSign picture = new PictureSign();
                picture.Sign(pathToFiles + "input/" + signature.pdfName,
                    pathToFiles + "input/sign_" + signature.pdfName,
                    pathToFiles + "pic/" + signature.picName,
                    signature.posX, signature.posY
                    );

                /*section save data to DB*/
                using (SignatureDBEntities entities = new SignatureDBEntities())
                {
                    var entity = entities.SignatureTables
                        .FirstOrDefault(e => e.id == id);
                    if (entity == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Id not found");
                        return Request.CreateErrorResponse(
                            HttpStatusCode.BadRequest, "Id not found");
                    }
                    else
                    {
                        entity.author = signature.author;
                        entity.title = signature.title;
                        entity.subject = signature.subject;
                        entity.keyword = signature.keyword;
                        entity.reason = signature.reason;
                        entity.email = signature.email;
                        entity.issuerId = signature.issuerId;
                        entity.requestorId = signature.requestorId;
                        entity.status = "waiting";
                        entity.doubleSign = 1;
                        entities.SaveChanges();
                        System.Diagnostics.Debug.WriteLine("Sukses Sign File");
                    }

                    var message = Request.CreateResponse(
                        HttpStatusCode.Created, signature);
                    message.Headers.Location = new Uri(
                        Request.RequestUri + "/" + signature.id.ToString()
                        );
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, ex);
            }
        }

    }
}
