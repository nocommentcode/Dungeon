using UnityEngine;
using System.Linq;

/// <summary>
/// Move to Treasuer behavior factory
/// Moves the theif towards the treasure if there is still treasure in the dungeon
/// </summary> 
public class MoveToTreasureBeahviourFactory : AMoveBehaviourFactory
{
    public MoveToTreasureBeahviourFactory(DungeonGrid dungeonGrid, Transform transform) : base(dungeonGrid, transform)
    {
    }

    protected override float Delay => TheifSettings.TheifSpeed;

    public override bool Condition()
    {
        // move to treasure if there is treasure left to pickup
        return _dungeonGrid.TreasureTiles.Count > 0;
    }

    /// <summary>
    /// Finds the closes treasure tile
    /// </summary>
    /// <returns>Closest treasure</returns>
    private DungeonTile FindClosestTreasure()
    {
        var currentTile = GetCurrentTile();

        return _dungeonGrid.TreasureTiles
                            .OrderBy(treasure => Vector3.Distance(treasure.Key.GetTilemapPosition(), currentTile.GetTilemapPosition()))
                            .FirstOrDefault()
                            .Key;
    }

    protected override DungeonTile GetDestTile()
    {
        // use a star to find tile to move to to get to closest treasure
        var closestTreasure = FindClosestTreasure();
        return AStarSearch(closestTreasure);
    }
}