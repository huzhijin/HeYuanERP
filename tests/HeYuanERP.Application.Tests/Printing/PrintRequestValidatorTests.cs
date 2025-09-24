using HeYuanERP.Application.Printing;
using HeYuanERP.Application.Printing.Validation;
using Xunit;

namespace HeYuanERP.Application.Tests.Printing;

/// <summary>
/// PrintRequestValidator 单元测试：覆盖合法/非法场景。
/// </summary>
public class PrintRequestValidatorTests
{
    [Theory]
    [InlineData("order")]
    [InlineData("delivery")]
    [InlineData("return")]
    [InlineData("invoice")]
    [InlineData("statement")]
    public void Should_Pass_For_KnownDocTypes(string docType)
    {
        var v = new PrintRequestValidator();
        var r = v.Validate(new PrintRequest { DocType = docType, Id = "1", Template = "default" });
        Assert.True(r.IsValid, string.Join("; ", r.Errors));
    }

    [Fact]
    public void Should_Fail_For_EmptyId()
    {
        var v = new PrintRequestValidator();
        var r = v.Validate(new PrintRequest { DocType = "order", Id = "", Template = "default" });
        Assert.False(r.IsValid);
    }

    [Theory]
    [InlineData("Default")] // 大写不允许
    [InlineData("bad name")] // 空格不允许
    [InlineData("../hack")]  // 特殊字符不允许
    public void Should_Fail_For_InvalidTemplate(string template)
    {
        var v = new PrintRequestValidator();
        var r = v.Validate(new PrintRequest { DocType = "order", Id = "1", Template = template });
        Assert.False(r.IsValid);
    }
}

