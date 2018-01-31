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
      public static IEnumerable<T> ReadItems<T>(string url)
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
            catch
            {
               data = null;
            }
         }

         return data;
      }
   }
}
