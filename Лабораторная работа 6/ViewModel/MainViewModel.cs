using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Лабораторная_работа_6.Model;

namespace Лабораторная_работа_6.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ArraySorter _sorter;
        private readonly SynchronizationContext _uiContext;
        private int[] _originalArray;
        private CancellationTokenSource _cancellationTokenSource;
        
        [ObservableProperty]
        private int _arraySize = 1000;
        [ObservableProperty]
        private string _originalArrayString;
        [ObservableProperty]
        private string _bubbleSortResult;
        [ObservableProperty]
        private string _quickSortResult;
        [ObservableProperty]
        private string _insertionSortResult;
        [ObservableProperty]
        private string _mergeSortResult;
        [ObservableProperty]
        private string _totalComparisons = "Общее число сравнений: 0";
        [ObservableProperty]
        private string _totalExecutionTime = "Общее время выполнения: 0 мс";
        [ObservableProperty]
        private bool _useSharedArray = false;
        [ObservableProperty]
        private double _bubbleSortProgress = 0;
        [ObservableProperty]
        private double _quickSortProgress = 0;
        [ObservableProperty]
        private double _insertionSortProgress = 0;
        [ObservableProperty]
        private double _mergeSortProgress = 0;
        [ObservableProperty]
        private bool _isCancelling = false;
        [ObservableProperty]
        private bool _isSorting = false;
        [ObservableProperty]
        private int _threadCount = 4;

        public MainViewModel()
        {
            _sorter = new ArraySorter();
            _uiContext = SynchronizationContext.Current ?? new SynchronizationContext();
            
            _sorter.BubbleSortCompleted += OnBubbleSortCompleted;
            _sorter.QuickSortCompleted += OnQuickSortCompleted;
            _sorter.InsertionSortCompleted += OnInsertionSortCompleted;
            _sorter.MergeSortCompleted += OnMergeSortCompleted;
            _sorter.BubbleSortProgress += OnBubbleSortProgress;
            _sorter.QuickSortProgress += OnQuickSortProgress;
            _sorter.InsertionSortProgress += OnInsertionSortProgress;
            _sorter.MergeSortProgress += OnMergeSortProgress;
        }
        
        partial void OnUseSharedArrayChanged(bool value) => _sorter.UseSharedArray = value;
        
        [RelayCommand(CanExecute = nameof(CanGenerateArray))]
        private void GenerateArray()
        {
            _originalArray = _sorter.GenerateRandomArray(ArraySize);
            OriginalArrayString = "Исходный массив: " + string.Join(", ", _originalArray, 0, Math.Min(20, _originalArray.Length)) + (ArraySize > 20 ? "..." : "");
            
            BubbleSortResult = QuickSortResult = InsertionSortResult = MergeSortResult = null;
            BubbleSortProgress = QuickSortProgress = InsertionSortProgress = MergeSortProgress = 0;
            TotalComparisons = "Общее число сравнений: 0";
            TotalExecutionTime = "Общее время выполнения: 0 мс";
            
            BubbleSortCommand.NotifyCanExecuteChanged();
            QuickSortCommand.NotifyCanExecuteChanged();
            InsertionSortCommand.NotifyCanExecuteChanged();
            MergeSortCommand.NotifyCanExecuteChanged();
            SortAllCommand.NotifyCanExecuteChanged();
        }
        private bool CanGenerateArray() => !_isSorting && !_isCancelling;
        
        [RelayCommand(CanExecute = nameof(CanSortBubble))]
        private void BubbleSort()
        {
            BubbleSortResult = "Сортируется...";
            BubbleSortCommand.NotifyCanExecuteChanged();
            new Thread(() => _sorter.BubbleSort(_originalArray)).Start();
        }
        private bool CanSortBubble() => _originalArray != null && !_isSorting && !_isCancelling;
        
        [RelayCommand(CanExecute = nameof(CanSortQuick))]
        private void QuickSort()
        {
            QuickSortResult = "Сортируется...";
            QuickSortCommand.NotifyCanExecuteChanged();
            new Thread(() => _sorter.QuickSort(_originalArray)).Start();
        }
        private bool CanSortQuick() => _originalArray != null && !_isSorting && !_isCancelling;
        
        [RelayCommand(CanExecute = nameof(CanSortInsertion))]
        private void InsertionSort()
        {
            InsertionSortResult = "Сортируется...";
            InsertionSortCommand.NotifyCanExecuteChanged();
            new Thread(() => _sorter.InsertionSort(_originalArray)).Start();
        }
        private bool CanSortInsertion() => _originalArray != null && !_isSorting && !_isCancelling;
        
        [RelayCommand(CanExecute = nameof(CanSortMerge))]
        private void MergeSort()
        {
            MergeSortResult = "Сортируется...";
            MergeSortCommand.NotifyCanExecuteChanged();
            new Thread(() => _sorter.MergeSort(_originalArray)).Start();
        }
        private bool CanSortMerge() => _originalArray != null && !_isSorting && !_isCancelling;
        
        [RelayCommand(CanExecute = nameof(CanSortAll))]
        private async Task SortAllAsync()
        {
            if (_originalArray == null || _isSorting) return;
            
            _isSorting = true;
            _isCancelling = false;
            _cancellationTokenSource = new CancellationTokenSource();
            _sorter.ResetComparisons();
            
            BubbleSortResult = "Сортируется...";
            QuickSortResult = "Сортируется...";
            InsertionSortResult = "Сортируется...";
            MergeSortResult = "Сортируется...";
            TotalExecutionTime = "Выполняется...";
            BubbleSortProgress = QuickSortProgress = InsertionSortProgress = MergeSortProgress = 0;
            
            SortAllCommand.NotifyCanExecuteChanged();
            CancelSortingCommand.NotifyCanExecuteChanged();
            
            var watch = Stopwatch.StartNew();
            var tasks = new List<Task>();
            var semaphore = new Semaphore(4, 4);
            var token = _cancellationTokenSource.Token;
            
            try
            {
                tasks.Add(Task.Run(() => RunSortWithSemaphore(() => _sorter.BubbleSort(_originalArray, token), semaphore, token), token));
                tasks.Add(Task.Run(() => RunSortWithSemaphore(() => _sorter.QuickSort(_originalArray, token), semaphore, token), token));
                tasks.Add(Task.Run(() => RunSortWithSemaphore(() => _sorter.InsertionSort(_originalArray, token), semaphore, token), token));
                tasks.Add(Task.Run(() => RunSortWithSemaphore(() => _sorter.MergeSort(_originalArray, token), semaphore, token), token));
                
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                _uiContext.Post(_ =>
                {
                    TotalExecutionTime = "Сортировка прервана пользователем";
                    _isCancelling = false;
                    _isSorting = false;
                    SortAllCommand.NotifyCanExecuteChanged();
                    CancelSortingCommand.NotifyCanExecuteChanged();
                }, null);
                return;
            }
            finally
            {
                watch.Stop();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
            
            _uiContext.Post(_ =>
            {
                if (!_isCancelling)
                    TotalExecutionTime = $"Общее время выполнения: {watch.Elapsed.TotalMilliseconds:F2} мс";
                _isSorting = false;
                _isCancelling = false;
                SortAllCommand.NotifyCanExecuteChanged();
                CancelSortingCommand.NotifyCanExecuteChanged();
            }, null);
        }
        
        private void RunSortWithSemaphore(Action sortAction, Semaphore semaphore, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;
            semaphore.WaitOne();
            try
            {
                token.ThrowIfCancellationRequested();
                sortAction();
            }
            finally { semaphore.Release(); }
        }
        
        private bool CanSortAll() => _originalArray != null && !_isSorting && !_isCancelling;
        
        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void CancelSorting()
        {
            if (_cancellationTokenSource != null && !_isCancelling)
            {
                _isCancelling = true;
                _cancellationTokenSource.Cancel();
                CancelSortingCommand.NotifyCanExecuteChanged();
                SortAllCommand.NotifyCanExecuteChanged();
            }
        }
        private bool CanCancel() => _isSorting && !_isCancelling;
        
        private void OnBubbleSortCompleted(int[] sortedArray, long comparisons, double elapsedMs)
        {
            _uiContext.Post(_ =>
            {
                if (!_isCancelling)
                {
                    BubbleSortResult = $"Пузырьковая: {FormatArray(sortedArray)}, время: {elapsedMs:F2} мс, сравнений: {comparisons}";
                    UpdateTotalComparisons();
                    BubbleSortCommand.NotifyCanExecuteChanged();
                    SortAllCommand.NotifyCanExecuteChanged();
                }
            }, null);
        }
        
        private void OnQuickSortCompleted(int[] sortedArray, long comparisons, double elapsedMs)
        {
            _uiContext.Post(_ =>
            {
                if (!_isCancelling)
                {
                    QuickSortResult = $"Быстрая: {FormatArray(sortedArray)}, время: {elapsedMs:F2} мс, сравнений: {comparisons}";
                    UpdateTotalComparisons();
                    QuickSortCommand.NotifyCanExecuteChanged();
                    SortAllCommand.NotifyCanExecuteChanged();
                }
            }, null);
        }
        
        private void OnInsertionSortCompleted(int[] sortedArray, long comparisons, double elapsedMs)
        {
            _uiContext.Post(_ =>
            {
                if (!_isCancelling)
                {
                    InsertionSortResult = $"Вставками: {FormatArray(sortedArray)}, время: {elapsedMs:F2} мс, сравнений: {comparisons}";
                    UpdateTotalComparisons();
                    InsertionSortCommand.NotifyCanExecuteChanged();
                    SortAllCommand.NotifyCanExecuteChanged();
                }
            }, null);
        }
        
        private void OnMergeSortCompleted(int[] sortedArray, long comparisons, double elapsedMs)
        {
            _uiContext.Post(_ =>
            {
                if (!_isCancelling)
                {
                    MergeSortResult = $"Слиянием: {FormatArray(sortedArray)}, время: {elapsedMs:F2} мс, сравнений: {comparisons}";
                    UpdateTotalComparisons();
                    MergeSortCommand.NotifyCanExecuteChanged();
                    SortAllCommand.NotifyCanExecuteChanged();
                }
            }, null);
        }
        
        private void OnBubbleSortProgress(int current, int total, double percent) => _uiContext.Post(_ => BubbleSortProgress = percent, null);
        private void OnQuickSortProgress(int current, int total, double percent) => _uiContext.Post(_ => QuickSortProgress = percent, null);
        private void OnInsertionSortProgress(int current, int total, double percent) => _uiContext.Post(_ => InsertionSortProgress = percent, null);
        private void OnMergeSortProgress(int current, int total, double percent) => _uiContext.Post(_ => MergeSortProgress = percent, null);
        
        private void UpdateTotalComparisons() => TotalComparisons = $"Общее число сравнений: {_sorter.TotalComparisons}";
        
        private string FormatArray(int[] arr) => arr.Length <= 10 ? string.Join(", ", arr) : string.Join(", ", arr, 0, 5) + "...";
    }
}
