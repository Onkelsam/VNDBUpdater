using CommunicationLib.VNDB;
using CommunicationLib.VNDB.Connection;
using System;
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

        public void AddToScoreList(VisualNovelModel model)
        {
            SetList<SetJSONObjects.Vote>(model, new SetJSONObjects.Vote { vote = model.Score }, _VNDBService.SetVote);
        }

        public void AddToVNList(VisualNovelModel model)
        {
            SetList<SetJSONObjects.State>(model, new SetJSONObjects.State() { status = (int)model.Category }, _VNDBService.SetVNList);
        }

        public void RemoveFromScoreList(VisualNovelModel model)
        {
            RemoveFromList(model, _VNDBService.DeleteVote);
        }

        public void RemoveFromVNList(VisualNovelModel model)
        {
            RemoveFromList(model, _VNDBService.DeleteVNFromVNList);
        }

        private  void SetList<T>(VisualNovelModel VN, T setObject, Func<int, T, VndbResponse> SetQuery)
        {
            VndbResponse result = SetQuery(VN.Basics.ID, setObject);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    SetList<T>(VN, setObject, SetQuery);
                }
            }
        }

        private  void RemoveFromList(VisualNovelModel VN, Func<int, VndbResponse> Query)
        {
            VndbResponse result = Query(VN.Basics.ID);

            if (result.ResponseType == VndbResponseType.Error)
            {
                if (_VNDBService.HandleError(result) == ErrorResponse.Throttled)
                {
                    RemoveFromList(VN, Query);
                }                    
            }
        }
    }
}
