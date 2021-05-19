using Verse;
using RimWorld;
using System.Text;
using HarmonyLib;
using System.Linq;
using CombatExtended;

namespace MoreInfo
{
    [HarmonyPatch(typeof(ThingWithComps))]
    [HarmonyPatch("GetInspectString")]
    public class Patch_Textile_Thing_GetInspectString
    {
        static void Postfix (ThingWithComps __instance, ref string __result)
        {
            if (__instance != null && __instance.def != null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(__result);
                if (MoreInfo_Settings.showTextileInfo && __instance.def.IsTextiles())
                    {
                        var immuneMod = __instance?.def?.stuffProps?.statFactors?.Where(stat => stat.stat.defName == "ImmunityGainSpeedFactor").First();
                        if (immuneMod != null)
                                builder.AppendInNewLine(GetFormattedString(immuneMod));

                        var restMod = __instance?.def?.stuffProps?.statFactors?.Where(stat => stat.stat.defName == "BedRestEffectiveness").First();
                        if (restMod != null)
                                builder.AppendInNewLine(GetFormattedString(restMod));
                    }
                if (MoreInfo_Settings.showApparelArmorStats && __instance.def.IsApparel)
                    {
                        builder.AppendInNewLine(GetCompactReportString(__instance, StatDefOf.ArmorRating_Sharp, StatDefOf.ArmorRating_Sharp.formatString, "0.00"));
                        builder.AppendInNewLine(GetCompactReportString(__instance, StatDefOf.ArmorRating_Blunt, StatDefOf.ArmorRating_Blunt.formatString, "0.00"));
                    }
                if (MoreInfo_Settings.showWorkTableSpeedFactor && __instance.def.IsWorkTable)
                    {
                        builder.AppendInNewLine(GetCompactReportString(__instance, StatDefOf.WorkTableWorkSpeedFactor, null, "P"));
                    }
                if (MoreInfo_Settings.showBedStats && __instance.def.IsBed)
                    {
                        builder.AppendInNewLine(GetCompactReportString(__instance, StatDefOf.ImmunityGainSpeedFactor, null, "P"));
                        builder.AppendInNewLine(GetCompactReportString(__instance, StatDefOf.BedRestEffectiveness, null, "P"));
                    }
                if (MoreInfo_Settings.showLoadedAmmoStats && __instance.def.IsRangedWeapon)
                    {
                        var ammoComp = __instance.TryGetComp<CompAmmoUser>();
                        if (ammoComp != null)
                        {
                            builder.AppendInNewLine(ammoComp.CurrentAmmo.LabelCap);
                            builder.AppendInNewLine(ammoComp.CurAmmoProjectile.GetProjectileReadout(__instance));
                        }
                    } 
                __result = builder.ToString().TrimEndNewlines();
            }
        }

        private static string GetCompactReportString(Thing item, StatDef def, string formatValueString = null, string formatValue = null)
        {
            var statValue = item.GetStatValue(def);
            var formatedValue = formatValueString != null ? string.Format(formatValueString, statValue.ToString(formatValue)) : statValue.ToString(formatValue);
            return string.Format("{0}: {1}", def.LabelCap, formatedValue);
        }

        private static string GetFormattedString(StatModifier modifier)
        {
            return string.Format("{0}: {1}", modifier.stat.LabelForFullStatListCap, modifier.ToStringAsFactor);
        }
    }
}
