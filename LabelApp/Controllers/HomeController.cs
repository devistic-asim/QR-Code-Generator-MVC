﻿using LabelApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZXing;

namespace LabelApp.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Generate()
        {
            return View("Generate");
        }

        [HttpPost]
        public ActionResult Generate(QRCodeModel qrcode)
        {
            try
            {
                qrcode.QRCodeImagePath = GenerateQRCode(qrcode.QRCodeText);
                ViewBag.Message = "QR Code Created successfully";
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex;
                return View("Generate");
                //catch exception if there is any
            }
            return View("Generate", qrcode);
        }

        private string GenerateQRCode(string qrcodeText)
        {
            string folderPath = "~/Images/";
            string imagePath = "~/Images/QrCode.jpg";
            // If the directory doesn't exist then create it.
            if (!Directory.Exists(Server.MapPath(folderPath)))
            {
                Directory.CreateDirectory(Server.MapPath(folderPath));
            }

            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            var result = barcodeWriter.Write(qrcodeText);

            string barcodePath = Server.MapPath(imagePath);
            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return imagePath;
        }

        public ActionResult Read()
        {
            return View(ReadQRCode());
        }

        private QRCodeModel ReadQRCode()
        {
            QRCodeModel barcodeModel = new QRCodeModel();
            string barcodeText = "";
            string imagePath = "~/Images/QrCode.jpg";
            string barcodePath = Server.MapPath(imagePath);
            var barcodeReader = new BarcodeReader();

            var result = barcodeReader.Decode(new Bitmap(barcodePath));
            if (result != null)
            {
                barcodeText = result.Text;
            }
            return new QRCodeModel() { QRCodeText = barcodeText, QRCodeImagePath = imagePath };
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}