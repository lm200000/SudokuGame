using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Numbers.Converters;

/// <summary>
/// 单元格背景颜色转换器
/// 根据单元格的选中状态和高亮状态返回对应的背景颜色
/// </summary>
public class CellBackgroundConverter : IMultiValueConverter
{
    public static readonly CellBackgroundConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        // 参数顺序：IsSelected, IsSameValue, IsSameRowOrColOrBox, IsHighlighted
        if (values.Count < 4) return Brushes.Transparent;

        bool isSelected = values[0] is bool s && s;
        bool isSameValue = values[1] is bool v && v;
        bool isSameRowOrColOrBox = values[2] is bool r && r;
        bool isHighlighted = values[3] is bool h && h;

        // 优先级：选中 > 相同数字 > 同行列宫 > 无高亮
        if (isSelected)
            return new SolidColorBrush(Color.Parse("#C8E6C9"));  // 浅绿色
        if (isSameValue)
            return new SolidColorBrush(Color.Parse("#81C784"));    // 绿色
        if (isSameRowOrColOrBox)
            return new SolidColorBrush(Color.Parse("#E8F5E9"));  // 淡绿色
        
        return Brushes.Transparent;  // 透明
    }
}

/// <summary>
/// 单元格文字颜色转换器
/// 根据单元格的编辑状态和正确性返回对应的文字颜色
/// </summary>
public class CellTextColorConverter : IMultiValueConverter
{
    public static readonly CellTextColorConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        // 参数顺序：IsEditable, IsCorrect
        if (values.Count < 2) return Brushes.Black;

        bool isEditable = values[0] is bool e && e;
        bool isCorrect = values[1] is bool c && c;

        // 优先级：初始数字 > 错误 > 正确
        if (!isEditable)
            return new SolidColorBrush(Color.Parse("#424242"));  // 深灰色（初始数字）
        
        if (!isCorrect)
            return new SolidColorBrush(Color.Parse("#D32F2F"));  // 红色（错误）

        return new SolidColorBrush(Color.Parse("#4CAF50"));       // 绿色（正确）
    }
}

/// <summary>
/// 单元格边框粗细转换器
/// 根据单元格是否为九宫格边界返回不同的边框粗细
/// </summary>
public class CellBorderThicknessConverter : IMultiValueConverter
{
    public static readonly CellBorderThicknessConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        // 参数顺序：IsBoxRightBorder, IsBoxBottomBorder
        if (values.Count < 2) return new Thickness(1);

        bool isBoxRightBorder = values[0] is bool r && r;
        bool isBoxBottomBorder = values[1] is bool b && b;

        // 普通边框1px，九宫格边界3px
        double left = 1;
        double top = 1;
        double right = isBoxRightBorder ? 3 : 1;
        double bottom = isBoxBottomBorder ? 3 : 1;

        return new Thickness(left, top, right, bottom);
    }
}