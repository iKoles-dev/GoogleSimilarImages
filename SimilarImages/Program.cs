using Homebrew;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

//Client ID: b9ef8e8cf59266c
//Client secret: 659bbeeb7ef531e699bcc3b5720a18077ae55e7d

namespace SimilarImages
{
    class Program
    {
        static void Main(string[] args)
        {
            bool correctProxyPath = true;
            do
            {
                Console.WriteLine("Enter path to your proxy-file.");
                correctProxyPath = Proxies.SetProxyFile(Console.ReadLine().Replace("\"", "").Replace("'", ""));
            }
            while (!correctProxyPath);
            Console.WriteLine("Enter path to photo or directory with photos.");
            ImageController imageController = new ImageController();
            imageController.SetPath(Console.ReadLine().Replace("\"", "").Replace("'", ""));
            Console.WriteLine("Crawling has been started");
            imageController.StartParsing();
            Console.WriteLine("Downloading has been started.");
            imageController.StartDownloadImage();
            Console.WriteLine("Creating meta-file.");
            imageController.CreateMetaFile();
            Console.WriteLine("Work has been done!");
        }
    }
}
