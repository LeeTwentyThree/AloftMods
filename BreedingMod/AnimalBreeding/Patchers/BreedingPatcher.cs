using System;
using HarmonyLib;
using Level_Manager;
using NPC;
using NPC.Abstract.Animals;
using Terrain.Platforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnimalBreeding.Patchers;

// Patches animal-related classes to reintroduce the breeding mechanics
internal static class BreedingPatcher
{
    [HarmonyPatch(typeof(NpcReproduction))]
    public static class NpcReproductionPatches
    {
        [HarmonyPatch(nameof(NpcReproduction.CheckImpregnate))]
        [HarmonyPostfix]
        public static void CheckImpregnatePostfix(NpcReproduction __instance)
        {
            // Do not impregnate if already pregnant
            if (__instance._controller.Data.Social.SocialData.IsPregnant)
                return;
            // Do not let male animals get pregnant
            if (__instance._controller.Header.IsMale)
                return;
            // Disable reproduction if the creature has another stage of growth
            if (__instance._controller.Data.Agent.SAgent.SocialMatureNextSteps.Length != 0)
                return;
            // Non-animals cannot reproduce
            if (__instance._controller.Brain is not NpcBrainAnimal animalBrain)
                return;
            // Make sure the animal has met its spouse
            if (!animalBrain.SocialBehaviour.Crowd.MetSpouse)
                return;
            // Make sure the creature is fed
            if (__instance._controller.Data.Survival.IsHungry)
                return;
            // Finally, we can safely become pregnant
            __instance.Impregnate();
            Plugin.Logger.LogDebug($"NPC of ID '{__instance._controller.Data.Agent.SAgent.NpcId}' has become pregnant!");
        }

        [HarmonyPatch(nameof(NpcReproduction.GiveBirth))]
        [HarmonyPostfix]
        public static void GiveBirthPostfix(NpcReproduction __instance)
        {
            Plugin.Logger.LogDebug($"NPC of ID '{__instance._controller.Data.Agent.SAgent.NpcId}' is giving birth.");
            try
            {
                // int motherId = __instance._controller.Header.UniqueId;
                Random.GetRandomUnitCircle(out var randomCirclePosition);
                var spawnOffset = new Vector3(randomCirclePosition.x, 0, randomCirclePosition.y) * 1.3f;
                var soul = __instance._controller.Data.Transform.GlobalData.Soul;
                soul.SpawnNpcTracker(
                    __instance._controller.Data.Agent.SAgent.SocialGestationSpawnling,
                    __instance._controller.Data.Transform.TransformData.LocalPlatformPosition + spawnOffset);
                __instance._controller.Data.Social.SocialData.IsPregnant = false;
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError("Exception thrown while giving birth: " + e);
            }
        }
    }
}