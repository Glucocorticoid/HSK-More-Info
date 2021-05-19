using Verse;
using UnityEngine;

namespace MoreInfo
{
	public static class MoreInfo_Utils
	{
		public static float GetSuppressionThreshold(Pawn pawn)
        {
            float result = 0f;
            if (pawn != null)
            {
                float valueOrDefault = (pawn.mindState?.mentalBreaker?.BreakThresholdMajor).GetValueOrDefault();
                float num = pawn.needs?.mood?.CurLevel ?? 0.5f;
                result = Mathf.Sqrt(Mathf.Max(0f, num - valueOrDefault)) * 1050f * 0.125f;
            }
            else
            {
                Log.Error("CE tried to get suppression threshold of non-pawn");
            }
            return result;
        }
	}
}
