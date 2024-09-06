using System;
using System.Collections.Generic;
using UnityEngine;

//My Own Custom Implementation of Observer pattern, Scriptable based events make code reading hard
public static class GameEvents
{
    public static class AssetLoadingEvents
    {
        public static readonly GameEvent<float> AssetLoadProgressUpdated = new();
        public static readonly GameEvent AssetLoaded = new();
    }
}
