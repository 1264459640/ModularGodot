# GodotGameLogger API 文档

## `GodotGameLogger` 类

`GodotGameLogger` 是一个用于在 Godot 引擎中进行日志记录的类。它提供了不同级别的日志方法，包括调试、信息、警告、错误和致命错误。

### 构造函数

#### `GodotGameLogger(string name)`

创建一个新的 `GodotGameLogger` 实例。

- `name`: 日志记录器的名称。

### 方法

#### `void Debug(string message)`

记录一条调试消息。

- `message`: 要记录的调试消息。

#### `void Info(string message)`

记录一条信息消息。

- `message`: 要记录的信息消息。

#### `void Warn(string message)`

记录一条警告消息。

- `message`: 要记录的警告消息。

#### `void Error(string message)`

记录一条错误消息。

- `message`: 要记录的错误消息。

#### `void Fatal(string message)`

记录一条致命错误消息。

- `message`: 要记录的致命错误消息。

## `GodotGameLogger<T>` 泛型类

`GodotGameLogger<T>` 是 `GodotGameLogger` 的泛型版本，它会自动使用类型 `T` 的名称作为日志记录器的名称。

### 构造函数

#### `GodotGameLogger()`

创建一个新的 `GodotGameLogger<T>` 实例，并使用类型 `T` 的名称作为日志记录器的名称。

## `GodotLoggerFactory` 类

`GodotLoggerFactory` 是一个用于创建 `GodotGameLogger` 实例的工厂类。

### 方法

#### `GodotGameLogger GetLogger(string name)`

获取一个指定名称的 `GodotGameLogger` 实例。如果该名称的日志记录器已存在，则返回现有实例；否则，创建一个新的实例。

- `name`: 日志记录器的名称。

#### `GodotGameLogger<T> GetLogger<T>()`

获取一个泛型 `GodotGameLogger<T>` 实例。如果该类型的日志记录器已存在，则返回现有实例；否则，创建一个新的实例。