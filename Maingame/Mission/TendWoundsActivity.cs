namespace Origin.Mission
{
    public class TendWoundsActivity : ImmediateActivity
    {
        public Character Healee { get; }

        public TendWoundsActivity(Character actor, Character healee) : base(actor, 1,"Healing")
        {
            Healee = healee;
        }

        public override Tile Tile => Healee.Occupies;

        public override void Complete()
        {
            Healee.HP += 0.1f;
            if (Healee.HP >= Healee.MaxHP)
            {
                Healee.HP = Healee.MaxHP;
                Healee.Wounded = false;
                Actor.Occupies.Speak("Bleeding stopped!");
            }
            else
            {
                Actor.GainNewGoal(new TendWoundsActivity(Actor, Healee));
            }
        }

        public Interaction ToInteraction()
        {
            return new ActivityInteraction("Heal " + Healee.Name, "Tend to this person's wounds so that they can regain use of their limbs", this);
        }
    }
}