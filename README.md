# Fleck_PLC_WebGL

WebSocket ↔ Siemens PLC 桥接服务。

通过 WebSocket 将 WebGL 前端与西门子 PLC（S7 协议）连接，实现对 PLC 数据的实时读写。

## 功能

- **WebSocket 服务器**（基于 Fleck 库）
- **PLC 连接管理**：连接/断开西门子 S7 系列 PLC
- **数据读写**：支持 Bool、Byte、Short、Int、Float、Double、String 等类型
- **Watch 机制**：后台轮询 PLC 地址，值变化时主动推送
- **JSON 协议**：基于 JSON-RPC 风格的请求/响应

## 配置

编辑 `App.config`：

```xml
<appSettings>
  <add key="WsIp" value="127.0.0.1" />
  <add key="WsPort" value="8181" />
</appSettings>
```

## 支持 PLC 型号

- S7-1200（默认）
- S7-1500
- S7-300
- S7-400
- S7-200

## WebSocket 协议

### 请求格式

```json
{
  "method": "方法名",
  "params": { ... }
}
```

### 响应格式

```json
{
  "method": "OnResultName",
  "params": {
    "ok": true,
    "...": "业务字段"
  }
}
```

### 方法列表

| 方法 | 方向 | 回执 | 说明 |
|------|------|------|------|
| `Ping` | C→S | `Pong` | 检测连接状态 |
| `PlcStatus` | C→S | `OnPlcStatus` | 获取 PLC 连接状态 |
| `PlcConnect` | C→S | `OnPlcConnectResult` | 连接到 PLC（参数 ip/cpu/rack/slot） |
| `PlcDisconnect` | C→S | `OnPlcDisconnectResult` | 断开 PLC |
| `Read` | C→S | `OnReadResult` | 读取 PLC 地址 |
| `Write` | C→S | `OnWriteResult` | 写入 PLC 地址（需 type） |
| `StartWatch` | C→S | `OnWatchStartResult` | 启动后台轮询（参数 intervalMs/addresses[]） |
| `StopWatch` | C→S | `OnWatchEndResult` | 停止后台轮询 |
| `OnWatchData` | S→C |（主动推送）| 值变化时推送 `{ values: [{address, value}] }` |

### Watch 机制

Agent 后台线程按 `intervalMs` 间隔轮询所有地址，**首次全量推送，后续仅推送变化值**。

### 类型映射

| Wire | C# 类型 |
|------|---------|
| `bool` | `bool` |
| `byte` | `byte` |
| `short` | `short` |
| `ushort` | `ushort` |
| `int` | `int` |
| `uint` | `uint` |
| `float` / `real` | `float` |
| `double` | `double` |
| `string` | `string` |

## 已知问题

### DBD/MD 地址 REAL 类型读取

S7.Net 用 `plc.ReadAsync("DB1.DBD32")` 读取 REAL 时返回 `uint`（如 `1106247680`），而非 `float`（`30.0`）。
Agent 在 `Read` 和 `WatchLoop` 中自动检测 DBD/MD 地址，调用 `BitConverter.ToSingle` 转换。

## 技术栈

- .NET Framework 4.7.2
- Fleck (WebSocket)
- S7netplus (Siemens PLC)
- Newtonsoft.Json

## 版本

- **v0.3** — DBD/MD float 自动转换 + Watch 变化推送