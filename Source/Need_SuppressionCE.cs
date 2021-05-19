using Verse;
using RimWorld;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using CombatExtended;

namespace MoreInfo
{
    public class Need_SuppressionCE : Need
    {
        public Need_SuppressionCE(Pawn pawn)
		{
			this.pawn = pawn;
		}

        public CompSuppressable SuppressionTracker
		{
			get
			{
				return this.pawn.TryGetComp<CompSuppressable>();
			}
		}

        public float SuppressionThreshold
        {
            get
            {
                return SuppressionTracker != null ? MoreInfo_Utils.GetSuppressionThreshold(this.pawn) : 1f ;
            }
        }

        public override float MaxLevel
		{
			get
			{
				return  SuppressionTracker != null ? 1050f : 1f;
			}
		}

        public override void SetInitialLevel()
		{
			this.CurLevel = SuppressionTracker != null ? SuppressionTracker.CurrentSuppression : 1f;
		}

        public override float CurLevel
        {
            get
            {
                return SuppressionTracker != null ? this.SuppressionTracker.CurrentSuppression : 1f;
            }
            set
            {
            }
        }

        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = 2147483647, float customMargin = -1f, bool drawArrows = true, bool doTooltip = true)
		{
			if (this.threshPercents == null)
			{
				this.threshPercents = new List<float>();
			}
			this.threshPercents.Clear();
            this.threshPercents.Add(SuppressionThreshold / this.MaxLevel);
            this.threshPercents.Add(SuppressionThreshold * 10 > this.MaxLevel ? 1f : SuppressionThreshold * 10 / this.MaxLevel );
			base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
		}

        public override string GetTipString()
        {
            var builder = new StringBuilder();
            builder.AppendInNewLine(string.Concat(new string[]
            {
                base.LabelCap,
                ": ",
                base.CurLevelPercentage.ToStringPercent(),
                " (",
                this.CurLevel.ToString("0.##"),
                " / ",
                this.MaxLevel.ToString("0.##"),
                ")\n"
            }));
            builder.AppendInNewLine(string.Format("{0}: {1:P0}", "MWI_Pawn_suppressability".Translate(), pawn.GetStatValue(CE_StatDefOf.Suppressability)));
            builder.AppendInNewLine(string.Format("{0}: {1:0.00}", "MWI_Suppression_threshold".Translate(), SuppressionThreshold));
            builder.AppendInNewLine(string.Format("{0}: {1:0} \n", "MWI_Accumulated_suppression_amount".Translate(), SuppressionTracker.CurrentSuppression));
            builder.AppendInNewLine(this.def.description);
            return builder.ToString();
        }

        public override void NeedInterval()
		{
			
		}
    }
}
