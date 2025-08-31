using RimWorld;
using Verse;

namespace KYTechProgress
{
    public class RitualOutcomeEffectWorker_TechProgress : RitualOutcomeEffectWorker_FromQuality
    {
        public RitualOutcomeEffectWorker_TechProgress(){}
        public RitualOutcomeEffectWorker_TechProgress(RitualOutcomeEffectDef_DoubleChance def) : base(def)
        {
            this.def_DoubleChance = def;
        }

        public RitualOutcomeEffectDef_DoubleChance def_DoubleChance;
        public override bool SupportsAttachableOutcomeEffect => false;

        public override RitualOutcomePossibility GetOutcome(float quality, LordJob_Ritual ritual)
        {
            if (RitualOutcomeComp_FinishResearchPercentage.Result >= TechProgress_ModSettings.ThresholdForProgress)
            {
                return def_DoubleChance.anotherOutcomeChances.Where(o => OutcomePossible(o, ritual)).RandomElementByWeight(c => ChanceWithQuality(c, quality));
            }

            return def.outcomeChances.Where(o => OutcomePossible(o, ritual)).RandomElementByWeight(c => ChanceWithQuality(c, quality));
        }

        protected override void ApplyExtraOutcome(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, RitualOutcomePossibility outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
        {
            extraOutcomeDesc = null;
            if (outcome.positivityIndex >= 0)
            {
                Faction.OfPlayer.def.techLevel += 1; // 防溢出在 RitualObligationTargetWorker_TechProgress 类里
            }

            // 从所有地图获取所有玩家pawn，并合并成一个序列，筛选出不存在于totalPresence字典中的pawn
            IEnumerable<Pawn> pawnsToGiveMemory = Find.Maps.SelectMany(map => map.mapPawns.PawnsInFaction(Faction.OfPlayer)).Where(pawn => !totalPresence.ContainsKey(pawn));

            switch (outcome.positivityIndex)
            {
                case 1:
                {
                    foreach (var pawn in pawnsToGiveMemory)
                    {
                        GiveMemoryToPawn(pawn, DefDatabase<ThoughtDef>.GetNamed("EffectiveTechProgressForOther"), jobRitual);
                    }

                    break;
                }
                case 2:
                {
                    foreach (var pawn in pawnsToGiveMemory)
                    {
                        GiveMemoryToPawn(pawn, DefDatabase<ThoughtDef>.GetNamed("UnforgettableTechProgressForOther"), jobRitual);
                    }

                    var eligiblePawns = Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer).Where(p => p.IsColonist && p.Inspiration == null).ToList();
                    if (eligiblePawns.NullOrEmpty())
                    {
                        break;
                    }

                    int inspirationCount = Rand.Range(1, Math.Min(3, eligiblePawns.Count));
                    List<Pawn> targetPawns = eligiblePawns.InRandomOrder().Take(inspirationCount).ToList();
                    foreach (var pawn in targetPawns)
                    {
                        if (pawn.mindState.inspirationHandler.GetRandomAvailableInspirationDef() is InspirationDef randomInspirationDef)
                        {
                            pawn.mindState.inspirationHandler.TryStartInspiration(randomInspirationDef, "TechProgressMeetingInspiration".Translate(pawn.Named("PAWN")));
                        }
                    }

                    break;
                }
            }
        }

        public List<ResearchProjectDef> optionalResearch
        {
            get { return TechProgress_Mod.GetDefListByNameList(TechProgress_ModSettings.ResearchProjectDefOptional).Where(rp => rp.techLevel == Faction.OfPlayer.def.techLevel).ToList(); }
        }
    }
}