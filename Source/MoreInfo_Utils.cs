using Verse;
using UnityEngine;
using System.Text;
using System.Reflection;
using System.Collections;
using RimWorld;
using Verse.AI;

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

        public static string ToStringAbstract(this object obj)
        {
            string value = "";
            if (obj == null)
                    value = "null";
            else
            {
                if (obj is IEnumerable collection)
                {
                    foreach (var c in collection)
                        value += c.ToString() + ", ";
                    value = "(" + value.TrimEnd(new char[] {',', ' '}) + ")";
                }
                else 
                    value = obj.ToString();
            }
            return value;
        }

        public static string ToStringDeep(this object obj, string spacer = "")
        {
            var builder = new StringBuilder();
            var fields = obj.GetType().GetFields();
            foreach(var fieldInfo in fields)
                builder.AppendInNewLine($"{spacer}{fieldInfo.Name} = {fieldInfo.GetValue(obj).ToStringAbstract()}");
            return builder.ToString();
        }

        public static float GetMoveSpeed(Pawn pawn)
        {
            float movePerTick = 60 / pawn.GetStatValue(StatDefOf.MoveSpeed, false);
            movePerTick +=  pawn.Map.pathGrid.CalculatedCostAt(pawn.Position, false, pawn.Position);
            Building edifice = pawn.Position.GetEdifice(pawn.Map);
            if (edifice != null)
            {
                movePerTick += (int)edifice.PathWalkCostFor(pawn);
            }

            if (pawn.CurJob != null)
            {
                switch (pawn.CurJob.locomotionUrgency)
                {
                    case LocomotionUrgency.Amble:
                        movePerTick *= 3;
                        if (movePerTick < 60)
                        {
                            movePerTick = 60;
                        }
                        break;
                    case LocomotionUrgency.Walk:
                        movePerTick *= 2;
                        if (movePerTick < 50)
                        {
                            movePerTick = 50;
                        }
                        break;
                    case LocomotionUrgency.Jog:
                        break;
                    case LocomotionUrgency.Sprint:
                        movePerTick = Mathf.RoundToInt(movePerTick * 0.75f);
                        break;
                }
            }
            return 60 / movePerTick;
        }
	}
}
