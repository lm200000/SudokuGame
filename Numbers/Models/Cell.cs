using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Numbers.Models;

/// <summary>
/// 表示数独棋盘上的单个单元格
/// </summary>
public class Cell : INotifyPropertyChanged
{
    // 用户填入的数字，null 表示空格
    private int? _value;
    // 是否可编辑，初始数字不可编辑
    private bool _isEditable = true;
    // 当前值是否正确
    private bool _isCorrect = true;
    // 是否被选中
    private bool _isSelected;
    // 是否高亮显示（当前选中的单元格）
    private bool _isHighlighted;
    // 是否与选中单元格值相同（用于高亮相同数字）
    private bool _isSameValue;
    // 是否与选中单元格同行/列/宫（用于高亮）
    private bool _isSameRowOrColOrBox;
    // 正确的答案数字
    private int? _correctValue;

    /// <summary>
    /// 用户填入的数字，null 表示空格
    /// </summary>
    public int? Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 是否可编辑，初始数字不可编辑
    /// </summary>
    public bool IsEditable
    {
        get => _isEditable;
        set
        {
            if (_isEditable != value)
            {
                _isEditable = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 当前值是否正确
    /// </summary>
    public bool IsCorrect
    {
        get => _isCorrect;
        set
        {
            if (_isCorrect != value)
            {
                _isCorrect = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 是否被选中
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 是否高亮显示（当前选中的单元格）
    /// </summary>
    public bool IsHighlighted
    {
        get => _isHighlighted;
        set
        {
            if (_isHighlighted != value)
            {
                _isHighlighted = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 是否与选中单元格值相同（用于高亮相同数字）
    /// </summary>
    public bool IsSameValue
    {
        get => _isSameValue;
        set
        {
            if (_isSameValue != value)
            {
                _isSameValue = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 是否与选中单元格同行/列/宫（用于高亮）
    /// </summary>
    public bool IsSameRowOrColOrBox
    {
        get => _isSameRowOrColOrBox;
        set
        {
            if (_isSameRowOrColOrBox != value)
            {
                _isSameRowOrColOrBox = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 正确的答案数字
    /// </summary>
    public int? CorrectValue
    {
        get => _correctValue;
        set
        {
            if (_correctValue != value)
            {
                _correctValue = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 单元格所在行（0-8）
    /// </summary>
    public int Row { get; init; }

    /// <summary>
    /// 单元格所在列（0-8）
    /// </summary>
    public int Col { get; init; }

    /// <summary>
    /// 所属九宫格编号（0-8）
    /// 计算公式：(Row / 3) * 3 + (Col / 3)
    /// </summary>
    public int Box => (Row / 3) * 3 + (Col / 3);

    /// <summary>
    /// 是否为九宫格右边界（用于边框渲染）
    /// </summary>
    public bool IsBoxRightBorder => (Col + 1) % 3 == 0 && Col < 8;

    /// <summary>
    /// 是否为九宫格下边界（用于边框渲染）
    /// </summary>
    public bool IsBoxBottomBorder => (Row + 1) % 3 == 0 && Row < 8;

    /// <summary>
    /// 重置单元格状态为初始值
    /// </summary>
    public void Reset()
    {
        Value = null;
        IsEditable = true;
        IsCorrect = true;
        IsSelected = false;
        IsHighlighted = false;
        IsSameValue = false;
        IsSameRowOrColOrBox = false;
    }

    /// <summary>
    /// 更新高亮状态，根据选中的单元格计算
    /// </summary>
    /// <param name="selectedCell">当前选中的单元格</param>
    public void UpdateHighlight(Cell? selectedCell)
    {
        // 如果没有选中任何单元格，清除所有高亮
        if (selectedCell == null)
        {
            IsHighlighted = false;
            IsSameValue = false;
            IsSameRowOrColOrBox = false;
            return;
        }

        // 如果是当前选中的单元格，设置高亮状态
        if (selectedCell == this)
        {
            IsHighlighted = true;
            IsSameValue = false;
            IsSameRowOrColOrBox = false;
            return;
        }

        // 清除自身高亮
        IsHighlighted = false;
        
        // 判断是否与选中单元格同行/列/宫
        bool sameRow = Row == selectedCell.Row;
        bool sameCol = Col == selectedCell.Col;
        bool sameBox = Box == selectedCell.Box;
        
        IsSameRowOrColOrBox = sameRow || sameCol || sameBox;
        
        // 判断是否与选中单元格值相同
        bool sameValue = Value.HasValue && selectedCell.Value.HasValue && Value == selectedCell.Value;
        IsSameValue = sameValue;
    }

    /// <summary>
    /// 属性变更事件，用于通知 UI 更新
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 触发属性变更事件
    /// </summary>
    /// <param name="propertyName">发生变更的属性名称</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}