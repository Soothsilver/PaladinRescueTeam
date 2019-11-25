namespace Origin.Mission
{
    public class TalkActivity : ImmediateActivity
    {
        public override Tile Tile => Target.Occupies;
        public Character Target { get; }

        public TalkActivity(Character actor, Character target) : base(actor, 4, "Talking")
        {
            Target = target;
        }

        public Interaction ToInteraction()
        {
            return new ActivityInteraction("Converse with " + this.Target.Name,
                "Ask this civilian for useful information", this);
        }

        public override bool WithinRange()
        {
            return Actor.DistanceTo(Target) <= 1.2;
        }

        public override void Commence()
        {
            Actor.Occupies.Speak("Hello!");
        }

        public override void Complete()
        {
            Target.Occupies.Speak(Target.GetConversationTopic());
        }
    }
}