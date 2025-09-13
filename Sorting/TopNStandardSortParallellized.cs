using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    public class TopNStandardSortParallellized<T> : ITopNSort<T>
    {
        public string Name { get { return "TopNStandardSortParallellized - PLINQ"; } }

        public T[] TopNSort(T[] inputOutput, int n)
        {
            var topN = inputOutput
                .AsParallel()
                .Aggregate(() => new SortedSet<T>(), // make a per-partition seed of a sorted set

                    (local, item) => // local is the "holder" for the best n numbers. Its called a "partition-local accumulator". Item is a singular item inside it
                    {
                        local.Add(item);
                        if (local.Count > n) //Keep the "holder" the correct size n
                            local.Remove(local.Max); // drop the worst fitting number (use max for the ascending comparer)
                        return local;
                    },


                    (left, right) => // merge two partitions and then trim to n numbers
                    {
                        foreach (var item in right)
                        {
                            left.Add(item);
                            if (left.Count > n)
                                left.Remove(left.Max);
                        }
                        return left;
                    },

                    final => final.ToArray());

            return topN;
        }

        public T[] TopNSort(T[] inputOutput, int n, IComparer<T> comparer)
        {
            var topN = inputOutput
            .AsParallel()
            .Aggregate(() => new SortedSet<T>(comparer), // make a per-partition seed of a sorted set
            
                (local, item) => // local is the "holder" for the best n numbers. Its called a "partition-local accumulator". Item is a singular item inside it
                {
                    local.Add(item);
                    if (local.Count > n) //Keep the "holder" the correct size n
                        local.Remove(local.Max); // drop the worst fitting number (use max for the ascending comparer)
                    return local;
                },

                
                (left, right) => // merge two partitions and then trim to n numbers
                {
                    foreach (var item in right)
                    {
                        left.Add(item);
                        if (left.Count > n)
                            left.Remove(left.Max);
                    }
                    return left;
                },

                final => final.ToArray());

            return topN;
        }
    }
}
