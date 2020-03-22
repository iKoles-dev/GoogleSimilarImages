namespace ImageLibrary.Control
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using CsvHelper;

    public class LibraryController
    {
        private string _workDirectory;
        private List<ImageFolder> _allImageFolders = new List<ImageFolder>();
        private List<LibraryCsv> _libraryCsv = new List<LibraryCsv>();

        public LibraryController(string workDirectory)
        {
            _workDirectory = workDirectory;
            if (Directory.Exists(_workDirectory))
            {
                Console.WriteLine("Directory loaded successfully.");
            }
            else
            {
                Console.WriteLine("Directory doesn't exist. Restart program and enter valid directory path.");
            }
        }

        public void Start()
        {
            ParsAllFiles();
            RebuildMetaCsv();
            DeleteDuplicates();
            SaveMetas();
            CreateLibraryCsv();
            WriteLibrary();
        }

        private void ParsAllFiles()
        {
            List<string> _allDirectories = new List<string>(Directory.GetDirectories(_workDirectory));
            _allDirectories.ForEach(
                directory =>
                    {
                        ImageFolder imageFolder = new ImageFolder(directory);
                        if (imageFolder.IsWorkDirectory)
                        {
                            _allImageFolders.Add(imageFolder);
                        }
                    });
            Console.WriteLine($"Found {_allImageFolders.Count} work directories.");
        }

        private void RebuildMetaCsv()
        {
            _allImageFolders.ForEach(
                folder =>
                    {
                        folder.SearchPhotoMetaData(_allImageFolders);
                    });
        }

        private void DeleteDuplicates()
        {
            for (int i = 0; i < _allImageFolders.Count; i++)
            {
                List<ImageFolder> foldersToCompare = new List<ImageFolder>();
                for (int j = i+1; j < _allImageFolders.Count; j++)
                {
                    foldersToCompare.Add(_allImageFolders[j]);
                }
                _allImageFolders[i].DeleteDuplicates(foldersToCompare);
            }
        }

        private void SaveMetas()
        {
            _allImageFolders.ForEach(
                folder =>
                    {
                        folder.SaveMeta();
                    });
        }

        private void CreateLibraryCsv()
        {
            _allImageFolders.ForEach(
                imageFolder =>
                    {
                        _libraryCsv.AddRange(imageFolder.GetLibrary());
                    });
        }

        private void WriteLibrary()
        {
            if (File.Exists(_workDirectory + "/Library.csv"))
            {
                File.Delete(_workDirectory + "/Library.csv");
            }

            File.Create(_workDirectory + "/Library.csv").Dispose();
            using (var writer = new StreamWriter(_workDirectory + "/Library.csv"))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(_libraryCsv);
                }
            }

        }
    }
}