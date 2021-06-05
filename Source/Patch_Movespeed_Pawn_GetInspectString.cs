using Verse;
using RimWorld;
using System.Text;
using HarmonyLib;
using CombatExtended;
using Verse.AI;

namespace MoreInfo
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("GetInspectString")]
    public class Patch_Movespeed_Pawn_GetInspectString
    {
        static void Postfix (Pawn __instance, ref string __result)
        {
            if (MoreInfo_Settings.showMovingSpeed)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(__result);

                var actualSpeed = MoreInfo_Utils.GetMoveSpeed(__instance);
                var moution = __instance.CurJob?.locomotionUrgency ?? LocomotionUrgency.None;
                var speedText = string.Format("MWI_Format".Translate(), "MWI_Pawn_CurrentMoveSpeed".Translate(), actualSpeed, moution.ToString().Translate());
                stringBuilder.AppendInNewLine(speedText);

                __result = stringBuilder.ToString().TrimEndNewlines();
            }   
        }
    }
}
