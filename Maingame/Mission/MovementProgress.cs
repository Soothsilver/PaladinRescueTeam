using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Origin.Mission
{
    public class MovementProgress
    {
        public ImmediateActivity TheGoal { get; }
        public LinkedList<Tile> Path = new LinkedList<Tile>();
        public Tile UltimateTarget;
        
        public Tile CurrentlyMovingTo;
        public Vector2 CurrentSpeed;
        public float TimeRemaining = 0;

        public MovementProgress(LinkedList<Tile> path, ImmediateActivity theGoal = null)
        {
            TheGoal = theGoal;
            Path = path;
            UltimateTarget = path.Last.Value;
        }
    }
}