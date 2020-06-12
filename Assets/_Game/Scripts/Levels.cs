
using UnityEngine;

public static class Levels
{
    private static int[,] Level01 => new int[,] {
        {2,  8,  4,  4,  4, 16},
        {8,  4,  2,  4,  4, 32},
        {64, 2, 16, 16, 32, 16},
        {64, 32, 4,  2, 16,  2},
        {32, 32, 16, 4, 64, 16}
    };

    private static int[,] Level02 => new int[,] {
        {2,  8,  4,  4,  4, 2},
        {8,  8,  4,  4,  4, 2},
        {4,  8, 16,  4, 16, 2},
        {8,  2, 64,  8, 16, 4},
        {2, 64,  0,  4, 64, 8},
        {0,  0, 64, 32, 64, 2},
    };

    private static int[,] Level03 => new int[,] {
        {2,  8,  4,  4,  4, 2},
        {2,  0,  4,  4,  4, 2},
        {4,  0,  0,  0,  4, 2},
        {16, 0,  0,  0,  4, 2},
    };

    private static int[,] Level04 => new int[,] {
        { 8, 64,  8, 32, 64, 32},
        { 8,  8,  8,  8,  2,  0},
        { 8,  2, 16, 32,  4,  0},
        {64,  0,  0,  2,  0,  0},
        { 8,  0,  0, 32, 64,  0},
        { 0,  0,  0,  2, 16,  0},
        { 0,  0,  0,  0, 32,  0},
    };

    private static int[,] Level05 => new int[,] {
        { 8, 64,  8, 32, 64, 32},
        {64,  8,  8,  8,  2,  0},
        {32, 16,  0,  0,  0,  0},
        {64,  4,  2,  0,  0,  0},
        { 8,  0,  0, 32,  0,  0},
        { 0,  0,  2,  0,  0,  0},
        { 0,  0,  0,  0,  0,  0},
    };

    private static int[,] Level06 => new int[,] {
        { 8, 64,  8, 32, 64, 32},
        {64,  8,  8,  8,  2,  0},
        {32, 16,  0,  0,  0,  0},
        {128, 256, 2, 256,  128, 128},
        { 2,  4,  8, 16, 32, 64},
    };

    private static int[,] Level07 => new int[,] {
        { 2, 2,  2, 2, 2, 2},
        {128, 256, 256, 256, 128, 128},
        { 256, 256, 256, 128, 128, 256}
    };
    
    public static int[,] GetLevel(int level = 0)
    {
        const int maxLevelNumber = 7;
        if (level < 1 || maxLevelNumber < level )
            level = Random.Range(1, maxLevelNumber);
            
        switch (level)
        {
            case 1: return Level01;
            case 2: return Level02;
            case 3: return Level03;
            case 4: return Level04;
            case 5: return Level05;
            case 6: return Level06;
            case 7: return Level07;
        }

        return null;
    }
}
