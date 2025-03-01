﻿
using UnityEngine;

public static class Levels
{
    public static int[,] Level_01 => new int[,] {
        {2,  8,  4,  4,  4, 16},
        {8,  4,  2,  4,  4, 32},
        {64, 2, 16, 16, 32, 16},
        {64, 32, 4,  2, 16,  2},
        {32, 32, 16, 4, 64, 16}
    };
    
    public static int[,] Level_02 => new int[,] {
        {2,  8,  4,  4,  4, 2},
        {8,  8,  4,  4,  4, 2},
        {4,  8, 16,  4, 16, 2},
        {8,  2, 64,  8, 16, 4},
        {2, 64,  0,  4, 64, 8},
        {0,  0, 64, 32, 64, 2},
    };
    
    public static int[,] Level_03 => new int[,] {
        {2,  8,  4,  4,  4, 2},
        {2,  0,  4,  4,  4, 2},
        {4,  0,  0,  0,  4, 2},
        {16, 0,  0,  0,  4, 2},
    };
    
    public static int[,] Level_04 => new int[,] {
        { 8, 64,  8, 32, 64, 32},
        { 8,  8,  8,  8,  2,  0},
        { 8,  2, 16, 32,  4,  0},
        {64,  0,  0,  2,  0,  0},
        { 8,  0,  0, 32, 64,  0},
        { 0,  0,  0,  2, 16,  0},
        { 0,  0,  0,  0, 32,  0},
    };
    
    public static int[,] Level_05 => new int[,] {
        { 8, 64,  8, 32, 64, 32},
        {64,  8,  8,  8,  2,  0},
        {32, 16,  0,  0,  0,  0},
        {64,  4,  2,  0,  0,  0},
        { 8,  0,  0, 32,  0,  0},
        { 0,  0,  2,  0,  0,  0},
        { 0,  0,  0,  0,  0,  0},
    };
    
    public static int[,] Level_06 => new int[,] {
        { 8, 64,  8, 32, 64, 32},
        {64,  8,  8,  8,  2,  0},
        {32, 16,  0,  0,  0,  0},
        {128, 256, 2, 256,  128, 128},
        { 2,  4,  8, 16, 32, 64},
    };

    public static int[,] GetLevel(int level = 0)
    {
        if (level < 1 || level > 6)
            level = Random.Range(1, 6);
            
        switch (level)
        {
            case 1: return Level_01;
            case 2: return Level_02;
            case 3: return Level_03;
            case 4: return Level_04;
            case 5: return Level_05;
            case 6: return Level_06;
        }

        return null;
    }
}
