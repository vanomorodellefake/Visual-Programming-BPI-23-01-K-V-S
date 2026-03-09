using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Лабораторая_работа_6._2.Model
{
    public partial class ArraySorter
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

        // Делегаты и события для прогресса сортировки
        public delegate void ProgressHandler(double percent);
        public event ProgressHandler BubbleSortProgress;
        public event ProgressHandler QuickSortProgress;
        public event ProgressHandler InsertionSortProgress;
        public event ProgressHandler MergeSortProgress;

        // Свойство для доступа к общему счётчику
        public long TotalComparisons => _totalComparisons;

        // Сброс счётчика сравнений
        public void ResetComparisons()
        {
            lock (_locker)
            {
                _totalComparisons = 0;
            }
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
        public void BubbleSort(int[] originalArray, CancellationToken token = default)
        {
            int[] array = CopyArray(originalArray);
            long comparisons = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            int totalIterations = array.Length * (array.Length - 1) / 2;
            int currentIteration = 0;
            int updateIntervalIterations = Math.Max(1, totalIterations / 100);

            // Сообщаем о начале (0%)
            BubbleSortProgress?.Invoke(0);
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
                    BubbleSortProgress?.Invoke(percent);
                }
            }
            watch.Stop();
            lock (_locker)
            {
                _totalComparisons += comparisons;
            }
            // Сообщаем о завершении (100%)
            BubbleSortProgress?.Invoke(100);
            BubbleSortCompleted?.Invoke(array, comparisons, watch.Elapsed.TotalMilliseconds);
        }
        // Метод для быстрой сортировки (обёртка)
        public void QuickSort(int[] originalArray, CancellationToken token = default)
        {
            int[] array = CopyArray(originalArray);
            long comparisons = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            int totalElements = array.Length;
            int processedElements = 0;
            int lastReportedPercent = -1;

            // Сообщаем о начале (0%)
            QuickSortProgress?.Invoke(0);

            QuickSortRecursive(array, 0, array.Length - 1, ref comparisons, totalElements, ref processedElements, ref lastReportedPercent, token);
            watch.Stop();
            lock (_locker)
            {
                _totalComparisons += comparisons;
            }
            // Сообщаем о завершении (100%)
            QuickSortProgress?.Invoke(100);
            QuickSortCompleted?.Invoke(array, comparisons, watch.Elapsed.TotalMilliseconds);
        }
        private void QuickSortRecursive(int[] arr, int left, int right, ref long comparisons, int totalElements, ref int processedElements, ref int lastReportedPercent,
            CancellationToken token = default)
        {
            if (left < right)
            {
                token.ThrowIfCancellationRequested();

                int pivotIndex = Partition(arr, left, right, ref comparisons);

                QuickSortRecursive(arr, left, pivotIndex - 1, ref comparisons, totalElements, ref processedElements, ref lastReportedPercent, token);
                QuickSortRecursive(arr, pivotIndex + 1, right, ref comparisons, totalElements, ref processedElements, ref lastReportedPercent, token);

                processedElements += (right - left + 1);
                int percent = Math.Min(processedElements * 100 / totalElements, 100);

                // Обновляем прогресс только если процент изменился
                if (percent != lastReportedPercent && percent > lastReportedPercent)
                {
                    QuickSortProgress?.Invoke(percent);
                    lastReportedPercent = percent;
                }
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
        public void InsertionSort(int[] originalArray, CancellationToken token = default)
        {
            int[] array = CopyArray(originalArray);
            long comparisons = 0;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            int totalIterations = array.Length;
            int updateIntervalIterations = Math.Max(1, totalIterations / 100);

            // Сообщаем о начале (0%)
            InsertionSortProgress?.Invoke(0);

            for (int i = 1; i < array.Length; i++)
            {
                token.ThrowIfCancellationRequested();

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

                if (i % updateIntervalIterations == 0)
                {
                    int percent = i * 100 / totalIterations;
                    InsertionSortProgress?.Invoke(percent);
                }
            }
            watch.Stop();
            lock (_locker)
            {
                _totalComparisons += comparisons;
            }
            // Сообщаем о завершении (100%)
            InsertionSortProgress?.Invoke(100);
            InsertionSortCompleted?.Invoke(array, comparisons, watch.Elapsed.TotalMilliseconds);
        }

        // Метод для сортировки слиянием (обёртка)
        public void MergeSort(int[] originalArray, CancellationToken token = default)
        {
            int[] array = CopyArray(originalArray);
            long comparisons = 0;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            int totalElements = array.Length;
            int processedElements = 0;
            int lastReportedPercent = -1;

            // Сообщаем о начале (0%)
            MergeSortProgress?.Invoke(0);

            MergeSortRecursive(array, 0, array.Length - 1, ref comparisons, totalElements, ref processedElements, ref lastReportedPercent, token);
            watch.Stop();
            lock (_locker)
            {
                _totalComparisons += comparisons;
            }
            // Сообщаем о завершении (100%)
            MergeSortProgress?.Invoke(100);
            MergeSortCompleted?.Invoke(array, comparisons, watch.Elapsed.TotalMilliseconds);
        }

        private void MergeSortRecursive(int[] arr, int left, int right, ref long comparisons, int totalElements, ref int processedElements, ref int lastReportedPercent, CancellationToken token = default)
        {
            if (left < right)
            {
                token.ThrowIfCancellationRequested();

                int mid = left + (right - left) / 2;
                MergeSortRecursive(arr, left, mid, ref comparisons, totalElements, ref processedElements, ref lastReportedPercent, token);
                MergeSortRecursive(arr, mid + 1, right, ref comparisons, totalElements, ref processedElements, ref lastReportedPercent, token);
                Merge(arr, left, mid, right, ref comparisons);

                processedElements += (right - left + 1);
                int percent = processedElements * 100 / totalElements;

                // Обновляем прогресс только если процент изменился
                if (percent != lastReportedPercent && percent > lastReportedPercent)
                {
                    MergeSortProgress?.Invoke(percent);
                    lastReportedPercent = percent;
                }
            }
        }

        private void Merge(int[] arr, int left, int mid, int right, ref long comparisons)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;

            int[] leftArr = new int[n1];
            int[] rightArr = new int[n2];

            Array.Copy(arr, left, leftArr, 0, n1);
            Array.Copy(arr, mid + 1, rightArr, 0, n2);

            int i = 0, j = 0, k = left;

            while (i < n1 && j < n2)
            {
                comparisons++;
                if (leftArr[i] <= rightArr[j])
                {
                    arr[k] = leftArr[i];
                    i++;
                }
                else
                {
                    arr[k] = rightArr[j];
                    j++;
                }
                k++;
            }

            while (i < n1)
            {
                arr[k] = leftArr[i];
                i++;
                k++;
            }

            while (j < n2)
            {
                arr[k] = rightArr[j];
                j++;
                k++;
            }
        }
    }
}
