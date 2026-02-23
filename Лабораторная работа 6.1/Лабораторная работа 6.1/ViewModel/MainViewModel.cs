using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Лабораторная_работа_6._1.Model;

namespace Лабораторная_работа_6._1.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ArraySorter _sorter;
        private int[] _originalArray;
        private int _arraySize = 1000;
        private string _originalArrayString;
        private string _bubbleSortResult;
        private string _quickSortResult;
        private string _insertionSortResult;
        private string _mergeSortResult;
        private string _totalComparisons = "Общее число сравнений: 0";
        private bool _isBubbleSorting;
        private bool _isQuickSorting;
        private bool _isInsertionSorting;
        private bool _isMergeSorting;
        private double _bubbleSortProgress;
        private double _quickSortProgress;
        private double _insertionSortProgress;
        private double _mergeSortProgress;

        // CancellationTokenSource для отмены всех сортировок
        private CancellationTokenSource _cancellationTokenSource;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public int ArraySize
        {
            get => _arraySize;
            set => SetField(ref _arraySize, value);
        }

        public string OriginalArrayString
        {
            get => _originalArrayString;
            set => SetField(ref _originalArrayString, value);
        }

        public string BubbleSortResult
        {
            get => _bubbleSortResult;
            set => SetField(ref _bubbleSortResult, value);
        }

        public string QuickSortResult
        {
            get => _quickSortResult;
            set => SetField(ref _quickSortResult, value);
        }

        public string InsertionSortResult
        {
            get => _insertionSortResult;
            set => SetField(ref _insertionSortResult, value);
        }

        public string MergeSortResult
        {
            get => _mergeSortResult;
            set => SetField(ref _mergeSortResult, value);
        }

        public string TotalComparisons
        {
            get => _totalComparisons;
            set => SetField(ref _totalComparisons, value);
        }

        public bool IsBubbleSorting
        {
            get => _isBubbleSorting;
            set => SetField(ref _isBubbleSorting, value);
        }

        public bool IsQuickSorting
        {
            get => _isQuickSorting;
            set => SetField(ref _isQuickSorting, value);
        }

        public bool IsInsertionSorting
        {
            get => _isInsertionSorting;
            set => SetField(ref _isInsertionSorting, value);
        }

        public bool IsMergeSorting
        {
            get => _isMergeSorting;
            set => SetField(ref _isMergeSorting, value);
        }

        public double BubbleSortProgress
        {
            get => _bubbleSortProgress;
            set => SetField(ref _bubbleSortProgress, value);
        }

        public double QuickSortProgress
        {
            get => _quickSortProgress;
            set => SetField(ref _quickSortProgress, value);
        }

        public double InsertionSortProgress
        {
            get => _insertionSortProgress;
            set => SetField(ref _insertionSortProgress, value);
        }

        public double MergeSortProgress
        {
            get => _mergeSortProgress;
            set => SetField(ref _mergeSortProgress, value);
        }

        public bool IsAnySorting => IsBubbleSorting || IsQuickSorting || IsInsertionSorting || IsMergeSorting;

        public IAsyncRelayCommand GenerateArrayCommand { get; }
        public IAsyncRelayCommand BubbleSortCommand { get; }
        public IAsyncRelayCommand QuickSortCommand { get; }
        public IAsyncRelayCommand InsertionSortCommand { get; }
        public IAsyncRelayCommand MergeSortCommand { get; }
        public IRelayCommand CancelAllCommand { get; }

        public MainViewModel()
        {
            _sorter = new ArraySorter();
            _cancellationTokenSource = new CancellationTokenSource();
            GenerateArrayCommand = new AsyncRelayCommand(GenerateArrayAsync, CanGenerateArray);
            BubbleSortCommand = new AsyncRelayCommand(BubbleSortAsync, CanSortBubble);
            QuickSortCommand = new AsyncRelayCommand(QuickSortAsync, CanSortQuick);
            InsertionSortCommand = new AsyncRelayCommand(InsertionSortAsync, CanSortInsertion);
            MergeSortCommand = new AsyncRelayCommand(MergeSortAsync, CanSortMerge);
            CancelAllCommand = new RelayCommand(CancelAll, CanCancelAll);
        }

        private bool CanGenerateArray() => !GenerateArrayCommand.IsRunning && !IsAnySorting;
        private bool CanSortBubble() => _originalArray != null && !IsBubbleSorting;
        private bool CanSortQuick() => _originalArray != null && !IsQuickSorting;
        private bool CanSortInsertion() => _originalArray != null && !IsInsertionSorting;
        private bool CanSortMerge() => _originalArray != null && !IsMergeSorting;
        private bool CanCancelAll() => IsAnySorting;

        private void UpdateCanExecute()
        {
            GenerateArrayCommand.NotifyCanExecuteChanged();
            BubbleSortCommand.NotifyCanExecuteChanged();
            QuickSortCommand.NotifyCanExecuteChanged();
            InsertionSortCommand.NotifyCanExecuteChanged();
            MergeSortCommand.NotifyCanExecuteChanged();
            CancelAllCommand.NotifyCanExecuteChanged();
        }

        private async Task GenerateArrayAsync()
        {
            await Task.Delay(100);
            _originalArray = _sorter.GenerateRandomArray(ArraySize);
            OriginalArrayString = "Исходный массив: " + string.Join(", ", _originalArray.Take(20)) + (ArraySize > 20 ? "..." : "");
            BubbleSortResult = null;
            QuickSortResult = null;
            InsertionSortResult = null;
            MergeSortResult = null;
            TotalComparisons = "Общее число сравнений: 0";
            IsBubbleSorting = false;
            IsQuickSorting = false;
            IsInsertionSorting = false;
            IsMergeSorting = false;
            BubbleSortProgress = 0;
            QuickSortProgress = 0;
            InsertionSortProgress = 0;
            MergeSortProgress = 0;
            OnPropertyChanged(nameof(IsAnySorting));
            UpdateCanExecute();
        }

        private async Task BubbleSortAsync()
        {
            IsBubbleSorting = true;
            BubbleSortProgress = 0;
            OnPropertyChanged(nameof(IsAnySorting));
            BubbleSortResult = "Сортируется...";
            UpdateCanExecute();
            var progress = new Progress<double>(p => BubbleSortProgress = p);
            try
            {
                var result = await _sorter.BubbleSortAsync(_originalArray, progress, _cancellationTokenSource.Token);
                BubbleSortResult = $"Пузырьковая: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
                UpdateTotalComparisons();
            }
            catch (OperationCanceledException)
            {
                BubbleSortResult = "Отменено";
            }
            finally
            {
                IsBubbleSorting = false;
                BubbleSortProgress = 100;
                OnPropertyChanged(nameof(IsAnySorting));
                UpdateCanExecute();
            }
        }

        private async Task QuickSortAsync()
        {
            IsQuickSorting = true;
            QuickSortProgress = 0;
            OnPropertyChanged(nameof(IsAnySorting));
            QuickSortResult = "Сортируется...";
            UpdateCanExecute();
            var progress = new Progress<double>(p => QuickSortProgress = p);
            try
            {
                var result = await _sorter.QuickSortAsync(_originalArray, progress, _cancellationTokenSource.Token);
                QuickSortResult = $"Быстрая: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
                UpdateTotalComparisons();
            }
            catch (OperationCanceledException)
            {
                QuickSortResult = "Отменено";
            }
            finally
            {
                IsQuickSorting = false;
                QuickSortProgress = 100;
                OnPropertyChanged(nameof(IsAnySorting));
                UpdateCanExecute();
            }
        }

        private async Task InsertionSortAsync()
        {
            IsInsertionSorting = true;
            InsertionSortProgress = 0;
            OnPropertyChanged(nameof(IsAnySorting));
            InsertionSortResult = "Сортируется...";
            UpdateCanExecute();
            var progress = new Progress<double>(p => InsertionSortProgress = p);
            try
            {
                var result = await _sorter.InsertionSortAsync(_originalArray, progress, _cancellationTokenSource.Token);
                InsertionSortResult = $"Вставками: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
                UpdateTotalComparisons();
            }
            catch (OperationCanceledException)
            {
                InsertionSortResult = "Отменено";
            }
            finally
            {
                IsInsertionSorting = false;
                InsertionSortProgress = 100;
                OnPropertyChanged(nameof(IsAnySorting));
                UpdateCanExecute();
            }
        }

        private async Task MergeSortAsync()
        {
            IsMergeSorting = true;
            MergeSortProgress = 0;
            OnPropertyChanged(nameof(IsAnySorting));
            MergeSortResult = "Сортируется...";
            UpdateCanExecute();
            var progress = new Progress<double>(p => MergeSortProgress = p);
            try
            {
                var result = await _sorter.MergeSortAsync(_originalArray, progress, _cancellationTokenSource.Token);
                MergeSortResult = $"Слиянием: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
                UpdateTotalComparisons();
            }
            catch (OperationCanceledException)
            {
                MergeSortResult = "Отменено";
            }
            finally
            {
                IsMergeSorting = false;
                MergeSortProgress = 100;
                OnPropertyChanged(nameof(IsAnySorting));
                UpdateCanExecute();
            }
        }

        private void CancelAll()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                // Создаём новый CancellationTokenSource для будущих операций
                _cancellationTokenSource = new CancellationTokenSource();
                IsBubbleSorting = false;
                IsQuickSorting = false;
                IsInsertionSorting = false;
                IsMergeSorting = false;
                BubbleSortProgress = 0;
                QuickSortProgress = 0;
                InsertionSortProgress = 0;
                MergeSortProgress = 0;
                OnPropertyChanged(nameof(IsAnySorting));
                UpdateCanExecute();
            }
        }

        private void UpdateTotalComparisons()
        {
            TotalComparisons = $"Общее число сравнений: {_sorter.TotalComparisons}";
        }

        private string FormatArray(int[] arr)
        {
            if (arr.Length <= 10)
                return string.Join(", ", arr);
            else
                return string.Join(", ", arr.Take(5)) + "...";
        }
    }
}
