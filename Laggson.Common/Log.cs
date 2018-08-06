using System;

namespace Laggson.Common
{
   public static class Log
   {
      public static void Write(string message)
      {
         string date = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
         Console.WriteLine(date + @": " + message);
      }
   }
}
