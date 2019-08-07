using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SignData;

namespace SignSkrip.Controllers
{
    public class ValidatorController : ApiController
    {
        [Route("api/validator/sign")]
        public HttpResponseMessage Get()
        {
            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, 
                        entities.SignatureTables.OrderByDescending(e => e.id)
                        .ToList());
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

            }
        }

        [Route("api/validator/sign/detail")]
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

        [Route("api/validator/updateStatus")]
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
                        entity.status = signature.status;
                        entities.SaveChanges();
                        System.Diagnostics.Debug.WriteLine("Sukses Sign File");
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
