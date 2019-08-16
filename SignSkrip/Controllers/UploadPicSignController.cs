using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SignData;

namespace SignSkrip.Controllers
{
    public class UploadPicSignController : ApiController
    {
        [Route("api/picture/getPictureSign")]
        public HttpResponseMessage Get(string memberId)
        {
            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        entities.SignPics
                        .Where(e => e.memberId.ToLower() == memberId)
                        .ToList());
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

            }
        }

        [Route("api/picture/uploadPictureSign")]
        public HttpResponseMessage Post([FromBody] SignPic signPic)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/UploadFile/pic/" + signPic.namePic);
            File.WriteAllBytes(filePath, Convert.FromBase64String(signPic.basePic));
            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                SignPic pic = new SignPic();
                pic.memberId = signPic.memberId;
                pic.namePic = signPic.namePic;

                entities.SignPics.Add(pic);
                entities.SaveChanges();
            }
            return Request.CreateResponse(HttpStatusCode.OK, signPic);
        }
    }
}
