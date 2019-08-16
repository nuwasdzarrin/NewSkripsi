using org.bouncycastle.crypto;
using System.Collections;
using org.bouncycastle.pkcs;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.xml.xmp;

namespace SignSkrip.SignProcess
{
    /// <summary>
    /// This class hold the certificate and extract private key needed for e-signature 
    /// </summary>
    class Cert
    {
        #region Attributes

        private string path = "";
        private string password = "";
        private AsymmetricKeyParameter akp;
        private org.bouncycastle.x509.X509Certificate[] chain;

        #endregion

        #region Accessors
        public org.bouncycastle.x509.X509Certificate[] Chain
        {
            get { return chain; }
        }
        public AsymmetricKeyParameter Akp
        {
            get { return akp; }
        }

        public string Path
        {
            get { return path; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        #endregion

        #region Helpers

        private void processCert()
        {
            string alias = null;
            PKCS12Store pk12;

            //First we'll read the certificate file
            pk12 = new PKCS12Store(new FileStream(this.Path, FileMode.Open, FileAccess.Read), this.password.ToCharArray());

            //then Iterate throught certificate entries to find the private key entry
            IEnumerator i = pk12.aliases();
            while (i.MoveNext())
            {
                alias = ((string)i.Current);
                if (pk12.isKeyEntry(alias))
                    break;
            }

            this.akp = pk12.getKey(alias).getKey();
            X509CertificateEntry[] ce = pk12.getCertificateChain(alias);
            this.chain = new org.bouncycastle.x509.X509Certificate[ce.Length];
            for (int k = 0; k < ce.Length; ++k)
                chain[k] = ce[k].getCertificate();

        }
        #endregion

        #region Constructors
        public Cert()
        { }
        public Cert(string cpath)
        {
            this.path = cpath;
            this.processCert();
        }
        public Cert(string cpath, string cpassword)
        {
            this.path = cpath;
            this.Password = cpassword;
            this.processCert();
        }
        #endregion

    }

    /// <summary>
    /// This is a holder class for PDF metadata
    /// </summary>
    class MetaData
    {
        private Hashtable info = new Hashtable();

        public Hashtable Info
        {
            get { return info; }
            set { info = value; }
        }

        public string Author
        {
            get { return (string)info["Author"]; }
            set { info.Add("Author", value); }
        }
        public string Title
        {
            get { return (string)info["Title"]; }
            set { info.Add("Title", value); }
        }
        public string Subject
        {
            get { return (string)info["Subject"]; }
            set { info.Add("Subject", value); }
        }
        public string Keywords
        {
            get { return (string)info["Keywords"]; }
            set { info.Add("Keywords", value); }
        }
        public string Producer
        {
            get { return (string)info["Producer"]; }
            set { info.Add("Producer", value); }
        }

        public string Creator
        {
            get { return (string)info["Creator"]; }
            set { info.Add("Creator", value); }
        }

        public Hashtable getMetaData()
        {
            return this.info;
        }
        public byte[] getStreamedMetaData()
        {
            MemoryStream os = new System.IO.MemoryStream();
            XmpWriter xmp = new XmpWriter(os, this.info);
            xmp.Close();
            return os.ToArray();
        }

    }


    /// <summary>
    /// this is the most important class
    /// it uses iTextSharp library to sign a PDF document
    /// </summary>
    class PDFSigner
    {
        private string inputPDF = "";
        private string outputPDF = "";
        private Cert myCert;
        private MetaData metadata;

        public PDFSigner(string input, string output)
        {
            this.inputPDF = input;
            this.outputPDF = output;
        }

        public PDFSigner(string input, string output, Cert cert)
        {
            this.inputPDF = input;
            this.outputPDF = output;
            this.myCert = cert;
        }
        public PDFSigner(string input, string output, MetaData md)
        {
            this.inputPDF = input;
            this.outputPDF = output;
            this.metadata = md;
        }
        public PDFSigner(string input, string output, Cert cert, MetaData md)
        {
            this.inputPDF = input;
            this.outputPDF = output;
            this.myCert = cert;
            this.metadata = md;
        }

        /*public void Verify(string pdfFile, Stream fileStream)
        {
            
            //KeyStore kall = PdfPKCS7.loadCacertsKeyStore();
            var parser = new X509CertificateParser(fileStream);
            var certifi = parser.ReadCertificate ();
            fileStream.Dispose();

            string pathToFiles = HttpContext.Current.Server.MapPath("~/UploadFile/output/ForCekTandaTangan.pdf");
            PdfReader reader = new PdfReader(pathToFiles);
            AcroFields af = reader.AcroFields;
            var names = af.GetSignatureNames();
            if (names.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("Tidak ada ttdnya");
            }
            else
            {
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
                System.Diagnostics.Debug.WriteLine("Document revision: " + af.GetRevision(name));
                
                PdfPKCS7 pk = af.VerifySignature(name);
                var cal = pk.SignDate;
                var pkc = pk.Certificates;
                // TimeStampToken ts = pk.TimeStampToken;
                if (!pk.Verify())
                {
                    System.Diagnostics.Debug.WriteLine("The signature could not be verified");
                } else
                {
                    System.Diagnostics.Debug.WriteLine("Name signature: " + pk.SignName);
                    System.Diagnostics.Debug.WriteLine("Reason signature: " + pk.Reason);
                    System.Diagnostics.Debug.WriteLine("Location signature: " + pk.Location);
                    System.Diagnostics.Debug.WriteLine("Date signature: " + pk.SignDate);
                    System.Diagnostics.Debug.WriteLine("Version signature: " + pk.SigningInfoVersion);
                    System.Diagnostics.Debug.WriteLine("Sertificate signature: " + pk.SigningCertificate);
                }

                //IList<VerificationException>[] fails = PdfPKCS7.VerifyCertificates(pkc, new X509Certificate[] { certifi }, null, cal);
                //Object[] fails = PdfPKCS7.VerifyCertificates(pkc, new X509Certificate[] { }, null, cal);
                //if (fails != null)
                //{
                //    System.Diagnostics.Debug.WriteLine("The file is not signed using the specified key-pair.");
                //}
            }
        }*/
        //To disable Multi signatures uncomment this line : every new signature will invalidate older ones ! line 251
        //PdfStamper st = PdfStamper.CreateSignature(reader, new FileStream(this.outputPDF, FileMode.Create, FileAccess.Write), '\0'); 

        public void Sign(string SigReason, string SigContact,
            string SigLocation, string pic, bool visible, int posX, int posY)
        {
            //Activate MultiSignatures
            PdfReader reader = new PdfReader(this.inputPDF);
            PdfStamper st = PdfStamper.CreateSignature(reader,
                new FileStream(this.outputPDF, FileMode.Create, FileAccess.Write),
                '\0', null, true);

            //iTextSharp.text.Image sigImg = iTextSharp.text.Image.GetInstance(pic);
            Image sigImg = Image.GetInstance(pic);
            // MAX_WIDTH, MAX_HEIGHT
            sigImg.ScaleToFit(150, 50);
            // Set signature position on page
            sigImg.SetAbsolutePosition(posX, 840-posY);
            // Add signatures to desired page
            PdfContentByte over = st.GetOverContent(1);
            over.AddImage(sigImg);

            st.MoreInfo = this.metadata.getMetaData();
            st.XmpMetadata = this.metadata.getStreamedMetaData();
            PdfSignatureAppearance sap = st.SignatureAppearance;

            sap.SetCrypto(this.myCert.Akp, this.myCert.Chain, 
                null, PdfSignatureAppearance.WINCER_SIGNED);
            sap.Reason = SigReason;
            sap.Contact = SigContact;
            sap.Location = SigLocation;
            if (visible)
                sap.SetVisibleSignature(
                    new Rectangle(posX, 840 - posY, posX + 150, (840 - posY) + 50), 1, null);
            st.Close();
        }

    }
}