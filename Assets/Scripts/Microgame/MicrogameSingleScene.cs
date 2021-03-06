﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Microgame/Single Scene")]
public class MicrogameSingleScene : Microgame
{
    public override bool SceneDeterminesDifficulty() => false;
    
    new public class Session : Microgame.Session
    {
        public override string GetSceneName() => microgame.microgameId;

        public Session(Microgame microgame, MicrogameEventListener eventListener, int difficulty, bool debugMode)
            : base(microgame, eventListener, difficulty, debugMode)
        {
        }
    }

    public override Microgame.Session CreateSession(MicrogameEventListener eventListener, int difficulty, bool debugMode = false)
    {
        return new Session(this, eventListener, difficulty, debugMode);
    }
}

