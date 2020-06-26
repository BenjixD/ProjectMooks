using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Reference: Ragnarok Online: https://irowiki.org/wiki/Stats
public class StatLevelHelpers
{

    // level => total number of stat points
    public static Dictionary<int, int> levelToStatPoints;

    public const int MIN_LEVEL = 1;
    public const int MAX_LEVEL = 99;

    public static void InitializeCache() {

        levelToStatPoints[MIN_LEVEL] = 0;
        for (int i = MIN_LEVEL + 1; i <= MAX_LEVEL; i++) {
            levelToStatPoints[i] = levelToStatPoints[i-1] + ((i/5) + 3);
        }

    }

    public static int GetNumStatPointsForLevelUp(int currentLevel, int nextLevel = -1) {
        if (nextLevel == -1) {
            nextLevel = currentLevel + 1;
        }

        ErrorIfNotInBounds(currentLevel);
        ErrorIfNotInBounds(nextLevel);

        if (currentLevel == nextLevel) {
            return 0;
        }
        
        if (nextLevel < currentLevel) {
            Debug.LogError("ERROR: next level cannot be less than current");
        }

        return levelToStatPoints[nextLevel] - levelToStatPoints[currentLevel];
    }

    public static int GetTotalNumberStatPointsForLevel(int level) {
        ErrorIfNotInBounds(level);
        return levelToStatPoints[level];
    }


    public static bool IsLevelInBounds(int level) {
        return level <= MAX_LEVEL && level >= MIN_LEVEL;
    }

    public static void ErrorIfNotInBounds(int level) {
        if (!IsLevelInBounds(level)) {
            Debug.LogError("ERROR: Level outside of bounds!");
        }
    }
}
