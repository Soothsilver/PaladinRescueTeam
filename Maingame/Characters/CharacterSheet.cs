using System.Collections.Generic;

namespace Origin.Characters
{
    public class CharacterSheet
    {
        public string Name;
        public CharacterClass Class;
        public bool Exhausted;
        public bool Wounded;
        public int PowerPoints = 0;
        public List<PowerName> Powers = new List<PowerName>();
        public bool CanSelectNewPowers => Powers.Count < 3 && (PowerPoints > 0 || Treasure.Instance.CheatMode);

        public string DescribeSelf()
        {
            return "{b}" + this.Name + "{/b}\nClass: {b}" + this.Class.Humanize() + "{/b}\n"
                   + (this.Exhausted ? " (Exhausted!)" : "")
                   + (this.Wounded ? " (Wounded and bleeding!)" : "")
                   + (this.CanSelectNewPowers ? " (can select new powers)" : "");
        }

        public static CharacterSheet CreateWarrior()
        {
            return new CharacterSheet()
            {
                Name = RandomNameGenerator.Generate(),
                Class = CharacterClass.Warrior,
                Powers = new List<PowerName>
                {
                    PowerName.StrongBody
                }
            };
        }

        public static CharacterSheet CreateBlueMage()
        {
            return new CharacterSheet()
            {
                Name = RandomNameGenerator.Generate(),
                Class = CharacterClass.BlueWizard,
                Powers = new List<PowerName>
                {
                    PowerName.CastWater
                }
            };
        }
    }
}