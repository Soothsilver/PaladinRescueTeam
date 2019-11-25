using System.Collections.Generic;

namespace Origin.Mission.Interactions
{
    public class MoveInteraction : Interaction
    {
        public Character Mover { get; }
        public Tile TargetTile { get; }

        public MoveInteraction(Character mover, Tile targetTile) : base("Move here", null, mover)
        {
            Mover = mover;
            TargetTile = targetTile;
            this.IsDirectlyExecutable = true;
        }

        public override void Execute()
        {
            LinkedList<Tile> path = Pathfinding.AStar(Mover, TargetTile, Mover.Session, PathfindingMode.FindClosestIfDirectIsImpossible);
            if (path != null)
            {
                Mover.MovementProgress = new MovementProgress(path);
            }
        }
    }
}