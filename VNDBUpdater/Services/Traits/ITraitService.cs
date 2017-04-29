using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.Traits
{
    public interface ITraitService : ITagsAndTraits<TraitModel>
    {
        TraitModel GetLastParentTrait(TraitModel trait);
    }
}
