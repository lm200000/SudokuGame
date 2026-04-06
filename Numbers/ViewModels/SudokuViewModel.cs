using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Numbers.Models;
using Numbers.Services;

namespace Numbers.ViewModels;

/// <summary>
/// 数独游戏的主 ViewModel，负责游戏逻辑和数据管理
/// </summary>
public partial class SudokuViewModel : ViewModelBase
{
    /// <summary>
    /// 81个单元格的集合，构成9x9数独棋盘
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Cell> _cells = new();

    /// <summary>
    /// 当前选中的单元格
    /// </summary>
    [ObservableProperty]
    private Cell? _selectedCell;

    /// <summary>
    /// 当前选择的难度级别
    /// </summary>
    [ObservableProperty]
    private Difficulty _selectedDifficulty = Difficulty.Easy;

    /// <summary>
    /// 游戏是否已完成
    /// </summary>
    [ObservableProperty]
    private bool _isGameCompleted;

    /// <summary>
    /// 游戏已用时间
    /// </summary>
    [ObservableProperty]
    private TimeSpan _elapsedTime = TimeSpan.Zero;

    /// <summary>
    /// 计时器是否正在运行
    /// </summary>
    [ObservableProperty]
    private bool _isTimerRunning;

    /// <summary>
    /// 计时器实例
    /// </summary>
    private System.Timers.Timer? _timer;

    /// <summary>
    /// 计时器开始时间
    /// </summary>
    private DateTime _startTime;

    /// <summary>
    /// 难度选项列表（Easy, Medium, Hard）
    /// </summary>
    public IReadOnlyList<Difficulty> Difficulties { get; } = Enum.GetValues<Difficulty>();

    /// <summary>
    /// 构造函数，初始化游戏
    /// </summary>
    public SudokuViewModel()
    {
        InitializeCells();
        NewGameCommand.Execute(null);
    }

    /// <summary>
    /// 初始化81个单元格
    /// </summary>
    private void InitializeCells()
    {
        Cells.Clear();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                Cells.Add(new Cell { Row = row, Col = col });
            }
        }
    }

    /// <summary>
    /// 开始新游戏，生成数独并重置计时器
    /// </summary>
    [RelayCommand]
    private void NewGame()
    {
        // 生成数独谜题和答案
        var (board, solution) = SudokuGenerator.Generate(SelectedDifficulty);
        
        // 初始化81个单元格
        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            var cell = Cells[i];
            int puzzleValue = board[row, col];
            int solutionValue = solution[row, col];
            
            // 设置正确答案（从完整答案中获取）
            cell.CorrectValue = solutionValue;
            // 设置用户可见的值（-1表示空格，渲染为null）
            cell.Value = puzzleValue > 0 ? puzzleValue : null;
            // 空格可编辑，初始数字不可编辑
            cell.IsEditable = puzzleValue <= 0;
            // 重置状态
            cell.IsCorrect = true;
            cell.IsSelected = false;
            cell.IsHighlighted = false;
            cell.IsSameValue = false;
            cell.IsSameRowOrColOrBox = false;
        }
        
        // 清除选中状态
        SelectedCell = null;
        // 重置游戏完成状态
        IsGameCompleted = false;
        
        // 重置并启动计时器
        ResetTimer();
        StartTimer();
    }

    /// <summary>
    /// 选中单元格变化时更新所有单元格的高亮状态
    /// </summary>
    partial void OnSelectedCellChanged(Cell? value)
    {
        foreach (var cell in Cells)
        {
            cell.IsSelected = cell == value;
            cell.UpdateHighlight(value);
        }
    }

    /// <summary>
    /// 选择单元格命令
    /// </summary>
    [RelayCommand]
    private void SelectCell(Cell? cell)
    {
        if (cell != null)
        {
            SelectedCell = cell;
        }
    }

    /// <summary>
    /// 输入数字命令（通过 Command 绑定调用）
    /// </summary>
    [RelayCommand]
    private void InputNumber(string numberStr)
    {
        // 只有选中可编辑的单元格才能输入
        if (SelectedCell == null || !SelectedCell.IsEditable)
            return;

        if (int.TryParse(numberStr, out int number))
        {
            SelectedCell.Value = number;
            
            // 检查答案是否正确
            bool isCorrect = SelectedCell.CorrectValue.HasValue && 
                            SelectedCell.CorrectValue > 0 && 
                            SelectedCell.CorrectValue == number;
            SelectedCell.IsCorrect = isCorrect;
            
            // 检查游戏是否完成
            CheckCompletion();
        }
    }

    /// <summary>
    /// 清除单元格命令
    /// </summary>
    [RelayCommand]
    private void ClearCell()
    {
        if (SelectedCell == null || !SelectedCell.IsEditable)
            return;

        SelectedCell.Value = null;
        SelectedCell.IsCorrect = true;
    }

    /// <summary>
    /// 提示命令，显示选中单元格的正确答案
    /// 如果没有选中或选中单元格已有正确答案，随机选择一个需要填写的单元格
    /// </summary>
    [RelayCommand]
    private void Hint()
    {
        // 如果没有选中可编辑的单元格，或选中单元格为空或错误
        if (SelectedCell == null || !SelectedCell.IsEditable || 
            SelectedCell.Value == null || !SelectedCell.IsCorrect)
        {
            // 找到所有需要填写的单元格（可编辑且为空或错误）
            var targetCells = Cells.Where(c => c.IsEditable && 
                (c.Value == null || !c.IsCorrect)).ToList();
            
            if (targetCells.Count > 0)
            {
                // 随机选择一个（此处代码被注释，可能导致提示不自动选中）
                //SelectedCell = targetCells[Random.Shared.Next(targetCells.Count)];
            }
        }
        
        // 填入正确答案
        if (SelectedCell?.IsEditable == true && SelectedCell.CorrectValue > 0)
        {
            SelectedCell.Value = SelectedCell.CorrectValue;
            SelectedCell.IsCorrect = true;
            CheckCompletion();
        }
    }

    /// <summary>
    /// 检查命令，验证所有用户填写的答案是否正确
    /// </summary>
    [RelayCommand]
    private void Check()
    {
        foreach (var cell in Cells.Where(c => c.IsEditable))
        {
            if (cell.Value.HasValue)
            {
                cell.IsCorrect = cell.Value == cell.CorrectValue;
            }
        }
    }

    /// <summary>
    /// 检查游戏是否完成
    /// 所有可编辑的单元格都正确填写的游戏完成
    /// </summary>
    private void CheckCompletion()
    {
        // 检查所有可编辑单元格是否都已正确填写
        bool completed = Cells.All(c => !c.IsEditable || 
            (c.Value.HasValue && c.CorrectValue.HasValue && c.Value == c.CorrectValue));
        
        if (completed && IsTimerRunning)
        {
            // 游戏完成，停止计时器
            IsGameCompleted = true;
            StopTimer();
        }
        else
        {
            IsGameCompleted = false;
        }
    }

    /// <summary>
    /// 处理键盘输入（用于快捷键）
    /// </summary>
    /// <param name="number">数字（1-9）或特殊值（0表示清除）</param>
    public void HandleKeyInput(int number)
    {
        if (number >= 1 && number <= 9)
        {
            InputNumberDirect(number);
        }
        else if (number == 0 || number == -1)
        {
            ClearCellCommand.Execute(null);
        }
    }

    /// <summary>
    /// 直接输入数字（通过按钮点击调用）
    /// </summary>
    /// <param name="number">要填入的数字</param>
    public void InputNumberDirect(int number)
    {
        if (SelectedCell == null || !SelectedCell.IsEditable)
            return;

        SelectedCell.Value = number;
        
        // 检查答案是否正确
        bool isCorrect = SelectedCell.CorrectValue.HasValue && 
                        SelectedCell.CorrectValue > 0 && 
                        SelectedCell.CorrectValue == number;
        SelectedCell.IsCorrect = isCorrect;
        
        CheckCompletion();
    }

    /// <summary>
    /// 启动计时器
    /// </summary>
    private void StartTimer()
    {
        _timer = new System.Timers.Timer(1000);  // 每秒触发一次
        _timer.Elapsed += OnTimerElapsed;
        _startTime = DateTime.Now;
        _timer.Start();
        IsTimerRunning = true;
    }

    /// <summary>
    /// 停止计时器
    /// </summary>
    private void StopTimer()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        IsTimerRunning = false;
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    private void ResetTimer()
    {
        StopTimer();
        ElapsedTime = TimeSpan.Zero;
    }

    /// <summary>
    /// 计时器触发事件，更新已用时间
    /// </summary>
    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        ElapsedTime = DateTime.Now - _startTime;
    }
}