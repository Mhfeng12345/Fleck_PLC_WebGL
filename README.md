# Fleck_PLC_WebGL

WebSocket 鈫?Siemens PLC 妗ユ帴鏈嶅姟銆?
閫氳繃 WebSocket 灏?WebGL 鍓嶇涓庤タ闂ㄥ瓙 PLC锛圫7 鍗忚锛夎繛鎺ワ紝瀹炵幇瀵?PLC 鏁版嵁鐨勫疄鏃惰鍐欎笌鍙樺寲鎺ㄩ€併€?
## 鍔熻兘

- **WebSocket 鏈嶅姟鍣?*锛堝熀浜?Fleck 搴擄級
- **PLC 杩炴帴绠＄悊**锛氳繛鎺?鏂紑瑗块棬瀛?S7 绯诲垪 PLC
- **鏁版嵁璇诲啓**锛氭敮鎸?Bool銆丅yte銆丼hort銆両nt銆丗loat銆丏ouble銆丼tring 绛夌被鍨?- **鍚庡彴 Watch 杞**锛歚StartWatch` 鍚姩鍚庡彴绾跨▼瀹氭椂杞锛岄娆″叏閲忔帹閫併€佸悗缁粎鍊煎彉鍖栨椂涓诲姩鎺ㄩ€?`OnWatchData`
- **JSON 鍗忚**锛氬熀浜?JSON-RPC 椋庢牸鐨勮姹?鍝嶅簲

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

## WebSocket API

### 璇锋眰鏍煎紡

```json
{
  "method": "鏂规硶鍚?,
  "params": { ... }
}
```

### 鏂规硶鍒楄〃

| 鏂规硶 | 鏂瑰悜 | 璇存槑 |
|:---|:---|:---|
| `Ping` | C鈫扐 | 妫€娴嬭繛鎺ョ姸鎬?|
| `PlcStatus` | C鈫扐 | 鑾峰彇 PLC 杩炴帴鐘舵€?|
| `PlcConnect` | C鈫扐 | 杩炴帴鍒?PLC |
| `PlcDisconnect` | C鈫扐 | 鏂紑 PLC |
| `Read` | C鈫扐 | 璇诲彇 PLC 鍦板潃 |
| `Write` | C鈫扐 | 鍐欏叆 PLC 鍦板潃锛堥渶 type锛?|
| `StartWatch` | C鈫扐 | 鍚姩鍚庡彴杞 `{ intervalMs, addresses[] }` |
| `StopWatch` | C鈫扐 | 鍋滄鍚庡彴杞 |
| `OnWatchData` | A鈫扖 | 涓诲姩鎺ㄩ€侊細鍊煎彉鍖栨椂 `{ values: [{address, value}] }` |

## 鎶€鏈爤

- .NET Framework 4.7.2
- Fleck (WebSocket)
- S7netplus (Siemens PLC)
- Newtonsoft.Json

## 鏇存柊鏃ュ織

- **v0.2** 鈥?鏂板 StartWatch/StopWatch锛氬悗鍙?Task.Run 杞锛屽€煎彉鍖栨帹閫?OnWatchData
- **v0.1** 鈥?鍩虹璇诲啓娑堟伅妗ワ紝WebSocket 鈫?S7.Net
