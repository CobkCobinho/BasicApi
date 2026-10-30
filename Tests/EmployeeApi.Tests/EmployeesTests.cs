using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace EmployeeApi.Tests;

public class EmployeesTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;

    public EmployeesTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    public void Dispose() => _client.Dispose();

    [Fact]
    public async Task Create_AssignsIdAndReturnsReadableLocation()
    {
        using var response = await _client.PostAsJsonAsync("/employees", new
        {
            id = -123,
            name = "  Maria Silva  ",
            position = "  Designer  ",
            salary = 7000m
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var employee = await response.Content.ReadFromJsonAsync<Employee>();
        Assert.NotNull(employee);
        Assert.True(employee.Id > 0);
        Assert.Equal("Maria Silva", employee.Name);
        Assert.Equal("Designer", employee.Position);
        Assert.Equal(7000m, employee.Salary);
        Assert.NotNull(response.Headers.Location);

        var fetched = await _client.GetFromJsonAsync<Employee>(response.Headers.Location);
        Assert.NotNull(fetched);
        Assert.Equal(employee.Id, fetched.Id);
        Assert.Equal(employee.Name, fetched.Name);
    }

    [Theory]
    [InlineData("{}", "Name")]
    [InlineData("{\"name\":null,\"position\":\"QA\",\"salary\":1}", "Name")]
    [InlineData("{\"name\":\"   \",\"position\":\"QA\",\"salary\":1}", "Name")]
    [InlineData("{\"name\":\"Ana\",\"position\":\"  \",\"salary\":1}", "Position")]
    [InlineData("{\"name\":\"Ana\",\"position\":\"QA\"}", "Salary")]
    [InlineData("{\"name\":\"Ana\",\"position\":\"QA\",\"salary\":null}", "Salary")]
    [InlineData("{\"name\":\"Ana\",\"position\":\"QA\",\"salary\":-0.01}", "Salary")]
    public async Task Create_InvalidFieldsReturnValidationErrors(string json, string field)
    {
        using var body = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync("/employees", body);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var problem = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(problem.RootElement.GetProperty("errors").TryGetProperty(field, out _));
    }

    [Theory]
    [InlineData("name")]
    [InlineData("position")]
    public async Task Create_RejectsFieldsOver100Characters(string field)
    {
        var payload = new Dictionary<string, object>
        {
            ["name"] = "Ana", ["position"] = "QA", ["salary"] = 100m
        };
        payload[field] = new string('a', 101);
        using var response = await _client.PostAsJsonAsync("/employees", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_AcceptsBoundaryValues()
    {
        using var response = await _client.PostAsJsonAsync("/employees", new
        {
            name = new string('a', 100), position = new string('b', 100), salary = 0m
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesOnlyTheRequestedEmployee()
    {
        using var created = await _client.PostAsJsonAsync("/employees", new
        {
            name = "Delete test", position = "QA", salary = 1m
        });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        using var unrelated = await _client.PostAsJsonAsync("/employees", new
        {
            name = "Keep this employee", position = "QA", salary = 1m
        });
        Assert.Equal(HttpStatusCode.Created, unrelated.StatusCode);
        var location = created.Headers.Location;
        using var removed = await _client.DeleteAsync(location);
        using var missing = await _client.GetAsync(location);
        using var removedAgain = await _client.DeleteAsync(location);
        Assert.Equal(HttpStatusCode.NoContent, removed.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, removedAgain.StatusCode);
        using var stillPresent = await _client.GetAsync(unrelated.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, stillPresent.StatusCode);
    }

    [Fact]
    public async Task UnknownId_ReturnsNotFound()
    {
        using var read = await _client.GetAsync("/employees/-1");
        using var delete = await _client.DeleteAsync("/employees/-1");
        Assert.Equal(HttpStatusCode.NotFound, read.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
    }

    [Fact]
    public async Task ConcurrentCreatesAndReads_PreserveEveryEmployeeAndUniqueIds()
    {
        var writes = Enumerable.Range(0, 48).Select(i => Task.Run(async () =>
        {
            using var response = await _client.PostAsJsonAsync("/employees", new
            {
                name = $"Concurrent {i}", position = "QA", salary = 1m
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var employee = await response.Content.ReadFromJsonAsync<Employee>();
            Assert.NotNull(employee);
            return employee.Id;
        })).ToArray();

        var reads = Enumerable.Range(0, 48).Select(_ => Task.Run(async () =>
        {
            using var response = await _client.GetAsync("/employees");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(await response.Content.ReadFromJsonAsync<List<Employee>>());
        })).ToArray();

        var ids = await Task.WhenAll(writes);
        await Task.WhenAll(reads);
        Assert.Equal(ids.Length, ids.Distinct().Count());
        var stored = await _client.GetFromJsonAsync<List<Employee>>("/employees");
        Assert.NotNull(stored);
        Assert.All(ids, id => Assert.Single(stored, employee => employee.Id == id));
    }
}
