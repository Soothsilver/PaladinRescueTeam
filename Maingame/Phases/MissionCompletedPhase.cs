using Auxiliary;
using Origin.Mission;

namespace Origin.Phases
{
    public class MissionCompletedPhase : DisplayLongTextPhase
    {
        private string text;
        public MissionCompletedPhase(EmergencyRecords records)
        {
            text = "";
            switch (records.Reason)
            {
                case EndReason.Retreat:
                    text +=
                        "You have retreated.\n\nSometimes, you make the best decisions and they're still not enough.\n\nSometimes, you make the wrong decisions and decide to pull out before things get even worse.\n\nEither way, there is no shame in retreating. That said, said lack of shame is not going to be of any use to those who are dead because of your inaction. Learn from your experience and make better decisions next time.";
                    break;
                case EndReason.Victory:
                    text += records.Text;
                    break;
                case EndReason.PaladinDeath:
                    text +=
                        "You have lost.\n\nAll of your paladins are now dead!\n\nIt is understandable that you panic in the face of an emergency but the best decisions are made with a cold head. Do not hurry. Deliberate. Do not endanger your paladins' safety.\n\nSometimes, even the best decisions lead to deaths for everyone involved. But that was not the case here.";
                    break;
                case EndReason.CivilianDeath:
                    text +=
                        "You have lost.\n\nToo many civilians have met their death in this emergency. Perhaps they bled out from their injuries because you did not reach them in time.\n\nYou made the decision to be cautious and that is commendable, but in this profession, sometimes you need to make a decision quickly and commit to it.\n\nYes, you are putting yourself at risk -- but to risk yourself to save others is a decision you made a long time ago, when you joined the Team.";
                    break;
                case EndReason.FireOutOfControl:
                    text +=
                        "You have lost.\n\nFires have gotten out of control and are now threatening nearby areas.\n\nIt is understandable that you made the decision to focus elsewhere but wildfires can spiral out of control quickly and must be suppressed.\n\nSometimes, you just don't have enough resources and the decision that saves the most people is to not get involved at all, but that was not the case here.";
                    break;
                default:
                    text += "The emergency is over, and you failed.";
                    break;
            }

            text += "\n\nClick anywhere to return to your monastery.";
        }

        public override string ComposeHelpString()
        {
            return text;
        }

        protected override void OnClick()
        {
            Root.PopFromPhase();
            Root.PhaseStack[Root.PhaseStack.Count - 2].ScheduledForElimination = true;
        }
    }
}