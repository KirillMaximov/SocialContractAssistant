using Bytescout.PDF;
using SocialContractAssistant.DataAccess;
using SocialContractAssistant.Logs;
using SocialContractAssistant.Models;
using System.Text;

namespace SocialContractAssistant.Documents
{
    internal class BytescountPdf
    {
        LogController log = new LogController();
        public void CreateDocument(String fileName, IQueryable<AnswerModel> answers)
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
                currentTop += DrowText(canvas, GetDocuments(answers), 12, 20 + 10, currentTop);
                currentTop += DrowText(canvas, GetHeaderFromDB("stagesNext"), 12, 20, currentTop);

                // Save document to file
                pdfDocument.Save(fileName);

                // Cleanup 
                pdfDocument.Dispose();
            }
            catch (Exception exp)
            {
                log.Error("BytescountPdf", "CreateDocument", exp.Message);
            }
        }

        public void CreateDocumentTest(String fileName)
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
                currentTop += DrowText(canvas, GetAllDocuments(), 12, 20, currentTop);
                currentTop += DrowText(canvas, GetHeaderFromDB("stagesNext"), 12, 20, currentTop);

                // Save document to file
                pdfDocument.Save(fileName);

                // Cleanup 
                pdfDocument.Dispose();
            }
            catch (Exception exp)
            {
                log.Error("BytescountPdf", "CreateDocument", exp.Message);
            }
        }

        private Int32 DrowText(Canvas canvas, StringBuilder text, Int32 fontSize, Int32 left, Int32 top)
        {
            Font font = new Font("Arial", fontSize);
            Brush brush = new SolidBrush();
            StringFormat stringFormat = new StringFormat();
            stringFormat.WordSpacing = 0.0f;

            canvas.DrawString(text.ToString(), font, brush, left, top);

            var lineHeight = fontSize + 1; //Высота текста
            var textLines = text.ToString().Split('\n').Length - 1;

            return lineHeight * textLines;
        }

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

        private StringBuilder GetDocuments(IQueryable<AnswerModel> answers)
        {
            StringBuilder docs = new StringBuilder();
            List<Int32> addedDocuments = new List<Int32>();
            try
            {
                using (var db = new DataBaseContext())
                {
                    foreach (var item in answers)
                    {
                        var option = db.Options.FirstOrDefault(p => p.Id == item.OptionId);
                        var linkDocuments = db.LinkDocuments.Where(p => p.LinkId == option!.LinkDocumentId);

                        using (var dblink = new DataBaseContext())
                        {
                            foreach (var link in linkDocuments)
                            {
                                var document = dblink.Documents.FirstOrDefault(p => p.Id == link.DocumentId && !addedDocuments.Contains(p.Id));
                                if (document != null)
                                {
                                    docs.AppendLine(document.Name);

                                    if (!String.IsNullOrEmpty(document.Description))
                                    {
                                        docs.AppendLine(document.Description);
                                    }

                                    addedDocuments.Add(document.Id);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("BytescountPdf", "GetDocuments", exp.Message);
            }
            return docs;
        }

        private StringBuilder GetAllDocuments()
        {
            StringBuilder docs = new StringBuilder();
            try
            {
                using (var db = new DataBaseContext())
                {

                    using (var dblink = new DataBaseContext())
                    {
                        var documents = dblink.Documents;

                        foreach (var document in documents)
                        {
                            if (document != null)
                            {
                                docs.AppendLine(document.Name);

                                if (!String.IsNullOrEmpty(document.Description))
                                {
                                    docs.AppendLine(document.Description);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("BytescountPdf", "GetAllDocuments", exp.Message);
            }
            return docs;
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
    }
}
