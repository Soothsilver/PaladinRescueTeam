using System;
using System.Collections.Generic;
using Origin.Characters;

namespace Origin
{
    [Serializable]
    public class Treasure
    {
        public static Treasure Instance { get; private set; } = LoadTreasure();

        public int LastCompletedLevel = 0;
        public bool ReadInstructions = false;
     //   public List<CharacterSheet> Characters = new List<CharacterSheet>();
        public bool CheatMode = false;

        public bool IsFirstLaunch => LastCompletedLevel == -1;
        public bool ShowFireMode { get; set; }
        public bool TimeDilationFast { get; set; }

        public Treasure()
        {
        }
        
        private static Treasure LoadTreasure()
        {
            Treasure treasure = new Treasure();
            treasure = XmlSave.LoadSettings();
            /*
            if (treasure.Characters.Count == 0)
            {
                AddBasicCharacters(treasure);
            }*/
            return treasure;
        }
/*
        private static void AddBasicCharacters(Treasure treasure)
        {
            treasure.Characters.Add(new CharacterSheet()
            {
                Name = RandomNameGenerator.Generate(),
                Class = CharacterClass.Warrior,
                Powers = new List<PowerName>
                {
                    PowerName.StrongBody
                }
            });
            treasure.Characters.Add(new CharacterSheet()
            {
                Name = RandomNameGenerator.Generate(),
                Class = CharacterClass.Warrior,
                Powers = new List<PowerName>
                {
                    PowerName.StrongBody
                }
            });
            treasure.Characters.Add(new CharacterSheet()
            {
                Name = RandomNameGenerator.Generate(),
                Class = CharacterClass.BlueWizard,
                Powers = new List<PowerName>
                {
                    PowerName.CastWater
                }
            });
        }
*/
        public void Save()
        {
            XmlSave.SaveSettings(this);
        }

        public static void Clear()
        {
            Instance = new Treasure();
         //   AddBasicCharacters(Instance);
        }
    }
}