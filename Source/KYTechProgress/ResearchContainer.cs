using UnityEngine;
using Verse;

namespace KYTechProgress
{
    public class ResearchContainer
    {
        public static void DrawResearchContainer(Rect researchContainer, List<ResearchProjectDef> researchList, ref Vector2 techContainerScrollPos, Action onListChanged1)
        {
            // 画一个框来显示 techContainer 的范围与位置
            // Widgets.DrawBox(researchContainer);
            Widgets.DrawHighlight(researchContainer);
            Widgets.BeginGroup(researchContainer);
            // 先把元素排一遍得出显示这些元素需要的高度(实际上没显示)
            Rect rect1 = GenUI.DrawElementStack(new Rect(0f, 0f, researchContainer.width - 20f, researchContainer.height), 22f, researchList, delegate { }, r => Text.CalcSize(r.LabelCap).x + 10f);
            Widgets.BeginScrollView(new Rect(0f, 0f, researchContainer.width, researchContainer.height), ref techContainerScrollPos, new Rect(0f, 0f, researchContainer.width, rect1.height + 20f), false);
            GenUI.DrawElementStack(new Rect(10f, 10f, researchContainer.width - 20f, researchContainer.height), 22f, researchList, delegate(Rect r, ResearchProjectDef rp)
            {
                Color color1 = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, 0.1f);
                GUI.DrawTexture(r, BaseContent.WhiteTex);
                GUI.color = color1;
                if (Mouse.IsOver(r))
                {
                    Widgets.DrawHighlight(r); // 使被鼠标指到的项目突出显示
                }

                Widgets.Label(new Rect(r.x + 5f, r.y, r.width - 10f, r.height), rp.LabelCap);
                GUI.color = Color.white;
                if (Mouse.IsOver(r))
                {
                    ResearchProjectDef rpLocal = rp;
                    TooltipHandler.TipRegion(r, new TipSignal(() => rpLocal.Description, 100)); // 显示研究项目的描述
                }

                string rpDefName = rp.defName;
                bool changed = false;
                // 左键单击: 移动到 "必要"
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && r.Contains(Event.current.mousePosition) && !Event.current.shift)
                {
                    if (!TechProgress_ModSettings.ResearchProjectDefNecessary.Contains(rpDefName))
                    {
                        TechProgress_ModSettings.ResearchProjectDefNecessary.Add(rpDefName);
                    }

                    TechProgress_ModSettings.ResearchProjectDefNotAllocation.Remove(rpDefName);
                    TechProgress_ModSettings.ResearchProjectDefOptional.Remove(rpDefName);
                    changed = true;
                }

                // 中键单击 或 Shift + 左键单击: 移动到 "可选"
                if ((Event.current.type == EventType.MouseDown && Event.current.button == 2 && r.Contains(Event.current.mousePosition)) || (Event.current.shift && Event.current.type == EventType.MouseDown && Event.current.button == 0 && r.Contains(Event.current.mousePosition)))
                {
                    if (!TechProgress_ModSettings.ResearchProjectDefOptional.Contains(rpDefName))
                    {
                        TechProgress_ModSettings.ResearchProjectDefOptional.Add(rpDefName);
                    }

                    TechProgress_ModSettings.ResearchProjectDefNotAllocation.Remove(rpDefName);
                    TechProgress_ModSettings.ResearchProjectDefNecessary.Remove(rpDefName);
                    changed = true;
                }

                // 右键单击: 移动到 "未分配"
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && r.Contains(Event.current.mousePosition) && !Event.current.shift)
                {
                    if (!TechProgress_ModSettings.ResearchProjectDefNotAllocation.Contains(rpDefName))
                    {
                        TechProgress_ModSettings.ResearchProjectDefNotAllocation.Add(rpDefName);
                    }

                    TechProgress_ModSettings.ResearchProjectDefNecessary.Remove(rpDefName);
                    TechProgress_ModSettings.ResearchProjectDefOptional.Remove(rpDefName);
                    changed = true;
                }

                // 如果发生了移动，则调用回调函数并消费事件
                if (changed)
                {
                    onListChanged1?.Invoke(); // 触发刷新
                    Event.current.Use();
                }
            }, r => Text.CalcSize(r.LabelCap).x + 10f);
            Widgets.EndScrollView();
            Widgets.EndGroup();
        }
    }
}