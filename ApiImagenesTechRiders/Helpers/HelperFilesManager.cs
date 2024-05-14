using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp;

namespace ApiImagenesTechRiders.Helpers
{
    public class HelperFilesManager
    {
        private string pathImgs;
        private string urlServerHost;

        public HelperFilesManager(string absolutePath, string urlServerHost)
        {
            this.urlServerHost = urlServerHost;
            List<string> folders = absolutePath.Split(Path.DirectorySeparatorChar).ToList();
            folders.RemoveRange(folders.Count - 4, 3);
            folders.Add("imgs");
            pathImgs = Path.DirectorySeparatorChar + Path.Combine(folders.ToArray());
            Console.Write(pathImgs);
        }

        public bool ImgExists(string imgName)
        {
            string imgPath = Path.Combine(pathImgs, imgName);
            return File.Exists(imgPath);
        }

        public async Task<string> CreateImg(string imgName, Stream imgStream)
        {
            if (ImgExists(imgName))
            {
                throw new InvalidOperationException("Image already exists.");
            }

            string imgPath = Path.Combine(pathImgs, imgName);
            Console.Write(imgPath);
            using (Stream jpegStream = await ConvertToJpeg(imgStream))
            {
                using (FileStream fs = File.OpenWrite(imgPath))
                {
                    await jpegStream.CopyToAsync(fs);
                }
            }
            return urlServerHost + "imgs" + "/" + imgName;
        }

        public async Task<string> UpdateImg(string imgName, Stream imgStream)
        {
            if (!ImgExists(imgName))
            {
                throw new FileNotFoundException("Image not found.");
            }

            string imgPath = Path.Combine(pathImgs, imgName);
            Console.Write(imgPath);
            using (Stream jpegStream = await ConvertToJpeg(imgStream))
            {
                using (FileStream fs = File.OpenWrite(imgPath))
                {
                    await jpegStream.CopyToAsync(fs);
                }
            }
            return urlServerHost + "imgs" + "/" + imgName;
        }

        public void DeleteImg(string imgName)
        {
            if (!ImgExists(imgName))
            {
                throw new FileNotFoundException("Image not found.");
            }

            string imgPath = Path.Combine(pathImgs, imgName);
            File.Delete(imgPath);
        }

        public int GetIdImage(string fileName)
        {
            string idAndExtension = fileName.Split('-').Last();
            string idImg = idAndExtension.Split(".").First();
            int id = int.Parse(idImg);
            return id;
        }

        public async Task<MemoryStream> ConvertToJpeg(Stream imgStream)
        {
            try
            {
                using (var image = Image.Load(imgStream))
                {
                    using (var outputStream = new MemoryStream())
                    {
                        image.Save(outputStream, new JpegEncoder { Quality = 75 });
                        outputStream.Position = 0;
                        var jpegStream = new MemoryStream(outputStream.ToArray());
                        jpegStream.Position = 0;
                        return jpegStream;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting image to JPEG: " + ex.Message);
                throw;
            }
        }
    }
}
