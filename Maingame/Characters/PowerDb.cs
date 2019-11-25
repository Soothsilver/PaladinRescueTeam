using System.Collections.Generic;

namespace Origin.Characters
{
    internal static class PowerDb
    {
        public static Power GetPower(PowerName powerName)
        {
            switch (powerName)
            {
                case PowerName.StrongBody:
                    return new Power(powerName, "Strong Body", "You deal +100% damage in melee and you take half damage from all sources.");
                case PowerName.LayOnHands:
                    return new Power(powerName, "Lay on Hands", "Lay hands on an injured person near you to heal them. Laying on hands is faster than stabilization and causes the injured to recover consciousness.");
                case PowerName.SecondWind:
                    return new Power(powerName, "Second Wind", "Once per emergency, catch second wind and immediately heal of all injuries. Use this ability by right-clicking yourself.");
                case PowerName.Flight:
                    return new Power(powerName, "Flight", "You levitate above ground, moving at double speed. You can also move across water and ignore difficult terrain.");
                case PowerName.FavoredEnemySupernatural:
                    return new Power(powerName, "Favored Enemy: Supernatural", "You deal an extra +100% damage to supernatural enemies in any combat.");
                case PowerName.CastWater:
                    return new Power(powerName, "Cast Water", "A ray of water springs from your hands and douses a distant flame or hurts an enemy.");
                case PowerName.WaterAffinity:
                    return new Power(powerName, "Water Affinity", "You create triple as much water while adjacent to a well or a body of water.");
                case PowerName.SummonWaterElemental:
                    return new Power(powerName, "Summon Water Elemental", "Once per emergency, summon a water elemental that tries to extinguish nearby flames. Cast this spell by right-clicking yourself. The elemental disperses after some time.");
                default:
                    return new Power(powerName, powerName.ToString(), "This is a not-yet-implemented power and has no effect.");
            }
        }

        public static IEnumerable<Power> LearnableBy(CharacterSheet characterSheet)
        {
            foreach (PowerName name in PowerNamesLearnableBy(characterSheet))
            {
                if (!characterSheet.Powers.Contains(name))
                {
                    yield return PowerDb.GetPower(name);
                }
            }
        }

        public static IEnumerable<PowerName> PowerNamesLearnableBy(CharacterSheet characterSheet)
        {
            int powerCount = characterSheet.Powers.Count;
            switch (characterSheet.Class)
            {
                case CharacterClass.Warrior:
                    if (powerCount >= 1)
                    {
                        yield return PowerName.LayOnHands;
                        yield return PowerName.SecondWind;
                    }

                    if (powerCount >= 2)
                    {
                        yield return PowerName.Flight;
                        yield return PowerName.FavoredEnemySupernatural;
                    }
                    break;
                case CharacterClass.BlueWizard:
                    if (powerCount >= 1)
                    {
                        yield return PowerName.WaterAffinity;
                        yield return PowerName.SummonWaterElemental;
                    }

                    if (powerCount >= 2)
                    {
                        yield return PowerName.LayOnHands;
                        yield return PowerName.Flight;
                    }

                    break;
            }
        }
    }

    public class Power
    {
        public PowerName Id;
        public string Name;
        public string Description;

        public Power(PowerName id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}