namespace Origin.Mission
{
    public class EmergencyRecords
    {
        public EndReason Reason;

        public string Text { get; set; }
    }

    public enum EndReason
    {
        PaladinDeath,
        Retreat,
        CivilianDeath,
        FireOutOfControl,
        Victory
    }
}