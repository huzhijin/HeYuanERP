namespace HeYuanERP.Api.DTOs;

// 仓库（Warehouse）相关 DTO：用于列表/详情/新建/编辑

// 列表查询
public class WarehouseListQueryDto
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;
    public string? Keyword { get; set; } // 编码/名称 模糊
    public bool? Active { get; set; }    // 启用状态过滤
}

// 列表项
public class WarehouseListItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; }
    public string? Address { get; set; }
    public string? Contact { get; set; }
    public string? Phone { get; set; }
}

// 详情
public class WarehouseDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; }
    public string? Address { get; set; }
    public string? Contact { get; set; }
    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

// 新建/编辑输入
public class WarehouseCreateDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
    public string? Address { get; set; }
    public string? Contact { get; set; }
    public string? Phone { get; set; }
}

public class WarehouseUpdateDto : WarehouseCreateDto
{
}

