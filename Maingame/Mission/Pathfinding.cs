using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Priority_Queue;

namespace Origin.Mission
{
    public class Pathfinding
    {
        private static int LastPathfindingIndex = -1;

        public static void FloodFillToReveal(Tile startFrom, int sightRemaining)
        {
            LastPathfindingIndex++;
            Queue<Tile> openQueue = new Queue<Tile>();
            openQueue.Enqueue(startFrom);
            openQueue.Enqueue(null);
            while (openQueue.Count > 0 && sightRemaining >= 0)
            {
                Tile t = openQueue.Dequeue();
                if (t == null)
                {
                    sightRemaining--;
                    openQueue.Enqueue(null);
                    continue;
                }

                if (t.LastClosedBySearch == LastPathfindingIndex)
                {
                    // Closed already
                    continue;
                }
                t.LastClosedBySearch = LastPathfindingIndex;
                t.Blackened = false;
                if (!t.BlocksLineOfSight && sightRemaining > 0)
                {
                    foreach (var neighbour in t.Neighbours)
                    {
                        if (neighbour.LastClosedBySearch == LastPathfindingIndex)
                        {
                            // Closed already
                            continue;
                        }
                        openQueue.Enqueue(neighbour);
                    }
                }
            }
        } 
        
        private static int mapTotalSize;
        private static FastPriorityQueue<Tile> openSet;
        private static int pathfindingSearchId = 0;
        public static LinkedList<Tile> AStar(Character who, Tile target, TSession map, PathfindingMode mode)
        {
            InitializeOpenSetIfNecessary(map);
            pathfindingSearchId++;
            Tile start = who.Occupies;
            if (target == null)
            {
                return null;
            }
            Tile closestToTargetSoFar = start;
            openSet.Clear();
            int initH = Pathfinding.Heuristic(start, target);
            SetPathfindingInformation(start, pathfindingSearchId, false, 0, initH, null);
            openSet.Enqueue(start, initH);
            int closestToTargetHeuristicSoFar = initH;

            while (openSet.Count > 0)
            {
                Tile current = openSet.Dequeue();
                if (current == target)
                {
                    return ReconstructPrettyPath(start, target);
                }
                current.Pathfinding_Closed = true;
                foreach (var neighbour in current.Neighbours) // TODO change to traversable neighbours
                {
//                    Tile neighbour = edge.Destination;
                    if (neighbour.Pathfinding_EncounteredDuringSearch == pathfindingSearchId &&
                        neighbour.Pathfinding_Closed) continue;
                    if (neighbour.BlocksMovement) continue;

                    if (neighbour.Pathfinding_EncounteredDuringSearch < pathfindingSearchId)
                    {
                        SetPathfindingInformation(neighbour, pathfindingSearchId, false, int.MaxValue, int.MaxValue, current);
                        openSet.Enqueue(neighbour, int.MaxValue);
                    }

                    int tentativeGScore = current.Pathfinding_G + (IsDiagonal(current, neighbour) ? 14 : 10);
                    if (tentativeGScore >= neighbour.Pathfinding_G) continue;

                    neighbour.Pathfinding_Parent = current;
                    neighbour.Pathfinding_G = tentativeGScore;
                    int heuristic = Heuristic(neighbour, target);
                    neighbour.Pathfinding_F = neighbour.Pathfinding_G + heuristic;
                    openSet.UpdatePriority(neighbour, neighbour.Pathfinding_F);
                    if (heuristic < closestToTargetHeuristicSoFar)
                    {
                        closestToTargetSoFar = neighbour;
                        closestToTargetHeuristicSoFar = heuristic;
                    }
                }
            }
            
            if (mode == PathfindingMode.FindClosestIfDirectIsImpossible)
            {
                return ReconstructPrettyPath(start, closestToTargetSoFar);
            }
            return null;
        }

        private static bool IsDiagonal(Tile current, Tile neighbour)
        {
            return current.X != neighbour.X && current.Y != neighbour.Y;
        }

        private static LinkedList<Tile> ReconstructPrettyPath(Tile start, Tile reachableDestination)
        {
            var l = ReconstructPath(start, reachableDestination);
            LinkedList<Tile> result = new LinkedList<Tile>();
            foreach (Tile tl in l)
            {
                result.AddLast(tl);
            }
            return result;
        }

        
        private static void InitializeOpenSetIfNecessary(TSession map)
        {
            int totalSize = map.MapWidth * map.MapHeight;
            if (mapTotalSize < totalSize)
            {
                openSet = new FastPriorityQueue<Tile>(totalSize);
                mapTotalSize = totalSize;
            }
        }

        private static LinkedList<Tile> ReconstructPath(Tile start, Tile current)
        {
            LinkedList<Tile> path = new LinkedList<Tile>();
            path.AddLast(current);
            while (current.Pathfinding_Parent != null &&
                current.Pathfinding_Parent != start)
            {
                path.AddFirst(current.Pathfinding_Parent);
                current = current.Pathfinding_Parent;
            }
            return path;
        }

        private static int Heuristic(Tile start, Tile target)
        {
            return Math.Max(Math.Abs(start.X - target.X), Math.Abs(start.Y - target.Y)) * 64;
        }

        private static void SetPathfindingInformation(Tile t, int searchId, bool closed, int g, int f, Tile parent)
        {
            t.Pathfinding_EncounteredDuringSearch = searchId;
            t.Pathfinding_Closed = closed;
            t.Pathfinding_F = f;
            t.Pathfinding_G = g;
            t.Pathfinding_Parent = parent;
        }
    }

    public enum PathfindingMode
    {
        Normal,
        FindClosestIfDirectIsImpossible
    }
}