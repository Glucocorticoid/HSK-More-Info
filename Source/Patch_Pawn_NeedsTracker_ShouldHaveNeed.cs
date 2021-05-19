using Verse;
using RimWorld;
using HarmonyLib;
using CombatExtended;
using System.Reflection;

namespace MoreInfo
{
    [HarmonyPatch(typeof(Pawn_NeedsTracker))]
    [HarmonyPatch("ShouldHaveNeed")]
    public class Patch_Pawn_NeedsTracker_ShouldHaveNeed
    {
        public static void Postfix (Pawn_NeedsTracker __instance, NeedDef nd, ref bool __result)
        {
            if (__result && nd == MoreNeedDefOf.SuppressionCE)
            {
                if (CombatExtended.Controller.settings.EnabledSuppression && MoreInfo_Settings.showColonistSuppressionNeeds)
                {
                    var trackerType = typeof(Pawn_NeedsTracker);
                    var pawnField = trackerType.GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                    var pawn = pawnField.GetValue(__instance) as Pawn;
                    if (pawn != null)
                    {
                        if (pawn.TryGetComp<CompSuppressable>() == null)
                            __result = false;
                    }   
                }
                else
                    __result = false;
            }
        }
    }
}
