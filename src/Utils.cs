using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace PupaPasswordGenerator.Utils {
   #region UTILS
   public static class Utils {
      public static Task<string> InputStr(string prompt) {
         while(true) {
            Console.Out.Write($"{prompt} >> ");
            var res = Console.ReadLine();
            if(string.IsNullOrWhiteSpace(res)) {
               FailMessage($"Строка пустая");
               continue;
            }
            return Task.FromResult(res);
         }
      }
      public static void FailMessage(string message) {
         Console.ForegroundColor = ConsoleColor.Red;
         Console.Out.WriteLine(message);
         Console.ResetColor();
      }
      public static async Task<int> InputInt(string prompt, Vector2 range) {
         while(true) {
            var str = await InputStr(prompt);
            if(int.TryParse(str, out var res) == false) {
               FailMessage($"Это не Int32");
               continue;
            }
            if(range.X > res || range.Y < res) {
               FailMessage($"от {range.X} до {range.Y} ваш {res}");
               continue;
            }
            return res;
         }
      }
      public static string ComputeSha512Hash(string rawData) {
         IEnumerable<byte> bytes = SHA512.HashData(Encoding.UTF8.GetBytes(rawData));
         var builder = new StringBuilder();
         builder.AppendJoin(string.Empty, bytes.Select(x => x.ToString("x2")));
         return builder.ToString();
      }
   }
   #endregion
}