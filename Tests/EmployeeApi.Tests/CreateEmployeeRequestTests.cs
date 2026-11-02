using System.ComponentModel.DataAnnotations;
using EmployeeApi.Contracts;
using Xunit;

namespace EmployeeApi.Tests;

public class CreateEmployeeRequestTests
{
    private static List<ValidationResult> Validate(CreateEmployeeRequest request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
        return results;
    }

    [Theory]
    [InlineData("Name", null)]
    [InlineData("Name", "")]
    [InlineData("Name", "   ")]
    [InlineData("Position", null)]
    [InlineData("Position", "")]
    [InlineData("Position", "   ")]
    public void RequiredText_RejectsMissingOrBlankValues(string field, string? value)
    {
        var request = new CreateEmployeeRequest { Name = "Ana", Position = "QA", Salary = 1m };
        if (field == "Name") request.Name = value!;
        else request.Position = value!;

        Assert.Contains(Validate(request), result => result.MemberNames.Contains(field));
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("Position")]
    public void Text_RejectsMoreThan100Characters(string field)
    {
        var request = new CreateEmployeeRequest { Name = "Ana", Position = "QA", Salary = 1m };
        if (field == "Name") request.Name = new string('a', 101);
        else request.Position = new string('a', 101);

        Assert.Contains(Validate(request), result => result.MemberNames.Contains(field));
    }

    [Fact]
    public void Salary_IsRequired()
    {
        var request = new CreateEmployeeRequest { Name = "Ana", Position = "QA" };

        Assert.Contains(Validate(request), result => result.MemberNames.Contains("Salary"));
    }

    [Fact]
    public void Salary_RejectsNegativeValues()
    {
        var request = new CreateEmployeeRequest { Name = "Ana", Position = "QA", Salary = -0.01m };

        Assert.Contains(Validate(request), result => result.MemberNames.Contains("Salary"));
    }

    [Fact]
    public void ZeroSalaryAnd100CharacterFields_AreAccepted()
    {
        var request = new CreateEmployeeRequest { Name = new string('a', 100), Position = new string('b', 100), Salary = 0m };

        Assert.Empty(Validate(request));
    }

    [Fact]
    public void MaximumDecimalSalary_IsAccepted()
    {
        var request = new CreateEmployeeRequest { Name = "Ana", Position = "QA", Salary = decimal.MaxValue };

        Assert.Empty(Validate(request));
    }
}
