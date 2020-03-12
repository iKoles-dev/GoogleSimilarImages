namespace SimilarImages
{
    using System;

    using Homebrew;

    using RestSharp;

    public class ImgurUploader
    {
        public string ImageLink { get; private set; }
        public ImgurUploader(string imagePath)
        {
            string base64 = GetBase64StringForImage(imagePath);
            string imageLink = Upload(base64);
            ImageLink = imageLink.ParsFromTo("\"link\":\"", "\"").Replace("\\\\", "").Replace("\\", "");
        }
        private string GetBase64StringForImage(string imgPath)
        {
            byte[] imageBytes = System.IO.File.ReadAllBytes(imgPath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        private string Upload(string base64)
        {
            var client = new RestClient("https://api.imgur.com/3/image");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Client-ID b9ef8e8cf59266c");
            request.AlwaysMultipartFormData = true;
            request.AddParameter("image", base64);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}