using System.Collections.Generic;

namespace VNDBUpdater.Communication.VNDB
{
    public class VNInformationRoot
    {
        public List<VNInformation> items { get; set; }
        public int num { get; set; }
        public bool more { get; set; }
    }    

    public class VNLinks
    {
        public object encubed { get; set; }
        public object renai { get; set; }
        public object wikipedia { get; set; }
    }

    public class VNScreenshots
    {
        public int height { get; set; }
        public bool nsfw { get; set; }
        public int rid { get; set; }
        public string image { get; set; }
        public int width { get; set; }
    }

    public class VNInformation
    {
        public List<string> platforms { get; set; }
        public int votecount { get; set; }
        public string released { get; set; }
        public List<string> orig_lang { get; set; }
        public int? length { get; set; }
        public string image { get; set; }
        public string title { get; set; }
        public double rating { get; set; }
        public string description { get; set; }
        public VNLinks links { get; set; }
        public List<List<double>> tags { get; set; }
        public List<string> languages { get; set; }
        public int id { get; set; }
        public string aliases { get; set; }
        public double popularity { get; set; }
        public object original { get; set; }
        public List<VNScreenshots> screens { get; set; }
        public bool image_nsfw { get; set; }
    }
    
    public class VNCharacterInformationRoot
    {
        public List<VNCharacterInformation> items { get; set; }
        public bool more { get; set; }
        public int num { get; set; }
    }

    public class VNCharacterInformation
    {
        public string name { get; set; }
        public object description { get; set; }
        public List<object> birthday { get; set; }
        public string original { get; set; }
        public List<List<int>> traits { get; set; }
        public List<List<object>> vns { get; set; }
        public int id { get; set; }
        public string gender { get; set; }
        public string image { get; set; }
        public object aliases { get; set; }
        public object bloodt { get; set; }
    }

    public class GetTemplate<T>
    {
        public int num { get; set; }
        public bool more { get; set; }

        public List<T> items { get; set; }
    }

    public class VNListRoot : GetTemplate<VN>
    {
    }

    public class VN
    {
        public int vn { get; set; }
        public object notes { get; set; }
        public int added { get; set; }
        public int status { get; set; }
    }

    public class WishListRoot : GetTemplate<Wish>
    {
    }

    public class Wish
    {
        public int priority { get; set; }
        public int added { get; set; }
        public int vn { get; set; }
    }

    public class VoteListRoot : GetTemplate<Vote>
    {
    }

    public class Vote
    {
        public int vn { get; set; }
        public int vote { get; set; }
        public int added { get; set; }
    }

    public class Error
    {
        public double minwait { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string msg { get; set; }
        public double fullwait { get; set; }
    }
}
