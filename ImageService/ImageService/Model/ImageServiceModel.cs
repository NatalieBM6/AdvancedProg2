using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Model
{
    public class ImageServiceModel : IImageServiceModel
    {

        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size
        private static Regex r = new Regex(":");
        #endregion

        /************************************************************************
        *The Input: The output directory and the thumbnail size.
        *The Output: -
        *The Function operation: The function builds an ImageServiceModel.
        *************************************************************************/
        public ImageServiceModel(string outputDir, int thumbnailSize)
        {
            m_thumbnailSize = thumbnailSize;
            m_OutputFolder = outputDir;
        }

        /************************************************************************
        *The Input: The directory of the files and whether it succeded.
        *The Output: If it exists then an error message.
        *The Function operation: The function copies the an image to the output
        *directory.
        *************************************************************************/
        public string AddFile(string path, out bool result)
        {
            try
            {
                //Get the pictures' date time.
                DateTime picTime = GetDateTakenFromImage(path);
                //Creating strings in order to create the directory.
                string imageName = Path.GetFileName(path);
                string picFolder = Path.Combine(picTime.Year.ToString(), picTime.Month.ToString());
                string picDestFolder = Path.Combine(m_OutputFolder, picFolder);
                string thumbFolderDest = Path.Combine(m_OutputFolder, "thumbnails", picFolder);
                string destPath = Path.Combine(picDestFolder, imageName);
                //Creating a folder for the picture and the thumbnail.
                Directory.CreateDirectory(thumbFolderDest);
                Directory.CreateDirectory(picDestFolder);
                // If the file already exists we'll give it a new name.
                if (File.Exists(destPath))
                {
                    destPath = DuplicateFile(destPath);
                }
                //Saving the created thumbnail and moving the image.
                File.Move(path, destPath);
                SaveThumbnail(destPath, Path.Combine(thumbFolderDest, imageName));
                result = true;
                return destPath;
            }
            catch (Exception msg)
            {
                result = false;
                return msg.Message;
            }

        }

        /************************************************************************
        *The Input: a path to an image.
        *The Output: The datetime.
        *The Function operation: The function finds the image's datetime.
        *************************************************************************/
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                try
                {
                    System.Drawing.Imaging.PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
                catch
                {
                    //Incase the image doesn't have a date return the files date.
                    return File.GetCreationTime(path);
                }

            }
        }

        /************************************************************************
        *The Input: a path to an image and a path to a thumbnail.
        *The Output: -.
        *The Function operation: The function saves the thumbnail according to the
        *thumbnail size.
        *************************************************************************/
        private void SaveThumbnail(string path, string destDir)
        {
            Image myImage = Image.FromFile(path);
            Image thumb = myImage.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);
            thumb.Save(destDir);
            //Releasing the image.
            myImage.Dispose();
        }

        /************************************************************************
        *The Input: a file path and a path to a new file.
        *The Output: -.
        *The Function operation: The function handles a situation where is a file
        *with the same name as the new file.
        *************************************************************************/
        private string DuplicateFile(string path)
        {
            int i = 1;
            string imageName = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            string pathFolder = Path.GetDirectoryName(path);
            //While the file exists- rename it. 
            while (File.Exists(path))
            {
                imageName = imageName + "(" + i.ToString() + ")";
                path = Path.Combine(pathFolder, imageName + extension);
                i++;
            }
            return path;
        }
    }
}
