using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Utility
{
    public class SyncList<T, T1> : List<T1> where T1 : T where T : class
    {
        private List<T> ToSyncList;

        public SyncList(List<T> toSyncList)
        {
            ToSyncList = toSyncList;
        }

        public new void Add(T1 item)
        {
            if (!ToSyncList.Contains(item)) ToSyncList.Add(item);
            base.Add(item);
        }

        public new void AddRange(IEnumerable<T1> collection)
        {
            var enumerable = collection as T1[] ?? collection.ToArray();

            foreach (var item in enumerable)
            {
                if (!ToSyncList.Contains(item))
                    ToSyncList.Add(item);
            }

            base.AddRange(enumerable);
        }

        public new bool Remove([CanBeNull] T1 item)
        {
            return ToSyncList.Remove(item) && base.Remove(item);
        }
    }
}