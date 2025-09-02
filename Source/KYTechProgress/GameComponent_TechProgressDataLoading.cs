using RimWorld;
using Verse;

namespace KYTechProgress;

public class GameComponent_TechProgressDataLoading : GameComponent
{
    public static TechLevel currentGameTechLevel;

    public GameComponent_TechProgressDataLoading(Game game)
    {
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref currentGameTechLevel, "CurrentGameTechLevel");
    }

    public override void StartedNewGame()
    {
        base.StartedNewGame();
        Faction.OfPlayer.def.techLevel = DefDatabase<FactionDef>.GetNamed(Faction.OfPlayer.def.defName).techLevel;
        currentGameTechLevel = Faction.OfPlayer.def.techLevel;
    }

    public override void LoadedGame()
    {
        base.LoadedGame();
        Faction.OfPlayer.def.techLevel = currentGameTechLevel;
    }
}