namespace SimilarImages
{
    public class CsvImage
    {
        public string Link { get; set; }
        public string Path { get; set; }

        public CsvImage(string link, string path)
        {
            Link = link;
            Path = path;
        }
    }
}