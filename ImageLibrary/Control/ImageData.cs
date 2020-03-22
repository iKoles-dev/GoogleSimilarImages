namespace ImageLibrary.Control
{
    using System;
    using System.IO;

    public class ImageData
    {
        public string ImagePath { get; private set; }
        public string XmlPath { get; private set; }


        public ImageData(string imagePath)
        {
            ImagePath = imagePath;
            string filename = Path.GetFileName(ImagePath);
            string xmlName = filename.Split('.')[0]+".xml";
            if (File.Exists(ImagePath.Replace(filename, xmlName)))
            {
                XmlPath = ImagePath.Replace(filename, xmlName);
            }
            else
            {
                XmlPath = "";
            }
        }

        public void Delete()
        {
            try
            {

                File.Delete(ImagePath);
                if (File.Exists(XmlPath))
                {
                    File.Delete(XmlPath);
                }
            }
            catch (Exception) { }
        }
    }
}