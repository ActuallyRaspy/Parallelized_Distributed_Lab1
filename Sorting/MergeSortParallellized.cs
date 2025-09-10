using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sorting
{
    public class MergeSortParallellized<T> : SortBase<T>
    {
        public override string Name { get { return "MergeSortParallellized"; } }
        public int elementThreshold = 1000; //Sets a limit for when parallell or sequential should be used. Prevents creating a new thread for each and every element.

        public override void Sort(T[] inputOutput, IComparer<T> comparer)
        {
            var temp = new T[inputOutput.Length]; // one-time allocation instead of creating and copying a new array for every recursive loop
            ParallelMergeSort(inputOutput, 0, inputOutput.Length, temp, comparer);
        }

        protected void ParallelMergeSort(T[] array, int start, int end, T[] temp, IComparer<T> comparer) //Use index based instead to greatly reduce overhead
        {
            int length = end - start;
            if (length <= 1) return; // if its 1, its sorted and done

            int mid = start + length / 2;

            if (length <= elementThreshold)
            {
                SequentialMergeSort(array, start, end, temp, comparer);
            }
            else
            {
                Parallel.Invoke(
                    () => ParallelMergeSort(array, start, mid, temp, comparer),
                    () => ParallelMergeSort(array, mid, end, temp, comparer)
                );
            }
            Merge(array, start, mid, end, temp, comparer);
        }

        protected void SequentialMergeSort(T[] array, int start, int end, T[] temp, IComparer<T> comparer) 
        {
            int length = end - start;
            if (length <= 1) return; // if its 1, its sorted and done

            int mid = start + length / 2;

            // Recursively sort left half
            SequentialMergeSort(array, start, mid, temp, comparer);

            // Recursively sort right half
            SequentialMergeSort(array, mid, end, temp, comparer);

            // Merge sorted halves
            Merge(array, start, mid, end, temp, comparer);
        }

        protected void Merge(T[] array, int start, int mid, int end, T[] temp, IComparer<T> comparer)
        {
            int i = start, j = mid, k = start;

            while (i < mid && j < end)
            {
                if (comparer.Compare(array[i], array[j]) <= 0)
                    temp[k++] = array[i++];
                else
                    temp[k++] = array[j++];
            }

            while (i < mid) temp[k++] = array[i++];
            while (j < end) temp[k++] = array[j++];

            // Copy merged back into original array
            for (int t = start; t < end; t++)
                array[t] = temp[t];
        }
    }
}
