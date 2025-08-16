using RimWorld;
using Verse;

namespace KYTechProgress
{
    public class RitualOutcomeComp_FinishPercentageOverThreshold : RitualOutcomeComp_Quality
    {
        public override float Count(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
        {
            return 0;
        }

        public override QualityFactor GetQualityFactor(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
        {
            bool result = RitualOutcomeComp_FinishResearchPercentage.Result >= TechProgress_ModSettings.ThresholdForProgress;
            return new QualityFactor
            {
                label = label.CapitalizeFirst(),
                count = " ", 
                qualityChange = result.ToString(), // 显示的加值与最大加值
                quality = 0,
                positive = result,
                priority = 4f
            };
        }
    }
}