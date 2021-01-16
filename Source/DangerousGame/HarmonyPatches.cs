using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DangerousGame
{
    public static class HarmonyPatches
    {
        [StaticConstructorOnStartup]
        public static class DangerousGame
        {
            static DangerousGame()
            {
                Log.Message("More Dangerous Game initializing...");

                new Harmony("zylle.DangerousGame").PatchAll();
                ModifyAnimals();
            }

            static void ModifyAnimals()
            {
                DangerousGameSettings settings = DangerousGameMod.mod.settings;
                float revOnHarmMin = 0.01f * (float)settings.revOnDamage.min;
                float revOnHarmMax = 0.01f * (float)settings.revOnDamage.max;
                float revOnTameMin = 0.01f * (float)settings.revOnTameFail.min;
                float revOnTameMax = 0.01f * (float)settings.revOnTameFail.max;

                foreach (PawnKindDef pawn in DefDatabase<PawnKindDef>.AllDefs)
                {
                    if (pawn.RaceProps.Animal)
                    {
                        // set predator types
                        switch(settings.predType)
                        {
                            case PredType.Everything:
                                pawn.RaceProps.predator = true;
                                break;
                            case PredType.MeatEaters:
                                if (pawn.RaceProps.Eats(FoodTypeFlags.Corpse))
                                {
                                    pawn.RaceProps.predator = true;
                                }
                                break;
                            case PredType.Dangerous:
                                if (pawn.combatPower >= 100f)
                                {
                                    pawn.RaceProps.predator = true;
                                }
                                break;

                            default:        // default covers vanilla
                                break;
                        }


                        // revenge chance
                        pawn.RaceProps.manhunterOnDamageChance = Math.Min(revOnHarmMax, Math.Max(revOnHarmMin, pawn.RaceProps.manhunterOnDamageChance));
                        pawn.RaceProps.manhunterOnTameFailChance = Math.Min(revOnTameMax, Math.Max(revOnTameMin, pawn.RaceProps.manhunterOnDamageChance));

                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(RimWorld.FoodUtility), "IsAcceptablePreyFor")]
    static class PredatorPreyPatch
    {
        static bool Prefix(Pawn predator, Pawn prey, ref bool __result)
        {
            // Vanilla
            if (DangerousGameMod.mod.settings.validPrey == ValidPrey.Vanilla)
            {
                return true;
            }

            // Predators are not prey
            if (DangerousGameMod.mod.settings.validPrey == ValidPrey.Predators)
            {
                if (prey.RaceProps.predator && !prey.RaceProps.Humanlike)
                {
                    __result = false;
                    return false;
                }
            }

            // Wild predators only hunt humans. Tame predators don't hunt other predators.
            if (DangerousGameMod.mod.settings.validPrey == ValidPrey.Humans)
            {
                if (predator.Faction == null)
                {
                    __result = false;
                    if (prey.RaceProps.Humanlike)
                    {
                        __result = true;
                    }
                    return false;
                }
                else if (prey.RaceProps.predator && !prey.RaceProps.Humanlike)
                {
                    __result = false;
                    return false;
                }
            }

            // All predators hunt only colonists
            if (DangerousGameMod.mod.settings.validPrey == ValidPrey.Colonists)
            {
                __result = false;
                if (prey.RaceProps.Humanlike && prey.Faction == Faction.OfPlayer)
                {
                    __result = true;
                }
                return false;
            }


            return true;
        }
    }

}
