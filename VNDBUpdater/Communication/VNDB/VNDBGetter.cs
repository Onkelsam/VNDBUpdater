using CommunicationLib.VNDB.Connection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VNDBUpdater.Communication.VNDB.Entities;
using VNDBUpdater.Communication.VNDB.Interfaces;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.Services.Tags;
using VNDBUpdater.Services.Traits;

namespace VNDBUpdater.Communication.VNDB
{
    public class VNDBGetter : IVNDBGetter
    {
        private IVNDB _VNDBService;
        private ITagService _TagService;
        private ITraitService _TraitService;

        public VNDBGetter(IVNDB VNDBService, ITagService TagService, ITraitService TraitService)
        {
            _VNDBService = VNDBService;
            _TagService = TagService;
            _TraitService = TraitService;
        }

        public async Task<VisualNovelModel> Get(int ID)
        {
            var visualNovel = new VisualNovelModel();

            var basic = await GetBasicInformation(ID);
            var character = await GetCharInfo(ID);

            visualNovel.Basics = new BasicInformationModel(basic.items[0], _TagService);
            visualNovel.Characters = character.Select(x => new CharacterInformationModel(x, _TraitService)).ToList();

            return visualNovel;
        }

        public async Task<IList<VisualNovelModel>> Get(string title)
        {
            List<VNInformation> result = await GetBasicInformation(title);
            var models = new List<VisualNovelModel>();

            foreach (var vn in result)
            {
                var newVN = new VisualNovelModel
                {
                    Basics = new BasicInformationModel(vn, _TagService)
                };

                models.Add(newVN);
            }

            return models;
        }

        public async Task<IList<VisualNovelModel>> Get(List<int> IDs)
        {
            var vns = new List<VisualNovelModel>();

            var basicinfo = await GetBasicInformation(IDs);

            List<VNInformation> basicInformation = basicinfo.items;
            List<VNCharacterInformation> charInformation = await GetCharInfo(IDs);

            foreach (var id in IDs)
            {
                VNInformation basic = basicInformation.SingleOrDefault(x => x.id == id);
                List<VNCharacterInformation> chars = charInformation.FindAll(x => x.vns.Any(y => y.Any(u => u.ToString() == id.ToString()))).ToList();

                // Check if visual novel still exists.
                if (basic != null)
                {
                    var newVN = new VisualNovelModel
                    {

                        Basics = new BasicInformationModel(basic, _TagService),

                        Characters = chars.Select(x => new CharacterInformationModel(x, _TraitService)).ToList()
                    };

                    vns.Add(newVN);
                }
            }

            return vns;
        }

        private async Task<List<VNInformation>> GetBasicInformation(string title)
        {
            var infos = new List<VNInformation>();

            VNInformationRoot convertedResult;
            int page = 1;

            do
            {
                convertedResult = await QueryBasicInformation(title, page);

                page++;

                infos.AddRange(convertedResult.items);

            } while (convertedResult.more);

            return infos;
        }

        private async Task<VNInformationRoot> QueryBasicInformation(string title, int page)
        {
            VndbResponse result = await _VNDBService.SearchByTitle(title, page);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    return await QueryBasicInformation(title, page);
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

        private async Task<VNInformationRoot> GetBasicInformation(int ID)
        {
            VndbResponse result = await _VNDBService.QueryInformation(ID);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    return await GetBasicInformation(ID);
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

        private async Task<VNInformationRoot> GetBasicInformation(List<int> IDs)
        {
            VndbResponse result = await _VNDBService.QueryInformation(IDs.ToArray());

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    return await GetBasicInformation(IDs);
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

        private async Task<List<VNCharacterInformation>> GetCharInfo(int ID)
        {
            VndbResponse result = await _VNDBService.QueryCharacterInformation(ID);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    return await GetCharInfo(ID);
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

        private async Task<List<VNCharacterInformation>> GetCharInfo(List<int> IDs)
        {
            var Chars = new List<VNCharacterInformation>();

            VNCharacterInformationRoot convertedResult;
            int page = 1;

            do
            {
                var result = await QueryChar(_VNDBService.QueryCharacterInformation, page, IDs.ToArray());

                convertedResult = JsonConvert.DeserializeObject<VNCharacterInformationRoot>(result.Payload);

                page++;

                Chars.AddRange(convertedResult.items);

            } while (convertedResult.more);

            return Chars;
        }

        private async Task<VndbResponse> QueryChar(Func<int[], int, Task<VndbResponse>> Query, int page, int[] IDs)
        {
            VndbResponse result = await Query(IDs, page);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    return await QueryChar(Query, page, IDs);
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

        public async Task<IList<VN>> GetVNList()
        {
            return await GetList<VN, VNListRoot>(_VNDBService.QueryVNList);
        }

        public async Task<IList<Vote>> GetVoteList()
        {
            return await GetList<Vote, VoteListRoot>(_VNDBService.QueryVoteList);
        }

        private async Task<List<T>> GetList<T, U>(Func<int, Task<VndbResponse>> Query) where U : GetTemplate<T>
        {
            var List = new List<T>();

            U convertedResult;
            int page = 1;

            do
            {
                convertedResult = JsonConvert.DeserializeObject<U>(await QueryList(Query, page));
                page++;

                List.AddRange(convertedResult.items);

            } while (convertedResult.more);

            return List;
        }

        private async Task<string> QueryList(Func<int, Task<VndbResponse>> Query, int page)
        {
            VndbResponse result = await Query(page);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    return await QueryList(Query, page);
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
