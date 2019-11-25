using Cother;
using Origin.Characters;

namespace Origin
{
    public class RandomNameGenerator
    {
        public static string Generate()
        {
            string[] names = new[]
                {"Jen", "Skyla", "Loreos", "Niktian", "Salldronin", "Anna", "Morr", "Quel'shen", "Ellion", "Nasher"};
            return names.GetRandom();
        }
    }
}