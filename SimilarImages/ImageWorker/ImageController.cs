namespace SimilarImages
{
    using System;
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

        private string _saveFilesDirectory;

        public void SetPath(string path)
        {
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
            Console.WriteLine($"Totaly found {_allGoogleImagesLinks.Count} image links.");
        }

        public void StartDownloadImage()
        {
            _saveFilesDirectory = Directory.GetCurrentDirectory() + "/" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "/";
            Directory.CreateDirectory(_saveFilesDirectory);
            int badDownload = 0;
            foreach (var allGoogleImagesLink in _allGoogleImagesLinks)
            {
                string imageName = allGoogleImagesLink.Split("/")[allGoogleImagesLink.Split("/").Length - 1];
                imageName = new Random().Next().ToString()+imageName;
                Downloader downloader = new Downloader(allGoogleImagesLink, _saveFilesDirectory + imageName);
                new Thread((() =>
                                   {
                                       if (downloader.Download())
                                       {
                                           _csv.Add(new CsvImage(allGoogleImagesLink, _saveFilesDirectory + imageName));
                                       }
                                       else
                                       {
                                           badDownload++;
                                       }
                                       Console.WriteLine($"Currently downloaded {(((badDownload + _csv.Count)*1.0f/_allGoogleImagesLinks.Count)*100).ToString("##.00")}%");
                                   })).Start();
            }

            while (badDownload+_csv.Count!=_allGoogleImagesLinks.Count)
            {
                Thread.Sleep(100);
            }
            Console.WriteLine("Downloaded ended!");
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