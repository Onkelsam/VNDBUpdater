using VNDBUpdater.Communication.VNDB;

namespace VNUpdater.Helper
{
    public class VNIDsSplitter
    {
        public int NumberOfRequest { get; private set; }
        public int Remainder { get; private set; }
        public bool SplittingNeccessary { get; private set; }
        public int[] IDs { get; private set; }

        public VNIDsSplitter(int[] ids)
        {
            IDs = ids;
        }

        public void Split()
        {
            if (IDs.Length > VNDBCommunication.MAXVNSPERREQUEST)
            {
                SplittingNeccessary = true;
                NumberOfRequest = (IDs.Length / VNDBCommunication.MAXVNSPERREQUEST);
                Remainder = IDs.Length - (NumberOfRequest * VNDBCommunication.MAXVNSPERREQUEST);
            }
            else
                SplittingNeccessary = false;
        }
    }
}
