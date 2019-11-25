namespace Origin.Mission
{
    public abstract class ImmediateActivity
    {
        public float SecondsToComplete { get; }
        public float SecondsProgressed;
        public string Progress { get; }

        public Character Actor { get; set; }

        public abstract Tile Tile { get; }

        public ImmediateActivity(Character actor, float secondsToComplete, string progressMessage)
        {
            this.Actor = actor;
            SecondsToComplete = secondsToComplete;
            Progress = progressMessage;
        }

        public virtual bool WithinRange()
        {
            return Actor.DistanceTo(Tile) <= 1.2;
        }

        public virtual void Commence()
        {

        }

        public virtual void Complete()
        {
            
        }   
        
        public virtual void Abort()
        {
            
        }

        public void MakeProgress(float elapsedSeconds)
        {
            this.SecondsProgressed += elapsedSeconds;
            if (this.SecondsProgressed >= this.SecondsToComplete)
            {
                this.SecondsProgressed = this.SecondsToComplete;
                this.Actor.ImmediateActivity = null;
                this.Complete();
            }
        }
    }
}