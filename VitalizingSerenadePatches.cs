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
            if (__instance == null || MatchManager.Instance == null)
            {
                return;
            }
            int serenadeChance = 10;
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
            if (theEvent == Enums.EventActivation.BeginTurnAboutToDealCards)
            {
                Character hero = __instance;
                hero?.SetAura(hero, GetAuraCurseData("stanzai"), 1, useCharacterMods: false);
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
                    // string seed = AtOManager.Instance.currentMapNode + AtOManager.Instance.GetGameId();
                    // int randInt = MatchManager.Instance.GetRandomIntRange(0, 100);
                    int randInt = MatchManager.Instance.GetRandomIntRange(0, 100);
                    string cardName = GetVitalizingSerenadeUpgraded(randInt: randInt);

                    LogDebug($"Adding {cardName} to {hero?.SourceName}");
                    string cardInDictionary1 = MatchManager.Instance.CreateCardInDictionary(cardName);
                    MatchManager.Instance.GetCardData(cardInDictionary1);
                    MatchManager.Instance.GenerateNewCard(1, cardInDictionary1, false, Enums.CardPlace.RandomDeck, heroIndex: index);
                }


            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AtOManager), "GlobalAuraCurseModificationByTraitsAndItems")]
        // [HarmonyPriority(Priority.Last)]
        public static void GlobalAuraCurseModificationByTraitsAndItemsPostfix(ref AtOManager __instance, ref AuraCurseData __result, string _type, string _acId, Character _characterCaster, Character _characterTarget)
        {
            // LogInfo($"GACM {subclassName}");

            Character characterOfInterest = _type == "set" ? _characterTarget : _characterCaster;
            string traitOfInterest;
            switch (_acId)
            {
                case "vitality":
                    traitOfInterest = "vsguitarpick";
                    string traitOfInterest2 = "vsguitarpickrare";
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Item, traitOfInterest2, AppliesTo.Heroes))
                    {
                        __result.MaxMadnessCharges += 100;

                    }
                    else if (IfCharacterHas(characterOfInterest, CharacterHas.Item, traitOfInterest, AppliesTo.Heroes))
                    {
                        __result.MaxMadnessCharges += 50;

                    }
                    break;
                case "thorns":
                    traitOfInterest = "vsbriarcoat";
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Item, traitOfInterest, AppliesTo.Heroes))
                    {
                        __result.Removable = false;

                    }
                    traitOfInterest = "vsbriarcoatrare";
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Item, traitOfInterest, AppliesTo.Heroes))
                    {
                        __result.Removable = false;

                    }
                    break;


            }
        }

        // [HarmonyPostfix]
        // [HarmonyPatch(typeof(CardData), nameof(CardData.Init))]

        // public static void InitPostfix(ref string ___cardName, string newId)
        // {
        //     if (ChangeAllNames.Value)
        //     {
        //         ___cardName = "Vitalizing Serenade";

        //     }
        // }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CardItem), nameof(CardItem.SetCard))]

        public static void SetCardPostfix(
            string id,
            ref TMP_Text ___titleTextTM,
            ref TMP_Text ___titleTextTBlue,
            ref TMP_Text ___titleTextTGold,
            ref TMP_Text ___titleTextTRed,
            ref TMP_Text ___titleTextTPurple,
            bool deckScale = true,
            Hero _theHero = null,
            NPC _theNPC = null,
            bool GetFromGlobal = false,
            bool _generated = false
            )
        {
            if (ChangeAllNames.Value)
            {
                ___titleTextTM.text = "Vitalizing Serenade";
                // ___titleTextTBlue.text = "Vitalizing Serenade";
                // ___titleTextTGold.text = "Vitalizing Serenade";
                // ___titleTextTRed.text = "Vitalizing Serenade";
                // ___titleTextTPurple.text = "Vitalizing Serenade";

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
            LogDebug($"GetCardByRarityPostfix {rarity} {__result}");
            int serenadeRewardChance = 5;
            string seed = AtOManager.Instance?.currentMapNode ?? "" + AtOManager.Instance.GetGameId() + __result;
            UnityEngine.Random.InitState(seed.GetDeterministicHashCode());
            bool addSerenadeReward = UnityEngine.Random.Range(0, 100) < serenadeRewardChance;
            if (!addSerenadeReward)
            {
                return;
            }


            seed += "1";
            string cardName = GetVitalizingSerenadeUpgraded(seed);
            __result = cardName;

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hero), "SetInitialCards")]
        public static void SetInitialCardsPostfix(ref Hero __instance, HeroData heroData)
        {
            LogDebug("SetInitialCardsPostfix");
            // UnityEngine.Random.InitState((AtOManager.Instance.GetGameId() + __instance.SourceName + PluginInfo.PLUGIN_GUID).GetDeterministicHashCode());
            List<string> cards = __instance?.Cards;
            cards?.Add("vitalizingserenadespecialb");
            cards?.Add("vitalizingserenadespecialb");
            __instance.Cards = cards;
        }








    }
}