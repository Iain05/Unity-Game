using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[System.Serializable]
public class Cooldown {
    public enum CooldownTypes {
        Jump,
        Dash
    }
    // dictionary of constant cooldown times for each type of cooldown
    Dictionary<CooldownTypes, float> cooldownTimes = new Dictionary<CooldownTypes, float> {
        {CooldownTypes.Jump, 0.05f},
        {CooldownTypes.Dash, 2.0f}
    };
    // active cooldowns of each type
    Dictionary<CooldownTypes, float> cooldowns = new Dictionary<CooldownTypes, float> {
        {CooldownTypes.Jump, 0},
        {CooldownTypes.Dash, 0}
    };

    // check if a cooldown is active
    public bool jumpCoolingDown => Time.time < cooldowns[CooldownTypes.Jump];
    public bool dashCoolingDown => Time.time < cooldowns[CooldownTypes.Dash];
    // start a cooldown
    public void startCooldown(CooldownTypes type) {
        cooldowns[type] = Time.time + cooldownTimes[type];
    }
    
}