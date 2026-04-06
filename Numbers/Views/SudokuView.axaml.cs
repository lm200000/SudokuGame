using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Numbers.ViewModels;

namespace Numbers.Views;

/// <summary>
/// 数独游戏视图的代码后置类，处理用户交互
/// </summary>
public partial class SudokuView : UserControl
{
    /// <summary>
    /// 构造函数，初始化组件并订阅键盘事件
    /// </summary>
    public SudokuView()
    {
        InitializeComponent();
        
        this.KeyDown += OnKeyDown;
        this.Focusable = true;
    }

    /// <summary>
    /// 处理键盘按键事件，支持数字键输入和删除键清除
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">键盘事件参数</param>
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is SudokuViewModel vm)
        {
            // 处理数字键 1-9
            if (e.Key >= Key.D1 && e.Key <= Key.D9)
            {
                int num = e.Key - Key.D1 + 1;
                vm.HandleKeyInput(num);
                e.Handled = true;
            }
            // 处理删除键和退格键清除
            else if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                vm.HandleKeyInput(0);
                e.Handled = true;
            }
            // 处理 Tab 键聚焦到数独视图
            else if (e.Key == Key.Tab)
            {
                this.Focus();
            }
        }
    }

    /// <summary>
    /// 数字按钮点击事件处理
    /// </summary>
    /// <param name="sender">点击的按钮</param>
    /// <param name="e">路由事件参数</param>
    private void OnNumberButtonClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && int.TryParse(btn.Content?.ToString(), out int num))
        {
            if (DataContext is SudokuViewModel vm)
            {
                vm.InputNumberDirect(num);
            }
        }
    }

    /// <summary>
    /// 清除按钮点击事件处理
    /// </summary>
    /// <param name="sender">点击的按钮</param>
    /// <param name="e">路由事件参数</param>
    private void OnClearButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SudokuViewModel vm)
        {
            vm.ClearCellCommand.Execute(null);
        }
    }
}