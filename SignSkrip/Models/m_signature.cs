using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignSkrip.Models
{
    public class m_signature
    {
        public int id { get; set; }
        public string author { get; set; }
        public string title { get; set; }
        public string subject { get; set; }
        public string keyword { get; set; }
        public string reason { get; set; }
        public string email { get; set; }
        public string location { get; set; }
        public string pdfName { get; set; }
        public string certName { get; set; }
        public string password { get; set; }
        public string requestorId { get; set; }
        public string issuerId { get; set; }
        public string status { get; set; }
        public string picName { get; set; }
        public bool visible { get; set; }
        public int posX { get; set; }
        public int posY { get; set; }
    }
}