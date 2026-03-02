using System;
using System.Threading;
using System.Threading.Tasks;

namespace Лабораторая_работа_6._2.Model
{
    public partial class ArraySorter
    {
        // Асинхронная пузырьковая сортировка
        public async Task BubbleSortAsync(int[] originalArray, CancellationToken token = default)
        {
            await Task.Run(() => BubbleSort(originalArray, token), token);
        }

        // Асинхронная быстрая сортировка
        public async Task QuickSortAsync(int[] originalArray, CancellationToken token = default)
        {
            await Task.Run(() => QuickSort(originalArray, token), token);
        }

        // Асинхронная сортировка вставками
        public async Task InsertionSortAsync(int[] originalArray, CancellationToken token = default)
        {
            await Task.Run(() => InsertionSort(originalArray, token), token);
        }

        // Асинхронная сортировка слиянием
        public async Task MergeSortAsync(int[] originalArray, CancellationToken token = default)
        {
            await Task.Run(() => MergeSort(originalArray, token), token);
        }
    }
}
