using RimWorld;
using UnityEngine;
using Verse;

namespace KYTechProgress
{
    public class TechProgress_Mod : Mod
    {
        public TechProgress_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<TechProgress_ModSettings>(); //读取本地数据
        }

        public TechProgress_ModSettings settings;

        public override string SettingsCategory()
        {
            return "[KY]Technology Progress"; // mod选项面板中的名字
        }

        public override void DoSettingsWindowContents(Rect inRect) // 为你的mod选项面板添加GUI组件
        {
            // 首次打开时执行一次性设置
            if (!_isGlobalSetupDone)
            {
                RefreshNotAllocation();
                _isGlobalSetupDone = true;
            }

            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.GapLine(); // 分割线，纯装饰
            listing_Standard.End();

            Text.Font = GameFont.Medium;
            Rect rect1 = new Rect(10f, 60f, listing_Standard.ColumnWidth, Text.LineHeight);

            Widgets.Label(new Rect(170f, rect1.y, rect1.width - 170f, Text.LineHeight), "CurrentlyConfiguredTechLevelInfo".Translate());

            // 获取除“未定义”之外的科技阶段作为点击按钮后显示在浮动菜单中的选项
            List<FloatMenuOption> allTechLevelList = new();
            for (int i = 1; i < 7; i++)
            {
                TechLevel t = (TechLevel)i;
                FloatMenuOption option = new FloatMenuOption(t.ToStringHuman(), () =>
                {
                    _selectedTechLevel = t;
                    _showNotAllocationRPByTechLevel = GetDefListByNameList(TechProgress_ModSettings.ResearchProjectDefNotAllocation).Intersect(AllResearchProjects()).Where(r => r.techLevel == t).ToList();
                    _showNecessaryRPDefByTechLevel = GetDefListByNameList(TechProgress_ModSettings.ResearchProjectDefNecessary).Intersect(AllResearchProjects()).Where(r => r.techLevel == t).ToList();
                    _showOptionalRPDefByTechLevel = GetDefListByNameList(TechProgress_ModSettings.ResearchProjectDefOptional).Intersect(AllResearchProjects()).Where(r => r.techLevel == t).ToList();
                });
                allTechLevelList.Add(option);
            }

            string buttonLabel = _selectedTechLevel == TechLevel.Undefined ? "KYNoSelected".Translate() : _selectedTechLevel.ToStringHuman();

            Text.Font = GameFont.Small;
            if (Widgets.ButtonText(new Rect(rect1.x, rect1.y, 150f, Text.LineHeight + 10f), buttonLabel, drawBackground: true, doMouseoverSound: true, active: true))
            {
                // 点击按钮后显示浮动菜单
                Find.WindowStack.Add(new FloatMenu(allTechLevelList));
            }

            rect1.y += rect1.height + 10f;

            Rect rectSlider = new Rect(rect1.x, rect1.y, rect1.width, 25f);
            Rect rectSliderL = rectSlider.LeftHalf();
            Widgets.Label(rectSliderL, "ThresholdForProgress".Translate());
            Rect rectSliderR = rectSlider.RightHalf();
            rectSliderR.xMin += 20f;
            rectSliderR.xMax -= 20f;
            float currentSliderValue = TechProgress_ModSettings.ThresholdForProgress;
            currentSliderValue = Widgets.HorizontalSlider(rectSliderR, currentSliderValue, 0f, 1f, true, currentSliderValue.ToString("P0"), "0", "100%");
            TechProgress_ModSettings.ThresholdForProgress = currentSliderValue;

            rect1.y += rect1.height - 5f;

            rect1.height = Text.LineHeight * 3;
            Widgets.Label(new Rect(rect1), "TechProgressControlInfo".Translate());

            rect1.y += rect1.height - 10f;

            LabelMedium(new Rect(12f, rect1.y, BoxWidth, 40f), "TechProgressNecessary".Translate());
            LabelMedium(new Rect(listing_Standard.ColumnWidth / 3 + 17f, rect1.y, BoxWidth, 40f), "TechProgressOptional".Translate());
            LabelMedium(new Rect(listing_Standard.ColumnWidth * 2 / 3 + 22f, rect1.y, BoxWidth, 40f), "TechProgressNotAllocation".Translate());
            rect1.y += 35f;

            ResearchContainer.DrawResearchContainer(new Rect(12f, rect1.y, BoxWidth, 400f), _showNecessaryRPDefByTechLevel, ref _techContainerScrollPos1, RefreshDisplayedResearchLists);
            ResearchContainer.DrawResearchContainer(new Rect(listing_Standard.ColumnWidth / 3 + 17f, rect1.y, BoxWidth, 400f), _showOptionalRPDefByTechLevel, ref _techContainerScrollPos2, RefreshDisplayedResearchLists);
            ResearchContainer.DrawResearchContainer(new Rect(listing_Standard.ColumnWidth * 2 / 3 + 22f, rect1.y, BoxWidth, 400f), _showNotAllocationRPByTechLevel, ref _techContainerScrollPos3, RefreshDisplayedResearchLists);

            base.DoSettingsWindowContents(inRect);
        }

        // 当前选择的科技水平，默认为未定义
        private static TechLevel _selectedTechLevel = TechLevel.Undefined;

        // 静态字段，用于存储过滤后的显示列表
        private static List<ResearchProjectDef> _showNotAllocationRPByTechLevel = new();
        private static List<ResearchProjectDef> _showNecessaryRPDefByTechLevel = new();
        private static List<ResearchProjectDef> _showOptionalRPDefByTechLevel = new();

        // 控制滚动的内容,如果三个滚动条用同一个 techContainerScrollPos 就会被同步滚动
        private static Vector2 _techContainerScrollPos1 = Vector2.zero;
        private static Vector2 _techContainerScrollPos2 = Vector2.zero;
        private static Vector2 _techContainerScrollPos3 = Vector2.zero;

        private static readonly float BoxWidth = 250f;
        private static bool _isGlobalSetupDone;

        private void RefreshDisplayedResearchLists()
        {
            if (_selectedTechLevel == TechLevel.Undefined)
            {
                // 如果没有选择科技水平，则清空所有列表
                _showNotAllocationRPByTechLevel.Clear();
                _showNecessaryRPDefByTechLevel.Clear();
                _showOptionalRPDefByTechLevel.Clear();
            }

            // 从settings中的主列表里，根据当前选择的科技水平过滤出要显示的研究项目
            var allResearch = AllResearchProjects().ToList();
            _showNotAllocationRPByTechLevel = GetDefListByNameList(TechProgress_ModSettings.ResearchProjectDefNotAllocation).Intersect(allResearch).Where(r => r.techLevel == _selectedTechLevel).ToList();
            _showNecessaryRPDefByTechLevel = GetDefListByNameList(TechProgress_ModSettings.ResearchProjectDefNecessary).Intersect(allResearch).Where(r => r.techLevel == _selectedTechLevel).ToList();
            _showOptionalRPDefByTechLevel = GetDefListByNameList(TechProgress_ModSettings.ResearchProjectDefOptional).Intersect(allResearch).Where(r => r.techLevel == _selectedTechLevel).ToList();
        }

        private static IEnumerable<ResearchProjectDef> AllResearchProjects()
        {
            return DefDatabase<ResearchProjectDef>.AllDefs;
        }

        void RefreshNotAllocation()
        {
            var necessaryDefs = GetDefListByNameList(TechProgress_ModSettings.ResearchProjectDefNecessary);
            var optionalDefs = GetDefListByNameList(TechProgress_ModSettings.ResearchProjectDefOptional);
            var notAllocatedDefs = GetDefListByNameList(TechProgress_ModSettings.ResearchProjectDefNotAllocation);

            // 找出所有尚未被分配的研究项目，并将它们添加到“未分配”列表中
            var unassigned = DefDatabase<ResearchProjectDef>.AllDefs.Where(rp =>
                !necessaryDefs.Contains(rp) &&
                !optionalDefs.Contains(rp) &&
                !notAllocatedDefs.Contains(rp)).ToList();

            if (unassigned.Any())
            {
                TechProgress_ModSettings.ResearchProjectDefNotAllocation.AddRange(GetNameListByDefList(unassigned));
            }
        }

        public static List<ResearchProjectDef> GetDefListByNameList(List<string> list)
        {
            List<ResearchProjectDef> researchProjectList = new();
            // 使用HashSet进行快速查找，避免重复添加
            HashSet<string> nameSet = new HashSet<string>(list);
            foreach (var defName in nameSet)
            {
                ResearchProjectDef named = DefDatabase<ResearchProjectDef>.GetNamed(defName, false);
                if (named != null)
                {
                    researchProjectList.Add(named);
                }
            }

            return researchProjectList;
        }

        private List<string> GetNameListByDefList(List<ResearchProjectDef> list)
        {
            return list.Select(def => def.defName).ToList();
        }

        private static void LabelMedium(Rect rect, string label)
        {
            GameFont font = Text.Font;
            Text.Font = GameFont.Medium;
            Widgets.Label(rect, label);
            Text.Font = font;
        }
    }
}