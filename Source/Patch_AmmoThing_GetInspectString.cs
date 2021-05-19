using Verse;
using System.Text;
using HarmonyLib;
using System.Linq;
using CombatExtended;

namespace MoreInfo
{
    [HarmonyPatch(typeof(AmmoThing))]
    [HarmonyPatch("GetInspectString")]
    public class Patch_AmmoThing_GetInspectString
    {
        static void Postfix (AmmoThing __instance, ref string __result)
        {
            if (MoreInfo_Settings.showLoadedAmmoStats && __instance != null && __instance.def != null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(__result);
                var ammoDef = __instance.def as AmmoDef;
                if (ammoDef != null && ammoDef.AmmoSetDefs != null && ammoDef.AmmoSetDefs.Any())
                    {
                        foreach (var set in ammoDef.AmmoSetDefs)
                        {
                            var reportProj = set.ammoTypes.Where(link => link.ammo == ammoDef);
                            if (!reportProj.EnumerableNullOrEmpty())
                            {
                                builder.AppendInNewLine(reportProj.First().projectile.GetProjectileReadout(null));
                                break;
                            }
                        }
                    }
                __result = builder.ToString().TrimEndNewlines();
            }
        }
    }
}
