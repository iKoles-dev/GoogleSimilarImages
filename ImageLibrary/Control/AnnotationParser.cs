namespace ImageLibrary.Control
{
    using System.Collections.Generic;
    using System.IO;

    using Homebrew;

    public class AnnotationParser
    {
        public string Name { get; private set; } = "";
        public int Defects { get; private set; } = 0;

        public int[] Parametres { get; private set; } = new int[6] {0,0,0,0,0,0};
        private string _annotationPath;
        public AnnotationParser(string annotationPath)
        {
            _annotationPath = annotationPath;
            Pars();
        }

        private void Pars()
        {
            if (File.Exists(_annotationPath))
            {
                string xml = File.ReadAllText(_annotationPath).Replace("\n","").Replace("\r","");
                Name = xml.ParsFromTo("<name>", "</name>");
                List<string> defects = xml.ParsRegex("Defect(.*?)Defect", 1);
                Defects = defects.Count;
                defects.ForEach(
                    defect =>
                        {
                            SetParameter(defect.ParsFromTo("<Background>", "</Background>"), 0);
                            SetParameter(defect.ParsFromTo("<Crack>", "</Crack>"), 1);
                            SetParameter(defect.ParsFromTo("<Spallation>", "</Spallation>"), 2);
                            SetParameter(defect.ParsFromTo("<Efflorescence>", "</Efflorescence>"), 3);
                            SetParameter(defect.ParsFromTo("<ExposedBars>", "</ExposedBars>"), 4);
                            SetParameter(defect.ParsFromTo("<CorrosionStain>", "</CorrosionStain>"), 5);
                        });
            }
        }

        private void SetParameter(string parameter, int position)
        {
            if (int.TryParse(parameter, out int para))
            {
                Parametres[position] += para;
            }
        }
    }
}