using System;
using System.Configuration;
using System.Threading.Tasks;
using Fleck;
using Newtonsoft.Json.Linq;
using S7.Net;

class Program
{
    const int ConnectTimeoutMs = 3000;
    const int RwTimeoutMs = 2000;

    static IWebSocketConnection client;
    static Plc plc;
    static string plcIp = "";

    public static string WsIp => ConfigurationManager.AppSettings["WsIp"];
    public static int WsPort => int.Parse(ConfigurationManager.AppSettings["WsPort"]);
    private static string configWsIPPort => $"ws://{WsIp}:{WsPort}";
    static void Main()
    {
        FleckLog.Level = LogLevel.Warn;

        new WebSocketServer(configWsIPPort).Start(socket =>
        {
            socket.OnError = ex => Console.WriteLine("[WS] " + ex.Message);
            socket.OnOpen = () =>
            {
                client = socket;
                Console.WriteLine("[+] " + socket.ConnectionInfo.ClientIpAddress);
            };
            socket.OnClose = () =>
            {
                if (client == socket) client = null;
                Console.WriteLine("[-] closed");
            };
            socket.OnMessage = msg => HandleMessage(socket, msg);
        });

        Console.WriteLine($"{configWsIPPort}  connectTimeout={0}ms", ConnectTimeoutMs);
        Console.ReadLine();
        ClosePlc();
    }

    static void HandleMessage(IWebSocketConnection socket, string msg)
    {
        if (socket != client) return;
        Console.WriteLine("[IN] " + msg);

        try
        {
            var root = JObject.Parse(msg);
            var method = (string)root["method"] ?? "";
            var @params = root["params"] as JObject ?? new JObject();

            switch (method)
            {
                case "Ping":
                    Reply(socket, "Pong", true, new { connected = PlcOk(), ip = plcIp });
                    break;

                case "PlcStatus":
                    Reply(socket, "OnPlcStatus", true, new { connected = PlcOk(), ip = plcIp });
                    break;

                case "PlcConnect":
                    ConnectPlc(socket, @params);
                    break;

                case "PlcDisconnect":
                    ClosePlc();
                    Reply(socket, "OnPlcDisconnectResult", true, null);
                    break;

                case "Read":
                    EnsurePlc();
                    {
                        var address = ((string)@params["address"] ?? "").Trim();
                        var value = plc.Read(address);
                        Reply(socket, "OnReadResult", true, new { address, value });
                    }
                    break;

                case "Write":
                    EnsurePlc();
                    {
                        var address = ((string)@params["address"] ?? "").Trim();
                        var type = ((string)@params["type"] ?? "").Trim().ToLowerInvariant();
                        var value = ToClr(@params["value"], type);
                        plc.Write(address, value);
                        Reply(socket, "OnWriteResult", true, new { address, type, value });
                    }
                    break;

                default:
                    Reply(socket, "OnError", false, null, "UNKNOWN_METHOD", method);
                    break;
            }
        }
        catch (Exception ex)
        {
            Reply(socket, "OnError", false, null, "ERROR", ex.Message);
        }
    }

    // -------- PLC --------

    static void ConnectPlc(IWebSocketConnection socket, JObject @params)
    {
        var ip = (string)@params["ip"];
        var cpu = ParseCpu((string)@params["cpu"]);
        var rack = (short)@params["rack"];
        var slot = (short)@params["slot"];

        try
        {
            var opened = OpenPlc(ip, cpu, rack, slot);
            ClosePlc();
            plc = opened;
            plcIp = ip;
            Reply(socket, "OnPlcConnectResult", true, new
            {
                ip,
                cpu = cpu.ToString(),
                rack,
                slot
            });
        }
        catch (TimeoutException ex)
        {
            plcIp = ip ?? "";
            Reply(socket, "OnPlcConnectResult", false, new { ip }, "CONNECT_TIMEOUT", ex.Message);
        }
        catch (Exception ex)
        {
            plcIp = ip ?? "";
            Reply(socket, "OnPlcConnectResult", false, new { ip }, "CONNECT_FAIL", ex.Message);
        }
    }

    static Plc OpenPlc(string ip, CpuType cpu, short rack, short slot)
    {
        var task = Task.Run(() =>
        {
            var plc = new Plc(cpu, ip, rack, slot)
            {
                ReadTimeout = RwTimeoutMs,
                WriteTimeout = RwTimeoutMs
            };
            plc.Open();
            if (!plc.IsConnected)
            {
                try { plc.Close(); } catch { }
                throw new Exception("未建立连接");
            }
            return plc;
        });

        if (!task.Wait(ConnectTimeoutMs))
        {
            // 无法取消 Open：弃用结果，吞掉后续异常
            task.ContinueWith(t =>
            {
                try
                {
                    if (t.IsFaulted)
                    {
                        var _ = t.Exception;
                        return;
                    }
                    if (t.Status == TaskStatus.RanToCompletion && t.Result != null)
                        try { t.Result.Close(); } catch { }
                }
                catch { }
            });
            throw new TimeoutException("连接超时(" + (ConnectTimeoutMs / 1000) + "s): " + ip);
        }

        try
        {
            return task.Result;
        }
        catch (AggregateException ex)
        {
            throw ex.InnerException ?? ex;
        }
    }

    static void ClosePlc()
    {
        try { if (plc != null && plc.IsConnected) plc.Close(); } catch { }
        plc = null;
    }

    static bool PlcOk() => plc != null && plc.IsConnected;

    static void EnsurePlc()
    {
        if (!PlcOk()) throw new Exception("PLC 未连接");
    }

    static object ToClr(JToken value, string type)
    {
        if (value == null || value.Type == JTokenType.Null)
            throw new Exception("value 不能为空");

        switch (type)
        {
            case "bool": return value.Value<bool>();
            case "byte": return value.Value<byte>();
            case "short": return value.Value<short>();
            case "ushort": return value.Value<ushort>();
            case "int": return value.Value<int>();
            case "uint": return value.Value<uint>();
            case "float":
            case "real": return value.Value<float>();
            case "double": return value.Value<double>();
            case "string": return value.Value<string>();
            default: throw new Exception("不支持的 type: " + type);
        }
    }

    static CpuType ParseCpu(string name)
    {
        switch ((name ?? "").Trim().ToUpperInvariant())
        {
            case "S71500": return CpuType.S71500;
            case "S7300": return CpuType.S7300;
            case "S7400": return CpuType.S7400;
            case "S7200": return CpuType.S7200;
            default: return CpuType.S71200;
        }
    }

    // -------- JSON --------

    static void Reply(IWebSocketConnection socket, string method, bool ok, object data,
                      string code = null, string message = null)
    {
        if (socket == null || !socket.IsAvailable) return;

        var body = data == null ? new JObject() : JObject.FromObject(data);
        body["ok"] = ok;
        if (!ok)
        {
            if (code != null) body["code"] = code;
            if (message != null) body["message"] = message;
        }

        var json = new JObject { ["method"] = method, ["params"] = body }
            .ToString(Newtonsoft.Json.Formatting.None);
        socket.Send(json);
        Console.WriteLine("[OUT] " + json);
    }
}
