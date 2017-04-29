using CommunicationLib.VNDB.Connection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.Communication.VNDB.Interfaces;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.Services.Logger;
using VNDBUpdater.Services.Tags;
using VNDBUpdater.Services.Traits;

namespace VNDBUpdater.Communication.VNDB
{
    public class VNDBGetter : IVNDBGetter
    {
        private IVNDB _VNDBService;
        private ITagService _TagService;
        private ITraitService _TraitService;
        private ILoggerService _LoggerService;

        public VNDBGetter(IVNDB VNDBService, ITagService TagService, ITraitService TraitService, ILoggerService LoggerService)
        {
            _VNDBService = VNDBService;
            _TagService = TagService;
            _TraitService = TraitService;
            _LoggerService = LoggerService;
        }

        public VisualNovelModel Get(int ID)
        {
            var visualNovel = new VisualNovelModel();

            visualNovel.Basics = new BasicInformationModel(GetBasicInformation(ID).items[0], _TagService);
            visualNovel.Characters = GetCharInfo(ID).Select(x => new CharacterInformationModel(x, _TraitService)).ToList();

            return visualNovel;
        }

        public IList<VisualNovelModel> Get(string title)
        {
            List<VNInformation> result = GetBasicInformation(title);
            var models = new List<VisualNovelModel>();

            foreach (var vn in result)
            {
                var newVN = new VisualNovelModel();
                newVN.Basics = new BasicInformationModel(vn, _TagService, false);

                models.Add(newVN);
            }

            return models;
        }

        public IList<VisualNovelModel> Get(List<int> IDs)
        {
            var vns = new List<VisualNovelModel>();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<VNInformation> basicInformation = GetBasicInformation(IDs).items;

            _LoggerService.Log("Get basic information for: " + IDs.Count + " ids: " + sw.ElapsedMilliseconds.ToString());
            sw.Restart();


            List<VNCharacterInformation> charInformation = GetCharInfo(IDs);


            _LoggerService.Log("Get char information for: " + IDs.Count + " ids: " + sw.ElapsedMilliseconds.ToString());
            sw.Restart();


            foreach (var id in IDs)
            {
                VNInformation basic = basicInformation.Where(x => x.id == id).SingleOrDefault();
                List<VNCharacterInformation> chars = charInformation.FindAll(x => x.vns.Any(y => y.Any(u => u.ToString() == id.ToString()))).ToList();

                // Check if visual novel still exists.
                if (basic != null)
                {                    
                    var newVN = new VisualNovelModel();

                    newVN.Basics = new BasicInformationModel(basic, _TagService);

                    _LoggerService.Log("Build basic information: " + sw.ElapsedMilliseconds.ToString());
                    sw.Restart();

                    newVN.Characters = chars.Select(x => new CharacterInformationModel(x, _TraitService)).ToList();

                    _LoggerService.Log("Build Character information: " + sw.ElapsedMilliseconds.ToString());
                    sw.Restart();

                    vns.Add(newVN);
                }
            }

            return vns;
        }

        private List<VNInformation> GetBasicInformation(string title)
        {
            var infos = new List<VNInformation>();

            VNInformationRoot convertedResult;
            int page = 1;

            do
            {
                convertedResult = QueryBasicInformation(title, page);

                page++;

                infos.AddRange(convertedResult.items);

            } while (convertedResult.more);

            return infos;
        }

        private  VNInformationRoot QueryBasicInformation(string title, int page)
        {
            VndbResponse result = _VNDBService.SearchByTitle(title, page);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    return  QueryBasicInformation(title, page);
                }
                else
                {
                    return new VNInformationRoot();
                }
            }
            else
            {
                return JsonConvert.DeserializeObject<VNInformationRoot>(result.Payload);
            }
        }

        private  VNInformationRoot GetBasicInformation(int ID)
        {
            VndbResponse result = _VNDBService.QueryInformation(ID);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    return  GetBasicInformation(ID);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return JsonConvert.DeserializeObject<VNInformationRoot>(result.Payload);
            }
        }

        private  VNInformationRoot GetBasicInformation(List<int> IDs)
        {
            VndbResponse result = _VNDBService.QueryInformation(IDs.ToArray());

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    return  GetBasicInformation(IDs);
                }                    
                else
                {
                    return null;
                }
            }
            else
            {
                return JsonConvert.DeserializeObject<VNInformationRoot>(result.Payload);
            }
        }

        private  List<VNCharacterInformation> GetCharInfo(int ID)
        {
            VndbResponse result = _VNDBService.QueryCharacterInformation(ID);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    return  GetCharInfo(ID);
                }
                else
                {
                    return new List<VNCharacterInformation>();
                }
            }
            else
            {
                return JsonConvert.DeserializeObject<VNCharacterInformationRoot>(result.Payload).items;
            }
        }

        private List<VNCharacterInformation> GetCharInfo(List<int> IDs)
        {
            var Chars = new List<VNCharacterInformation>();

            VNCharacterInformationRoot convertedResult;
            int page = 1;

            do
            {
                convertedResult = JsonConvert.DeserializeObject<VNCharacterInformationRoot>(QueryChar(_VNDBService.QueryCharacterInformation, page,  IDs.ToArray()).Payload);

                page++;

                Chars.AddRange(convertedResult.items);

            } while (convertedResult.more);

            return Chars;
        }

        private VndbResponse QueryChar(Func<int[], int, VndbResponse> Query, int page, int[] IDs)
        {
            VndbResponse result = Query(IDs, page);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    return QueryChar(Query, page, IDs);
                }
                {
                    return null;
                }                    
            }
            else
            {
                return result;
            }                
        }

        public IList<VN> GetVNList()
        {
            return GetList<VN, VNListRoot>(_VNDBService.QueryVNList);
        }

        public IList<Vote> GetVoteList()
        {
            return GetList<Vote, VoteListRoot>(_VNDBService.QueryVoteList);
        }

        private List<T> GetList<T, U>(Func<int, VndbResponse> Query) where U : GetTemplate<T>
        {
            var List = new List<T>();

            U convertedResult;
            int page = 1;

            do
            {
                convertedResult = JsonConvert.DeserializeObject<U>(QueryList(Query, page));
                page++;

                List.AddRange(convertedResult.items);

            } while (convertedResult.more);

            return List;
        }

        private string QueryList(Func<int, VndbResponse> Query, int page)
        {
            VndbResponse result = Query(page);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    return QueryList(Query, page);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return result.Payload;
            }
        }
    }
}
