using System;
using System.Collections.Generic;
using HeYuanERP.Infrastructure.Logging.Audit;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace HeYuanERP_Server.Tests.Infrastructure
{
    /// <summary>
    /// 审计日志实现（Serilog）基础测试。
    /// </summary>
    public class AuditLoggerTests
    {
        private sealed class ListSink : ILogEventSink
        {
            public List<LogEvent> Events { get; } = new();
            public void Emit(LogEvent logEvent) => Events.Add(logEvent);
        }

        [Fact(DisplayName = "记录外部调用与替换说明字段")]
        public void Logs_ExternalCall_And_Replacement()
        {
            var sink = new ListSink();
            var logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Sink(sink).CreateLogger();
            var audit = new SerilogAuditLogger(logger);

            audit.LogExternalCall("OA", "SSO.Login", "mock://oa/sso", 200, TimeSpan.FromMilliseconds(12), true, "req", "resp", "trace-1");
            audit.LogReplacementNotice("IOaClient", "OaClientMock", "OA", "mock for dev");

            Assert.True(sink.Events.Count >= 2);

            var call = sink.Events[0];
            Assert.True(call.Properties.ContainsKey("event"));
            Assert.True(call.Properties.ContainsKey("system"));
            Assert.True(call.Properties.ContainsKey("action"));
            Assert.True(call.Properties.ContainsKey("url"));
            Assert.True(call.Properties.ContainsKey("duration_ms"));
            Assert.True(call.Properties.ContainsKey("success"));

            var repl = sink.Events[1];
            Assert.True(repl.Properties.ContainsKey("event"));
            Assert.True(repl.Properties.ContainsKey("component"));
            Assert.True(repl.Properties.ContainsKey("impl"));
        }
    }
}

