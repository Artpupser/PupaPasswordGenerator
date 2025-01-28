using System.Security.Cryptography;
using System.Text;

namespace PupaPasswordGenerator
{

   class Program
   {
      private static async Task Main(string[] args)
      {
         Console.WriteLine("Генератор паролей :)");
         Console.Write("Укажите версию: ");
         var ver = Console.ReadLine()!;
         while (true)
         {
            Console.Write("Length [20-128]: ");
            var length = int.Parse(Console.ReadLine()!);
            if (length < 20 || length > 128)
            {
               Console.Write("length not in diaposone");
               continue;
            }
            Console.Write("Secret word: ");
            var code = Console.ReadLine()!;
            Console.Write("Birthday, example: [D M Y]: ");
            var birthday = string.Join('.', Console.ReadLine()!.Split(' '));
            var time = DateTime.Now;
            Console.WriteLine($"secret: {PasswordGeneration(ver, length, code, birthday, time)}");
            Console.WriteLine($"time creation: {time.Year} {time.Month} {time.Day} {time.Hour}");
         }
      }
      private static string PasswordGeneration(string version, int length, string code, string birthday, DateTime time)
      {
         var request = $"{string.Join(' ', Encoding.UTF8.GetBytes(version).Select(x => x.ToString()))} {code} {birthday} {length} {time.Year} {time.Month} {time.Day} {time.Hour}";
         var hash = ComputeSha512Hash(request);
         var rnd = new Random(length);
         var sb = new StringBuilder();
         for (var i = 0; i < hash.Length; i++)
         {
            var symbol = hash[i].ToString();
            sb.Append(rnd.Next(int.MinValue, int.MaxValue) % 10 == 0 ? symbol.ToUpper() : symbol);
         }
         return sb.ToString()[0..length];
      }
      private static string ComputeSha512Hash(string rawData)
      {
         IEnumerable<byte> bytes = SHA512.HashData(Encoding.UTF8.GetBytes(rawData));
         var builder = new StringBuilder();
         builder.AppendJoin(string.Empty, bytes.Select(x => x.ToString("x2")));
         return builder.ToString();
      }
   }

}
