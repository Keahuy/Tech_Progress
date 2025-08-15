using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace KYTechProgress
{
    public class RitualOutcomeComp_FinishResearchPercentage : RitualOutcomeComp_Quality
    {
        public override float Count(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
        {
            return 0;
        }

        public override string GetDesc(LordJob_Ritual ritual = null, RitualOutcomeComp_Data data = null)
        {
            return "FinishResearchPercentageQualityEffect".Translate(Result.ToString("P0")) + "+" + $"{Result:P0}";
        }

        public override float QualityOffset(LordJob_Ritual ritual, RitualOutcomeComp_Data data) // 实际的加值
        {
            return Result;
        }

        public override QualityFactor GetQualityFactor(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
        {
            return new QualityFactor // 显示在UI上的影响因素
            {
                label = label.CapitalizeFirst(),
                count = $"{Result:P0}", // 显示的加值原因
                qualityChange = $"+{Result:P0}(+100%)", // 显示的加值与最大加值
                quality = Result,
                positive = true,
                priority = 10f
            };
        }

        public static float Result
        {
            get
            {
                List<ResearchProjectDef> optionalResearch = TechProgress_Mod.GetDefListByNameList(TechProgress_ModSettings.ResearchProjectDefOptional).Where(rp => rp.techLevel == Faction.OfPlayer.def.techLevel).ToList();
                if (optionalResearch.Count != 0)
                {
                    return optionalResearch.Where(rp => rp.IsFinished).Aggregate(0f, (current, _) => current + 1) / optionalResearch.Count;
                }

                return 1f;
            }
        }
    }
}