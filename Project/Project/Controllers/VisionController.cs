using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Project.Models;
using Project.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace Project.Controllers
{
    public class VisionController : Controller
    {
        private readonly ComputerVisionService _visionService;
        private readonly CosmosDbService _cosmosService;
        private readonly BlobStorageService _blob;
        private readonly AIService _aIService;

        public VisionController(ComputerVisionService visionService, CosmosDbService cosmosService, BlobStorageService blob, AIService aIService)
        {
            _visionService = visionService;
            _cosmosService = cosmosService;
            _blob = blob;
            _aIService = aIService;
        }

        // Strona Upload
        public IActionResult Upload()
        {
            return View();
        }

        // Obsługa przesyłania pliku
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return View("Upload");

            string imageUrl = await _blob.UploadAsync(file);

            using MemoryStream memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            string text = await _visionService.ExtractTextFromImageAsync(memoryStream.ToArray());

            ImageDocument doc = new ImageDocument
            {
                FileName = file.FileName,
                ImageUrl = imageUrl,
                ExtractedText = text
            };

            await _cosmosService.AddDocumentAsync(doc);

            string aiSummary = "AISummary: ";
            try
            {
                aiSummary += await _aIService.SummarizeAsync(text);
            }
            catch (Exception ex)
            {
                aiSummary += $"Error generating summary: {ex.Message}";
            }

            ViewBag.ImageUrl = imageUrl;
            ViewBag.ExtractedText = text;
            ViewBag.AISummary = aiSummary;

            return View("Upload");
        }


        public async Task<IActionResult> History()
        {
            List<ImageDocument> documents = await _cosmosService.GetAllDocumentsAsync();
            return View(documents);
        }

        public async Task<IActionResult> ExportPdf(string id)
        {
            var document = await _cosmosService.GetDocumentByIdAsync(id);

            if (document == null)
                return NotFound();

            byte[] pdfBytes = PdfGeneratorService.Generate(document);

            return File(
                pdfBytes,
                "application/pdf",
                $"{document.FileName}_Azure_MG.pdf"
            );
        }
    }
}