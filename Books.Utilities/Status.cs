

namespace Books.Utilities
{
    public static class Status
    {
        public enum StatusType
        {
            Pending,
            Approved,
            InProgress,
            Shipped,
            Canceled,
            Refunded
        }

        public enum Payment
        {
            Pending,
            Approved,
            Delayed,
            Rejected
        }
    }
}
