using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Лабораторная_работа_6._1.Model
{
    public class ArraySorter
    {
        // Общий счётчик сравнений (разделяемый ресурс)
        private long _totalComparisons;
        private readonly object _locker = new object();
        // Делегаты и события для уведомления о завершении сортировки
        public delegate void SortCompletedHandler(int[] sortedArray, long comparisons, double elapsedMilliseconds);
        public event SortCompletedHandler BubbleSortCompleted;
        public event SortCompletedHandler QuickSortCompleted;
        public event SortCompletedHandler InsertionSortCompleted;
        public event SortCompletedHandler MergeSortCompleted;
        // Свойство для доступа к общему счётчику
        public long TotalComparisons => _totalComparisons;

        // Результат сортировки
        public class SortResult
        {
            public int[] SortedArray { get; set; }
            public long Comparisons { get; set; }
            public double ElapsedMilliseconds { get; set; }
            public bool IsCancelled { get; set; }
        }
        // Генерация случайного массива заданного размера
        public int[] GenerateRandomArray(int size)
        {
            Random rand = new Random();
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
                array[i] = rand.Next(1000); // числа от 0 до 999
            return array;
        }
        // Копирование массива (чтобы каждый поток работал со своей копией)
        private int[] CopyArray(int[] source)
        {
            int[] copy = new int[source.Length];
            Array.Copy(source, copy, source.Length);
            return copy;
        }
        // Метод для пузырьковой сортировки (запускается в потоке)
        public void BubbleSort(int[] originalArray)
        {
            int[] array = CopyArray(originalArray);
            long comparisons = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = 0; j < array.Length - 1 - i; j++)
                {
                    comparisons++;
                    if (array[j] > array[j + 1])
                    {
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }
            watch.Stop();
            lock (_locker)
            {
                _totalComparisons += comparisons;
            }
            BubbleSortCompleted?.Invoke(array, comparisons, watch.Elapsed.TotalMilliseconds);
        }
        // Метод для быстрой сортировки (обёртка)
        public void QuickSort(int[] originalArray)
        {
            int[] array = CopyArray(originalArray);
            long comparisons = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            QuickSortRecursive(array, 0, array.Length - 1, ref comparisons);
            watch.Stop();
            lock (_locker)
            {
                _totalComparisons += comparisons;
            }
            QuickSortCompleted?.Invoke(array, comparisons, watch.Elapsed.TotalMilliseconds);
        }
        private void QuickSortRecursive(int[] arr, int left, int right, ref long comparisons)
        {
            if (left < right)
            {
                int pivotIndex = Partition(arr, left, right, ref comparisons);
                QuickSortRecursive(arr, left, pivotIndex - 1, ref comparisons);
                QuickSortRecursive(arr, pivotIndex + 1, right, ref comparisons);
            }
        }
        private int Partition(int[] arr, int left, int right, ref long comparisons)
        {
            int pivot = arr[right];
            int i = left - 1;
            for (int j = left; j < right; j++)
            {
                comparisons++;
                if (arr[j] < pivot)
                {
                    i++;
                    int temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
            }
            int temp1 = arr[i + 1];
            arr[i + 1] = arr[right];
            arr[right] = temp1;
            return i + 1;
        }
        // Метод для сортировки вставками
        public void InsertionSort(int[] originalArray)
        {
            int[] array = CopyArray(originalArray);
            long comparisons = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 1; i < array.Length; i++)
            {
                int key = array[i];
                int j = i - 1;
                while (j >= 0 && array[j] > key)
                {
                    comparisons++;
                    array[j + 1] = array[j];
                    j--;
                }
                comparisons++; // учёт последнего сравнения, когда условие не выполнено
                array[j + 1] = key;
            }
            watch.Stop();
            lock (_locker)
            {
                _totalComparisons += comparisons;
            }
            InsertionSortCompleted?.Invoke(array, comparisons, watch.Elapsed.TotalMilliseconds);
        }

        // Асинхронные обёртки для сортировок с поддержкой отмены и прогресса
        public async Task<SortResult> BubbleSortAsync(int[] array, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                int[] arr = CopyArray(array);
                long comparisons = 0;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                int totalIterations = arr.Length * (arr.Length - 1) / 2;
                int currentIteration = 0;
                int reportInterval = Math.Max(1, totalIterations / 100); // Обновлять каждые ~1%

                for (int i = 0; i < arr.Length - 1; i++)
                {
                    for (int j = 0; j < arr.Length - 1 - i; j++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        comparisons++;
                        if (arr[j] > arr[j + 1])
                        {
                            int temp = arr[j];
                            arr[j] = arr[j + 1];
                            arr[j + 1] = temp;
                        }
                        currentIteration++;
                        if (currentIteration % reportInterval == 0)
                        {
                            progress?.Report(currentIteration * 100.0 / totalIterations);
                        }
                    }
                }
                watch.Stop();
                lock (_locker)
                {
                    _totalComparisons += comparisons;
                }
                progress?.Report(100);
                return new SortResult
                {
                    SortedArray = arr,
                    Comparisons = comparisons,
                    ElapsedMilliseconds = watch.Elapsed.TotalMilliseconds,
                    IsCancelled = false
                };
            }, cancellationToken);
        }

        public async Task<SortResult> QuickSortAsync(int[] array, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                int[] arr = CopyArray(array);
                long comparisons = 0;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                int totalElements = arr.Length;
                int processedElements = 0;
                
                QuickSortRecursive(arr, 0, arr.Length - 1, ref comparisons, progress, cancellationToken, totalElements, ref processedElements);
                watch.Stop();
                lock (_locker)
                {
                    _totalComparisons += comparisons;
                }
                progress?.Report(100);
                return new SortResult
                {
                    SortedArray = arr,
                    Comparisons = comparisons,
                    ElapsedMilliseconds = watch.Elapsed.TotalMilliseconds,
                    IsCancelled = false
                };
            }, cancellationToken);
        }

        private void QuickSortRecursive(int[] arr, int left, int right, ref long comparisons, IProgress<double> progress, CancellationToken cancellationToken, int totalElements, ref int processedElements)
        {
            if (left < right)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int pivotIndex = Partition(arr, left, right, ref comparisons, cancellationToken);
                processedElements += (pivotIndex - left);
                int currentProgress = Math.Min(100, (int)(processedElements * 100.0 / totalElements));
                progress?.Report(currentProgress);
                
                QuickSortRecursive(arr, left, pivotIndex - 1, ref comparisons, progress, cancellationToken, totalElements, ref processedElements);
                QuickSortRecursive(arr, pivotIndex + 1, right, ref comparisons, progress, cancellationToken, totalElements, ref processedElements);
            }
        }

        private int Partition(int[] arr, int left, int right, ref long comparisons, CancellationToken cancellationToken)
        {
            int pivot = arr[right];
            int i = left - 1;
            for (int j = left; j < right; j++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                comparisons++;
                if (arr[j] < pivot)
                {
                    i++;
                    int temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
            }
            int temp1 = arr[i + 1];
            arr[i + 1] = arr[right];
            arr[right] = temp1;
            return i + 1;
        }

        public async Task<SortResult> InsertionSortAsync(int[] array, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                int[] arr = CopyArray(array);
                long comparisons = 0;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                int reportInterval = Math.Max(1, arr.Length / 100); // Обновлять каждые ~1%

                for (int i = 1; i < arr.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    int key = arr[i];
                    int j = i - 1;
                    while (j >= 0 && arr[j] > key)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        comparisons++;
                        arr[j + 1] = arr[j];
                        j--;
                    }
                    comparisons++;
                    arr[j + 1] = key;
                    
                    if (i % reportInterval == 0)
                    {
                        progress?.Report(i * 100.0 / arr.Length);
                    }
                }
                watch.Stop();
                lock (_locker)
                {
                    _totalComparisons += comparisons;
                }
                progress?.Report(100);
                return new SortResult
                {
                    SortedArray = arr,
                    Comparisons = comparisons,
                    ElapsedMilliseconds = watch.Elapsed.TotalMilliseconds,
                    IsCancelled = false
                };
            }, cancellationToken);
        }

        public async Task<SortResult> MergeSortAsync(int[] array, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                int[] arr = CopyArray(array);
                long comparisons = 0;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                int mergedElements = 0;
                
                MergeSortRecursive(arr, 0, arr.Length - 1, ref comparisons, progress, cancellationToken, arr.Length, ref mergedElements);
                watch.Stop();
                lock (_locker)
                {
                    _totalComparisons += comparisons;
                }
                progress?.Report(100);
                return new SortResult
                {
                    SortedArray = arr,
                    Comparisons = comparisons,
                    ElapsedMilliseconds = watch.Elapsed.TotalMilliseconds,
                    IsCancelled = false
                };
            }, cancellationToken);
        }

        private void MergeSortRecursive(int[] arr, int left, int right, ref long comparisons, IProgress<double> progress, CancellationToken cancellationToken, int totalElements, ref int mergedElements)
        {
            if (left < right)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int mid = (left + right) / 2;
                MergeSortRecursive(arr, left, mid, ref comparisons, progress, cancellationToken, totalElements, ref mergedElements);
                MergeSortRecursive(arr, mid + 1, right, ref comparisons, progress, cancellationToken, totalElements, ref mergedElements);
                Merge(arr, left, mid, right, ref comparisons, cancellationToken);
                mergedElements += (right - left + 1);
                int currentProgress = Math.Min(100, (int)(mergedElements * 100.0 / totalElements));
                progress?.Report(currentProgress);
            }
        }

        private void Merge(int[] arr, int left, int mid, int right, ref long comparisons, CancellationToken cancellationToken)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;
            int[] leftArr = new int[n1];
            int[] rightArr = new int[n2];

            for (int i = 0; i < n1; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                leftArr[i] = arr[left + i];
            }
            for (int j = 0; j < n2; j++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                rightArr[j] = arr[mid + 1 + j];
            }

            int iIdx = 0, jIdx = 0, kIdx = left;
            while (iIdx < n1 && jIdx < n2)
            {
                cancellationToken.ThrowIfCancellationRequested();
                comparisons++;
                if (leftArr[iIdx] <= rightArr[jIdx])
                {
                    arr[kIdx] = leftArr[iIdx];
                    iIdx++;
                }
                else
                {
                    arr[kIdx] = rightArr[jIdx];
                    jIdx++;
                }
                kIdx++;
            }

            while (iIdx < n1)
            {
                cancellationToken.ThrowIfCancellationRequested();
                arr[kIdx] = leftArr[iIdx];
                iIdx++;
                kIdx++;
            }

            while (jIdx < n2)
            {
                cancellationToken.ThrowIfCancellationRequested();
                arr[kIdx] = rightArr[jIdx];
                jIdx++;
                kIdx++;
            }
        }
    }
}
