using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace SignSkrip.SignProcess
{

    /// <summary>
    /// this is the most important class
    /// it uses iTextSharp library to sign a PDF document
    /// </summary>
    class PictureSign
    {
        /*
        private string inputPDF = "";
        private string outputPDF = "";
        private MetaData metadata;

        public PictureSign(string input, string output)
        {
            this.inputPDF = input;
            this.outputPDF = output;
        }
        public PictureSign(string input, string output, MetaData md)
        {
            this.inputPDF = input;
            this.outputPDF = output;
            this.metadata = md;
        }
        public void Sign(string SigReason, string SigContact,
            string SigLocation, string pic, int posX, int posY)
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
            sigImg.SetAbsolutePosition(posX, 840 - posY);
            // Add signatures to desired page
            PdfContentByte over = st.GetOverContent(1);
            over.AddImage(sigImg);

            st.MoreInfo = this.metadata.getMetaData();
            st.XmpMetadata = this.metadata.getStreamedMetaData();
            PdfSignatureAppearance sap = st.SignatureAppearance;
            sap.Reason = SigReason;
            sap.Contact = SigContact;
            sap.Location = SigLocation;

            st.Close();
        } */

        public void Sign(string src, string dest, string pic, int posX, int posY)
        { 
            PdfReader reader = new PdfReader(src);
            PdfStamper stamper = new PdfStamper(reader, new FileStream(dest, FileMode.Create, FileAccess.Write));
            Image sigImg = Image.GetInstance(pic);
            sigImg.ScaleToFit(150, 50);
            // Set signature position on page
            sigImg.SetAbsolutePosition(posX, 840 - posY);
            // Add signatures to desired page
            PdfContentByte over = stamper.GetOverContent(1);
            over.AddImage(sigImg);
            
            stamper.Close();
        }


    }
}