namespace SimilarImages
{
    using System;
    using System.Net;

    public class Downloader
    {
        private string _imageLink;

        private string _pathToSaveImage;
        public Downloader(string imageLink, string pathToSaveImage)
        {
            _imageLink = imageLink;
            _pathToSaveImage = pathToSaveImage;
        }

        public bool Download()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(_imageLink), _pathToSaveImage);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}