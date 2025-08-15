using RimWorld;
using Verse;

namespace KYTechProgress
{
    public class RitualSpectatorFilter_Reseachable: RitualSpectatorFilter
    {
        public override bool Allowed(Pawn p)
        {
            return !p.WorkTagIsDisabled(WorkTags.Intellectual);
        }
    }
}