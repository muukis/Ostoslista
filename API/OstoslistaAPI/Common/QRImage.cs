using System;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;

namespace OstoslistaAPI.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class QRImage
    {
        private readonly QRCodeGenerator _qrGenerator = new QRCodeGenerator();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string CreateBase64EncodedQRImageFromUrl(string url)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var qrCodeData = _qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                var qrCodeImage = qrCode.GetGraphic(5);
                qrCodeImage.Save(ms, ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
