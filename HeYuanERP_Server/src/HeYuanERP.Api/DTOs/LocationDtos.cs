namespace HeYuanERP.Api.DTOs;

// 库位（Location）相关 DTO：用于列表/详情/新建/编辑

// 列表查询
public class LocationListQueryDto
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
    public string? WarehouseId { get; set; } // 仓库过滤
    public string? Keyword { get; set; }     // 编码/名称 模糊
    public bool? Active { get; set; }        // 启用状态
}

// 列表项
public class LocationListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string WarehouseId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; }
}

// 详情
public class LocationDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string WarehouseId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

// 新建/编辑输入
public class LocationCreateDto
{
    public string WarehouseId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
}

public class LocationUpdateDto : LocationCreateDto
{
}

