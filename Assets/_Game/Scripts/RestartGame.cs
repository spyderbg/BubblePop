﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGame : MonoBehaviour
{
    public void WorldRestartGame()
    {
        World.Instance.RestartGame();
    }
}
