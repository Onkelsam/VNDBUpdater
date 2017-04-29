using VNDBUpdater.Data;

namespace VNDBUpdater.BackgroundTasks
{
    public class VNIDsSplitter
    {
        public int MaxVNsPerRequest { get; } = 25;
        public int NumberOfRequests { get; private set; }
        public int Remainder { get; private set; }
        public bool SplittingNeccessary { get; private set; }
        public int[] IDs { get; private set; }

        public VNIDsSplitter(int[] ids)
        {
            IDs = ids;
        }

        public void Split()
        {
            if (IDs.Length > MaxVNsPerRequest)
            {
                SplittingNeccessary = true;
                NumberOfRequests = (IDs.Length / MaxVNsPerRequest);
                Remainder = IDs.Length - (NumberOfRequests * MaxVNsPerRequest);
            }
            else
            {
                SplittingNeccessary = false;
            }                
        }
    }
}
