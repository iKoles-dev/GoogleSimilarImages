namespace ImageLibrary.Control
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using CsvHelper;

    using SimilarImages;

    public class ImageFolder
    {
        private string _folderPath = "";

        private string _metaCsvPath = "";
        private List<CsvImage> _metaCsv = new List<CsvImage>();
        private List<CsvImage> _newMetaCsv = new List<CsvImage>();
        private List<ImageData> _imageDatas = new List<ImageData>();
        public bool IsWorkDirectory { get; private set; } = false;


        public ImageFolder(string folderPath)
        {
            _folderPath = folderPath;
            List<string> allFilesInDirectory = new List<string>(Directory.GetFiles(_folderPath));
            allFilesInDirectory.ForEach(
                file =>
                    {
                        if (file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".jpeg")
                                                            || file.ToLower().EndsWith(".png"))
                        {
                            _imageDatas.Add(new ImageData(file));
                            IsWorkDirectory = true;
                        }
                        else if (file.ToLower().EndsWith("meta.csv"))
                        {
                            _metaCsvPath = file;
                            IsWorkDirectory = true;

                            using (var reader = new StreamReader(file))
                            {
                                using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
                                {
                                    csv.Configuration.Delimiter = ",";
                                    _metaCsv = csv.GetRecords<CsvImage>().ToList();
                                }
                            }
                        }
                    });

        }

        public void SearchPhotoMetaData(List<ImageFolder> allImageFolders)
        {
            _imageDatas.ForEach(
                imageData =>
                    {
                        string imageLink = "";
                        for (int i = 0; i < allImageFolders.Count; i++)
                        {
                            string tempImageLink = allImageFolders[i].GetImageDownloadLink(Path.GetFileName(imageData.ImagePath));
                            if (tempImageLink != "")
                            {
                                imageLink = tempImageLink;
                                break;
                            }
                        }
                        if (imageLink != "")
                        {
                            _newMetaCsv.Add(new CsvImage(imageLink,imageData.ImagePath));
                        }
                    });
        }

        public string GetImageDownloadLink(string imageName)
        {
            string imageLink = "";
            for (int i = 0; i < _metaCsv.Count; i++)
            {
                if (_metaCsv[i].Path.Contains(imageName))
                {
                    imageLink = _metaCsv[i].Link;
                    break;
                }
            }

            return imageLink;
        }

        public void DeleteDuplicates(List<ImageFolder> allImageFolders)
        {
            _newMetaCsv.ForEach(
                metaCsv =>
                    {
                        for (int i = 0; i < allImageFolders.Count; i++)
                        {
                            allImageFolders[i].DuplicatesCleaner(metaCsv.Link);
                        }
                    });
        }

        public void DuplicatesCleaner(string downloadLink)
        {
            for (int i = 0; i < _newMetaCsv.Count; i++)
            {
                if (_newMetaCsv[i].Link == downloadLink)
                {
                    for (int j = 0; j < _imageDatas.Count; j++)
                    {
                        if (_imageDatas[j].ImagePath == _newMetaCsv[i].Path)
                        {
                            _imageDatas[j].Delete();
                            _imageDatas.RemoveAt(j);
                            break;

                        }
                    }
                    _newMetaCsv.RemoveAt(i);
                    break;

                }
            }
        }

        public void SaveMeta()
        {
            try
            {
                if (File.Exists(_metaCsvPath))
                {
                    File.Delete(_metaCsvPath);
                }

                if (_newMetaCsv.Count > 0)
                {
                    var fileCreation = File.Create(_folderPath + "/meta.csv");
                    fileCreation.Dispose();
                    using (var writer = new StreamWriter(_folderPath + "/meta.csv"))
                    {
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.WriteRecords(_newMetaCsv);
                        }
                    }
                }
            }
            catch (Exception e) { }
        }

        public List<LibraryCsv> GetLibrary()
        {
            List<LibraryCsv> libraryCsv = new List<LibraryCsv>();
            _imageDatas.ForEach(
                image =>
                    {
                        LibraryCsv library = new LibraryCsv();
                        string downloadLink = GetImageDownloadLink(Path.GetFileName(image.ImagePath));
                        library.Url = downloadLink;
                        library.Path = image.ImagePath;
                        library.SetXmlParametres(image.XmlPath);
                        libraryCsv.Add(library);
                    });
            return libraryCsv;
        }

    }
}