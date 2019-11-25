using System.Collections.Generic;
using Origin.Characters;

namespace Origin.Levels
{
    public class Campaign
    {
        public static List<LevelSheet> Levels = new List<LevelSheet>();

        static Campaign()
        {   
            Levels.Add(new LevelSheet(
                MonasteryTags.MonasteryLevelId,
                "Monastery (tutorial)",
                "Learn to play {b}Paladin Rescue Team{/b} in the relative safety of your monastery!",
                1, "Monastery.tmx",
                new List<CharacterSheet>()
                {
                    CharacterSheet.CreateWarrior()
                }
            ));
            Levels.Add(new LevelSheet(
                HauntedHouseTags.HauntedHouseId,
                "Haunted House",
                "A group of children have decided to spend the night in a {b}haunted house{/b}. But they didn't know a monster was really haunting it and now they are trapped and threatened!\n\nTake your rescue team of {b}courageous paladins{/b} and get the children out of the house, but remember to keep yourself safe as well!",
                3, "HauntedHouse.tmx",
                new List<CharacterSheet>()
                {
                    CharacterSheet.CreateWarrior(),
                    CharacterSheet.CreateWarrior(),
                    CharacterSheet.CreateBlueMage()
                }
                ));
            /*
            Levels.Add(new LevelSheet(
                "Fire Cultists",
                "A group of cultists of the {b}Evil God of Fire{/b} set a village aflame. They have also captured a number of villagers to sacrifice to a mighty {b}Blood Fort{/b} spell to summon a terrible demon. Save the village, save the hostages and defeat the cultists, if it's not too much to ask.",
                5, "HauntedHouse.tmx"));
            Levels.Add(new LevelSheet("The Graveyard, the Forest, and the Town",
                "Cultists of the Evil God of Fire have regrouped are now raising {b}the undead{/b} in a graveyard. In addition, they have summoned a {b}fire elemental{/b} to torch the local forest, {b}and{/b} made deal with a murderer who spreads terror through the settlement. Can you really deal with all of these emergencies?",
                6, "HauntedHouse.tmx"));*/
        }
    }
}