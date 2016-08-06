using System.Collections.Generic;
using VNDBUpdater.Communication.VNDB;

namespace VNDBUpdater.Models
{
    public class BasicInformation : VNInformation
    {
        public List<Tag> ConvertedTags { get; set; }

        public BasicInformation(VNInformation basics)
        {
            if (basics != null)
            {
                aliases = basics.aliases;
                description = basics.description;
                id = basics.id;
                image = basics.image;
                image_nsfw = basics.image_nsfw;
                languages = basics.languages;
                length = basics.length;
                links = basics.links;
                original = basics.original;
                orig_lang = basics.orig_lang;
                platforms = basics.platforms;
                popularity = basics.popularity;
                rating = basics.rating;
                released = basics.released;
                screens = basics.screens;
                tags = basics.tags;
                title = basics.title;
                votecount = basics.votecount;
                ConvertedTags = Tag.FindMatchingTagsForVisualNovel(basics);
            }
        }
    }
}
