using RimWorld;
using Verse;

namespace KYTechProgress
{
    public class RitualOutcomeComp_MaxResearchSkill : RitualOutcomeComp_Quality
    {
        public override float Count(LordJob_Ritual ritual, RitualOutcomeComp_Data data)
        {
            return 0;
        }

        public override string GetDesc(LordJob_Ritual ritual = null, RitualOutcomeComp_Data data = null)
        {
            return "MaxIntelligenceSkillInParticipants".Translate() + "+" + QualityOffset(ritual, data).ToString("P0");
        }

        public override float QualityOffset(LordJob_Ritual ritual, RitualOutcomeComp_Data data) // 实际的加值
        {
            return (ritual.assignments.Participants.Any()) ? ritual.assignments.Participants.Select(p => p.skills.GetSkill(SkillDefOf.Intellectual).Level).Max() / 100f : 0f;
        }

        public override QualityFactor GetQualityFactor(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
        {
            float result = assignments.Participants.Any() ? assignments.Participants.Select(p => p.skills.GetSkill(SkillDefOf.Intellectual).Level).Max() / 100f : 0f;
            return new QualityFactor
            {
                label = label.CapitalizeFirst(),
                count = $"{result:P0}", // 显示的加值原因
                qualityChange = $"+{result:P0}", // 显示的加值与最大加值
                quality = result,
                positive = true,
                priority = 4f
            };
        }
    }
}