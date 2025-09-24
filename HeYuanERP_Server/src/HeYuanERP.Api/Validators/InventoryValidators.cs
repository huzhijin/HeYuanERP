using FluentValidation;
using HeYuanERP.Api.DTOs;

namespace HeYuanERP.Api.Validators;

// 库存与仓库/库位相关校验器

// 库存汇总查询
public class InventorySummaryQueryValidator : AbstractValidator<InventorySummaryQueryDto>
{
    public InventorySummaryQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.Size).InclusiveBetween(1, 200);
        RuleFor(x => x.ProductId).MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.ProductId));
        RuleFor(x => x.Whse).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Whse));
        RuleFor(x => x.Loc).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Loc));
    }
}

// 库存事务查询
public class InventoryTxnQueryValidator : AbstractValidator<InventoryTxnQueryDto>
{
    public InventoryTxnQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.Size).InclusiveBetween(1, 200);
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From).When(x => x.From.HasValue && x.To.HasValue).WithMessage("截止日期需不早于起始日期");
        RuleFor(x => x.ProductId).MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.ProductId));
        RuleFor(x => x.Whse).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Whse));
        RuleFor(x => x.Loc).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Loc));
        RuleFor(x => x.TxnCode).MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.TxnCode));
    }
}

// 仓库
public class WarehouseListQueryValidator : AbstractValidator<WarehouseListQueryDto>
{
    public WarehouseListQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.Size).InclusiveBetween(1, 200);
        RuleFor(x => x.Keyword).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Keyword));
    }
}

public class WarehouseCreateValidator : AbstractValidator<WarehouseCreateDto>
{
    public WarehouseCreateValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Address).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Address));
        RuleFor(x => x.Contact).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Contact));
        RuleFor(x => x.Phone).MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}

public class WarehouseUpdateValidator : AbstractValidator<WarehouseUpdateDto>
{
    public WarehouseUpdateValidator()
    {
        Include(new WarehouseCreateValidator());
    }
}

// 库位
public class LocationListQueryValidator : AbstractValidator<LocationListQueryDto>
{
    public LocationListQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.Size).InclusiveBetween(1, 200);
        RuleFor(x => x.WarehouseId).MaximumLength(64).When(x => !string.IsNullOrWhiteSpace(x.WarehouseId));
        RuleFor(x => x.Keyword).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Keyword));
    }
}

public class LocationCreateValidator : AbstractValidator<LocationCreateDto>
{
    public LocationCreateValidator()
    {
        RuleFor(x => x.WarehouseId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class LocationUpdateValidator : AbstractValidator<LocationUpdateDto>
{
    public LocationUpdateValidator()
    {
        Include(new LocationCreateValidator());
    }
}

