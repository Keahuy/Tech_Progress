using RimWorld;
using Verse;

namespace KYTechProgress
{
    public class RitualBehaviorWorker_TechProgress : RitualBehaviorWorker
    {
        public RitualBehaviorWorker_TechProgress(RitualBehaviorDef def) : base(def)
        {
        }

        public override string descriptionOverride // 显示在仪式窗口的描述
        {
            get
            {
                TechLevel CurrentTechLevel = Faction.OfPlayer.def.techLevel;
                TechLevel NextTechLevel = Faction.OfPlayer.def.techLevel + 1;
                return "KYRitualDescription".Translate() + "KYRitualCurrentTechLevel".Translate() + CurrentTechLevel.ToStringHuman() + "KYRitualNextTechLevel".Translate() + NextTechLevel.ToStringHuman();
            }
        }
    }
}