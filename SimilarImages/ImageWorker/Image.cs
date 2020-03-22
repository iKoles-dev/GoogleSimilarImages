namespace SimilarImages
{
    using System;
    using System.Collections.Generic;

    using Homebrew;

    public class Image
    {
        private string _imagePath;

        private string _imageUrl;
        public HashSet<string> CrowledImageLinks { get; private set; } = new HashSet<string>();
        public Image(string imagePath)
        {
            _imagePath = imagePath;
        }

        public void StartSearching()
        {
            GetImageUrl();
            string googleUrl = UploadImageToGoogle();
            GetAllGoogleImageLinks(googleUrl);
        }

        private void GetImageUrl()
        {
            ImgurUploader imgurUploader = new ImgurUploader(_imagePath);
            _imageUrl = imgurUploader.ImageLink;
            Console.WriteLine("Succesfull upload image to imgur.");
        }

        private string UploadImageToGoogle()
        {
            try
            {
                Console.Write("Trying connection to new proxy: ");
                ReqParametres req = new ReqParametres($"https://www.google.com/searchbyimage?image_url={_imageUrl}");
                req.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.53 Safari/537.36");
                //req.SetProxy(5000, Proxies.GetProxy());
                LinkParser link = new LinkParser(req.Request);
                if (link.IsError)
                {
                    Proxies.DeleteFirstProxy();
                    return UploadImageToGoogle();
                }

                if (link.Data.ParsFromTo("role=\"heading\"><a href=\"", "\"") == "")
                {
                    Proxies.DeleteFirstProxy();
                    return UploadImageToGoogle();
                }
                Console.WriteLine("Succes!");
                return "https://www.google.com" + link.Data.ParsFromTo("role=\"heading\"><a href=\"", "\"").Replace("amp;","");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Proxies.DeleteFirstProxy();
                return UploadImageToGoogle();
            }
        }

        private void GetAllGoogleImageLinks(string googleUrl)
        {
            for (int i = 0; i < 10000; i+=100)
            {
                try
                {
                    Console.WriteLine($"Trying to get page {i/100} of Google search results.");
                    ReqParametres req;
                    if (i == 0)
                    {
                        req = new ReqParametres($"{googleUrl}&async=_id:rg_s,_pms:s,_jsfs:Ffpdje,_fmt:pc&asearch=ichunk");
                    }
                    else
                    {
                        req = new ReqParametres($"{googleUrl}&start={i}&ijn={i / 100}&async=_id:rg_s,_pms:s,_jsfs:Ffpdje,_fmt:pc&asearch=ichunk");
                    }
                    req.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.53 Safari/537.36");
                    //req.SetProxy(3000, Proxies.GetProxy());
                    LinkParser linkParser = new LinkParser(req.Request);
                    if (linkParser.IsError)
                    {
                        throw new Exception();
                    }
                    List<string> allLinksOnSite = linkParser.Data.ParsRegex("\"ou\":\"(.*?)\"", 1);
                    if (allLinksOnSite.Count == 0)
                    {
                        break;
                    }

                    allLinksOnSite.ForEach(
                        x =>
                            {
                                if (x.ToLower().EndsWith(".jpg") || x.ToLower().EndsWith(".jpeg")
                                                                 || x.ToLower().EndsWith(".png"))
                                {

                                    CrowledImageLinks.Add(x);
                                }
                            });
                    Console.WriteLine($"{_imagePath}.....Found {CrowledImageLinks.Count} images.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed.");
                    Proxies.DeleteFirstProxy();
                    i -= 100;
                    continue;
                }

            }
        }
    }
}