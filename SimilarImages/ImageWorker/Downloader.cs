namespace SimilarImages
{
    using System;
    using System.Net;
    using System.Threading;

    public class Downloader
    {
        private string _imageLink;

        private string _pathToSaveImage;

        public int check = 0;
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

                check++;
                return true;
            }
            catch (Exception)
            {
                check++;
                return false;
            }
        }
    }
}