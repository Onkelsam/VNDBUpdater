using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.Data;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Services.Tags
{
    public class TagService : TagsAndTraitsBase, ITagService
    {
        private List<TagModel> _Tags;

        private const string _TagsDumpFileName = "tags.json";
        private const string _TagsZipFileName = "tags.json.gz";
        private const string _TagsDownloadLink = "http://vndb.org/api/tags.json.gz";

        public TagService()
        {
            _Tags = new List<TagModel>();

            GetTags();
        }

        public IList<TagModel> Get()
        {
            return _Tags;
        }

        public async Task Refresh()
        {
            await base.RefreshAsync(_TagsDownloadLink, _TagsZipFileName);

            _Tags = new List<TagModel>();

            GetTags();
        }

        public bool Show(SpoilerSetting UserSetting, SpoilerLevel TSpoiler)
        {
            return (int)UserSetting >= (int)TSpoiler;
        }

        private void GetTags()
        {
            var rawTags = new List<TagsLookUp>();

            if (File.Exists(_TagsDumpFileName))
            {
                rawTags = JsonConvert.DeserializeObject<List<TagsLookUp>>(File.ReadAllText(_TagsDumpFileName));

                foreach (var tag in rawTags)
                {
                    _Tags.Add(new TagModel(tag));
                }
            }
        }
    }
}
