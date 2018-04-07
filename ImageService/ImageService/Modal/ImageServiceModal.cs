using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;

namespace ImageService.Modal
{
    class ImageServiceModal : IImageServiceModal
    {
        private String outputDir = ConfigurationManager.AppSettings["OutputDir"];

        public string AddFile(string path, out bool result)
        {
            String response = "Image copied successfully.";
            try
            {
                DateTime date = GetDateTakenFromImage(path);
                string year = date.Year.ToString();
                string month = date.Month.ToString();

                //copying new image to output directory
                string newFile = Path.GetFileName(path);
                String dest = outputDir + @"\" + year + @"\" + month;
                response = CreateFolder(dest, out result);
                if (result == false)
                    return response;

                //checks if a picture with the same name is already exists in folder. If it does, add the name
                // of the picture the number of appearence in this folder.
                int i = 2;
                while (File.Exists(dest + @"\" + newFile))
                {
                    newFile = Path.GetFileNameWithoutExtension(path) + " (" + i + ")" + Path.GetExtension(path);
                    i++;
                }

                File.Copy(path, dest + @"\" + newFile);

                //creating thumbnail directory
                dest = outputDir + @"\Thumbnail\" + year + @"\" + month;
                response = addingThumbCopyToThumbnailFolder(path, dest, out result);
                if (result == false)
                    return response;
            }
            catch (Exception e)
            {
                result = false;
                return e.ToString();
            }

            result = true;
            return response;
        }

        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                // Get the Date Created property 
                PropertyItem propertyItem = myImage.PropertyItems.FirstOrDefault(i => i.Id == 306);
                if (propertyItem != null)
                {
                    // Extract the property value as a String. 
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    string text = encoding.GetString(propertyItem.Value, 0, propertyItem.Len - 1);

                    // Parse the date and time. 
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    return DateTime.ParseExact(text, "yyyy:MM:d H:m:s", provider);
                }
                else
                {
                    //If the creation time dosen't exists, the method will return the copy date.
                    return DateTime.Now;
                }
            }
        }

        //If it doesn't exists - create new folder in the destination path.
        public string CreateFolder(string dest, out bool result)
        {
            try
            {
                System.IO.Directory.CreateDirectory(dest);
            }
            catch (Exception e)
            {
                result = false;
                return "faild to create directory. error: " + e.ToString();
            }

            result = true;
            return "directory created successfully.";
        }

        //Create a thumb size copy of the picture and save it in the 'Thumbnail' filder
        public string addingThumbCopyToThumbnailFolder(string path, string dest, out bool result)
        {
            try
            {
                string response = CreateFolder(dest, out result);
                if (result == false)
                    return response;

                string thumbPath = dest + @"\" + "thumb-" + Path.GetFileName(path);

                FileInfo imageInfo = new FileInfo(path);
                Image image = Image.FromFile(path);
                int size;
                Int32.TryParse(ConfigurationManager.AppSettings["thumbnailSize"], out size);
                Image thumb = image.GetThumbnailImage(size, size, () => false, IntPtr.Zero);
                thumb.Save(thumbPath);
            }
            catch (Exception e)
            {
                result = false;
                return "faild to create thumb picture in thumbnail folder. error: " + e.ToString();
            }

            result = true;
            return "Thumb copy is saved in the 'Thumbnail' folder.";
        }
    }
}