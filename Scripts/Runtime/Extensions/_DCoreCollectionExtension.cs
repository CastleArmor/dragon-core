using System;
using System.Collections.Generic;

namespace Dragon.Core
{
    public static class _DCoreCollectionExtension
    {
        public static T GetRandom<T>(this List<T> list,T defaultValue = default)
        {
            if (list.Count == 0) return defaultValue;
            int selected = UnityEngine.Random.Range(0, list.Count);
            return list[selected];
        }
        
        public static void QuickSort<T>(this List<T> list,Func<T,T,bool> comparison)
        {
            QuickSortInternal(list, 0, list.Count - 1,comparison);
        }

        private static void QuickSortInternal<T>(List<T> list, int left, int right,Func<T,T,bool> comparison)
        {
            if(left >= right)
            {
                return;
            }

            int partition = PartitionInternal(list, left, right,comparison);

            QuickSortInternal(list, left, partition - 1,comparison);
            QuickSortInternal(list, partition + 1, right,comparison);
        }

        private static int PartitionInternal<T>(List<T> list, int left, int right,Func<T,T,bool> comparison)
        {
            T partition = list[right];

            // stack items smaller than partition from left to right
            int swapIndex = left;
            for (int i = left; i < right; i++)
            {
                T item = list[i];
                if(comparison.Invoke(item,partition))
                    //if(item.CompareTo(partition) <= 0)
                {
                    list[i] = list[swapIndex];
                    list[swapIndex] = item;

                    swapIndex++;
                }
            }

            // put the partition after all the smaller items
            list[right] = list[swapIndex];
            list[swapIndex] = partition;

            return right;
        }
        
        public static T Latest<T>(this List<T> list)
        {
            return list[^1];
        }
    }
}