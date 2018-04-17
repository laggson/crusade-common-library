using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Laggson.Common
{
   public static class Http
   {
      /// <summary>
      /// Ruft von der angegebenen Adresse ab und wandelt sie in eine Liste von Objekten des angegebenen Typs um.
      /// </summary>
      /// <typeparam name="T">Der Typ der Objekte, der erwartet wird.</typeparam>
      /// <param name="url">Die Adresse, von der abgerufen werden soll.</param>
      /// <returns>Eine Aufzählung aller gefundener Items.</returns>
      public static IList<T> ReadItems<T>(string url)
      {
         List<T> data;

         using (var client = new WebClient())
         {
            client.BaseAddress = url;
            client.Encoding = Encoding.UTF8;

            try
            {
               var response = client.DownloadString(url);
               data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(response);
            }
            catch (Exception e)
            {
               if (e is WebException we)
                  throw we;

               data = null;
            }
         }

         return data;
      }

      public static T PostItem<T>(T item, string url)
      {
         using (var client = new WebClient())
         {
            client.BaseAddress = url;
            client.Encoding = Encoding.UTF8;
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            try
            {
               var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
               var result = client.UploadString(url, "POST", json);

               return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception e)
            {
               if (e is WebException we)
                  throw we;

               return default;
            }

         }
      }
   }
}
