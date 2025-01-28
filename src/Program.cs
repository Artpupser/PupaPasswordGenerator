using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using TextCopy;

namespace PupaPasswordGenerator {

   internal static class Program {
      #region BOOT
      private static async Task Main(string[] args) {
         Console.WriteLine("Генератор паролей :)");
         await Task.Delay(1000);
         var version = await InputStr("Версия");
         while(true) {
            var length = await InputInt("Длинна пароля (20-120)", new(20, 120));
            var code = await InputStr("Секретный код");
            var birthday = string.Join('.', (await InputStr("День рождение (День Mесяц Год)")).Split(' '));
            var time = DateTime.Now;
            var result = PasswordGeneration(version, length, code, birthday, time);
            Console.WriteLine($"Пароль: {result}");
            Console.WriteLine($"Дата создания: {time.Year} {time.Month} {time.Day} {time.Hour}");
            Console.WriteLine($"Необходимые данный скопированны в буфер обмена.");
            await ClipboardService.SetTextAsync($"Password: {result}\nLength: {length}\nTime: {time.Year} {time.Month} {time.Day} {time.Hour}\nVersion: {version}");
         }
      }
      #endregion
      #region GENERATORS
      private static string PasswordGeneration(string version, int length, string code, string birthday, DateTime time) {
         var request = $"{string.Join(' ', Encoding.UTF8.GetBytes(version).Select(x => x.ToString()))} {code} {birthday} {length} {time.Year} {time.Month} {time.Day} {time.Hour}";
         var hash = ComputeSha512Hash(request);
         var rnd = new Random(length);
         var sb = new StringBuilder();
         for(var i = 0; i < hash.Length; i++) {
            var symbol = hash[i].ToString();
            sb.Append(rnd.Next(int.MinValue, int.MaxValue) % 10 == 0 ? symbol.ToUpper() : symbol);
         }
         return sb.ToString()[0..length];
      }
      #endregion
      #region UTILS
      private static Task<string> InputStr(string prompt) {
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
      private static void FailMessage(string message) {
         Console.ForegroundColor = ConsoleColor.Red;
         Console.Out.WriteLine(message);
         Console.ResetColor();
      }
      private static async Task<int> InputInt(string prompt, Vector2 range) {
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
      private static string ComputeSha512Hash(string rawData) {
         IEnumerable<byte> bytes = SHA512.HashData(Encoding.UTF8.GetBytes(rawData));
         var builder = new StringBuilder();
         builder.AppendJoin(string.Empty, bytes.Select(x => x.ToString("x2")));
         return builder.ToString();
      }
      #endregion
   }

}
