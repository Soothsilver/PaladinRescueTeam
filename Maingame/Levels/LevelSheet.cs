using System.Collections.Generic;
using Origin.Characters;

namespace Origin.Levels
{
    public class LevelSheet
    {
        public string MapFileName { get; }
        public List<CharacterSheet> AllPaladins { get; set; }

        public string Name;
        public string Intro;
        public int NumberOfPaladins;
        public string Id;

        public LevelSheet(string id, string name, string intro, int numberOfPaladins, string mapFileName, List<CharacterSheet> possiblePaladins)
        {
            Id = id;
            MapFileName = mapFileName;
            AllPaladins = possiblePaladins;
            Name = name;
            Intro = intro;
            NumberOfPaladins = numberOfPaladins;
        }
    }
}