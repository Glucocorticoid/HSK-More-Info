using Verse;
using System.Text;
using HarmonyLib;
using CombatExtended;

namespace MoreInfo
{
    [HarmonyPatch(typeof(Building_TurretGunCE))]
    [HarmonyPatch("GetInspectString")]
    public class Patch_Building_TurretGunCE_GetInspectString
    {
        static void Postfix (Building_TurretGunCE __instance, ref string __result)
        {
            if (MoreInfo_Settings.showLoadedAmmoStats && __instance != null && __instance.Gun != null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(__result);
                var ammoComp = __instance.Gun.TryGetComp<CompAmmoUser>();
                if (ammoComp != null)
                {
                    builder.AppendInNewLine(ammoComp.CurrentAmmo.LabelCap);
                    builder.AppendInNewLine(ammoComp.CurAmmoProjectile.GetProjectileReadout(__instance));
                }
                __result = builder.ToString().TrimEndNewlines();
            }
        }
    }
}
