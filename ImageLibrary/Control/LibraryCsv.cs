namespace ImageLibrary.Control
{
    public class LibraryCsv
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Defect { get; set; }
        public int Background { get; set; }
        public int Crack { get; set; }
        public int Spallation { get; set; }
        public int Efflorescence { get; set; }
        public int ExposedRebar { get; set; }
        public int CorrosionStain { get; set; }
        public string Url { get; set; }

        public void SetXmlParametres(string xmlPath)
        {
            AnnotationParser annotation = new AnnotationParser(xmlPath);
            Name = annotation.Name;
            Defect = annotation.Defects.ToString();
            Background = annotation.Parametres[0];
            Crack = annotation.Parametres[1];
            Spallation = annotation.Parametres[2];
            Efflorescence = annotation.Parametres[3];
            ExposedRebar = annotation.Parametres[4];
            CorrosionStain = annotation.Parametres[5];
        }
    }
}