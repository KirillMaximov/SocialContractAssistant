using Bytescout.PDF;
using SocialContractAssistant.DataAccess;
using SocialContractAssistant.Logs;
using SocialContractAssistant.Models;
using System.Text;

namespace SocialContractAssistant.Documents
{
    internal class BytescountPdf : IDocuments
    {
        LogController log = new LogController();
        
        public void CreateDocuments(String fileName, String documents)
        {
            try
            {
                // Create new document
                Document pdfDocument = new Document();
                pdfDocument.RegistrationName = "demo";
                pdfDocument.RegistrationKey = "demo";
                // Add page
                Page page = new Page(PaperFormat.A4);
                pdfDocument.Pages.Add(page);

                Canvas canvas = page.Canvas;

                var currentTop = 10; //Вертикальный отступ, 10 для отступа от верхней границы листа
                var indent = 15; //Отступ между блоками

                // 3 Пробела = 10px

                currentTop += DrowText(canvas, GetHeaderFromDB("stagesHead"), 16, 220, currentTop);
                currentTop += indent;
                currentTop += DrowText(canvas, GetHeaderFromDB("stagesFirst"), 12, 20, currentTop);
                currentTop += DrowText(canvas, documents, 12, 20 + 10, currentTop);
                currentTop += DrowText(canvas, GetHeaderFromDB("stagesNext"), 12, 20, currentTop);

                // Save document to file
                pdfDocument.Save(fileName);

                // Cleanup 
                pdfDocument.Dispose();
            }
            catch (Exception exp)
            {
                log.Error("BytescountPdf", "CreateDocuments", exp.Message);
            }
        }
        
        private String GetHeaderFromDB(String dataDocumentName)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var dataDocument = db.DataDocuments.FirstOrDefault(p => p.Name == dataDocumentName);

                    return dataDocument!.Text!;
                }
            }
            catch (Exception exp)
            {
                log.Error("BytescountPdf", "GetHeaderFromDB", exp.Message);
                return String.Empty;
            }
        }

        #region DrowText

        private Int32 DrowText(Canvas canvas, String text, Int32 fontSize, Int32 left, Int32 top)
        {
            Font font = new Font("Arial", fontSize);
            Brush brush = new SolidBrush();
            StringFormat stringFormat = new StringFormat();
            stringFormat.WordSpacing = 0.0f;

            canvas.DrawString(text, font, brush, left, top);

            var lineHeight = fontSize + 1; //Высота текста
            var textLines = text.Split('\n').Length;

            return lineHeight * textLines;
        }

        #endregion
    }
}
