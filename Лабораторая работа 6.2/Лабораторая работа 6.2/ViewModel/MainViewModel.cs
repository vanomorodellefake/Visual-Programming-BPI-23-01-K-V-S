using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Лабораторая_работа_6._2.Model;

namespace Лабораторая_работа_6._2.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ArraySorter _sorter;
        private readonly SynchronizationContext _uiContext;
        private int[] _originalArray;
        private CancellationTokenSource _cancellationTokenSource;
        // Наблюдаемые свойства
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
        // Для Async версии
        [ObservableProperty]
        private string _totalComparisonsAsync = "Общее число сравнений: 0";
        [ObservableProperty]
        private string _totalExecutionTimeAsync = "Общее время выполнения: 0 мс";
        [ObservableProperty]
        private bool _canGenerate = true;
        [ObservableProperty]
        private bool _isCancelling = false;
        // ProgressBar
        [ObservableProperty]
        private double _bubbleSortProgress;
        [ObservableProperty]
        private double _quickSortProgress;
        [ObservableProperty]
        private double _insertionSortProgress;
        [ObservableProperty]
        private double _mergeSortProgress;
        [ObservableProperty]
        private bool _isSorting;
        public MainViewModel()
        {
            _sorter = new ArraySorter();
            _uiContext = SynchronizationContext.Current ?? new SynchronizationContext();
            
            // Инициализация Progress<double> для обновления ProgressBar в UI потоке
            _bubbleSortProgressReporter = new Progress<double>(value => BubbleSortProgress = value);
            _quickSortProgressReporter = new Progress<double>(value => QuickSortProgress = value);
            _insertionSortProgressReporter = new Progress<double>(value => InsertionSortProgress = value);
            _mergeSortProgressReporter = new Progress<double>(value => MergeSortProgress = value);
            
            // Подписка на события завершения сортировки
            _sorter.BubbleSortCompleted += OnBubbleSortCompleted;
            _sorter.QuickSortCompleted += OnQuickSortCompleted;
            _sorter.InsertionSortCompleted += OnInsertionSortCompleted;
            _sorter.MergeSortCompleted += OnMergeSortCompleted;

            _sorter.BubbleSortProgress += OnBubbleSortProgress;
            _sorter.QuickSortProgress += OnQuickSortProgress;
            _sorter.InsertionSortProgress += OnInsertionSortProgress;
            _sorter.MergeSortProgress += OnMergeSortProgress;

        }
        // Команда генерации массива
        [RelayCommand(CanExecute = nameof(CanGenerateArray))]
        private void GenerateArray()
        {
            _originalArray = _sorter.GenerateRandomArray(ArraySize);
            // Отображаем первые 20 элементов
            OriginalArrayString = "Исходный массив: " + string.Join(", ", _originalArray, 0, Math.Min(20,
           _originalArray.Length)) + (ArraySize > 20 ? "..." : "");
            // Сбрасываем предыдущие результаты (Thread)
            BubbleSortResult = QuickSortResult = InsertionSortResult = MergeSortResult = null;
            BubbleSortProgress = QuickSortProgress = InsertionSortProgress = MergeSortProgress = 0;
            TotalComparisons = "Общее число сравнений: 0";
            TotalExecutionTime = "Общее время выполнения: 0 мс";
            // Сбрасываем предыдущие результаты (Async)
            BubbleSortResultAsync = QuickSortResultAsync = InsertionSortResultAsync = MergeSortResultAsync = null;
            BubbleSortProgressAsync = QuickSortProgressAsync = InsertionSortProgressAsync = MergeSortProgressAsync = 0;
            TotalComparisonsAsync = "Общее число сравнений: 0";
            TotalExecutionTimeAsync = "Общее время выполнения: 0 мс";
            // Обновляем состояние команд сортировок (Thread)
            BubbleSortCommand.NotifyCanExecuteChanged();
            QuickSortCommand.NotifyCanExecuteChanged();
            InsertionSortCommand.NotifyCanExecuteChanged();
            MergeSortCommand.NotifyCanExecuteChanged();
            SortAllCommand.NotifyCanExecuteChanged();
            // Обновляем состояние команд сортировок (Async)
            BubbleSortAsyncCommand.NotifyCanExecuteChanged();
            QuickSortAsyncCommand.NotifyCanExecuteChanged();
            InsertionSortAsyncCommand.NotifyCanExecuteChanged();
            MergeSortAsyncCommand.NotifyCanExecuteChanged();
            SortAllAsyncCommand.NotifyCanExecuteChanged();
        }
        private bool CanGenerateArray() => CanGenerate;
        // Пузырьковая сортировка
        private bool CanSortBubble() => _originalArray != null && !_isSorting && !_isSorting;
        [RelayCommand(CanExecute = nameof(CanSortBubble))]
        private void BubbleSort()
        {
            _isSorting = true;
            _isCancelling = false;
            _cancellationTokenSource = new CancellationTokenSource();
            _sorter.ResetComparisons();
            
            BubbleSortResult = "Сортируется...";
            BubbleSortCommand.NotifyCanExecuteChanged();
            CancelSortingCommand.NotifyCanExecuteChanged();
            
            new Thread(() =>
            {
                try
                {
                    _sorter.BubbleSort(_originalArray, _cancellationTokenSource.Token);
                    _uiContext.Post(_ =>
                    {
                        _isSorting = false;
                        _isCancelling = false;
                        BubbleSortCommand.NotifyCanExecuteChanged();
                        CancelSortingCommand.NotifyCanExecuteChanged();
                    }, null);
                }
                catch (OperationCanceledException)
                {
                    _uiContext.Post(_ =>
                    {
                        BubbleSortResult = "Прервано";
                        _isSorting = false;
                        _isCancelling = false;
                        BubbleSortCommand.NotifyCanExecuteChanged();
                        CancelSortingCommand.NotifyCanExecuteChanged();
                    }, null);
                }
            })
            { IsBackground = true }.Start();
        }
        // Быстрая сортировка
        private bool CanSortQuick() => _originalArray != null && !_isSorting && !_isSorting;
        [RelayCommand(CanExecute = nameof(CanSortQuick))]
        private void QuickSort()
        {
            _isSorting = true;
            _isCancelling = false;
            _cancellationTokenSource = new CancellationTokenSource();
            _sorter.ResetComparisons();
            
            QuickSortResult = "Сортируется...";
            QuickSortCommand.NotifyCanExecuteChanged();
            CancelSortingCommand.NotifyCanExecuteChanged();
            
            new Thread(() =>
            {
                try
                {
                    _sorter.QuickSort(_originalArray, _cancellationTokenSource.Token);
                    _uiContext.Post(_ =>
                    {
                        _isSorting = false;
                        _isCancelling = false;
                        QuickSortCommand.NotifyCanExecuteChanged();
                        CancelSortingCommand.NotifyCanExecuteChanged();
                    }, null);
                }
                catch (OperationCanceledException)
                {
                    _uiContext.Post(_ =>
                    {
                        QuickSortResult = "Прервано";
                        _isSorting = false;
                        _isCancelling = false;
                        QuickSortCommand.NotifyCanExecuteChanged();
                        CancelSortingCommand.NotifyCanExecuteChanged();
                    }, null);
                }
            })
            { IsBackground = true }.Start();
        }
        // Сортировка вставками
        private bool CanSortInsertion() => _originalArray != null && !_isSorting && !_isSorting;
        [RelayCommand(CanExecute = nameof(CanSortInsertion))]
        private void InsertionSort()
        {
            _isSorting = true;
            _isCancelling = false;
            _cancellationTokenSource = new CancellationTokenSource();
            _sorter.ResetComparisons();
            
            InsertionSortResult = "Сортируется...";
            InsertionSortCommand.NotifyCanExecuteChanged();
            CancelSortingCommand.NotifyCanExecuteChanged();
            
            new Thread(() =>
            {
                try
                {
                    _sorter.InsertionSort(_originalArray, _cancellationTokenSource.Token);
                    _uiContext.Post(_ =>
                    {
                        _isSorting = false;
                        _isCancelling = false;
                        InsertionSortCommand.NotifyCanExecuteChanged();
                        CancelSortingCommand.NotifyCanExecuteChanged();
                    }, null);
                }
                catch (OperationCanceledException)
                {
                    _uiContext.Post(_ =>
                    {
                        InsertionSortResult = "Прервано";
                        _isSorting = false;
                        _isCancelling = false;
                        InsertionSortCommand.NotifyCanExecuteChanged();
                        CancelSortingCommand.NotifyCanExecuteChanged();
                    }, null);
                }
            })
            { IsBackground = true }.Start();
        }
        [RelayCommand(CanExecute = nameof(CanSortMerge))]
        private void MergeSort()
        {
            _isSorting = true;
            _isCancelling = false;
            _cancellationTokenSource = new CancellationTokenSource();
            _sorter.ResetComparisons();
            
            MergeSortResult = "Сортируется...";
            MergeSortCommand.NotifyCanExecuteChanged();
            CancelSortingCommand.NotifyCanExecuteChanged();
            
            new Thread(() =>
            {
                try
                {
                    _sorter.MergeSort(_originalArray, _cancellationTokenSource.Token);
                    _uiContext.Post(_ =>
                    {
                        _isSorting = false;
                        _isCancelling = false;
                        MergeSortCommand.NotifyCanExecuteChanged();
                        CancelSortingCommand.NotifyCanExecuteChanged();
                    }, null);
                }
                catch (OperationCanceledException)
                {
                    _uiContext.Post(_ =>
                    {
                        MergeSortResult = "Прервано";
                        _isSorting = false;
                        _isCancelling = false;
                        MergeSortCommand.NotifyCanExecuteChanged();
                        CancelSortingCommand.NotifyCanExecuteChanged();
                    }, null);
                }
            })
            { IsBackground = true }.Start();
        }
        private bool CanSortMerge() => _originalArray != null && !_isSorting && !_isSorting;

        [RelayCommand(CanExecute = nameof(CanSortAll))]
        private void SortAll()
        {
            new Thread(() =>
            {
                if (_originalArray == null || _isSorting) return;

                _isSorting = true;
                _isCancelling = false;
                _cancellationTokenSource = new CancellationTokenSource();
                _sorter.ResetComparisons();

                _uiContext.Post(_ =>
                {
                    BubbleSortResult = "Сортируется...";
                    QuickSortResult = "Сортируется...";
                    InsertionSortResult = "Сортируется...";
                    MergeSortResult = "Сортируется...";
                    TotalExecutionTime = "Выполняется...";
                    BubbleSortProgress = QuickSortProgress = InsertionSortProgress = MergeSortProgress = 0;
                    SortAllCommand.NotifyCanExecuteChanged();
                }, null);

                var watch = Stopwatch.StartNew();
                var token = _cancellationTokenSource.Token;
                var threads = new List<Thread>();

                try
                {
                    threads.Add(new Thread(() => _sorter.BubbleSort(_originalArray, token)));
                    threads.Add(new Thread(() => _sorter.QuickSort(_originalArray, token)));
                    threads.Add(new Thread(() => _sorter.InsertionSort(_originalArray, token)));
                    threads.Add(new Thread(() => _sorter.MergeSort(_originalArray, token)));

                    threads.ForEach(t => t.Start());
                    threads.ForEach(t => t.Join());

                    watch.Stop();

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
                }
                finally
                {
                    _cancellationTokenSource?.Dispose();
                    _cancellationTokenSource = null;
                }
            })
            { IsBackground = true }.Start();
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
        private bool CanCancel() => !_isCancelling;
        // Обработчики событий (вызываются из фоновых потоков)
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
        private void UpdateTotalComparisons() => TotalComparisons = $"Общее число сравнений: {_sorter.TotalComparisons}";
        private string FormatArray(int[] arr) => arr.Length <= 10 ? string.Join(", ", arr) : string.Join(", ", arr, 0, 5) + "...";

        // IProgress<double> для обновления ProgressBar
        private readonly IProgress<double> _bubbleSortProgressReporter;
        private readonly IProgress<double> _quickSortProgressReporter;
        private readonly IProgress<double> _insertionSortProgressReporter;
        private readonly IProgress<double> _mergeSortProgressReporter;

        private void OnBubbleSortProgress(double percent) => _bubbleSortProgressReporter.Report(percent);
        private void OnQuickSortProgress(double percent) => _quickSortProgressReporter.Report(percent);
        private void OnInsertionSortProgress(double percent) => _insertionSortProgressReporter.Report(percent);
        private void OnMergeSortProgress(double percent) => _mergeSortProgressReporter.Report(percent);

        // === Async версии методов ===
        // Свойства для Async версии
        [ObservableProperty]
        private string _bubbleSortResultAsync;
        [ObservableProperty]
        private string _quickSortResultAsync;
        [ObservableProperty]
        private string _insertionSortResultAsync;
        [ObservableProperty]
        private string _mergeSortResultAsync;
        [ObservableProperty]
        private double _bubbleSortProgressAsync;
        [ObservableProperty]
        private double _quickSortProgressAsync;
        [ObservableProperty]
        private double _insertionSortProgressAsync;
        [ObservableProperty]
        private double _mergeSortProgressAsync;

        // Async команды (созданные вручную)
        private IAsyncRelayCommand? _bubbleSortAsyncCommand;
        private IAsyncRelayCommand? _quickSortAsyncCommand;
        private IAsyncRelayCommand? _insertionSortAsyncCommand;
        private IAsyncRelayCommand? _mergeSortAsyncCommand;
        private IAsyncRelayCommand? _sortAllAsyncCommand;

        public IAsyncRelayCommand BubbleSortAsyncCommand => _bubbleSortAsyncCommand ??= new AsyncRelayCommand(BubbleSortAsyncRun, CanSortBubbleAsync);
        public IAsyncRelayCommand QuickSortAsyncCommand => _quickSortAsyncCommand ??= new AsyncRelayCommand(QuickSortAsyncRun, CanSortQuickAsync);
        public IAsyncRelayCommand InsertionSortAsyncCommand => _insertionSortAsyncCommand ??= new AsyncRelayCommand(InsertionSortAsyncRun, CanSortInsertionAsync);
        public IAsyncRelayCommand MergeSortAsyncCommand => _mergeSortAsyncCommand ??= new AsyncRelayCommand(MergeSortAsyncRun, CanSortMergeAsync);
        public IAsyncRelayCommand SortAllAsyncCommand => _sortAllAsyncCommand ??= new AsyncRelayCommand(SortAllAsyncRun, CanSortAllAsync);

        private async Task BubbleSortAsyncRun()
        {
            _isSorting = true;
            _isCancelling = false;
            _cancellationTokenSource = new CancellationTokenSource();
            _sorter.ResetComparisons();

            BubbleSortResultAsync = "[ASYNC] Сортируется...";
            BubbleSortProgressAsync = 0;
            BubbleSortAsyncCommand.NotifyCanExecuteChanged();
            CancelSortingCommand.NotifyCanExecuteChanged();

            var progress = new Progress<int>(percent => BubbleSortProgressAsync = percent);

            try
            {
                var result = await _sorter.BubbleSortAsync(_originalArray, progress, _cancellationTokenSource.Token);
                BubbleSortResultAsync = $"[ASYNC] Пузырьковая: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
            }
            catch (OperationCanceledException)
            {
                BubbleSortResultAsync = "[ASYNC] Прервано";
            }
            finally
            {
                _isSorting = false;
                _isCancelling = false;
                BubbleSortAsyncCommand.NotifyCanExecuteChanged();
                CancelSortingCommand.NotifyCanExecuteChanged();
            }
        }

        private async Task QuickSortAsyncRun()
        {
            _isSorting = true;
            _isCancelling = false;
            _cancellationTokenSource = new CancellationTokenSource();
            _sorter.ResetComparisons();

            QuickSortResultAsync = "[ASYNC] Сортируется...";
            QuickSortProgressAsync = 0;
            QuickSortAsyncCommand.NotifyCanExecuteChanged();
            CancelSortingCommand.NotifyCanExecuteChanged();

            var progress = new Progress<int>(percent => QuickSortProgressAsync = percent);

            try
            {
                var result = await _sorter.QuickSortAsync(_originalArray, progress, _cancellationTokenSource.Token);
                QuickSortResultAsync = $"[ASYNC] Быстрая: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
            }
            catch (OperationCanceledException)
            {
                QuickSortResultAsync = "[ASYNC] Прервано";
            }
            finally
            {
                _isSorting = false;
                _isCancelling = false;
                QuickSortAsyncCommand.NotifyCanExecuteChanged();
                CancelSortingCommand.NotifyCanExecuteChanged();
            }
        }

        private async Task InsertionSortAsyncRun()
        {
            _isSorting = true;
            _isCancelling = false;
            _cancellationTokenSource = new CancellationTokenSource();
            _sorter.ResetComparisons();

            InsertionSortResultAsync = "[ASYNC] Сортируется...";
            InsertionSortProgressAsync = 0;
            InsertionSortAsyncCommand.NotifyCanExecuteChanged();
            CancelSortingCommand.NotifyCanExecuteChanged();

            var progress = new Progress<int>(percent => InsertionSortProgressAsync = percent);

            try
            {
                var result = await _sorter.InsertionSortAsync(_originalArray, progress, _cancellationTokenSource.Token);
                InsertionSortResultAsync = $"[ASYNC] Вставками: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
            }
            catch (OperationCanceledException)
            {
                InsertionSortResultAsync = "[ASYNC] Прервано";
            }
            finally
            {
                _isSorting = false;
                _isCancelling = false;
                InsertionSortAsyncCommand.NotifyCanExecuteChanged();
                CancelSortingCommand.NotifyCanExecuteChanged();
            }
        }

        private async Task MergeSortAsyncRun()
        {
            _isSorting = true;
            _isCancelling = false;
            _cancellationTokenSource = new CancellationTokenSource();
            _sorter.ResetComparisons();

            MergeSortResultAsync = "[ASYNC] Сортируется...";
            MergeSortProgressAsync = 0;
            MergeSortAsyncCommand.NotifyCanExecuteChanged();
            CancelSortingCommand.NotifyCanExecuteChanged();

            var progress = new Progress<int>(percent => MergeSortProgressAsync = percent);

            try
            {
                var result = await _sorter.MergeSortAsync(_originalArray, progress, _cancellationTokenSource.Token);
                MergeSortResultAsync = $"[ASYNC] Слиянием: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
            }
            catch (OperationCanceledException)
            {
                MergeSortResultAsync = "[ASYNC] Прервано";
            }
            finally
            {
                _isSorting = false;
                _isCancelling = false;
                MergeSortAsyncCommand.NotifyCanExecuteChanged();
                CancelSortingCommand.NotifyCanExecuteChanged();
            }
        }

        private async Task SortAllAsyncRun()
        {
            if (_originalArray == null || _isSorting) return;

            _isSorting = true;
            _isCancelling = false;
            _cancellationTokenSource = new CancellationTokenSource();
            _sorter.ResetComparisons();

            BubbleSortResultAsync = "[ASYNC] Сортируется...";
            QuickSortResultAsync = "[ASYNC] Сортируется...";
            InsertionSortResultAsync = "[ASYNC] Сортируется...";
            MergeSortResultAsync = "[ASYNC] Сортируется...";
            TotalExecutionTimeAsync = "[ASYNC] Выполняется...";
            BubbleSortProgressAsync = QuickSortProgressAsync = InsertionSortProgressAsync = MergeSortProgressAsync = 0;

            SortAllAsyncCommand.NotifyCanExecuteChanged();
            CancelSortingCommand.NotifyCanExecuteChanged();

            var progressBubble = new Progress<int>(percent => BubbleSortProgressAsync = percent);
            var progressQuick = new Progress<int>(percent => QuickSortProgressAsync = percent);
            var progressInsertion = new Progress<int>(percent => InsertionSortProgressAsync = percent);
            var progressMerge = new Progress<int>(percent => MergeSortProgressAsync = percent);

            var watch = Stopwatch.StartNew();
            var token = _cancellationTokenSource.Token;

            try
            {
                var bubbleTask = _sorter.BubbleSortAsync(_originalArray, progressBubble, token);
                var quickTask = _sorter.QuickSortAsync(_originalArray, progressQuick, token);
                var insertionTask = _sorter.InsertionSortAsync(_originalArray, progressInsertion, token);
                var mergeTask = _sorter.MergeSortAsync(_originalArray, progressMerge, token);

                var results = await Task.WhenAll(bubbleTask, quickTask, insertionTask, mergeTask);

                watch.Stop();

                // Обновляем результаты каждой сортировки
                BubbleSortResultAsync = $"[ASYNC] Пузырьковая: {FormatArray(results[0].SortedArray)}, время: {results[0].ElapsedMilliseconds:F2} мс, сравнений: {results[0].Comparisons}";
                QuickSortResultAsync = $"[ASYNC] Быстрая: {FormatArray(results[1].SortedArray)}, время: {results[1].ElapsedMilliseconds:F2} мс, сравнений: {results[1].Comparisons}";
                InsertionSortResultAsync = $"[ASYNC] Вставками: {FormatArray(results[2].SortedArray)}, время: {results[2].ElapsedMilliseconds:F2} мс, сравнений: {results[2].Comparisons}";
                MergeSortResultAsync = $"[ASYNC] Слиянием: {FormatArray(results[3].SortedArray)}, время: {results[3].ElapsedMilliseconds:F2} мс, сравнений: {results[3].Comparisons}";

                TotalComparisonsAsync = $"[ASYNC] Общее число сравнений: {_sorter.TotalComparisons}";

                if (!_isCancelling)
                    TotalExecutionTimeAsync = $"[ASYNC] Общее время выполнения: {watch.Elapsed.TotalMilliseconds:F2} мс";
            }
            catch (OperationCanceledException)
            {
                TotalExecutionTimeAsync = "[ASYNC] Сортировка прервана пользователем";
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                _isSorting = false;
                _isCancelling = false;
                SortAllAsyncCommand.NotifyCanExecuteChanged();
                CancelSortingCommand.NotifyCanExecuteChanged();
            }
        }

        // Удалить старые методы с атрибутами
        private bool CanSortBubbleAsync() => _originalArray != null && !_isSorting && !_isCancelling;
        private bool CanSortQuickAsync() => _originalArray != null && !_isSorting && !_isCancelling;
        private bool CanSortInsertionAsync() => _originalArray != null && !_isSorting && !_isCancelling;
        private bool CanSortMergeAsync() => _originalArray != null && !_isSorting && !_isCancelling;
        private bool CanSortAllAsync() => _originalArray != null && !_isSorting && !_isCancelling;
    }
}
