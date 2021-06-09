using Microsoft.AspNetCore.Hosting;
using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Server.Controllers.HelperClasses
{
    public class FileImageHandler
    {
        private readonly IWebHostEnvironment env;
        public FileImageHandler(IWebHostEnvironment environment)
        {
            env = environment;
        }
        public async Task<FileImage> GetImageAsync(string Uri)
        {
            byte[] bytes = null;
            try
            {
                if (string.IsNullOrEmpty(Uri))
                {
                    return null;
                }

                using (var stream = File.Open(Uri, FileMode.Open, FileAccess.Read))
                {
                    bytes = new byte[stream.Length];
                    int readbytes = 0;
                    while (readbytes < stream.Length)
                        readbytes = await stream.ReadAsync(bytes, 0, (int)stream.Length);
                }

                var img = new FileImage { FileName = Path.GetFileName(Uri), Content = bytes };
                return img;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> SaveImageAsync(FileImage image)
        {
            try
            {
                var imgSaveLocation = Path.Combine(env.ContentRootPath, "imgs", DateTime.Now.Ticks + image.FileName); //TODO make it not so hardcody
                using (var stream = new FileStream(imgSaveLocation, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    await stream.WriteAsync(image.Content, 0, image.Content.Length);
                    stream.Flush();
                }
                return imgSaveLocation;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        internal bool DeleteImage(FileImage productPic)
        {
            try
            {
                if (productPic != null)
                {
                    var imgSaveLocation = Path.Combine(env.ContentRootPath, "imgs", productPic.FileName);
                    if (File.Exists(imgSaveLocation)) //maybe do some more checks and responses 
                    {
                        // If file found, delete it    
                        File.Delete(imgSaveLocation);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
    }
}