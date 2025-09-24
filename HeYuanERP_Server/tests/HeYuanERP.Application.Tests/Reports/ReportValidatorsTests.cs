// 版权所有(c) HeYuanERP
// 说明：报表参数 FluentValidation 测试（xUnit，中文注释）。

using System;
using System.Collections.Generic;
using FluentValidation.TestHelper;
using HeYuanERP.Application.Reports.Validation;
using HeYuanERP.Contracts.Common;
using HeYuanERP.Contracts.Reports;
using Xunit;

namespace HeYuanERP.Application.Tests.Reports;

public class ReportValidatorsTests
{
    [Fact]
    public void SalesStatParamsValidator_Should_Pass_When_Group_Valid_And_Range_Valid()
    {
        var validator = new SalesStatParamsValidator();
        var dto = new SalesStatParamsDto
        {
            GroupBy = "day",
            Range = new DateRange { StartUtc = DateTimeOffset.UtcNow.AddDays(-7), EndUtc = DateTimeOffset.UtcNow }
        };
        var result = validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SalesStatParamsValidator_Should_Fail_When_Group_Invalid()
    {
        var validator = new SalesStatParamsValidator();
        var dto = new SalesStatParamsDto { GroupBy = "bad" };
        var result = validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.GroupBy);
    }

    [Fact]
    public void InvoiceStatParamsValidator_Should_Fail_When_Range_Invalid()
    {
        var validator = new InvoiceStatParamsValidator();
        var dto = new InvoiceStatParamsDto
        {
            Range = new DateRange { StartUtc = DateTimeOffset.UtcNow, EndUtc = DateTimeOffset.UtcNow.AddDays(-1) }
        };
        var result = validator.TestValidate(dto);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void POQueryParamsValidator_Should_Validate_Paging()
    {
        var validator = new POQueryParamsValidator();
        var dtoBad = new POQueryParamsDto { Page = 0, Size = 999 };
        var bad = validator.TestValidate(dtoBad);
        bad.ShouldHaveValidationErrorFor(x => x.Page);
        bad.ShouldHaveValidationErrorFor(x => x.Size);

        var dtoGood = new POQueryParamsDto { Page = 1, Size = 50 };
        var ok = validator.TestValidate(dtoGood);
        ok.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InventoryParamsValidator_Should_Validate_Paging_And_Length()
    {
        var validator = new InventoryParamsValidator();
        var dto = new InventoryQueryParamsDto { Page = 1, Size = 20, ProductId = new string('A', 10) };
        var ok = validator.TestValidate(dto);
        ok.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ReportExportRequestValidator_Should_Validate_Format_And_Params()
    {
        var validator = new ReportExportRequestValidator();
        var good = new ReportExportRequestDto { Format = "pdf", Params = new Dictionary<string, object?> { ["k"] = 1 } };
        validator.TestValidate(good).ShouldNotHaveAnyValidationErrors();

        var bad = new ReportExportRequestDto { Format = "xls", Params = new Dictionary<string, object?> { ["k"] = 1 } };
        validator.TestValidate(bad).ShouldHaveValidationErrorFor(x => x.Format);
    }
}

