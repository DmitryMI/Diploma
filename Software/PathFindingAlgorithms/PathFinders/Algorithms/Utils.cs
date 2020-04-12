using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Algorithms
{
    public static class Utils
    {
        public static ICollection<T> GetItemsBytIndexes<T>(T[] array, params int[] indexes)
        {
            List<T> result = new List<T>(indexes.Length);

            foreach (var index in indexes)
            {
                result.Add(array[index]);
            }

            return result;
        }

        public static T[] GetItemsByFilters<T>(ICollection<T> collection, params Func<T, bool>[] filters)
        {
            T[] result = new T[filters.Length];
            bool[] filterPassed = new bool[filters.Length];
            foreach (var item in collection)
            {
                for (int i = 0; i < filters.Length; i++)
                {
                    if (filterPassed[i])
                    {
                        continue;
                    }
                    if (filters[i](item))
                    {
                        result[i] = item;
                        filterPassed[i] = true;
                    }
                }
            }

            return result;
        }

        public static int[] GetIndexesByFilters<T>(ICollection<T> collection, params Func<T, bool>[] filters)
        {
            int[] result = new int[filters.Length];
            for (int i = 0; i < filters.Length; i++)
            {
                result[i] = -1;
            }
            bool[] filterPassed = new bool[filters.Length];
            int index = 0;
            foreach (var item in collection)
            {
                for (int i = 0; i < filters.Length; i++)
                {
                    if (filterPassed[i])
                    {
                        continue;
                    }
                    if (filters[i](item))
                    {
                        result[i] = index;
                        filterPassed[i] = true;
                    }
                }

                index++;
            }

            return result;
        }

        public static void InsertSortedDescending<T>(IList<T> list, T value, IComparer<T> comparer)
        {
            if (list.Count == 0)
            {
                list.Add(value);
                return;
            }

            int index = list.Count - 1;
            while (index >= 0)
            {
                int comparison = comparer.Compare(value, list[index]);

                if (comparison > 0)
                {
                    index--;
                }
                else
                {
                    
                    break;
                }
            }
            index++;
            list.Insert(index, value);
        }

        public static List<T> GetInvertedList<T>(IList<T> list)
        {
            if (list == null)
                return null;
            List<T> result = new List<T>(list.Count);

            for (int i = list.Count - 1; i >= 0; i--)
            {
                result.Add(list[i]);
            }

            return result;
        }
       
    }
}
