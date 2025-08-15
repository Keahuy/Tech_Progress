using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace KYTechProgress
{
    public class RitualObligationTargetWorker_TechProgress : RitualObligationTargetWorker_ThingDef
    {
        public RitualObligationTargetWorker_TechProgress(RitualObligationTargetFilterDef def) :
            base(def) // 必要的构造器
        {
        }

        public override IEnumerable<TargetInfo> GetTargets(RitualObligation obligation, Map map)
        {
            if (def.thingDefs.NullOrEmpty())
            {
                yield break;
            }

            for (int i = 0; i < def.thingDefs.Count; i++)
            {
                ThingDef thingDef = def.thingDefs[i];
                List<Thing> things = map.listerThings.ThingsOfDef(thingDef);
                for (int j = 0; j < things.Count; j++)
                {
                    if (CanUseTarget(things[j], obligation).canUse)
                    {
                        yield return things[j];
                    }
                }
            }
        }

        protected override RitualTargetUseReport CanUseTargetInternal(TargetInfo target, RitualObligation obligation) // 检查仪式集合点是否可用
        {
            List<ResearchProjectDef> list = TechProgress_Mod.GetDefListByNameList(TechProgress_ModSettings.ResearchProjectDefNecessary).Where(rp => rp.techLevel == Faction.OfPlayer.def.techLevel).ToList();
            RitualTargetUseReport result = base.CanUseTargetInternal(target, obligation);

            // 到达超凡时代后隐藏仪式Gizmo
            if (Faction.OfPlayer.def.techLevel == TechLevel.Archotech)
            {
                result.canUse = false;
                result.failReason = null;
                return result;
            }

            if (!base.CanUseTargetInternal(target, obligation).canUse)
            {
                return false;
            }

            if (!list.Any())
            {
                return "MissNecessaryResearchProjectSetting".Translate();
            }

            if (list.Select(rp => rp.IsFinished).Contains(false))
            {
                string text1 = "HasNecessaryResearchProjectUnfinished".Translate(list.Count(rp => !rp.IsFinished));
                string text2 = "\n";
                foreach (var researchProject in list.Where(rp => !rp.IsFinished))
                {
                    text2 += researchProject.LabelCap + ", ";
                }

                return $"{text1}{text2}";
            }

            return true;
        }

        public override IEnumerable<string> GetTargetInfos(RitualObligation obligation)
        {
            yield return "RitualTargetKYTechProgressTableInfo".Translate(); // Gizmo的描述中“集合点物件”的翻译
        }
    }
}