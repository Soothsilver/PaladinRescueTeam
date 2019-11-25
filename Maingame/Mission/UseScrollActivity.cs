namespace Origin.Mission
{
    public class UseScrollActivity : ImmediateActivity
    {
        public UseScrollActivity(Character actor, HeldItem first, Tile tile) : base(actor, 6, "Reading")
        {
            First = first;
            Tile = tile;
        }

        public HeldItem First { get; }
        public override Tile Tile { get; }

        public override void Commence()
        {
            Actor.Occupies.Speak("reading");
        }

        public override bool WithinRange()
        {
            return Actor.DistanceTo(Tile) <= 6;
        }

        public override void Complete()
        {
            switch (First.ItemId)
            {
                case HeldItemId.ScrollDisarmTrap:
                    SpellDb.GetSpell(SpellName.DisarmTraps).HaveEffect(Actor, Tile);
                    break;
                case HeldItemId.ScrollSparkFire:
                    SpellDb.GetSpell(SpellName.SparkFire).HaveEffect(Actor, Tile);
                    break;
            }

            Actor.HeldItems.Remove(First);
        }

        public Interaction ToInteraction()
        {
            return new ActivityInteraction("Read " + First.Name, "This will expend this single-use scroll.", this);
        }
    }
}