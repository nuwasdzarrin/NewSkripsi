using SignData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SignSkrip.Controllers
{
    public class ReqSignController : ApiController
    {
        public HttpResponseMessage Get(string id)
        {
            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entities.SignatureTables.Where(e => e.requestorId.ToLower() == id).ToList());
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
    }
}
