using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
// using static Obeliskial_Essentials.Essentials;
using System;
using static VitalizingSerenade.Plugin;
using static VitalizingSerenade.CustomFunctions;
using static VitalizingSerenade.VitalizingSerenadeFunctions;
using System.Collections.Generic;
using static Functions;
using UnityEngine;
// using Photon.Pun;
using TMPro;
using System.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEngine.UIElements;
// using Unity.TextMeshPro;

// Make sure your namespace is the same everywhere
namespace VitalizingSerenade
{

    [HarmonyPatch] // DO NOT REMOVE/CHANGE - This tells your plugin that this is part of the mod

    public class VitalizingSerenadePatches
    {



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Character), nameof(Character.SetEvent))]
        public static void SetEventPostfix(ref Character __instance,
            Enums.EventActivation theEvent,
            Character target = null,
            int auxInt = 0,
            string auxString = "")
        {
            int serenadeChance = 30;
            bool addSerenade = EnableBonusSerenades.Value && MatchManager.Instance.GetRandomIntRange(0, 100) < serenadeChance;
            if (addSerenade && theEvent == Enums.EventActivation.BeginRound)
            {
                Character hero = __instance;
                if (!IsLivingHero(hero)
                )
                {
                    return;
                }
                int index = hero.HeroIndex;
                // string seed = AtOManager.Instance.currentMapNode + index + AtOManager.Instance.GetGameId() + MatchManager.Instance.GetCurrentRound();
                int randInt = MatchManager.Instance.GetRandomIntRange(0, 100);
                string cardName = GetVitalizingSerenadeUpgraded(randInt: randInt);

                LogDebug($"Adding {cardName} to {hero.SourceName}");
                string cardInDictionary1 = MatchManager.Instance.CreateCardInDictionary(cardName);
                MatchManager.Instance.GetCardData(cardInDictionary1);
                MatchManager.Instance.GenerateNewCard(1, cardInDictionary1, false, Enums.CardPlace.RandomDeck, heroIndex: index);
            }

            if (theEvent == Enums.EventActivation.BeginCombat)
            {


                Character hero = __instance;
                if (!IsLivingHero(hero))
                {
                    return;
                }
                if (addSerenade)
                {
                    int index = hero.HeroIndex;
                    string seed = AtOManager.Instance.currentMapNode + AtOManager.Instance.GetGameId();
                    // int randInt = MatchManager.Instance.GetRandomIntRange(0, 100);
                    string cardName = GetVitalizingSerenadeUpgraded(seed);
                    LogDebug($"Adding {cardName} to {hero.SourceName}");
                    string cardInDictionary1 = MatchManager.Instance.CreateCardInDictionary(cardName);
                    MatchManager.Instance.GetCardData(cardInDictionary1);
                    MatchManager.Instance.GenerateNewCard(1, cardInDictionary1, false, Enums.CardPlace.RandomDeck, heroIndex: index);
                }

                hero.SetAura(hero, GetAuraCurseData("stanzai"), 1, useCharacterMods: false);


            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardCraftManager), nameof(CardCraftManager.ShowElements))]
        public static void ShowElementsPostfix(ref CardCraftManager __instance, string direction, ref BotonGeneric ___BG_Remove, string cardId = "")
        {
            if (__instance == null)
            {
                LogDebug("__instance == null");
                return;
            }

            CardData cardData = Globals.Instance.GetCardData(cardId, false);
            bool isVitalizingSerenade = cardData?.Id?.StartsWith("vitalizingserenade") ?? false;
            if (isVitalizingSerenade && __instance.craftType == 1)
            {
                LogDebug("Preventing vitalizingSerenade from being removed");
                ___BG_Remove.Disable();
                return;
            }
            return;
        }





        [HarmonyPostfix]
        [HarmonyPatch(typeof(Functions), nameof(Functions.GetCardByRarity))]
        public static void GetCardByRarityPostfix(ref string __result, int rarity, CardData _cardData, bool isChallenge = false)
        {

            if (!EnableRandomSerenades.Value)
            {
                return;
            }
            int serenadeRewardChance = 5;
            string seed = AtOManager.Instance.currentMapNode + AtOManager.Instance.GetGameId() + __result;
            bool addSerenadeReward = SafeRandomInt(0, 100) < serenadeRewardChance;
            if (!addSerenadeReward)
            {
                return;
            }


            seed += "1";
            string cardName = GetVitalizingSerenadeUpgraded(seed);
            __result = cardName;

        }







    }
}