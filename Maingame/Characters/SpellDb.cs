using System;

namespace Origin.Mission
{
    public class SpellDb
    {
        public static Spell GetSpell(SpellName spellName)
        {
            switch (spellName)
            {
                case SpellName.DisarmTraps:
                    return new Spell(spellName, "Disarm Traps");
                case SpellName.CastWater:
                    return new Spell(spellName, "Cast Water");
                case SpellName.SparkFire:
                    return new Spell(spellName, "Spark Fire");
                default:
                    throw new Exception("UNknowon");
            }
        }
    }

    public class Spell
    {
        public SpellName Id { get; }
        public string Name;

        public Spell(SpellName id, string name)
        {
            Id = id;
            Name = name;
        }

        public void HaveEffect(Character spellcaster, Tile tile)
        {
            spellcaster.Occupies.Speak(Name + "!");
            // TODO effect
            switch (Id)
            {
                case SpellName.DisarmTraps:
                    tile.Trap = TrapId.NotTrapped;
                    break;
                case SpellName.SparkFire:
                    tile.Heat.StartHeavyFire();
                    break;
            }
        }
    }
}