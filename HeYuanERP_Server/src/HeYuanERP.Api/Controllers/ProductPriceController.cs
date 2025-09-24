using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Services.ProductPrice;
using HeYuanERP.Api.Common;
using HeYuanERP.Api.Services.Authorization;

namespace HeYuanERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductPriceController : ControllerBase
{
    private readonly IProductPriceService _productPriceService;

    public ProductPriceController(IProductPriceService productPriceService)
    {
        _productPriceService = productPriceService;
    }

    // 产品分类管理
    [HttpPost("categories")]
    [RequirePermission("ProductPrice.Category.Create")]
    public async Task<IActionResult> CreateProductCategory([FromBody] ProductCategory category)
    {
        var result = await _productPriceService.CreateProductCategoryAsync(category);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("categories/{id}")]
    [RequirePermission("ProductPrice.Category.Update")]
    public async Task<IActionResult> UpdateProductCategory(int id, [FromBody] ProductCategory category)
    {
        if (id != category.Id)
            return BadRequest("ID不匹配");

        var result = await _productPriceService.UpdateProductCategoryAsync(category);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("categories/{id}")]
    [RequirePermission("ProductPrice.Category.Delete")]
    public async Task<IActionResult> DeleteProductCategory(int id)
    {
        var result = await _productPriceService.DeleteProductCategoryAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("categories/{id}")]
    [RequirePermission("ProductPrice.Category.View")]
    public async Task<IActionResult> GetProductCategoryById(int id)
    {
        var result = await _productPriceService.GetProductCategoryByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("categories")]
    [RequirePermission("ProductPrice.Category.View")]
    public async Task<IActionResult> GetProductCategories()
    {
        var result = await _productPriceService.GetProductCategoriesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("categories/tree")]
    [RequirePermission("ProductPrice.Category.View")]
    public async Task<IActionResult> GetProductCategoryTree()
    {
        var result = await _productPriceService.GetProductCategoryTreeAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("categories/{parentId}/subcategories")]
    [RequirePermission("ProductPrice.Category.View")]
    public async Task<IActionResult> GetSubCategories(int parentId)
    {
        var result = await _productPriceService.GetSubCategoriesAsync(parentId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 产品管理
    [HttpPost("products")]
    [RequirePermission("ProductPrice.Product.Create")]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        var result = await _productPriceService.CreateProductAsync(product);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("products/{id}")]
    [RequirePermission("ProductPrice.Product.Update")]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product product)
    {
        if (id != product.Id)
            return BadRequest("ID不匹配");

        var result = await _productPriceService.UpdateProductAsync(product);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("products/{id}")]
    [RequirePermission("ProductPrice.Product.Delete")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var result = await _productPriceService.DeleteProductAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("products/{id}")]
    [RequirePermission("ProductPrice.Product.View")]
    public async Task<IActionResult> GetProductById(string id)
    {
        var result = await _productPriceService.GetProductByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("products/by-code/{code}")]
    [RequirePermission("ProductPrice.Product.View")]
    public async Task<IActionResult> GetProductByCode(string code)
    {
        var result = await _productPriceService.GetProductByCodeAsync(code);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("products/by-category/{categoryId}")]
    [RequirePermission("ProductPrice.Product.View")]
    public async Task<IActionResult> GetProductsByCategory(int categoryId)
    {
        var result = await _productPriceService.GetProductsByCategoryAsync(categoryId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("products/search")]
    [RequirePermission("ProductPrice.Product.View")]
    public async Task<IActionResult> SearchProducts([FromQuery] string searchTerm)
    {
        var result = await _productPriceService.SearchProductsAsync(searchTerm);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("products")]
    [RequirePermission("ProductPrice.Product.View")]
    public async Task<IActionResult> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var result = await _productPriceService.GetProductsAsync(pageNumber, pageSize);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("products/{id}/stock")]
    [RequirePermission("ProductPrice.Product.UpdateStock")]
    public async Task<IActionResult> UpdateProductStock(string id, [FromBody] decimal newStock)
    {
        var result = await _productPriceService.UpdateProductStockAsync(id, newStock);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("products/low-stock")]
    [RequirePermission("ProductPrice.Product.View")]
    public async Task<IActionResult> GetLowStockProducts()
    {
        var result = await _productPriceService.GetLowStockProductsAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("products/out-of-stock")]
    [RequirePermission("ProductPrice.Product.View")]
    public async Task<IActionResult> GetOutOfStockProducts()
    {
        var result = await _productPriceService.GetOutOfStockProductsAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 价格策略管理
    [HttpPost("strategies")]
    [RequirePermission("ProductPrice.Strategy.Create")]
    public async Task<IActionResult> CreatePriceStrategy([FromBody] PriceStrategy strategy)
    {
        var result = await _productPriceService.CreatePriceStrategyAsync(strategy);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("strategies/{id}")]
    [RequirePermission("ProductPrice.Strategy.Update")]
    public async Task<IActionResult> UpdatePriceStrategy(int id, [FromBody] PriceStrategy strategy)
    {
        if (id != strategy.Id)
            return BadRequest("ID不匹配");

        var result = await _productPriceService.UpdatePriceStrategyAsync(strategy);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("strategies/{id}")]
    [RequirePermission("ProductPrice.Strategy.Delete")]
    public async Task<IActionResult> DeletePriceStrategy(int id)
    {
        var result = await _productPriceService.DeletePriceStrategyAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("strategies/{id}")]
    [RequirePermission("ProductPrice.Strategy.View")]
    public async Task<IActionResult> GetPriceStrategyById(int id)
    {
        var result = await _productPriceService.GetPriceStrategyByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("strategies")]
    [RequirePermission("ProductPrice.Strategy.View")]
    public async Task<IActionResult> GetPriceStrategies()
    {
        var result = await _productPriceService.GetPriceStrategiesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("strategies/active")]
    [RequirePermission("ProductPrice.Strategy.View")]
    public async Task<IActionResult> GetActivePriceStrategies()
    {
        var result = await _productPriceService.GetActivePriceStrategiesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("calculate-price")]
    [RequirePermission("ProductPrice.Price.Calculate")]
    public async Task<IActionResult> CalculatePriceWithStrategy([FromBody] PriceCalculationRequest request)
    {
        var result = await _productPriceService.CalculatePriceWithStrategyAsync(
            request.ProductId, request.CustomerId, request.Quantity, request.StrategyId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("strategies/applicable")]
    [RequirePermission("ProductPrice.Strategy.View")]
    public async Task<IActionResult> GetApplicablePriceStrategies(
        [FromQuery] string productId, [FromQuery] int customerId, [FromQuery] decimal quantity)
    {
        var result = await _productPriceService.GetApplicablePriceStrategiesAsync(productId, customerId, quantity);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 特价申请管理
    [HttpPost("price-requests")]
    [RequirePermission("ProductPrice.PriceRequest.Create")]
    public async Task<IActionResult> CreatePriceRequest([FromBody] PriceRequest request)
    {
        var result = await _productPriceService.CreatePriceRequestAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("price-requests/{id}")]
    [RequirePermission("ProductPrice.PriceRequest.Update")]
    public async Task<IActionResult> UpdatePriceRequest(int id, [FromBody] PriceRequest request)
    {
        if (id != request.Id)
            return BadRequest("ID不匹配");

        var result = await _productPriceService.UpdatePriceRequestAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("price-requests/{id}")]
    [RequirePermission("ProductPrice.PriceRequest.Delete")]
    public async Task<IActionResult> DeletePriceRequest(int id)
    {
        var result = await _productPriceService.DeletePriceRequestAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("price-requests/{id}")]
    [RequirePermission("ProductPrice.PriceRequest.View")]
    public async Task<IActionResult> GetPriceRequestById(int id)
    {
        var result = await _productPriceService.GetPriceRequestByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("price-requests/by-requester/{requesterId}")]
    [RequirePermission("ProductPrice.PriceRequest.View")]
    public async Task<IActionResult> GetPriceRequestsByRequester(int requesterId)
    {
        var result = await _productPriceService.GetPriceRequestsByRequesterAsync(requesterId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("price-requests/by-status/{status}")]
    [RequirePermission("ProductPrice.PriceRequest.View")]
    public async Task<IActionResult> GetPriceRequestsByStatus(string status)
    {
        var result = await _productPriceService.GetPriceRequestsByStatusAsync(status);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("price-requests/pending")]
    [RequirePermission("ProductPrice.PriceRequest.Approve")]
    public async Task<IActionResult> GetPendingPriceRequests()
    {
        var result = await _productPriceService.GetPendingPriceRequestsAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("price-requests/{id}/approve")]
    [RequirePermission("ProductPrice.PriceRequest.Approve")]
    public async Task<IActionResult> ApprovePriceRequest(int id, [FromBody] PriceApprovalRequest approval)
    {
        var result = await _productPriceService.ApprovePriceRequestAsync(id, approval.ApprovedPrice, approval.Comments, approval.ApproverId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("price-requests/{id}/reject")]
    [RequirePermission("ProductPrice.PriceRequest.Approve")]
    public async Task<IActionResult> RejectPriceRequest(int id, [FromBody] PriceRejectionRequest rejection)
    {
        var result = await _productPriceService.RejectPriceRequestAsync(id, rejection.Reason, rejection.ApproverId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 报价管理
    [HttpPost("quotations")]
    [RequirePermission("ProductPrice.Quotation.Create")]
    public async Task<IActionResult> CreateQuotation([FromBody] Quotation quotation)
    {
        var result = await _productPriceService.CreateQuotationAsync(quotation);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("quotations/{id}")]
    [RequirePermission("ProductPrice.Quotation.Update")]
    public async Task<IActionResult> UpdateQuotation(int id, [FromBody] Quotation quotation)
    {
        if (id != quotation.Id)
            return BadRequest("ID不匹配");

        var result = await _productPriceService.UpdateQuotationAsync(quotation);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("quotations/{id}")]
    [RequirePermission("ProductPrice.Quotation.Delete")]
    public async Task<IActionResult> DeleteQuotation(int id)
    {
        var result = await _productPriceService.DeleteQuotationAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("quotations/{id}")]
    [RequirePermission("ProductPrice.Quotation.View")]
    public async Task<IActionResult> GetQuotationById(int id)
    {
        var result = await _productPriceService.GetQuotationByIdAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("quotations/by-number/{quotationNumber}")]
    [RequirePermission("ProductPrice.Quotation.View")]
    public async Task<IActionResult> GetQuotationByNumber(string quotationNumber)
    {
        var result = await _productPriceService.GetQuotationByNumberAsync(quotationNumber);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 高级报表分析：产品销售统计（按报价单）
    [HttpGet("analytics/product-sales")]
    [RequirePermission("ProductPrice.Analytics.View")]
    public async Task<IActionResult> GetProductSalesStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _productPriceService.GetProductSalesStatsAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 高级报表分析：报价单统计（按状态）
    [HttpGet("analytics/quotation-stats")]
    [RequirePermission("ProductPrice.Analytics.View")]
    public async Task<IActionResult> GetQuotationStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _productPriceService.GetQuotationStatsAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // 新增：按状态获取报价单列表（简化版，不分页）
    [HttpGet("quotations/by-status/{status}")]
    [RequirePermission("ProductPrice.Quotation.View")]
    public async Task<IActionResult> GetQuotationsByStatus(string status)
    {
        var result = await _productPriceService.GetQuotationsByStatusAsync(status);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// 请求模型类
public class PriceCalculationRequest
{
    public string ProductId { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public decimal Quantity { get; set; }
    public int? StrategyId { get; set; }
}

public class PriceApprovalRequest
{
    public decimal ApprovedPrice { get; set; }
    public string Comments { get; set; } = string.Empty;
    public int ApproverId { get; set; }
}

public class PriceRejectionRequest
{
    public string Reason { get; set; } = string.Empty;
    public int ApproverId { get; set; }
}
