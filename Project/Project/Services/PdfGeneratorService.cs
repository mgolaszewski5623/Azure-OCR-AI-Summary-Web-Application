using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Project.Models;

namespace Project.Services
{
    public static class PdfGeneratorService
    {
        public static byte[] Generate(ImageDocument doc)
        {
            byte[] imageBytes;
            using (var http = new HttpClient())
            {
                imageBytes = http.GetByteArrayAsync(doc.ImageUrl).Result;
            }
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Column(header =>
                    {
                        header.Item().Text(doc.FileName)
                                     .FontSize(20)
                                     .Bold()
                                     .FontColor(Colors.Blue.Darken2);
                        header.Item().Text($"Uploaded: {doc.Timestamp.ToLocalTime():yyyy-MM-dd HH:mm}")
                                     .FontSize(12)
                                     .FontColor(Colors.Grey.Darken1);
                    });

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text("Extracted Text:")
                                     .Bold()
                                     .FontSize(14);

                        column.Item().Text(doc.ExtractedText)
                                     .FontSize(12)
                                     .FontColor(Colors.Black)
                                     .WrapAnywhere();
                    });

                    page.Footer().AlignCenter().Text($"Project Azure - Michał Gołaszewski -  {DateTime.Now}")
                                   .FontSize(10)
                                   .FontColor(Colors.Grey.Lighten2)
                                   .Italic(); ;
                });
            }).GeneratePdf();
        }
    }
}