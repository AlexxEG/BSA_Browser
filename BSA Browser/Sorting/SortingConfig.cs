using System;
using BSA_Browser.Enums;

namespace BSA_Browser.Sorting
{
    public struct SortingConfig : IEquatable<SortingConfig>
    {
        public bool Descending { get; set; }
        public ArchiveFileSortOrder Order { get; set; }

        public SortingConfig(bool descending, ArchiveFileSortOrder order)
        {
            Descending = descending;
            Order = order;
        }

        public bool Equals(SortingConfig other)
        {
            return Descending == other.Descending && Order == other.Order;
        }

        public override bool Equals(object obj)
        {
            return obj is SortingConfig && this.Equals((SortingConfig)obj);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + Descending.GetHashCode();
            hash = hash * 31 + Order.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"{typeof(SortingConfig)}: {nameof(Descending)}: {Descending} - {nameof(Order)}: {Order}";
        }

        public static bool operator ==(SortingConfig sc1, SortingConfig sc2)
        {
            return sc1.Equals(sc2);
        }

        public static bool operator !=(SortingConfig sc1, SortingConfig sc2)
        {
            return !sc1.Equals(sc2);
        }
    }
}
