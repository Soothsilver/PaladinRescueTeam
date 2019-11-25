using System.Collections.Generic;

namespace Origin.Mission
{
    public class ActivityInteraction : Interaction
    {
        public ImmediateActivity Activity { get; }

        public ActivityInteraction(string caption, string description, ImmediateActivity activity)
            : base(caption, description, activity.Actor)
        {
            Activity = activity;
        }

        public override void Execute()
        {
            Actor.GainNewGoal( this.Activity);
        }
    }
}