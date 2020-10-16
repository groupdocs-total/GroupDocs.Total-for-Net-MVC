using System.Threading;

namespace GroupDocs.Total.MVC.Products.Search.Service
{
    internal static class UniqueIdProvider
    {
        private static long _id;

        public static long GetId()
        {
            return Interlocked.Increment(ref _id);
        }
    }
}
