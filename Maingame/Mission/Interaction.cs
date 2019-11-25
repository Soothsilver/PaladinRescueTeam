namespace Origin.Mission
{
    public class Interaction
    {
        public Character Actor { get; }
        public string Caption;
        public string Description;
        public bool IsDirectlyExecutable;

        public Interaction(string caption, string description, Character actor)
        {
            Actor = actor;
            Caption = caption;
            Description = description;
        }

        public void FullExecute()
        {
            Actor.ImmediateActivity?.Abort();
            Actor.ImmediateActivity = null;
            Execute();
        }
        public virtual void Execute()
        {
            
        }
    }
}