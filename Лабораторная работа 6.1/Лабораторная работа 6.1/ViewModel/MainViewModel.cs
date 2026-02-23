using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private string _totalComparisons = "Общее число сравнений: 0";

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

        public string TotalComparisons
        {
            get => _totalComparisons;
            set => SetField(ref _totalComparisons, value);
        }

        public IAsyncRelayCommand GenerateArrayCommand { get; }
        public IAsyncRelayCommand BubbleSortCommand { get; }
        public IAsyncRelayCommand QuickSortCommand { get; }
        public IAsyncRelayCommand InsertionSortCommand { get; }

        public MainViewModel()
        {
            _sorter = new ArraySorter();
            GenerateArrayCommand = new AsyncRelayCommand(GenerateArrayAsync, CanGenerateArray);
            BubbleSortCommand = new AsyncRelayCommand(BubbleSortAsync, CanSortBubble);
            QuickSortCommand = new AsyncRelayCommand(QuickSortAsync, CanSortQuick);
            InsertionSortCommand = new AsyncRelayCommand(InsertionSortAsync, CanSortInsertion);
        }

        private bool CanGenerateArray() => !GenerateArrayCommand.IsRunning;
        private bool CanSortBubble() => _originalArray != null && !BubbleSortCommand.IsRunning;
        private bool CanSortQuick() => _originalArray != null && !QuickSortCommand.IsRunning;
        private bool CanSortInsertion() => _originalArray != null && !InsertionSortCommand.IsRunning;

        private async Task GenerateArrayAsync()
        {
            await Task.Delay(100);
            _originalArray = _sorter.GenerateRandomArray(ArraySize);
            OriginalArrayString = "Исходный массив: " + string.Join(", ", _originalArray.Take(20)) + (ArraySize > 20 ? "..." : "");
            BubbleSortResult = null;
            QuickSortResult = null;
            InsertionSortResult = null;
            TotalComparisons = "Общее число сравнений: 0";
            BubbleSortCommand.NotifyCanExecuteChanged();
            QuickSortCommand.NotifyCanExecuteChanged();
            InsertionSortCommand.NotifyCanExecuteChanged();
        }

        private async Task BubbleSortAsync()
        {
            BubbleSortResult = "Сортируется...";
            var result = await _sorter.BubbleSortAsync(_originalArray);
            BubbleSortResult = $"Пузырьковая: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
            UpdateTotalComparisons();
        }

        private async Task QuickSortAsync()
        {
            QuickSortResult = "Сортируется...";
            var result = await _sorter.QuickSortAsync(_originalArray);
            QuickSortResult = $"Быстрая: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
            UpdateTotalComparisons();
        }

        private async Task InsertionSortAsync()
        {
            InsertionSortResult = "Сортируется...";
            var result = await _sorter.InsertionSortAsync(_originalArray);
            InsertionSortResult = $"Вставками: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
            UpdateTotalComparisons();
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
