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
        [HarmonyPatch(typeof(AtOManager), "GetItemList")]
        public static void GetItemListPostfix(ref List<string> __result, string itemListId)
        {
            if (__result == null || __result.Count == 0)
            {
                return;
            }
            if (!EnableIncreasedRods.Value)
            {
                return;
            }

            string seed = AtOManager.Instance.GetGameId() + AtOManager.Instance.currentMapNode;
            int rodChance = 60;
            bool replaceItem = SafeRandomInt(0, 100) < rodChance;
            if (!replaceItem)
            {
                return;
            }

            Dictionary<string, int> fishingRods = new() {
                // { "fishingrod", 100},
                { "steelrod", 10 * AtOManager.Instance.GetActNumberForText() },
                { "taintedrod", 4 * AtOManager.Instance.GetActNumberForText() },
                { "thorimsrod", 7 * AtOManager.Instance.GetActNumberForText() },
                { "fishcage", 10}
                };


            __result[0] = GetRandomStringFromDict(fishingRods, "fishingrod", seed);

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Character), nameof(Character.SetEvent))]
        public static void SetEventPostfix(ref Character __instance,
            Enums.EventActivation theEvent,
            Character target = null,
            int auxInt = 0,
            string auxString = "")
        {
            int fishChance = 30;
            bool addFish = EnableBonusFish.Value && SafeRandomInt(0, 100) < fishChance;
            if (addFish && theEvent == Enums.EventActivation.BeginTurnAboutToDealCards)
            {
                Hero hero = MatchManager.Instance.GetHeroHeroActive();
                if (!IsLivingHero(hero)
                )
                {
                    return;
                }
                int index = hero.HeroIndex;
                Hero[] teamHero = MatchManager.Instance.GetTeamHero();
                Dictionary<string, int> fish = new() {
                    // { "carp", 100},
                    { "bass", 10 * AtOManager.Instance.GetActNumberForText() },
                    { "pufferfish", 3 * AtOManager.Instance.GetActNumberForText() },
                    { "furiouscarp", 6 * AtOManager.Instance.GetActNumberForText() },
                    { "frozencarp", 5 * AtOManager.Instance.GetActNumberForText() }
                };

                string cardName = GetRandomStringFromDict(fish, "carp");
                if (SafeRandomInt(0, 100) < 15 && cardName != "furiouscarp")
                {
                    cardName += "rare";
                }
                LogDebug($"Adding {cardName} to {hero.SourceName}");
                string cardInDictionary1 = MatchManager.Instance.CreateCardInDictionary(cardName);
                MatchManager.Instance.GetCardData(cardInDictionary1);
                MatchManager.Instance.GenerateNewCard(1, cardInDictionary1, false, Enums.CardPlace.RandomDeck, heroIndex: teamHero[index].HeroIndex);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AtOManager), "GetTeamNPCReward")]
        public static bool GetTeamNPCRewardPrefix(ref TierRewardData __result, ref AtOManager __instance)
        {
            int num = 0;
            string[] teamNPCAtO = __instance.GetTeamNPC();
            for (int index = 0; index < teamNPCAtO.Length; ++index)
            {
                if (teamNPCAtO[index] != null && teamNPCAtO[index] != "")
                {
                    NPCData npcData = Globals.Instance.GetNPC(teamNPCAtO[index]);
                    if (npcData != null)
                    {
                        if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && __instance.PlayerHasRequirement(Globals.Instance.GetRequirementData("_tier2")) && (UnityEngine.Object)npcData.UpgradedMob != (UnityEngine.Object)null)
                            npcData = npcData.UpgradedMob;
                        if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && __instance.GetNgPlus() > 0 && npcData.NgPlusMob != null)
                            npcData = npcData.NgPlusMob;
                        if (npcData != null && MadnessManager.Instance.IsMadnessTraitActive("despair") && (UnityEngine.Object)npcData.HellModeMob != (UnityEngine.Object)null)
                            npcData = npcData.HellModeMob;
                        if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && (UnityEngine.Object)npcData.TierReward != (UnityEngine.Object)null && npcData.TierReward.TierNum > num)
                            num = npcData.TierReward.TierNum;
                    }
                }
            }
            __result = Globals.Instance.GetTierRewardData(num);
            return false; // do not run original method
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(Functions), nameof(Functions.GetCardByRarity))]
        public static void GetCardByRarityPostfix(ref string __result, int rarity, CardData _cardData, bool isChallenge = false)
        {

            if (!EnableRandomFish.Value)
            {
                return;
            }
            int fishChance = 6;
            bool addFish = SafeRandomInt(0, 100) < fishChance;
            if (!addFish)
            {
                return;
            }

            Dictionary<string, int> fish = new() {
                    // { "carp", 100},
                    { "bass", 10 * AtOManager.Instance.GetActNumberForText() },
                    { "pufferfish", 3 * AtOManager.Instance.GetActNumberForText() },
                    { "furiouscarp", 6 * AtOManager.Instance.GetActNumberForText() },
                    { "frozencarp", 5 * AtOManager.Instance.GetActNumberForText() }
                };

            string cardName = GetRandomStringFromDict(fish, "carp");
            if (SafeRandomInt(0, 100) < 15 && cardName != "furiouscarp")
            {
                cardName += "rare";
            }
            __result = cardName;

        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(Functions), nameof(Functions.GetRandomCombat))]
        public static void GetRandomCombatPrefix(
            NPCData[] __result,
            ref Enums.CombatTier combatTier,
            int seed,
            string nodeSelectedId,
            bool forceIsThereRare = false)
        {

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Functions), nameof(Functions.GetRandomCombat))]
        public static void GetRandomCombatPostfix(
            NPCData[] __result,
            ref Enums.CombatTier combatTier,
            int seed,
            string nodeSelectedId,
            bool forceIsThereRare = false)
        {


        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Globals), nameof(Globals.GetNPCForRandom))]
        public static bool GetNPCForRandom(
            Globals __instance,
            ref NPCData __result,
            bool _rare,
            int position,
            Enums.CombatTier _ct,
            NPCData[] _teamNPC)
        {
            if (!(AtOManager.Instance.GetActNumberForText() == 2 || AtOManager.Instance.GetActNumberForText() == 3))
            {
                return true;
            }

            LogDebug($"Random NPC for Combat Tier {(int)_ct}");
            List<string> regulars1 = ["archerfish", "fisher", "hypnocrab", "octopus", "sentinel", "stingray", "skirmisher", "skyhunter", "stormbringer", "stormcaller", "swordfish"];
            List<string> regulars2 = ["boatswain", "darkjellyfish", "harpooner", "lightjellyfish", "pufferfish", "shark", "siren", "spellbinder", "stormbringer", "warden"];
            List<string> champs1 = [
                    "colin",
                    "lana",
                    "mugur",
                    "shredder",
                    ];
            List<string> champs2 = [
                    "ariel",
                    "neptu",
                    "phil",
                    ];
            bool firstHalf = true;
            if ((int)_ct > (int)Enums.CombatTier.T4)
            {
                firstHalf = false;
            }
            List<string> champs = firstHalf ? champs1 : champs2;
            List<string> regulars = firstHalf ? regulars1 : regulars2;
            if ((int)_ct == (int)Enums.CombatTier.T3)
            {
                regulars = ["archerfish",
                    "fisher",
                    "forecaster",
                    "hermit",
                    "lumberjack",
                    "octopus",
                    "parrot",
                    "sentinel",
                    "stingray",
                    "swordfish"];
                champs = ["hugo", "lana"];
            }
            List<string> listToCheck = _rare ? champs : regulars;

            string randomNPC = listToCheck[UnityEngine.Random.Range(0, listToCheck.Count)];
            if (AtOManager.Instance.GetActNumberForText() == 3)
            {
                randomNPC += "_b";
            }
            LogDebug($"chosen NPC {randomNPC}");
            NPCData chosenNPC = __instance.GetNPC(randomNPC);
            if (__instance.GetNPC(randomNPC).UpgradedMob && AtOManager.Instance.GetActNumberForText() == 3)
            {
                chosenNPC = chosenNPC.UpgradedMob;
            }
            if (chosenNPC.NgPlusMob != null && AtOManager.Instance.GetNgPlus() > 0)
            {
                chosenNPC = chosenNPC.NgPlusMob;
            }
            if (chosenNPC.HellModeMob != null && HasCorruptor(Corruptors.Despair))
            {
                chosenNPC = chosenNPC.HellModeMob;
            }

            chosenNPC.IsBoss = false;
            chosenNPC.FinishCombatOnDead = false;
            __result = chosenNPC;

            LogDebug($"chosen NPC result {__result.Id}");
            return false;

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AtOManager), "GetExperienceFromCombat")]
        public static bool GetExperienceFromCombatPrefix(ref int __result, ref AtOManager __instance)
        {
            int experienceFromCombat = 0;
            string[] teamNPCAtO = __instance.GetTeamNPC();
            if (teamNPCAtO != null)
            {
                for (int index = 0; index < teamNPCAtO.Length; ++index)
                {
                    if (teamNPCAtO[index] != null && teamNPCAtO[index] != "")
                    {
                        NPCData npcData = Globals.Instance.GetNPC(teamNPCAtO[index]);
                        if (npcData != null)
                        {
                            if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && __instance.PlayerHasRequirement(Globals.Instance.GetRequirementData("_tier2")) && (UnityEngine.Object)npcData.UpgradedMob != (UnityEngine.Object)null)
                                npcData = npcData.UpgradedMob;
                            if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && __instance.GetNgPlus() > 0 && npcData.NgPlusMob != null)
                                npcData = npcData.NgPlusMob;
                            if (npcData != null && MadnessManager.Instance.IsMadnessTraitActive("despair") && (UnityEngine.Object)npcData.HellModeMob != (UnityEngine.Object)null)
                                npcData = npcData.HellModeMob;
                            if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && npcData.ExperienceReward > 0)
                                experienceFromCombat += npcData.ExperienceReward;
                        }
                    }
                }
            }
            __result = experienceFromCombat;
            return false; // do not run original method
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AtOManager), "GetGoldFromCombat")]
        public static bool GetGoldFromCombatPrefix(ref int __result, ref AtOManager __instance)
        {
            int goldFromCombat = 0;
            string[] teamNPCAtO = __instance.GetTeamNPC();
            if (teamNPCAtO != null)
            {
                for (int index = 0; index < teamNPCAtO.Length; ++index)
                {
                    if (teamNPCAtO[index] != null && teamNPCAtO[index] != "")
                    {
                        NPCData npcData = Globals.Instance.GetNPC(teamNPCAtO[index]);
                        if (npcData != null)
                        {
                            if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && __instance.PlayerHasRequirement(Globals.Instance.GetRequirementData("_tier2")) && (UnityEngine.Object)npcData.UpgradedMob != (UnityEngine.Object)null)
                                npcData = npcData.UpgradedMob;
                            if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && __instance.GetNgPlus() > 0 && npcData.NgPlusMob != null)
                                npcData = npcData.NgPlusMob;
                            if (npcData != null && MadnessManager.Instance.IsMadnessTraitActive("despair") && (UnityEngine.Object)npcData.HellModeMob != (UnityEngine.Object)null)
                                npcData = npcData.HellModeMob;
                            if ((UnityEngine.Object)npcData != (UnityEngine.Object)null && npcData.GoldReward > 0)
                                goldFromCombat += npcData.GoldReward;
                        }
                    }
                }
            }
            __result = goldFromCombat;
            return false; // do not run original method
        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetObeliskScore")]
        public static bool SetObeliskScorePrefix(ref SteamManager __instance, int score, bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetScore")]
        public static bool SetScorePrefix(ref SteamManager __instance, int score, bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetSingularityScore")]
        public static bool SetSingularityScorePrefix(ref SteamManager __instance, int score, bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetObeliskScoreLeaderboard")]
        public static bool SetObeliskScoreLeaderboardPrefix(ref SteamManager __instance, int score, bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetScoreLeaderboard")]
        public static bool SetScoreLeaderboardPrefix(ref SteamManager __instance, int score, bool singleplayer = true)
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamManager), "SetSingularityScoreLeaderboard")]
        public static bool SetSingularityScoreLeaderboardPrefix(ref SteamManager __instance, int score, bool singleplayer = true)
        {
            return false;
        }



    }
}