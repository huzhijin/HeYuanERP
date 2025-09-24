using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HeYuanERP.Application.Common.Models;
using HeYuanERP.Application.DTOs.Payments;
using HeYuanERP.Application.Interfaces;
using HeYuanERP.Domain.Entities;
using HeYuanERP.Application.Interfaces.Repositories;
using HeYuanERP.Application.Interfaces.Storage;
using Microsoft.Extensions.Logging;

namespace HeYuanERP.Application.Services.Payments;

/// <summary>
/// 收款应用服务实现。
/// 说明：当前实现直接依赖基础设施层的仓储与存储接口，后续可通过抽象接口解耦。
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repo;
    private readonly IFileStorage _storage;
    private readonly IValidator<PaymentCreateDto> _validator;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IPaymentRepository repo,
        IFileStorage storage,
        IValidator<PaymentCreateDto> validator,
        ILogger<PaymentService> logger)
    {
        _repo = repo;
        _storage = storage;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<PaymentListItemDto>>> ListAsync(PaymentListParams request, CancellationToken ct = default)
    {
        request ??= new PaymentListParams();
        request.NormalizeAll();

        var sortBy = string.IsNullOrWhiteSpace(request.SortBy) ? "paymentDate" : request.SortBy!.Trim();
        var sortDesc = string.IsNullOrWhiteSpace(request.SortOrder) || request.SortOrder!.Trim().ToLowerInvariant() != "asc";

        var qp = new PaymentQueryParameters
        {
            Method = request.Method,
            MinAmount = request.MinAmount,
            MaxAmount = request.MaxAmount,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            Keyword = request.Keyword
        };

        var (items, total) = await _repo.QueryAsync(qp, request.Page, request.PageSize, sortBy, sortDesc, ct);
        var list = items.Select(MapToListItem).ToList();
        var result = PagedResult<PaymentListItemDto>.Create(list, total, request.Page, request.PageSize);
        return ApiResponse<PagedResult<PaymentListItemDto>>.Ok(result);
    }

    public async Task<ApiResponse<PaymentDetailDto>> CreateAsync(PaymentCreateDto dto, IEnumerable<AttachmentUpload>? attachments, string? createdBy, string? orgId, CancellationToken ct = default)
    {
        var vr = await _validator.ValidateAsync(dto, ct);
        if (!vr.IsValid)
        {
            var msg = string.Join("; ", vr.Errors.Select(e => e.ErrorMessage));
            return ApiResponse<PaymentDetailDto>.Fail(msg);
        }

        var entity = new Payment
        {
            Method = dto.Method.Trim(),
            Amount = Math.Round(dto.Amount, 2, MidpointRounding.AwayFromZero),
            PaymentDate = dto.PaymentDate,
            Remark = string.IsNullOrWhiteSpace(dto.Remark) ? null : dto.Remark.Trim(),
            CreatedBy = string.IsNullOrWhiteSpace(createdBy) ? null : createdBy,
            OrgId = string.IsNullOrWhiteSpace(orgId) ? null : orgId
        };

        if (attachments != null)
        {
            foreach (var att in attachments)
            {
                var storageKey = await _storage.SaveAsync(att.Content, att.FileName, att.ContentType ?? "application/octet-stream", ct);
                entity.Attachments.Add(new PaymentAttachment
                {
                    FileName = att.FileName,
                    ContentType = string.IsNullOrWhiteSpace(att.ContentType) ? null : att.ContentType,
                    Size = SafeLength(att.Content),
                    StorageKey = storageKey,
                });
            }
        }

        var saved = await _repo.AddAsync(entity, ct);
        var detail = MapToDetail(saved);
        return ApiResponse<PaymentDetailDto>.Ok(detail, "创建成功");
    }

    public async Task<ApiResponse<PaymentDetailDto>> GetAsync(Guid id, CancellationToken ct = default)
    {
        var found = await _repo.GetByIdAsync(id, ct);
        if (found == null)
        {
            return ApiResponse<PaymentDetailDto>.Fail("未找到指定收款记录");
        }
        return ApiResponse<PaymentDetailDto>.Ok(MapToDetail(found));
    }

    private static PaymentListItemDto MapToListItem(Payment p)
        => new()
        {
            Id = p.Id,
            Method = p.Method,
            Amount = p.Amount,
            PaymentDate = p.PaymentDate,
            Remark = p.Remark,
            AttachmentCount = p.Attachments?.Count ?? 0
        };

    private static PaymentDetailDto MapToDetail(Payment p)
        => new()
        {
            Id = p.Id,
            Method = p.Method,
            Amount = p.Amount,
            PaymentDate = p.PaymentDate,
            Remark = p.Remark,
            Attachments = (p.Attachments ?? new List<PaymentAttachment>())
                .Select(a => new PaymentAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    ContentType = a.ContentType,
                    Size = a.Size
                }).ToList()
        };

    private static long SafeLength(Stream s)
    {
        try { return s.Length; } catch { return 0; }
    }
}
