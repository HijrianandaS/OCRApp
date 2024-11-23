using Microsoft.AspNetCore.Mvc;
using Tesseract;
using OpenCvSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OCRApp.Controllers
{
    public class OCRController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", image.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

            var ocrText = ExtractTextFromImage(filePath);
            ViewBag.OCRResult = ocrText;

            return View("Index");
        }

        /* private string ExtractTextFromImage(string imagePath)
         {
             string tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tessdata");

             try
             {
                 using var engine = new TesseractEngine(tessDataPath, "eng+ind", EngineMode.Default);
                 engine.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ");
                 engine.SetVariable("tessedit_pageseg_mode", "3"); // Assume a single uniform block of text

                 // Load the image using OpenCvSharp
                 Mat img = Cv2.ImRead(imagePath, ImreadModes.Grayscale);

                 // Apply Gaussian Blur to reduce noise
                 Mat cleanedImg = new Mat();
                 Cv2.GaussianBlur(img, cleanedImg, new OpenCvSharp.Size(3, 3), 0);

                 // Save the cleaned image temporarily
                 string cleanedImagePath = Path.Combine(Path.GetTempPath(), "cleaned_image.png");
                 cleanedImg.SaveImage(cleanedImagePath);

                 // Load the cleaned image into Tesseract
                 using var pix = Pix.LoadFromFile(cleanedImagePath);
                 using var page = engine.Process(pix);

                 return page.GetText();
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"Error during OCR: {ex.Message}");
                 throw;
             }
         }*/

        private string ExtractTextFromImage(string imagePath)
        {
            //string tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
            string tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tessdata");
            Console.WriteLine($"Tesseract data path: {tessDataPath}");

            if (!Directory.Exists(tessDataPath))
                throw new DirectoryNotFoundException($"Tessdata path not found: {tessDataPath}");

            // Inisialisasi Tesseract Engine
            using var engine = new TesseractEngine(tessDataPath, "eng+ind", EngineMode.Default);
            using var img = Pix.LoadFromFile(imagePath);
            using var page = engine.Process(img);

            return page.GetText(); // Ekstrak teks
        }

        [HttpGet]
        public IActionResult Test() => Content("Test GET is working");

        [HttpPost]
        public IActionResult TestPost() => Content("Test POST is working");

    }
}
