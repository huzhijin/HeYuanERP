using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Common;
using HeYuanERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HeYuanERP.Api.Services.ProductPrice;

public class ProductPriceService : IProductPriceService
{
    private readonly DbContext _context;

    public ProductPriceService(DbContext context)
    {
        _context = context;
    }

    // 产品分类管理
    public async Task<ApiResponse<ProductCategory>> CreateProductCategoryAsync(ProductCategory category)
    {
        try
        {
            _context.Set<ProductCategory>().Add(category);
            await _context.SaveChangesAsync();
            return ApiResponse<ProductCategory>.Ok(category);
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductCategory>.Error($"创建产品分类失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ProductCategory>> UpdateProductCategoryAsync(ProductCategory category)
    {
        try
        {
            category.UpdatedAt = DateTime.UtcNow;
            _context.Set<ProductCategory>().Update(category);
            await _context.SaveChangesAsync();
            return ApiResponse<ProductCategory>.Ok(category);
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductCategory>.Error($"更新产品分类失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteProductCategoryAsync(int id)
    {
        try
        {
            var category = await _context.Set<ProductCategory>().FindAsync(id);
            if (category == null)
                return ApiResponse<bool>.Error("产品分类不存在");

            // 检查是否有关联的产品
            var hasProducts = await _context.Set<Product>().AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
                return ApiResponse<bool>.Error("该分类下还有产品，无法删除");

            // 检查是否有子分类
            var hasSubCategories = await _context.Set<ProductCategory>().AnyAsync(c => c.ParentCategoryId == id);
            if (hasSubCategories)
                return ApiResponse<bool>.Error("该分类下还有子分类，无法删除");

            _context.Set<ProductCategory>().Remove(category);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"删除产品分类失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ProductCategory?>> GetProductCategoryByIdAsync(int id)
    {
        try
        {
            var category = await _context.Set<ProductCategory>()
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            return ApiResponse<ProductCategory?>.Ok(category);
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductCategory?>.Error($"获取产品分类失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ProductCategory>>> GetProductCategoriesAsync()
    {
        try
        {
            var categories = await _context.Set<ProductCategory>()
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.CategoryName)
                .ToListAsync();

            return ApiResponse<List<ProductCategory>>.Ok(categories);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ProductCategory>>.Error($"获取产品分类失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ProductCategory>>> GetProductCategoryTreeAsync()
    {
        try
        {
            var categories = await _context.Set<ProductCategory>()
                .Include(c => c.SubCategories)
                .Where(c => c.ParentCategoryId == null)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            return ApiResponse<List<ProductCategory>>.Ok(categories);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ProductCategory>>.Error($"获取产品分类树失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ProductCategory>>> GetSubCategoriesAsync(int parentId)
    {
        try
        {
            var subCategories = await _context.Set<ProductCategory>()
                .Where(c => c.ParentCategoryId == parentId)
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.CategoryName)
                .ToListAsync();

            return ApiResponse<List<ProductCategory>>.Ok(subCategories);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ProductCategory>>.Error($"获取子分类失败: {ex.Message}");
        }
    }

    // 产品管理
    public async Task<ApiResponse<Product>> CreateProductAsync(Product product)
    {
        try
        {
            // 检查产品编码是否重复
            var existingProduct = await _context.Set<Product>()
                .FirstOrDefaultAsync(p => p.Code == product.Code);
            if (existingProduct != null)
                return ApiResponse<Product>.Error("产品编码已存在");

            _context.Set<Product>().Add(product);
            await _context.SaveChangesAsync();
            return ApiResponse<Product>.Ok(product);
        }
        catch (Exception ex)
        {
            return ApiResponse<Product>.Error($"创建产品失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Product>> UpdateProductAsync(Product product)
    {
        try
        {
            // 检查产品编码是否与其他产品重复
            var existingProduct = await _context.Set<Product>()
                .FirstOrDefaultAsync(p => p.Code == product.Code && p.Id != product.Id);
            if (existingProduct != null)
                return ApiResponse<Product>.Error("产品编码已存在");

            product.UpdatedAt = DateTime.UtcNow;
            _context.Set<Product>().Update(product);
            await _context.SaveChangesAsync();
            return ApiResponse<Product>.Ok(product);
        }
        catch (Exception ex)
        {
            return ApiResponse<Product>.Error($"更新产品失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteProductAsync(string id)
    {
        try
        {
            var product = await _context.Set<Product>().FindAsync(id);
            if (product == null)
                return ApiResponse<bool>.Error("产品不存在");

            // 检查是否有关联的订单或其他业务数据
            // 这里可以添加更多的业务规则检查

            _context.Set<Product>().Remove(product);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"删除产品失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Product?>> GetProductByIdAsync(string id)
    {
        try
        {
            var product = await _context.Set<Product>()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            return ApiResponse<Product?>.Ok(product);
        }
        catch (Exception ex)
        {
            return ApiResponse<Product?>.Error($"获取产品失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Product?>> GetProductByCodeAsync(string code)
    {
        try
        {
            var product = await _context.Set<Product>()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Code == code);

            return ApiResponse<Product?>.Ok(product);
        }
        catch (Exception ex)
        {
            return ApiResponse<Product?>.Error($"获取产品失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<Product>>> GetProductsByCategoryAsync(int categoryId)
    {
        try
        {
            var products = await _context.Set<Product>()
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && p.Active)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return ApiResponse<List<Product>>.Ok(products);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Product>>.Error($"获取分类产品失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<Product>>> SearchProductsAsync(string searchTerm)
    {
        try
        {
            var products = await _context.Set<Product>()
                .Include(p => p.Category)
                .Where(p => p.Active &&
                           (p.Name.Contains(searchTerm) ||
                            p.Code.Contains(searchTerm) ||
                            p.BarCode.Contains(searchTerm) ||
                            p.Keywords.Contains(searchTerm)))
                .OrderBy(p => p.Name)
                .Take(100) // 限制搜索结果数量
                .ToListAsync();

            return ApiResponse<List<Product>>.Ok(products);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Product>>.Error($"搜索产品失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<Product>>> GetProductsAsync(int pageNumber = 1, int pageSize = 50)
    {
        try
        {
            var skip = (pageNumber - 1) * pageSize;
            var products = await _context.Set<Product>()
                .Include(p => p.Category)
                .Where(p => p.Active)
                .OrderBy(p => p.Name)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return ApiResponse<List<Product>>.Ok(products);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Product>>.Error($"获取产品列表失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> UpdateProductStockAsync(string productId, decimal newStock)
    {
        try
        {
            var product = await _context.Set<Product>().FindAsync(productId);
            if (product == null)
                return ApiResponse<bool>.Error("产品不存在");

            product.CurrentStock = newStock;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"更新产品库存失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<Product>>> GetLowStockProductsAsync()
    {
        try
        {
            var products = await _context.Set<Product>()
                .Include(p => p.Category)
                .Where(p => p.Active && p.CurrentStock <= p.SafetyStock)
                .OrderBy(p => p.CurrentStock)
                .ToListAsync();

            return ApiResponse<List<Product>>.Ok(products);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Product>>.Error($"获取低库存产品失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<Product>>> GetOutOfStockProductsAsync()
    {
        try
        {
            var products = await _context.Set<Product>()
                .Include(p => p.Category)
                .Where(p => p.Active && p.CurrentStock <= 0)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return ApiResponse<List<Product>>.Ok(products);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Product>>.Error($"获取缺货产品失败: {ex.Message}");
        }
    }

    // 价格策略管理
    public async Task<ApiResponse<PriceStrategy>> CreatePriceStrategyAsync(PriceStrategy strategy)
    {
        try
        {
            // 检查策略代码是否重复
            var existingStrategy = await _context.Set<PriceStrategy>()
                .FirstOrDefaultAsync(s => s.StrategyCode == strategy.StrategyCode);
            if (existingStrategy != null)
                return ApiResponse<PriceStrategy>.Error("策略代码已存在");

            _context.Set<PriceStrategy>().Add(strategy);
            await _context.SaveChangesAsync();
            return ApiResponse<PriceStrategy>.Ok(strategy);
        }
        catch (Exception ex)
        {
            return ApiResponse<PriceStrategy>.Error($"创建价格策略失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PriceStrategy>> UpdatePriceStrategyAsync(PriceStrategy strategy)
    {
        try
        {
            // 检查策略代码是否与其他策略重复
            var existingStrategy = await _context.Set<PriceStrategy>()
                .FirstOrDefaultAsync(s => s.StrategyCode == strategy.StrategyCode && s.Id != strategy.Id);
            if (existingStrategy != null)
                return ApiResponse<PriceStrategy>.Error("策略代码已存在");

            strategy.UpdatedAt = DateTime.UtcNow;
            _context.Set<PriceStrategy>().Update(strategy);
            await _context.SaveChangesAsync();
            return ApiResponse<PriceStrategy>.Ok(strategy);
        }
        catch (Exception ex)
        {
            return ApiResponse<PriceStrategy>.Error($"更新价格策略失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeletePriceStrategyAsync(int id)
    {
        try
        {
            var strategy = await _context.Set<PriceStrategy>().FindAsync(id);
            if (strategy == null)
                return ApiResponse<bool>.Error("价格策略不存在");

            // 检查是否有关联的价格请求
            var hasRequests = await _context.Set<PriceRequest>().AnyAsync(r => r.PriceStrategyId == id);
            if (hasRequests)
                return ApiResponse<bool>.Error("该策略已被使用，无法删除");

            _context.Set<PriceStrategy>().Remove(strategy);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"删除价格策略失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PriceStrategy?>> GetPriceStrategyByIdAsync(int id)
    {
        try
        {
            var strategy = await _context.Set<PriceStrategy>().FindAsync(id);
            return ApiResponse<PriceStrategy?>.Ok(strategy);
        }
        catch (Exception ex)
        {
            return ApiResponse<PriceStrategy?>.Error($"获取价格策略失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<PriceStrategy>>> GetPriceStrategiesAsync()
    {
        try
        {
            var strategies = await _context.Set<PriceStrategy>()
                .OrderByDescending(s => s.Priority)
                .ThenBy(s => s.StrategyName)
                .ToListAsync();

            return ApiResponse<List<PriceStrategy>>.Ok(strategies);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PriceStrategy>>.Error($"获取价格策略失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<PriceStrategy>>> GetActivePriceStrategiesAsync()
    {
        try
        {
            var now = DateTime.UtcNow;
            var strategies = await _context.Set<PriceStrategy>()
                .Where(s => s.Status == "Active" &&
                           s.EffectiveDate <= now &&
                           (s.ExpiryDate == null || s.ExpiryDate > now))
                .OrderByDescending(s => s.Priority)
                .ToListAsync();

            return ApiResponse<List<PriceStrategy>>.Ok(strategies);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PriceStrategy>>.Error($"获取活跃价格策略失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<decimal>> CalculatePriceWithStrategyAsync(string productId, int customerId, decimal quantity, int? strategyId = null)
    {
        try
        {
            var product = await _context.Set<Product>().FindAsync(productId);
            if (product == null)
                return ApiResponse<decimal>.Error("产品不存在");

            decimal basePrice = product.SalesPrice > 0 ? product.SalesPrice : product.StandardPrice;

            if (strategyId.HasValue)
            {
                var strategy = await _context.Set<PriceStrategy>().FindAsync(strategyId.Value);
                if (strategy != null && strategy.IsActive)
                {
                    basePrice = ApplyPriceStrategy(basePrice, strategy, quantity);
                }
            }
            else
            {
                // 自动寻找最适合的价格策略
                var applicableStrategies = await GetApplicablePriceStrategiesAsync(productId, customerId, quantity);
                if (applicableStrategies.Success && applicableStrategies.Data.Any())
                {
                    var bestStrategy = applicableStrategies.Data.First(); // 已按优先级排序
                    basePrice = ApplyPriceStrategy(basePrice, bestStrategy, quantity);
                }
            }

            return ApiResponse<decimal>.Ok(basePrice);
        }
        catch (Exception ex)
        {
            return ApiResponse<decimal>.Error($"计算价格失败: {ex.Message}");
        }
    }

    private decimal ApplyPriceStrategy(decimal basePrice, PriceStrategy strategy, decimal quantity)
    {
        // 简化的价格策略应用逻辑
        switch (strategy.PriceRule)
        {
            case "Discount":
                return basePrice * (1 - strategy.PriceValue / 100);
            case "Markup":
                return basePrice * (1 + strategy.PriceValue / 100);
            case "Fixed":
                return strategy.PriceValue;
            default:
                return basePrice;
        }
    }

    public async Task<ApiResponse<List<PriceStrategy>>> GetApplicablePriceStrategiesAsync(string productId, int customerId, decimal quantity)
    {
        try
        {
            var now = DateTime.UtcNow;
            var strategies = await _context.Set<PriceStrategy>()
                .Where(s => s.Status == "Active" &&
                           s.EffectiveDate <= now &&
                           (s.ExpiryDate == null || s.ExpiryDate > now) &&
                           s.MinOrderQuantity <= quantity)
                .OrderByDescending(s => s.Priority)
                .ToListAsync();

            // 这里可以添加更复杂的策略筛选逻辑，比如客户类型、产品分类等

            return ApiResponse<List<PriceStrategy>>.Ok(strategies);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PriceStrategy>>.Error($"获取适用价格策略失败: {ex.Message}");
        }
    }

    // 其他方法将在后续实现...
    public async Task<ApiResponse<PriceRequest>> CreatePriceRequestAsync(PriceRequest request)
    {
        try
        {
            // 生成申请编号
            if (string.IsNullOrEmpty(request.RequestCode))
            {
                request.RequestCode = $"PR{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
            }

            _context.Set<PriceRequest>().Add(request);
            await _context.SaveChangesAsync();
            return ApiResponse<PriceRequest>.Ok(request);
        }
        catch (Exception ex)
        {
            return ApiResponse<PriceRequest>.Error($"创建特价申请失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PriceRequest>> UpdatePriceRequestAsync(PriceRequest request)
    {
        try
        {
            request.UpdatedAt = DateTime.UtcNow;
            _context.Set<PriceRequest>().Update(request);
            await _context.SaveChangesAsync();
            return ApiResponse<PriceRequest>.Ok(request);
        }
        catch (Exception ex)
        {
            return ApiResponse<PriceRequest>.Error($"更新特价申请失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeletePriceRequestAsync(int id)
    {
        try
        {
            var request = await _context.Set<PriceRequest>().FindAsync(id);
            if (request == null)
                return ApiResponse<bool>.Error("特价申请不存在");

            if (request.Status == "Approved")
                return ApiResponse<bool>.Error("已批准的申请无法删除");

            _context.Set<PriceRequest>().Remove(request);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"删除特价申请失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PriceRequest?>> GetPriceRequestByIdAsync(int id)
    {
        try
        {
            var request = await _context.Set<PriceRequest>()
                .Include(r => r.PriceStrategy)
                .FirstOrDefaultAsync(r => r.Id == id);

            return ApiResponse<PriceRequest?>.Ok(request);
        }
        catch (Exception ex)
        {
            return ApiResponse<PriceRequest?>.Error($"获取特价申请失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<PriceRequest>>> GetPriceRequestsByRequesterAsync(int requesterId)
    {
        try
        {
            var requests = await _context.Set<PriceRequest>()
                .Where(r => r.RequesterId == requesterId)
                .Include(r => r.PriceStrategy)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return ApiResponse<List<PriceRequest>>.Ok(requests);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PriceRequest>>.Error($"获取申请人特价申请失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<PriceRequest>>> GetPriceRequestsByStatusAsync(string status)
    {
        try
        {
            var requests = await _context.Set<PriceRequest>()
                .Where(r => r.Status == status)
                .Include(r => r.PriceStrategy)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return ApiResponse<List<PriceRequest>>.Ok(requests);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PriceRequest>>.Error($"获取状态特价申请失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<PriceRequest>>> GetPendingPriceRequestsAsync()
    {
        try
        {
            var requests = await _context.Set<PriceRequest>()
                .Where(r => r.Status == "Pending")
                .Include(r => r.PriceStrategy)
                .OrderBy(r => r.RequestDate)
                .ToListAsync();

            return ApiResponse<List<PriceRequest>>.Ok(requests);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PriceRequest>>.Error($"获取待审批特价申请失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PriceRequest>> ApprovePriceRequestAsync(int id, decimal approvedPrice, string comments, int approverId)
    {
        try
        {
            var request = await _context.Set<PriceRequest>().FindAsync(id);
            if (request == null)
                return ApiResponse<PriceRequest>.Error("特价申请不存在");

            if (request.Status != "Pending")
                return ApiResponse<PriceRequest>.Error("只能审批待处理的申请");

            request.Status = "Approved";
            request.ApprovedPrice = approvedPrice;
            request.ApprovalComments = comments;
            request.ApproverId = approverId;
            request.ResponseDate = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ApiResponse<PriceRequest>.Ok(request);
        }
        catch (Exception ex)
        {
            return ApiResponse<PriceRequest>.Error($"审批特价申请失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PriceRequest>> RejectPriceRequestAsync(int id, string reason, int approverId)
    {
        try
        {
            var request = await _context.Set<PriceRequest>().FindAsync(id);
            if (request == null)
                return ApiResponse<PriceRequest>.Error("特价申请不存在");

            if (request.Status != "Pending")
                return ApiResponse<PriceRequest>.Error("只能拒绝待处理的申请");

            request.Status = "Rejected";
            request.RejectionReason = reason;
            request.ApproverId = approverId;
            request.ResponseDate = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ApiResponse<PriceRequest>.Ok(request);
        }
        catch (Exception ex)
        {
            return ApiResponse<PriceRequest>.Error($"拒绝特价申请失败: {ex.Message}");
        }
    }

    // 为了保持文件大小合理，其余方法将在后续实现
    // 这里先提供一些简化的实现

    public async Task<ApiResponse<Quotation>> CreateQuotationAsync(Quotation quotation)
    {
        try
        {
            if (string.IsNullOrEmpty(quotation.QuotationNumber))
            {
                quotation.QuotationNumber = $"QT{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
            }

            _context.Set<Quotation>().Add(quotation);
            await _context.SaveChangesAsync();
            return ApiResponse<Quotation>.Ok(quotation);
        }
        catch (Exception ex)
        {
            return ApiResponse<Quotation>.Error($"创建报价单失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Quotation>> UpdateQuotationAsync(Quotation quotation)
    {
        try
        {
            quotation.UpdatedAt = DateTime.UtcNow;
            _context.Set<Quotation>().Update(quotation);
            await _context.SaveChangesAsync();
            return ApiResponse<Quotation>.Ok(quotation);
        }
        catch (Exception ex)
        {
            return ApiResponse<Quotation>.Error($"更新报价单失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteQuotationAsync(int id)
    {
        try
        {
            var quotation = await _context.Set<Quotation>().FindAsync(id);
            if (quotation == null)
                return ApiResponse<bool>.Error("报价单不存在");

            if (quotation.Status == "Accepted")
                return ApiResponse<bool>.Error("已接受的报价单无法删除");

            _context.Set<Quotation>().Remove(quotation);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"删除报价单失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Quotation?>> GetQuotationByIdAsync(int id)
    {
        try
        {
            var quotation = await _context.Set<Quotation>()
                .Include(q => q.QuotationItems)
                .FirstOrDefaultAsync(q => q.Id == id);

            return ApiResponse<Quotation?>.Ok(quotation);
        }
        catch (Exception ex)
        {
            return ApiResponse<Quotation?>.Error($"获取报价单失败: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Quotation?>> GetQuotationByNumberAsync(string quotationNumber)
    {
        try
        {
            var quotation = await _context.Set<Quotation>()
                .Include(q => q.QuotationItems)
                .FirstOrDefaultAsync(q => q.QuotationNumber == quotationNumber);

            return ApiResponse<Quotation?>.Ok(quotation);
        }
        catch (Exception ex)
        {
            return ApiResponse<Quotation?>.Error($"获取报价单失败: {ex.Message}");
        }
    }

    // 其他接口方法的简化实现，后续可以完善
    public Task<ApiResponse<List<Quotation>>> GetQuotationsByCustomerAsync(int customerId) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<List<Quotation>>> GetQuotationsBySalespersonAsync(int salespersonId) =>
        throw new NotImplementedException("方法待实现");

    public async Task<ApiResponse<List<Quotation>>> GetQuotationsByStatusAsync(string status)
    {
        try
        {
            var list = await _context.Set<Quotation>()
                .AsNoTracking()
                .Where(q => q.Status == status)
                .OrderByDescending(q => q.QuotationDate)
                .ThenByDescending(q => q.UpdatedAt)
                .Take(500)
                .ToListAsync();
            return ApiResponse<List<Quotation>>.Ok(list);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<Quotation>>.Error($"获取报价单列表失败: {ex.Message}");
        }
    }

    public Task<ApiResponse<List<Quotation>>> GetExpiringQuotationsAsync(int days = 7) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<Quotation>> SendQuotationAsync(int id) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<Quotation>> AcceptQuotationAsync(int id) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<Quotation>> RejectQuotationAsync(int id, string reason) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<QuotationItem>> AddQuotationItemAsync(QuotationItem item) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<QuotationItem>> UpdateQuotationItemAsync(QuotationItem item) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<bool>> RemoveQuotationItemAsync(int id) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<List<QuotationItem>>> GetQuotationItemsAsync(int quotationId) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<bool>> RecalculateQuotationTotalsAsync(int quotationId) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<decimal>> CalculateOptimalPriceAsync(string productId, int customerId, decimal quantity) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<object>> GetPriceAnalysisAsync(string productId, DateTime startDate, DateTime endDate) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<object>> GetProfitMarginAnalysisAsync(string productId) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<object>> GetCompetitivePriceAnalysisAsync(string productId) =>
        throw new NotImplementedException("方法待实现");

    public async Task<ApiResponse<object>> GetProductSalesStatsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var fromDate = startDate.Date;
            var toDate = endDate.Date.AddDays(1).AddTicks(-1);

            var query = from q in _context.Set<Quotation>().AsNoTracking()
                        where q.QuotationDate >= fromDate && q.QuotationDate <= toDate
                        join qi in _context.Set<QuotationItem>().AsNoTracking() on q.Id equals qi.QuotationId
                        select new { qi.ProductId, qi.ProductName, qi.Quantity, qi.LineTotal };

            var items = await query
                .GroupBy(x => new { x.ProductId, x.ProductName })
                .Select(g => new
                {
                    productId = g.Key.ProductId,
                    productName = g.Key.ProductName,
                    totalQty = g.Sum(x => x.Quantity),
                    totalAmount = g.Sum(x => x.LineTotal)
                })
                .OrderByDescending(x => x.totalQty)
                .Take(100)
                .ToListAsync();

            var result = new
            {
                @from = fromDate,
                @to = toDate,
                totalQty = items.Sum(i => i.totalQty),
                totalAmount = items.Sum(i => i.totalAmount),
                items
            };

            return ApiResponse<object>.Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.Error($"获取产品销售统计失败: {ex.Message}");
        }
    }

    public Task<ApiResponse<object>> GetPriceRequestStatsAsync(DateTime startDate, DateTime endDate) =>
        throw new NotImplementedException("方法待实现");

    public async Task<ApiResponse<object>> GetQuotationStatsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var from = startDate.Date;
            var to = endDate.Date.AddDays(1).AddTicks(-1);
            var list = await _context.Set<Quotation>().AsNoTracking()
                .Where(q => q.QuotationDate >= from && q.QuotationDate <= to)
                .GroupBy(q => q.Status)
                .Select(g => new { status = g.Key, count = g.Count(), totalAmount = g.Sum(x => x.TotalAmount) })
                .OrderByDescending(x => x.count)
                .ToListAsync();

            var result = new
            {
                from,
                to,
                total = list.Sum(x => x.count),
                amount = list.Sum(x => x.totalAmount),
                byStatus = list
            };
            return ApiResponse<object>.Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.Error($"获取报价单统计失败: {ex.Message}");
        }
    }

    public Task<ApiResponse<object>> GetProfitabilityReportAsync(DateTime startDate, DateTime endDate) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<List<object>>> GetTopSellingProductsAsync(int topN = 10) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<List<object>>> GetTopProfitableProductsAsync(int topN = 10) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<object>> GetInventoryValueReportAsync() =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<List<Product>>> GetReorderRequiredProductsAsync() =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<bool>> UpdateMultipleProductStockAsync(List<(string productId, decimal newStock)> updates) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<List<object>>> GetProductPriceHistoryAsync(string productId) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<object>> GetPriceTrendAnalysisAsync(string productId, int months = 12) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<bool>> BulkUpdateProductPricesAsync(List<(string productId, decimal newPrice)> updates) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<bool>> BulkApplyPriceStrategyAsync(int strategyId, List<string> productIds) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<bool>> ImportProductsAsync(byte[] fileData, string fileName) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<byte[]>> ExportProductsAsync(List<string> productIds) =>
        throw new NotImplementedException("方法待实现");

    public Task<ApiResponse<byte[]>> ExportQuotationAsync(int quotationId, string format = "PDF") =>
        throw new NotImplementedException("方法待实现");
}
