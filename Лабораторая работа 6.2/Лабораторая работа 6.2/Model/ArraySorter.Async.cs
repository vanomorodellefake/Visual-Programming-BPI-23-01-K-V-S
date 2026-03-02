using System;
using System.Threading;
using System.Threading.Tasks;

namespace Лабораторая_работа_6._2.Model
{
    public partial class ArraySorter
    {
        // Результат сортировки
        public class SortResult
        {
            public int[] SortedArray { get; set; }
            public long Comparisons { get; set; }
            public double ElapsedMilliseconds { get; set; }
        }

        // Async версия пузырьковой сортировки - возвращает результат напрямую
        public async Task<SortResult> BubbleSortAsync(int[] originalArray, IProgress<int> progress = null, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                int[] array = CopyArray(originalArray);
                long comparisons = 0;
                var watch = System.Diagnostics.Stopwatch.StartNew();

                int totalIterations = array.Length * (array.Length - 1) / 2;
                int currentIteration = 0;
                int updateIntervalIterations = Math.Max(1, totalIterations / 100);

                // Сообщаем о начале (0%)
                progress?.Report(0);

                for (int i = 0; i < array.Length - 1; i++)
                {
                    for (int j = 0; j < array.Length - 1 - i; j++)
                    {
                        token.ThrowIfCancellationRequested();

                        comparisons++;
                        if (array[j] > array[j + 1])
                        {
                            int temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                        }
                        currentIteration++;
                    }

                    if (currentIteration % updateIntervalIterations == 0)
                    {
                        int percent = currentIteration * 100 / totalIterations;
                        progress?.Report(percent);
                    }
                }

                watch.Stop();
                lock (_locker)
                {
                    _totalComparisons += comparisons;
                }

                // Сообщаем о завершении (100%)
                progress?.Report(100);

                return new SortResult
                {
                    SortedArray = array,
                    Comparisons = comparisons,
                    ElapsedMilliseconds = watch.Elapsed.TotalMilliseconds
                };
            }, token);
        }

        // Async версия быстрой сортировки
        public async Task<SortResult> QuickSortAsync(int[] originalArray, IProgress<int> progress = null, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                int[] array = CopyArray(originalArray);
                long comparisons = 0;
                var watch = System.Diagnostics.Stopwatch.StartNew();

                int totalElements = array.Length;
                int processedElements = 0;
                int lastReportedPercent = -1;

                // Сообщаем о начале (0%)
                progress?.Report(0);

                QuickSortRecursiveAsync(array, 0, array.Length - 1, ref comparisons, totalElements, ref processedElements, ref lastReportedPercent, token);

                watch.Stop();
                lock (_locker)
                {
                    _totalComparisons += comparisons;
                }

                // Сообщаем о завершении (100%)
                progress?.Report(100);

                return new SortResult
                {
                    SortedArray = array,
                    Comparisons = comparisons,
                    ElapsedMilliseconds = watch.Elapsed.TotalMilliseconds
                };
            }, token);
        }

        private void QuickSortRecursiveAsync(int[] arr, int left, int right, ref long comparisons, int totalElements, ref int processedElements, ref int lastReportedPercent,
            CancellationToken token = default)
        {
            if (left < right)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                int pivotIndex = Partition(arr, left, right, ref comparisons);

                QuickSortRecursiveAsync(arr, left, pivotIndex - 1, ref comparisons, totalElements, ref processedElements, ref lastReportedPercent, token);
                QuickSortRecursiveAsync(arr, pivotIndex + 1, right, ref comparisons, totalElements, ref processedElements, ref lastReportedPercent, token);

                processedElements += (right - left + 1);
                int percent = Math.Min(processedElements * 100 / totalElements, 100);

                if (percent != lastReportedPercent && percent > lastReportedPercent)
                {
                    lastReportedPercent = percent;
                }
            }
        }

        // Async версия сортировки вставками
        public async Task<SortResult> InsertionSortAsync(int[] originalArray, IProgress<int> progress = null, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                int[] array = CopyArray(originalArray);
                long comparisons = 0;
                var watch = System.Diagnostics.Stopwatch.StartNew();

                int totalIterations = array.Length;
                int updateIntervalIterations = Math.Max(1, totalIterations / 100);

                // Сообщаем о начале (0%)
                progress?.Report(0);

                for (int i = 1; i < array.Length; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    int key = array[i];
                    int j = i - 1;
                    while (j >= 0 && array[j] > key)
                    {
                        comparisons++;
                        array[j + 1] = array[j];
                        j--;
                    }
                    comparisons++;
                    array[j + 1] = key;

                    if (i % updateIntervalIterations == 0)
                    {
                        int percent = i * 100 / totalIterations;
                        progress?.Report(percent);
                    }
                }

                watch.Stop();
                lock (_locker)
                {
                    _totalComparisons += comparisons;
                }

                // Сообщаем о завершении (100%)
                progress?.Report(100);

                return new SortResult
                {
                    SortedArray = array,
                    Comparisons = comparisons,
                    ElapsedMilliseconds = watch.Elapsed.TotalMilliseconds
                };
            }, token);
        }

        // Async версия сортировки слиянием
        public async Task<SortResult> MergeSortAsync(int[] originalArray, IProgress<int> progress = null, CancellationToken token = default)
        {
            return await Task.Run(() =>
            {
                int[] array = CopyArray(originalArray);
                long comparisons = 0;
                var watch = System.Diagnostics.Stopwatch.StartNew();

                int totalElements = array.Length;
                int processedElements = 0;
                int lastReportedPercent = -1;

                // Сообщаем о начале (0%)
                progress?.Report(0);

                MergeSortRecursiveAsync(array, 0, array.Length - 1, ref comparisons, totalElements, ref processedElements, ref lastReportedPercent, token);

                watch.Stop();
                lock (_locker)
                {
                    _totalComparisons += comparisons;
                }

                // Сообщаем о завершении (100%)
                progress?.Report(100);

                return new SortResult
                {
                    SortedArray = array,
                    Comparisons = comparisons,
                    ElapsedMilliseconds = watch.Elapsed.TotalMilliseconds
                };
            }, token);
        }

        private void MergeSortRecursiveAsync(int[] arr, int left, int right, ref long comparisons, int totalElements, ref int processedElements, ref int lastReportedPercent, CancellationToken token = default)
        {
            if (left < right)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                int mid = left + (right - left) / 2;
                MergeSortRecursiveAsync(arr, left, mid, ref comparisons, totalElements, ref processedElements, ref lastReportedPercent, token);
                MergeSortRecursiveAsync(arr, mid + 1, right, ref comparisons, totalElements, ref processedElements, ref lastReportedPercent, token);
                Merge(arr, left, mid, right, ref comparisons);

                processedElements += (right - left + 1);
                int percent = processedElements * 100 / totalElements;

                if (percent != lastReportedPercent && percent > lastReportedPercent)
                {
                    lastReportedPercent = percent;
                }
            }
        }
    }
}
