using CommunicationLib.VNDB;
using CommunicationLib.VNDB.Connection;
using System;
using System.Threading.Tasks;
using VNDBUpdater.Communication.VNDB.Interfaces;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.VNDB
{
    public class VNDBSetter : IVNDBSetter
    {
        private IVNDB _VNDBService;

        public VNDBSetter(IVNDB VNDBService)
        {
            _VNDBService = VNDBService;
        }

        public async Task AddToScoreListAsync(VisualNovelModel model)
        {
            await SetList<SetJSONObjects.Vote>(model, new SetJSONObjects.Vote { vote = model.Score }, _VNDBService.SetVoteAsync);
        }

        public async Task AddToVNListAsync(VisualNovelModel model)
        {
            await SetList<SetJSONObjects.State>(model, new SetJSONObjects.State() { status = (int)model.Category }, _VNDBService.SetVNListAsync);
        }

        public async Task RemoveFromScoreListAsync(VisualNovelModel model)
        {
            await RemoveFromList(model, _VNDBService.DeleteVoteAsync);
        }

        public async Task RemoveFromVNListAsync(VisualNovelModel model)
        {
            await RemoveFromList(model, _VNDBService.DeleteVNFromVNListAsync);
        }

        private async Task SetList<T>(VisualNovelModel VN, T setObject, Func<int, T, Task<VndbResponse>> SetQuery)
        {
            VndbResponse result = await SetQuery(VN.Basics.ID, setObject);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    await SetList<T>(VN, setObject, SetQuery);
                }
            }
        }

        private async Task RemoveFromList(VisualNovelModel VN, Func<int, Task<VndbResponse>> Query)
        {
            VndbResponse result = await Query(VN.Basics.ID);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    await RemoveFromList(VN, Query);
                }                    
            }
        }
    }
}
