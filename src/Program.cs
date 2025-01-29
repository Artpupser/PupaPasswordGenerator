using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using TextCopy;
using PupaPasswordGenerator.Generations;
using static PupaPasswordGenerator.Utils.Utils;

namespace PupaPasswordGenerator {

   #region BOOT
   internal static class Program {
      private static async Task Main(string[] args) {
         Console.WriteLine("Генератор паролей :)");
         await Task.Delay(TimeSpan.FromSeconds(1));
         var version = await InputStr("Версия");
         Console.WriteLine("Экспорт из фалйа? [Y/Any]");
         if(Console.ReadKey(true).Key == ConsoleKey.Y) {
            while(true) {
               var strings = await File.ReadAllLinesAsync($"{AppDomain.CurrentDomain.BaseDirectory}Input.txt");
               var birthday = string.Join('.', (await InputStr("День рождение (День Mесяц Год)")).Split(' '));
               var sb = new StringBuilder();
               for(var i = 0; i < strings.Length; i += 3) {
                  var code = strings[i];
                  var length = int.Parse(strings[i + 1]);
                  var time = DateTime.Now;
                  var password = GeneratorManager.GetGeneratorFromModel(strings[i + 2]).Item2.Invoke(version, length, code, birthday, time);
                  sb.AppendLine($"Version: {version}\nCode: {code}\nPassword: {password}\nLength: {length}\nTime: {time.Year} {time.Month} {time.Day} {time.Hour}\nModel: {strings[i + 2]}\n");
               }
               await File.WriteAllTextAsync($"{AppDomain.CurrentDomain.BaseDirectory}Output.txt", sb.ToString());
            }
         }
      start:
         var model = await InputStr($"Модель генератора [{string.Join(',', GeneratorManager.GetNames())}]");
         if(GeneratorManager.IsExitis(model) == false) {
            goto start;
         }
         var generator = GeneratorManager.GetGeneratorFromModel(model);
         while(true) {
            Console.WriteLine("Создание пороля");
            var length = await InputInt("Длинна пароля (20-120)", new(20, 120));
            var code = await InputStr("Секретный код");
            var birthday = string.Join('.', (await InputStr("День рождение (День Mесяц Год)")).Split(' '));
            var time = DateTime.Now;
            var result = generator.Item2.Invoke(version, length, code, birthday, time);
            Console.WriteLine($"Пароль: {result}");
            Console.WriteLine($"Дата создания: {time.Year} {time.Month} {time.Day} {time.Hour}");
            Console.WriteLine($"Необходимые данный скопированны в буфер обмена.");
            await ClipboardService.SetTextAsync($"Password: {result}\nLength: {length}\nTime: {time.Year} {time.Month} {time.Day} {time.Hour}\nVersion: {version}\nModel: {model}");
         }
      }
   }
   #endregion
}
