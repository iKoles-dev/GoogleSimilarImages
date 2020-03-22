using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Homebrew
{
    using System.IO;
    using System.Threading;

    public static class Proxies
    {
        private static  List<string> allProxies = new List<string>();

        public static bool SetProxyFile(string fileLink)
        {
            if (File.Exists(fileLink))
            {
                string proxiesInText = File.ReadAllText(fileLink);
                string[] proxies = proxiesInText.Split("\n");
                if (proxies.Length == 0)
                {
                    Console.WriteLine("No proxies found");
                    return false;
                }
                allProxies.AddRange(proxies);
                Console.WriteLine($"Loaded {proxies.Length} proxies");
                return true;
            }
            else
            {
                return false;
            }
        }

        //private static void CheckProxy()
        //{
        //    Console.WriteLine("Start proxy checking.");
        //    new Thread((() =>
        //                       {
        //                           rawProxies.ForEach(
        //                               proxy =>
        //                                   {
        //                                       new Thread((() =>
        //                                                          {
        //                                                              try
        //                                                              {
        //                                                                  ReqParametres req = new ReqParametres("https://www.google.com/search?&q=gmail");
        //                                                                  req.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.53 Safari/537.36");
        //                                                                  req.SetProxy(5000, proxy);
        //                                                                  LinkParser link = new LinkParser(req.Request);
        //                                                                  if (!link.IsError && link.Data.Contains("is email that"))
        //                                                                  {
        //                                                                      allProxies.Add(proxy);
        //                                                                  }

        //                                                              }
        //                                                              catch (Exception) { }
        //                                                          })).Start();
        //                                   });
        //                       } )).Start();
        //    while (allProxies.Count < 10)
        //    {
        //        Thread.Sleep(100);
        //    }
        //}
        public static string GetProxy()
        {
            if (allProxies.Count > 0)
            {
                string firstProxy = allProxies[0];
                Console.WriteLine(firstProxy);
                return firstProxy;
            }
            else
            {
                Console.WriteLine("All proxies used. Enter path to new proxy file");
                string newPath = Console.ReadLine();
                SetProxyFile(newPath);
                return GetProxy();
            }
        }
        public static void DeleteFirstProxy()
        {
            if (allProxies.Count > 0)
            {
                allProxies.RemoveAt(0);
            }
        }

    }
}
