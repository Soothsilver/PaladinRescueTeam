namespace Origin.Mission
{
    public class CastSpellActivity : ImmediateActivity
    {
        public SpellName SpellName { get; }
        public override Tile Tile { get; }

        public CastSpellActivity(SpellName spellName, Character actor, Tile targetTile) : 
            base(actor, 10, "Casting " + SpellDb.GetSpell(spellName).Name)
        {
            SpellName = spellName;
            Tile = targetTile;
        }

        public override void Commence()
        {
            Actor.Occupies.Speak("*begins casting*");
        }

        public override bool WithinRange()
        {
            return Actor.DistanceTo(Tile) <= 6;
        }

        public override void Complete()
        {
            SpellDb.GetSpell(SpellName).HaveEffect(Actor, Tile);
        }
    }
}