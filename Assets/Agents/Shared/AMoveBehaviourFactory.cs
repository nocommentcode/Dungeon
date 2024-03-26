using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Abstract Moving behavior Factory.
/// The Behavior will resulting in the transform moving this turn
/// Children must implement:
///     - Delay - the amount of time to wait
///     - Condition() - wether to perform the behavior or not
///     - GetDestTile() - the tile to move to
/// </summary> 
public abstract class AMoveBehaviourFactory : ABehaviourFactory
{
    protected AMoveBehaviourFactory(DungeonGrid dungeonGrid, Transform transform) : base(dungeonGrid, transform)
    {
    }

    /// <summary>
    /// A Star Implementation
    /// </summary>
    /// <param name="desitination">The tile to get to</param>
    /// <returns>The next tile to move to</returns> 
    protected DungeonTile AStarSearch(DungeonTile desitination){
        // measures the distance bewteen two tiles (fitness function)
        float Distance(DungeonTile from, DungeonTile to){
            return Vector3Int.Distance(from.GetTilemapPosition(), to.GetTilemapPosition());
        }

        // reconstructs the path from detination
        List<DungeonTile> ReconstructPath(Dictionary<DungeonTile, DungeonTile> cameFrom, DungeonTile dest)
        {
            var path = new List<DungeonTile>();
            while (cameFrom.ContainsKey(dest))
            {
                path.Add(dest);
                dest = cameFrom[dest];
                if(dest == null){
                    path.Reverse();
                    return path;
                }
            }
            return null;
        }
        
        // initialise data structures
        var position = GetCurrentTile();
        var frontier = new PriorityQueue<DungeonTile, float>();
        var cameFrom = new Dictionary<DungeonTile, DungeonTile>();
        var costSoFar = new Dictionary<DungeonTile, float>();

        // add starting position
        frontier.Enqueue(position, Distance(position, desitination));
        cameFrom[position] = null;
        costSoFar[position] = 0;


        while(!frontier.IsEmpty()){
            var currentTile = frontier.Dequeue();

            // goal test
            if(currentTile == desitination){
                var path = ReconstructPath(cameFrom, desitination);
                return path[1];
            }

            // add neighbours with cost
            foreach(var neighbour in currentTile.Neighbours.Values.Where(tile => tile.IsDungeon).ToList()){
                float cost = costSoFar[currentTile] + 1;
                if(!costSoFar.ContainsKey(neighbour) || cost < costSoFar[neighbour]){
                    costSoFar[neighbour] = cost;
                    cameFrom[neighbour] = currentTile;
                    frontier.Enqueue(neighbour, cost  + Distance(neighbour, desitination));
                }
            }

        }
        
        return null;
    }

    /// <summary>
    /// Implements the abstract PerformAction() method by calling GetDestTile()
    /// </summary>
    public override void PerformAction()
    {
        // get destination from abstract method
        var destination = GetDestTile();
        
        // check destination valid
        if(destination == null){
            return;
        }
        if(!destination.IsDungeon){
            return;
        }
        
        // move to destination
        _transform.position = destination.GetGlobalPosition();
    }

    /// <summary>
    /// Returns the destination of the move.
    /// Children must implement this.
    /// </summary>
    /// <returns>The destination tile</returns>
    protected abstract DungeonTile GetDestTile();
}
