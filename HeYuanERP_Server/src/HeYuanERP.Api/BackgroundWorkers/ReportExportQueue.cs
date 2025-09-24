// 版权所有(c) HeYuanERP
// 说明：报表导出内存队列（Channel 实现，中文注释）。

using System;
using System.Threading.Channels;
using HeYuanERP.Application.Reports;

namespace HeYuanERP.Api.BackgroundWorkers;

/// <summary>
/// 报表导出队列（内存 Channel）。
/// </summary>
public class ReportExportQueue : IReportExportQueue
{
    private readonly Channel<Guid> _channel;

    public ReportExportQueue(ReportExportWorkerOptions options)
    {
        var capacity = options?.QueueCapacity > 0 ? options.QueueCapacity : 200;
        var chOptions = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        };
        _channel = Channel.CreateBounded<Guid>(chOptions);
    }

    /// <summary>Writer 端用于入队。</summary>
    public void Enqueue(Guid jobId)
    {
        // 不阻塞当前线程，交给 Channel 自行调度
        _channel.Writer.TryWrite(jobId);
    }

    /// <summary>Reader 端用于后台 worker 消费。</summary>
    public ChannelReader<Guid> Reader => _channel.Reader;
}

