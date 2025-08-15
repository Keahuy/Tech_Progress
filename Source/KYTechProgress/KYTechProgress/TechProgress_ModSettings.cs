using System.Collections.Generic;
using Verse;

namespace KYTechProgress
{
    public class TechProgress_ModSettings : ModSettings // 将数据保存到config文件夹
    {
        public static List<string> ResearchProjectDefNecessary = new();
        public static List<string> ResearchProjectDefOptional = new();
        public static List<string> ResearchProjectDefNotAllocation = new();
        public static float ThresholdForProgress;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ThresholdForProgress, "thresholdToProgress", 0.9f); // 参数：要保存的变量，变量保存在存档里后的独特名字，如果变量为空给的初始值
            Scribe_Collections.Look(ref ResearchProjectDefNecessary, "necessaryResearchProject", LookMode.Value);
            Scribe_Collections.Look(ref ResearchProjectDefOptional, "optionalResearchProject", LookMode.Value);
            Scribe_Collections.Look(ref ResearchProjectDefNotAllocation, "notAllocationResearchProject", LookMode.Value);
        }
    }
}