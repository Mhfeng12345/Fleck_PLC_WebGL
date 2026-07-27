# Fleck_PLC_WebGL

WebSocket ↔ Siemens PLC 桥接服务。

通过 WebSocket 将 WebGL 前端与西门子 PLC（S7 协议）连接，实现对 PLC 数据的实时读写与变化推送。

## 功能

- **WebSocket 服务器**（基于 Fleck 库）
- **PLC 连接管理**：连接/断开西门子 S7 系列 PLC
- **数据读写**：支持 Bool、Byte、Short、Int、Float、Double、String 等类型
- **后台 Watch 轮询**：`StartWatch` 启动后台线程定时轮询，首次全量推送、后续仅值变化时主动推送 `OnWatchData`
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

## WebSocket API

### 请求格式

```json
{
  "method": "方法名",
  "params": { ... }
}
```

### 方法列表

| 方法 | 方向 | 说明 |
|:---|:---|:---|
| `Ping` | C→A | 检测连接状态 |
| `PlcStatus` | C→A | 获取 PLC 连接状态 |
| `PlcConnect` | C→A | 连接到 PLC |
| `PlcDisconnect` | C→A | 断开 PLC |
| `Read` | C→A | 读取 PLC 地址 |
| `Write` | C→A | 写入 PLC 地址（需 type） |
| `StartWatch` | C→A | 启动后台轮询 `{ intervalMs, addresses[] }` |
| `StopWatch` | C→A | 停止后台轮询 |
| `OnWatchData` | A→C | 主动推送：值变化时 `{ values: [{address, value}] }` |

## 技术栈

- .NET Framework 4.7.2
- Fleck (WebSocket)
- S7netplus (Siemens PLC)
- Newtonsoft.Json

## 更新日志

- **v0.2** — 新增 StartWatch/StopWatch：后台 Task.Run 轮询，值变化推送 OnWatchData
- **v0.1** — 基础读写消息桥，WebSocket ↔ S7.Net
