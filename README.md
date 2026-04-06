# 数独游戏 (Sudoku)

基于 Avalonia UI + .NET 8.0 开发的跨平台数独游戏。

## 项目概述

这是一个功能完整的数独游戏应用，支持：

- 难度分级（简单/中等/困难）
- 随机数独生成
- 数字输入（按钮 + 键盘）
- 提示功能
- 检查功能
- 单元格高亮（选中/同行列宫/相同数字）
- 计时器
- 游戏完成检测
- 响应式布局（适配手机）

## 技术栈

| 技术 | 说明 |
|------|------|
| .NET | 8.0 |
| Avalonia UI | 跨平台 UI 框架 |
| CommunityToolkit.Mvvm | MVVM 框架 |

## 项目结构

```
Numbers/
├── Models/
│   └── Cell.cs                    # 单元格数据模型
├── Services/
│   └── SudokuGenerator.cs         # 数独生成器
├── ViewModels/
│   ├── ViewModelBase.cs           # ViewModel 基类
│   └── SudokuViewModel.cs         # 主 ViewModel
├── Views/
│   ├── MainWindow.axaml           # 主窗口
│   ├── MainWindow.axaml.cs
│   ├── SudokuView.axaml           # 数独游戏界面
│   └── SudokuView.axaml.cs
├── Converters/
│   └── CellConverters.cs          # 值转换器
└── App.axaml                       # 应用配置
```

## 核心功能

### 1. 数独生成

使用回溯算法生成完整数独，随机挖空形成谜题：

```csharp
var (puzzle, solution) = SudokuGenerator.Generate(difficulty);
```

### 2. 难度分级

| 难度 | 挖空数量 |
|------|----------|
| 简单 | 35 |
| 中等 | 45 |
| 困难 | 55 |

### 3. 高亮功能

- 选中高亮：当前选中的单元格
- 行列宫高亮：选中单元格所在的行、列、九宫格
- 相同数字高亮：所有与选中单元格数字相同的单元格

### 4. 颜色方案

| 状态 | 颜色 |
|------|------|
| 初始数字 | 深灰 #424242 |
| 正确数字 | 绿色 #4CAF50 |
| 错误数字 | 红色 #D32F2F |
| 选中背景 | 浅绿 #C8E6C9 |
| 同行列宫 | 浅绿 #E8F5E9 |
| 相同数字 | 绿色 #81C784 |

## 运行

### Desktop

```bash
cd Numbers.Desktop
dotnet run
```

## 键盘快捷键

- 1-9: 输入数字
- Backspace/Delete: 清除单元格

## 许可证

MIT License
