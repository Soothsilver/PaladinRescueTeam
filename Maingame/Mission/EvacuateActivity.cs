namespace Origin.Mission
{
    public class EvacuateActivity : ImmediateActivity
    {
        public Character Target { get; }

        public EvacuateActivity(Character actor, Character target) : base(actor, 10, "Evacuating a civilian")
        {
            Target = target;
        }

        public override void Commence()
        {
            Actor.Occupies.Speak("Hey, you should leave...");
        }

        public override void Complete()
        {
            Target.Occupies.Speak("I'm getting out of here...");
            Target.MoveToTileIfPossible(Actor.Session.EvacuationTile);
        }

        public override Tile Tile => Target.Occupies;

        public Interaction ToInteraction()
        {
            return new ActivityInteraction("Evacuate " + Target.Name, "Ask this civilian to leave the emergency zone", this);
        }
    }
}