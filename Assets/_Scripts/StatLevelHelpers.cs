using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Reference: Ragnarok Online: https://irowiki.org/wiki/Stats
public class StatLevelHelpers
{

    // level => total number of stat points
    private static Dictionary<int, int> levelToStatPoints;

    // Note: Should this be Monohaviour just so we can play around with these numbers? ...
    public const int MIN_LEVEL = 1;
    public const int MAX_LEVEL = 99;
    public const int MIN_LEVEL_TO_STAT_INCREMENT = 3;
    public const int LEVEL_TO_STAT_INCREMENT = 3;
    public const int STAT_COST_INCREMENT = 10;
    public const int MIN_STAT_COST_INCREMENT = 1;

    public static void InitializeCache() {
        levelToStatPoints = new Dictionary<int, int>();
        levelToStatPoints[MIN_LEVEL] = 0;
        for (int i = MIN_LEVEL + 1; i <= MAX_LEVEL; i++) {
            levelToStatPoints[i] = levelToStatPoints[i-1] + ((i/LEVEL_TO_STAT_INCREMENT) + MIN_LEVEL_TO_STAT_INCREMENT);
        }

    }

    public static int GetNumStatPointsForLevelUp(int currentLevel, int nextLevel = -1) {
        if (levelToStatPoints == null) {
            Debug.LogError("ERROR: Call InitializeCache first!");
            return -1;
        }

        if (nextLevel == -1) {
            nextLevel = currentLevel + 1;
        }

        if ( !ErrorIfNotInBounds(currentLevel) || !ErrorIfNotInBounds(nextLevel)) {
            return -1;
        };

        if (currentLevel == nextLevel) {
            return 0;
        }
        
        if (nextLevel < currentLevel) {
            Debug.LogError("ERROR: next level cannot be less than current");
        }

        return levelToStatPoints[nextLevel] - levelToStatPoints[currentLevel];
    }

    public static int GetTotalNumberStatPointsForLevel(int level) {
        if (levelToStatPoints == null) {
            Debug.LogError("ERROR: Call InitializeCache first!");
            return -1;
        }

        if (!ErrorIfNotInBounds(level)) {
            return -1;
        }
        
        return levelToStatPoints[level];
    }

    public static int GetCostToLevelUpStat(int currentStatAmount, int divisor = 1) {
        currentStatAmount /= divisor;
        return ((currentStatAmount - 1) / STAT_COST_INCREMENT) + MIN_STAT_COST_INCREMENT;
    }


    public static bool IsLevelInBounds(int level) {
        return level <= MAX_LEVEL && level >= MIN_LEVEL;
    }

    public static bool ErrorIfNotInBounds(int level) {
        if (!IsLevelInBounds(level)) {
            Debug.LogError("ERROR: Level outside of bounds!");
            return false;
        }

        return true;
    }
}
