using EmployeeApi.Contracts;
using Xunit;

namespace EmployeeApi.Tests;

public class EmployeeStoreTests
{
    [Fact]
    public void GetAll_StartsWithTheTwoExampleEmployees()
    {
        var employees = new EmployeeStore().GetAll();

        Assert.Equal(new[] { 1, 2 }, employees.Select(employee => employee.Id));
        Assert.Equal("Ana Souza", employees[0].Name);
        Assert.Equal(8000m, employees[0].Salary);
        Assert.Equal("Carlos Lima", employees[1].Name);
        Assert.Equal(6500m, employees[1].Salary);
    }

    [Fact]
    public void Create_TrimsFieldsPreservesSalaryAndAssignsAnId()
    {
        var store = new EmployeeStore();
        var employee = store.Create(new CreateEmployeeRequest
        {
            Name = "  Maria  ", Position = "  Designer  ", Salary = 1234.56m
        });

        Assert.Equal(3, employee.Id);
        Assert.Equal("Maria", employee.Name);
        Assert.Equal("Designer", employee.Position);
        Assert.Equal(1234.56m, employee.Salary);
        Assert.Equal(employee, store.GetById(employee.Id));
    }

    [Fact]
    public void GetAll_ReturnsACopyOfTheList()
    {
        var store = new EmployeeStore();
        var snapshot = store.GetAll();
        snapshot.Clear();

        Assert.Equal(2, store.GetAll().Count);
    }

    [Fact]
    public void Delete_RemovesOnlyTheRequestedEmployee()
    {
        var store = new EmployeeStore();

        Assert.True(store.Delete(1));
        Assert.Null(store.GetById(1));
        Assert.Equal(2, Assert.Single(store.GetAll()).Id);
        Assert.False(store.Delete(1));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(999)]
    public void UnknownId_DoesNotChangeTheList(int id)
    {
        var store = new EmployeeStore();

        Assert.Null(store.GetById(id));
        Assert.False(store.Delete(id));
        Assert.Equal(2, store.GetAll().Count);
    }

    [Fact]
    public void Create_DoesNotReuseADeletedId()
    {
        var store = new EmployeeStore();
        var request = new CreateEmployeeRequest { Name = "Maria", Position = "QA", Salary = 0m };
        var first = store.Create(request);
        store.Delete(first.Id);
        var next = store.Create(request);

        Assert.Equal(first.Id + 1, next.Id);
    }
}
