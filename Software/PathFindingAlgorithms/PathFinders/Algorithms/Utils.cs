using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Algorithms
{
    static class Utils
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
       
    }
}
