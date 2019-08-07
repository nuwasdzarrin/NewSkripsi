using SignData;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SignSkrip.Controllers
{
    public class MemberController : ApiController
    {
        [Route("api/member/getIssuer")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entities.Members.Where(e => e.memberRole.ToLower() == "Issuer").ToList());
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

            }
        }

        [Route("api/member/detail")]
        public HttpResponseMessage Get(string memberId)
        {
            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        entities.Members
                        .FirstOrDefault(e => e.memberId.ToLower() == memberId));
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

            }
        }

        /*[Route("api/member/join")]
        [HttpGet]
        public HttpResponseMessage GetJoin(string par)
        {
            using (SignatureDBEntities entities = new SignatureDBEntities())
            {
                try
                {
                    /*var robotDogs = (from d in entities.Certifys
                                     join f in entities.Members
                                     on d.idMember equals f.id
                                     where f.firstName == "dana"
                                     select d).ToList();
                    //int idmember = 0;
                    var robotDogs = (from d in entities.Certifys
                                     join f in entities.Members
                                     on d.memberId equals f.memberId
                                     where f.firstName == par
                                     select new
                                     {
                                         idmember = d.memberId,
                                         firstnames = f.firstName,
                                         lastnames = f.lastName,
                                         cerName = d.certificate
                                     }).ToList();
                    /*
                     or write like this, where context equal to entities
                     var robotDogs = context.RobotDogs
                                    .Join(
                                        context.RobotFactories.Where(x => x.Location == "Texas"),
                                        d => d.RobotFactoryId,
                                        f => f.RobotFactoryId,
                                        (d, f) => d)
                                    .ToList();
                     
                    var robotDogs = entities.Certifys
                                    .Join(
                                        entities.Members.Where(x => x.firstName == "dana"),
                                        d => d.idMember,
                                        f => f.id,
                                        (d, f) => f)
                                    .ToList();
                    
                    change that select d in:
                    select new {
                      Location = f.Location,
                      RobotName = d.Name,
                      IsArmed = d.Armed
                    }
                    

                    return Request.CreateResponse(HttpStatusCode.OK, robotDogs);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

            }
        }
        */
    }
}
