using System.Collections.Generic;
using System.Linq;
using System.Text;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Helper;

namespace VNDBUpdater.Models
{
    public class Filter
    {
        public string Name { get; set; }

        public List<Tag> IncludedWithAnd { get; private set; }
        public List<Tag> IncludedWithOr { get; private set; }
        public List<Tag> Exluded { get; private set; }

        public Filter()
        {
            IncludedWithAnd = new List<Tag>();
            IncludedWithOr = new List<Tag>();
            Exluded = new List<Tag>();
        }

        public void AddAnd(Tag tag)
        {
            IncludedWithAnd.Add(tag);
        }

        public void AddOr(Tag tag)
        {
            IncludedWithOr.Add(tag);
        }

        public void AddExlude(Tag tag)
        {
            Exluded.Add(tag);
        }

        public bool ContainsTag(Tag tag)
        {
            return ((IncludedWithAnd.Any(x => x.Name == tag.Name) || IncludedWithOr.Any(x => x.Name == tag.Name) || Exluded.Any(x => x.Name == tag.Name)));
        }

        public void Delete()
        {
            RedisCommunication.DeleteFilter(this);
        }

        public bool ShouldVNBeFilteredOut(VisualNovel vn)
        {
            bool containsAnd = false, containsOr = false, containsExcluded = false;

            var tags = new List<Tag>();

            foreach (var tagID in vn.Basics.tags)
                if (TagHelper.LocalTags.Any(x => x.ID == tagID[0]))
                    tags.Add(TagHelper.LocalTags.Where(x => x.ID == tagID[0]).First());

            foreach (var and in IncludedWithAnd)
                if (tags.Any(x => x.Name == and.Name))
                    containsAnd = true;

            foreach (var or in IncludedWithOr)
                if (tags.Any(x => x.Name == or.Name))
                    containsOr = true;

            foreach (var not in Exluded)
                if (tags.Any(x => x.Name == not.Name))
                    containsExcluded = true;

            if (containsExcluded)
                return true;
            else if (containsAnd || containsOr)
                return false;
            else
                return true;
        }

        public string ToDetailedString()
        {
            return new StringBuilder()
                .Append("Included with AND: ")
                .Append(string.Join(",", IncludedWithAnd))
                .AppendLine()
                .Append("Included with OR: ")
                .Append(string.Join(",", IncludedWithOr))
                .AppendLine()
                .Append("Excluded: ")
                .Append(string.Join(",", Exluded))
                .ToString();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
