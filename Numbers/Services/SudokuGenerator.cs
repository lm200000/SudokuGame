using System;
using System.Collections.Generic;

namespace Numbers.Services;

/// <summary>
/// 难度级别枚举
/// </summary>
public enum Difficulty
{
    /// <summary>简单难度，挖掉35个格子</summary>
    Easy,
    /// <summary>中等难度，挖掉45个格子</summary>
    Medium,
    /// <summary>困难难度，挖掉55个格子</summary>
    Hard
}

/// <summary>
/// 数独生成器，使用回溯算法生成完整的数独盘面并随机挖空
/// </summary>
public static class SudokuGenerator
{
    /// <summary>
    /// 生成数独谜题和完整答案
    /// </summary>
    /// <param name="difficulty">难度级别</param>
    /// <returns>元组，包含谜题数组(puzzle)和完整答案数组(solution)</returns>
    public static (int[,] puzzle, int[,] solution) Generate(Difficulty difficulty)
    {
        // 创建谜题和答案数组
        int[,] solution = new int[9, 9];
        int[,] puzzle = new int[9, 9];
        
        // 生成完整数独盘面
        GenerateCompleteBoard(solution);
        
        // 拷贝完整答案到谜题数组
        Array.Copy(solution, puzzle, solution.Length);
        
        // 根据难度挖空单元格
        RemoveCells(puzzle, difficulty);
        
        return (puzzle, solution);
    }

    /// <summary>
    /// 生成完整的数独盘面
    /// </summary>
    /// <param name="board">9x9数独数组</param>
    private static void GenerateCompleteBoard(int[,] board)
    {
        var random = new Random();
        SolveSudoku(board, random);
    }

    /// <summary>
    /// 使用回溯算法填充数独
    /// </summary>
    /// <param name="board">9x9数独数组</param>
    /// <param name="random">随机数生成器</param>
    /// <returns>是否成功填充</returns>
    private static bool SolveSudoku(int[,] board, Random random)
    {
        // 收集所有空单元格位置
        List<(int row, int col)> emptyCells = new();
        
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col] == 0)
                {
                    emptyCells.Add((row, col));
                }
            }
        }

        return FillCells(board, emptyCells, 0, random);
    }

    /// <summary>
    /// 递归填充单元格（回溯算法核心）
    /// </summary>
    /// <param name="board">9x9数独数组</param>
    /// <param name="cells">空单元格位置列表</param>
    /// <param name="index">当前处理到第几个空单元格</param>
    /// <param name="random">随机数生成器</param>
    /// <returns>是否成功填充</returns>
    private static bool FillCells(int[,] board, List<(int row, int col)> cells, int index, Random random)
    {
        // 所有空单元格都已填充完成
        if (index >= cells.Count)
            return true;

        var (row, col) = cells[index];
        // 获取打乱顺序的数字列表
        List<int> nums = GetShuffledNumbers(random);

        // 尝试填充每个数字
        foreach (var num in nums)
        {
            if (IsValid(board, row, col, num))
            {
                board[row, col] = num;
                // 递归处理下一个单元格
                if (FillCells(board, cells, index + 1, random))
                    return true;
                // 回溯：撤销填充
                board[row, col] = 0;
            }
        }

        // 无法找到有效数字
        return false;
    }

    /// <summary>
    /// 获取打乱顺序的1-9数字列表
    /// </summary>
    /// <param name="random">随机数生成器</param>
    /// <returns>打乱后的数字列表</returns>
    private static List<int> GetShuffledNumbers(Random random)
    {
        List<int> nums = [1, 2, 3, 4, 5, 6, 7, 8, 9];
        
        // Fisher-Yates 洗牌算法
        for (int i = nums.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (nums[i], nums[j]) = (nums[j], nums[i]);
        }
        
        return nums;
    }

    /// <summary>
    /// 检查数字 placement 是否合法
    /// </summary>
    /// <param name="board">9x9数独数组</param>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    /// <param name="num">要填入的数字</param>
    /// <returns>是否合法</returns>
    private static bool IsValid(int[,] board, int row, int col, int num)
    {
        // 检查行和列
        for (int i = 0; i < 9; i++)
        {
            if (board[row, i] == num || board[i, col] == num)
                return false;
        }

        // 计算所在九宫格的起始位置
        int boxRow = (row / 3) * 3;
        int boxCol = (col / 3) * 3;
        // 检查九宫格内是否有重复数字
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[boxRow + i, boxCol + j] == num)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 根据难度挖空单元格
    /// </summary>
    /// <param name="board">9x9数独数组</param>
    /// <param name="difficulty">难度级别</param>
    private static void RemoveCells(int[,] board, Difficulty difficulty)
    {
        // 根据难度确定要挖掉的数量
        int cellsToRemove = difficulty switch
        {
            Difficulty.Easy => 35,
            Difficulty.Medium => 45,
            Difficulty.Hard => 55,
            _ => 35
        };

        var random = new Random();
        
        // 收集所有单元格位置
        List<(int row, int col)> cells = new();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                cells.Add((i, j));
            }
        }

        // 随机打乱单元格顺序
        for (int i = cells.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (cells[i], cells[j]) = (cells[j], cells[i]);
        }

        // 挖空指定数量的单元格（设置为-1表示空格）
        int removed = 0;
        for (int i = 0; i < cells.Count && removed < cellsToRemove; i++)
        {
            var (row, col) = cells[i];
            board[row, col] = -1;
            removed++;
        }
    }
}