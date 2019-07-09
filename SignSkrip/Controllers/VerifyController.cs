using System;
using System.Web.Http;
using System.Web;
using System.Threading.Tasks;

using SignSkrip.Models;
using iTextSharp.text.pdf;

using System.IO;
using System.Text;
using SignSkrip.SignProcess;
using System.Security.Cryptography.X509Certificates;
using org.bouncycastle.x509;

namespace SignSkrip.Controllers
{
    public class VerifyController : ApiController
    {
        private string avai = "";
        private string nFile = "";
        private bool cove = false;
        private string nSign = "";
        private string reaso = "";
        private string locatio = "";
        private DateTime dat = DateTime.Now;
        private int versio = 0;
        string pathToFiles = HttpContext.Current.Server.MapPath("~/UploadFile/output/sign_ForCekTandaTangan.pdf");

        public m_verify Get()
        {
            string sert = HttpContext.Current.Server.MapPath("~/UploadFile/sertifikat/nunu.pfx");
            string outp = HttpContext.Current.Server.MapPath("~/UploadFile/sertifikat/nani.cer");

            X509Certificate2 certificate = new X509Certificate2(sert, "nuwas1903");
            StringBuilder publicBuilder = new StringBuilder();

            publicBuilder.AppendLine("--BEGIN CERTIFICATE--");
            publicBuilder.AppendLine(Convert.ToBase64String(certificate.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            publicBuilder.AppendLine("—–END CERTIFICATE—–");
            publicBuilder.ToString();

            System.Diagnostics.Debug.WriteLine(publicBuilder.ToString());
            File.WriteAllText(outp, publicBuilder.ToString());

            Stream fileStream = new FileStream(outp, FileMode.Open);

            PDFVerify verif = new PDFVerify();
            var ver = verif.Verify(pathToFiles, fileStream);


            //KeyStore kall = PdfPKCS7.loadCacertsKeyStore();
            //var parser = new X509CertificateParser(fileStream);
            //var certifi = parser.ReadCertificate();
            //fileStream.Dispose();

            //List<m_verify> results = new List<m_verify>();

            PdfReader reader = new PdfReader(pathToFiles);
            AcroFields af = reader.AcroFields;
            var names = af.GetSignatureNames();
            if (names.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("Tidak ada ttdnya");
            }
            else
            {
                this.avai = "Digital Signature Available";
                System.Diagnostics.Debug.WriteLine("IKI lo TTD ne yooow");
            }
            foreach (string name in names)
            {
                if (!af.SignatureCoversWholeDocument(name))
                {
                    System.Diagnostics.Debug.WriteLine("The signature: {0} does not covers the whole document.", name);
                }

                System.Diagnostics.Debug.WriteLine("Signature Name: " + name);
                System.Diagnostics.Debug.WriteLine("Signature covers whole document: " + af.SignatureCoversWholeDocument(name));

                this.nFile = name;
                this.cove = af.SignatureCoversWholeDocument(name);

                PdfPKCS7 pk = af.VerifySignature(name);
                var cal = pk.SignDate;
                var pkc = pk.Certificates;
                // TimeStampToken ts = pk.TimeStampToken;
                if (!pk.Verify())
                {
                    System.Diagnostics.Debug.WriteLine("The signature could not be verified");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Name signature: " + pk.SignName);
                    System.Diagnostics.Debug.WriteLine("Reason signature: " + pk.Reason);
                    System.Diagnostics.Debug.WriteLine("Location signature: " + pk.Location);
                    System.Diagnostics.Debug.WriteLine("Date signature: " + pk.SignDate);
                    System.Diagnostics.Debug.WriteLine("Version signature: " + pk.SigningInfoVersion);
                    System.Diagnostics.Debug.WriteLine("Sertificate signature: " + pk.SigningCertificate);

                    this.nSign = pk.SignName;
                    this.reaso = pk.Reason;
                    this.locatio = pk.Location;
                    this.dat = pk.SignDate;
                    this.versio = pk.SigningInfoVersion;
                }

                //IList<VerificationException>[] fails = PdfPKCS7.VerifyCertificates(pkc, new X509Certificate[] { certifi }, null, cal);
                //object[] fails = PdfPKCS7.VerifyCertificates(pkc, new X509Certificate[] { certifi }, null, cal);
                //if (fails != null)
                //{
                //    System.Diagnostics.Debug.WriteLine("The file is not signed using the specified key-pair.");
                //}

            }
            return new m_verify
            {
                available = this.avai,

                nameFile = this.nFile,

                cover = this.cove,

                nameSign = this.nSign,

                reason = this.reaso,

                location = this.locatio,

                date = this.dat,

                version = this.versio
            };
        }
    }
}
