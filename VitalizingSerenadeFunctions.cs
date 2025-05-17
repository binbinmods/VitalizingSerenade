﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
// using Obeliskial_Content;
// using Obeliskial_Essentials;
using System.IO;
using static UnityEngine.Mathf;
using UnityEngine.TextCore.LowLevel;
using static VitalizingSerenade.Plugin;
using System.Collections.ObjectModel;
using UnityEngine;

namespace VitalizingSerenade
{
    public class VitalizingSerenadeFunctions
    {

        public enum Corruptors
        {
            ImpedingDoom,
            Decadence,
            RestrictedPower,
            ResistantMonsters,
            Equalizer,
            Poverty,
            OverchargedMonsters,
            RandomCombats,
            Despair

        }

        public static bool HasCorruptor(Corruptors corruptor)
        {
            return MadnessManager.Instance.IsMadnessTraitActive(corruptor.ToString().ToLower());
        }

        public static float GetFishPercent()
        {
            return 0;
        }

        public static int SafeRandomInt(int min, int max, string type = "default", string seed = "")
        {
            if (MatchManager.Instance)
            {
                return MatchManager.Instance.GetRandomIntRange(min, max, type, seed);
            }
            if (MapManager.Instance)
            {
                return MapManager.Instance.GetRandomIntRange(min, max);
            }
            // if(MapManager.Instance)
            // {
            //     return Functions.Random(min, max, seed);
            // }
            UnityEngine.Random.InitState(Functions.GetDeterministicHashCode(seed));
            return UnityEngine.Random.Range(min, max);
        }

        public static string GetRandomStringFromDict(Dictionary<string, int> dict, string defaultString = "", string seed = "")
        {
            int max = Math.Max(100, dict.Values.Sum());
            int current = SafeRandomInt(0, max, seed: seed);
            foreach (KeyValuePair<string, int> kvp in dict)
            {
                current -= kvp.Value;
                if (current <= 0)
                {
                    return kvp.Key;
                }
            }
            return defaultString;

        }

        public static string GetVitalizingSerenadeUpgraded(string seed = "", int randInt = 0)
        {
            string cardName = "vitalizingserenadespecial";
            randInt = randInt == 0 ? SafeRandomInt(0, 100, seed: seed) : randInt;
            if (randInt < 10)
            {
                cardName += "rare";
            }
            else if (randInt < 30)
            {
                cardName += "a";
            }
            else if (randInt < 50)
            {
                cardName += "b";
            }
            return cardName;

        }




    }
}

