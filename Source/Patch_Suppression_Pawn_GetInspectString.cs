using Verse;
using RimWorld;
using System.Text;
using HarmonyLib;
using CombatExtended;

namespace MoreInfo
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("GetInspectString")]
    public class Patch_Suppression_Pawn_GetInspectString
    {
        static void Postfix (Pawn __instance, ref string __result)
        {
            if (CombatExtended.Controller.settings.EnabledSuppression)
            {
                if ((__instance.IsColonist 
                    && MoreInfo_Settings.showColonistSuppression) 
                        || (__instance.RaceProps.Humanlike
                            && __instance.HostileTo(Find.FactionManager.OfPlayer) 
                            && MoreInfo_Settings.showEnemySuppression))
                {
                    CompSuppressable compSuppressable = __instance.TryGetComp<CompSuppressable>();
                    if (compSuppressable != null)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append(__result);

                        var supAmount = string.Format("{0}: {1:0}", "MWI_Accumulated_suppression_amount".Translate(), compSuppressable.CurrentSuppression);
                        stringBuilder.AppendInNewLine(supAmount);
                        if (compSuppressable.isSuppressed)
                            stringBuilder.Append(string.Format("({0})", "MWI_Suppressed".Translate().ToString()));

                        var thresh = MoreInfo_Utils.GetSuppressionThreshold(__instance);
                        var supThresh = string.Format("{0}: {1:0.00}", "MWI_Suppression_threshold".Translate(), thresh);
                        stringBuilder.AppendInNewLine(supThresh);

                        var suppresab = __instance.GetStatValue(CE_StatDefOf.Suppressability);
                        var suppresabText = string.Format("{0}: {1:P0}", "MWI_Pawn_suppressability".Translate(), suppresab);
                        stringBuilder.AppendInNewLine(suppresabText);

                        __result = stringBuilder.ToString().TrimEndNewlines();
                    } 
                }
            }   
            /*static void Postfix (Pawn __instance, ref string __result)
            {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(__result);
            if (Prefs.DevMode)
            {
                if (__instance.Faction != null)
                {
                    if (__instance.Faction.HostileTo(Find.FactionManager.OfPlayer))
                    {
                        string text = null;
                        Lord lord = __instance.GetLord();
                        if (lord != null && lord.LordJob != null)
                        {
                            text = lord.LordJob.GetReport(__instance);
                        }
                        if (__instance.jobs.curJob != null)
                        {
                            try
                            {
                                string text2 = __instance.jobs.curDriver.GetReport().CapitalizeFirst();
                                if (!text.NullOrEmpty())
                                {
                                    text = text + ": " + text2;
                                }
                                else
                                {
                                    text = text2;
                                }
                            }
                            catch (Exception arg)
                            {
                                Log.Error("JobDriver.GetReport() exception: " + arg, false);
                            }
                        }
                        if (!text.NullOrEmpty())
                        {
                            text = "\nDEBUG: " + text;
                            stringBuilder.AppendLine(text);
                        }
                    }
                }
            }
            __result = stringBuilder.ToString().TrimEndNewlines();
        }
        */
        }
    }
}
