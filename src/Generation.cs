using System.Reflection;
using System.Text;
using static PupaPasswordGenerator.Utils.Utils;

namespace PupaPasswordGenerator.Generations;

#region GENERATOR
[AttributeUsage(AttributeTargets.Method)]
public sealed class GeneratorModelAttribute(string model) : Attribute {
   public string Model { get; } = model;
}
public static class GeneratorManager {
   public static (GeneratorModelAttribute, Func<string, int, string, string, DateTime, string>) GetGeneratorFromModel(string model) {
      var models = typeof(GeneratorManager).GetMethods().Where(x => x.GetCustomAttribute<GeneratorModelAttribute>() != null).Select(x => (x, x.GetCustomAttribute<GeneratorModelAttribute>())).Where(x => x.x != null);
      var item = models.FirstOrDefault(x => x.Item2?.Model == model);
      return new(item.Item2!, item.x.CreateDelegate<Func<string, int, string, string, DateTime, string>>());
   }
   public static bool IsExitis(string model) {
      var models = typeof(GeneratorManager).GetMethods().Where(x => x.GetCustomAttribute<GeneratorModelAttribute>() != null).Select(x => (x, x.GetCustomAttribute<GeneratorModelAttribute>())).Where(x => x.x != null);
      return models.FirstOrDefault(x => x.Item2?.Model == model) != default;
   }
   public static IEnumerable<string> GetNames() => typeof(GeneratorManager).GetMethods().Where(x => x.GetCustomAttribute<GeneratorModelAttribute>() != null).Select(x => x.GetCustomAttribute<GeneratorModelAttribute>()!.Model);
   [GeneratorModel("alpha")]
   public static string PasswordGenerationAlpha(string version, int length, string code, string birthday, DateTime time) {
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
   [GeneratorModel("beta")]
   public static string PasswordGenerationBeta(string version, int length, string code, string birthday, DateTime time) {
      var request = $"{string.Join(' ', Encoding.UTF8.GetBytes(version).Select(x => x.ToString()))} {code} {birthday} {length} {time.Year} {time.Month} {time.Day} {time.Hour}";
      var hash = ComputeSha512Hash(request);
      var rnd = new Random(length);
      var sb = new StringBuilder();
      for(var i = 0; i < hash.Length; i++) {
         var symbol = hash[i].ToString();
         if(int.TryParse(symbol, out _)) {
            sb.Append(symbol);
            continue;
         }
         sb.Append(rnd.Next(int.MinValue, int.MaxValue) % 5 == 0 ? symbol.ToUpper() : symbol);
      }
      return sb.ToString()[0..length];
   }
}
#endregion