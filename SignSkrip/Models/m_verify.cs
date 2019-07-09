using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignSkrip.Models
{
    public class m_verify
    {
        public string available { get; set; }
        public string nameFile { get; set; }
        public bool cover { get; set; }
        public string nameSign { get; set; }
        public string reason { get; set; }
        public string location { get; set; }
        public DateTime date { get; set; }
        public int version { get; set; }
    }
}