# Fleck_PLC_WebGL

WebSocket 鈫?Siemens PLC 妗ユ帴鏈嶅姟銆?
閫氳繃 WebSocket 灏?WebGL 鍓嶇涓庤タ闂ㄥ瓙 PLC锛圫7 鍗忚锛夎繛鎺ワ紝瀹炵幇瀵?PLC 鏁版嵁鐨勫疄鏃惰鍐欍€?
## 鍔熻兘

- **WebSocket 鏈嶅姟鍣?*锛堝熀浜?Fleck 搴擄級
- **PLC 杩炴帴绠＄悊**锛氳繛鎺?鏂紑瑗块棬瀛?S7 绯诲垪 PLC
- **鏁版嵁璇诲啓**锛氭敮鎸?Bool銆丅yte銆丼hort銆両nt銆丗loat銆丏ouble銆丼tring 绛夌被鍨?- **Watch 鏈哄埗**锛氬悗鍙拌疆璇?PLC 鍦板潃锛屽€煎彉鍖栨椂涓诲姩鎺ㄩ€?- **JSON 鍗忚**锛氬熀浜?JSON-RPC 椋庢牸鐨勮姹?鍝嶅簲

## 閰嶇疆

缂栬緫 `App.config`锛?
```xml
<appSettings>
  <add key="WsIp" value="127.0.0.1" />
  <add key="WsPort" value="8181" />
</appSettings>
```

## 鏀寔 PLC 鍨嬪彿

- S7-1200锛堥粯璁わ級
- S7-1500
- S7-300
- S7-400
- S7-200

## WebSocket 鍗忚

### 璇锋眰鏍煎紡

```json
{
  "method": "鏂规硶鍚?,
  "params": { ... }
}
```

### 鍝嶅簲鏍煎紡

```json
{
  "method": "OnResultName",
  "params": {
    "ok": true,
    "...": "涓氬姟瀛楁"
  }
}
```

### 鏂规硶鍒楄〃

| 鏂规硶 | 鏂瑰悜 | 鍥炴墽 | 璇存槑 |
|------|------|------|------|
| `Ping` | C鈫扴 | `Pong` | 妫€娴嬭繛鎺ョ姸鎬?|
| `PlcStatus` | C鈫扴 | `OnPlcStatus` | 鑾峰彇 PLC 杩炴帴鐘舵€?|
| `PlcConnect` | C鈫扴 | `OnPlcConnectResult` | 杩炴帴鍒?PLC锛堝弬鏁?ip/cpu/rack/slot锛?|
| `PlcDisconnect` | C鈫扴 | `OnPlcDisconnectResult` | 鏂紑 PLC |
| `Read` | C鈫扴 | `OnReadResult` | 璇诲彇 PLC 鍦板潃 |
| `Write` | C鈫扴 | `OnWriteResult` | 鍐欏叆 PLC 鍦板潃锛堥渶 type锛?|
| `StartWatch` | C鈫扴 | `OnWatchStartResult` | 鍚姩鍚庡彴杞锛堝弬鏁?intervalMs/addresses[]锛?|
| `StopWatch` | C鈫扴 | `OnWatchEndResult` | 鍋滄鍚庡彴杞 |
| `OnWatchData` | S鈫扖 |锛堜富鍔ㄦ帹閫侊級| 鍊煎彉鍖栨椂鎺ㄩ€?`{ values: [{address, value}] }` |

### Watch 鏈哄埗

Agent 鍚庡彴绾跨▼鎸?`intervalMs` 闂撮殧杞鎵€鏈夊湴鍧€锛?*棣栨鍏ㄩ噺鎺ㄩ€侊紝鍚庣画浠呮帹閫佸彉鍖栧€?*銆?
### 绫诲瀷鏄犲皠

| Wire | C# 绫诲瀷 |
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

## 宸茬煡闂

### DBD/MD 鍦板潃 REAL 绫诲瀷璇诲彇

S7.Net 鐢?`plc.ReadAsync("DB1.DBD32")` 璇诲彇 REAL 鏃惰繑鍥?`uint`锛堝 `1106247680`锛夛紝鑰岄潪 `float`锛坄30.0`锛夈€?Agent 鍦?`Read` 鍜?`WatchLoop` 涓嚜鍔ㄦ娴?DBD/MD 鍦板潃锛岃皟鐢?`BitConverter.ToSingle` 杞崲銆?
## 鎶€鏈爤

- .NET Framework 4.7.2
- Fleck (WebSocket)
- S7netplus (Siemens PLC)
- Newtonsoft.Json

## 鐗堟湰

- **v0.3** 鈥?DBD/MD float 鑷姩杞崲 + Watch 鍙樺寲鎺ㄩ€