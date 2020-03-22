namespace SimilarImages
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading;

    using CsvHelper;

    public class ImageController
    {
        private List<Image> _allImages = new List<Image>();
        private HashSet<string> _allGoogleImagesLinks = new HashSet<string>();
        private List<CsvImage> _csv = new List<CsvImage>();

        private string _path = "";
        private int badDownload = 0;
        private int count = 0;

        private string _saveFilesDirectory;

        public void SetPath(string path)
        {
            _path = path;
            if (path.ToLower().EndsWith(".jpg") || path.ToLower().EndsWith(".jpeg") || path.ToLower().EndsWith(".png"))
            {
                Console.WriteLine($"Found images at path {path}.");
                _allImages.Add(new Image(path));
            }
            else if (Directory.Exists(path))
            {
                List<string> fileNames = new List<string>(Directory.GetFiles(path));
                fileNames.ForEach(
                    file =>
                        {
                            if (file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".jpeg")
                                                                || file.ToLower().EndsWith(".png"))
                            {
                                Console.WriteLine($"Found images at path {file}.");
                                _allImages.Add(new Image(file));
                            }
                        });
            }
            Console.WriteLine($"Found {_allImages.Count} images.");
        }

        public void StartParsing()
        {
            _allImages.ForEach(
                image =>
                    {
                        image.StartSearching();
                        foreach (var imageCrowledImageLink in image.CrowledImageLinks)
                        {
                            _allGoogleImagesLinks.Add(imageCrowledImageLink);
                        }
                    });
            Console.WriteLine($"Found {_allGoogleImagesLinks.Count} links in total.");
        }

        public void StartDownloadImage()
        {
            _saveFilesDirectory = Directory.GetCurrentDirectory() + "/"+ Path.GetFileName(_path) + " " + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "/";
            Directory.CreateDirectory(_saveFilesDirectory);
            foreach (var allGoogleImagesLink in _allGoogleImagesLinks)
            {
                string param = allGoogleImagesLink;
                new Thread(
                    () =>
                        {
                            DownloadCheck(param);
                            count++;
                        }).Start();
                Thread.Sleep(100);
            }

            while (count<_allGoogleImagesLinks.Count)
            {
                int tempCount = count;
                Thread.Sleep(10000);
                if (tempCount == count && count < _allGoogleImagesLinks.Count)
                {
                    count = _allGoogleImagesLinks.Count;
                }
            }
            Console.WriteLine("Download finished!");
        }

        private void DownloadCheck(string allGoogleImagesLink)
        {
            string imageName = allGoogleImagesLink.Split("/")[allGoogleImagesLink.Split("/").Length - 1];
            imageName = new Random().Next().ToString() + imageName;
            Downloader downloader = new Downloader(allGoogleImagesLink, _saveFilesDirectory + imageName);
            if (downloader.Download())
            {
                _csv.Add(new CsvImage(allGoogleImagesLink, _saveFilesDirectory + imageName));
            }
            else
            {
                badDownload++;
            }

            if (count == _allGoogleImagesLinks.Count)
            {
                return;
            }
            Console.WriteLine($"Currently downloading {(((badDownload + _csv.Count) * 1.0f / _allGoogleImagesLinks.Count) * 100).ToString("##.00")}%");
        }

        public void CreateMetaFile()
        {
            var fileCreation = File.Create(_saveFilesDirectory + "meta.csv");
            fileCreation.Dispose();
            using (var writer = new StreamWriter(_saveFilesDirectory + "meta.csv"))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(_csv);
                }
            }
        }
    }
}