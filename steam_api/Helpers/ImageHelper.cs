﻿using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace SKYNET.Helpers
{
    public class ImageHelper
    {
        public static Bitmap Resize(Bitmap image, int width, int height)
        {
            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap bitmap = new Bitmap(width, height);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (ImageAttributes imageAttributes = new ImageAttributes())
                {
                    imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
                    return bitmap;
                }
            }
        }

        public static byte[] ImageToBytes(Image image)
        {
            byte[] imageArray = new byte[0];

            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Jpeg);
                stream.Close();
                imageArray = stream.ToArray();
            }
            return imageArray;
        }

        public static byte[] ConvertToRGBA(Bitmap image)
        {
            byte[] rgbaB = new byte[4 * (image.Width * image.Height)];
            int i = 0;
            for (var y = 0; y < image.Width; y++)
            {
                for (var x = 0; x < image.Height; x++)
                {
                    Color pix = image.GetPixel(x, y);
                    rgbaB[i++] = pix.R;
                    rgbaB[i++] = pix.G;
                    rgbaB[i++] = pix.B;
                    rgbaB[i++] = pix.A;
                }
            }
            return rgbaB;
        }

        public static Bitmap GetDesktopWallpaper(bool Aspect4x4 = false)
        {
            try
            {
                RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                if (Key == null) return null;
                string filePath = (string)Key.GetValue("WallPaper");
                if (string.IsNullOrEmpty(filePath)) return null;
                if (!File.Exists(filePath)) return null;
                if (Aspect4x4)
                {
                    return (Bitmap)AspectRatio4x4((Bitmap)Bitmap.FromFile(filePath));
                }
                return (Bitmap)Bitmap.FromFile(filePath);
            }
            catch 
            {
                return null;
            }
        }

        public static Image AspectRatio4x4(Image img)
        {
            // 4:3 Aspect Ratio. You can also add it as parameters
            double aspectRatio_X = 4;
            double aspectRatio_Y = 4;

            double imgWidth = Convert.ToDouble(img.Width);
            double imgHeight = Convert.ToDouble(img.Height);

            if (imgWidth / imgHeight > (aspectRatio_X / aspectRatio_Y))
            {
                double extraWidth = imgWidth - (imgHeight * (aspectRatio_X / aspectRatio_Y));
                double cropStartFrom = extraWidth / 2;
                Bitmap bmp = new Bitmap((int)(img.Width - extraWidth), img.Height);
                Graphics grp = Graphics.FromImage(bmp);
                grp.DrawImage(img, new Rectangle(0, 0, (int)(img.Width - extraWidth), img.Height), new Rectangle((int)cropStartFrom, 0, (int)(imgWidth - extraWidth), img.Height), GraphicsUnit.Pixel);
                return bmp;
            }
            else
                return null;
        }


        public static Image ImageFromBytes(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            return Image.FromStream(stream);
        }
    }
}
