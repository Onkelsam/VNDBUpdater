using VNDBUpdater.Communication.VNDB;
using VNUpdater.Data;

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
            if (IDs.Length > Constants.MaxVNsPerRequest)
            {
                SplittingNeccessary = true;
                NumberOfRequest = (IDs.Length / Constants.MaxVNsPerRequest);
                Remainder = IDs.Length - (NumberOfRequest * Constants.MaxVNsPerRequest);
            }
            else
                SplittingNeccessary = false;
        }
    }
}
