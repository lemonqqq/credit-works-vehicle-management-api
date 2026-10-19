using System.ComponentModel.DataAnnotations;
using CreditWorks.VehicleManagement.Application.Models.Commands;
using CreditWorks.VehicleManagement.Application.Models.Queries;
using FieldSortOrder = CreditWorks.VehicleManagement.Application.Models.Queries.Dto.FieldSortOrder;
using CreditWorks.VehicleManagement.Application.Validators;
using CreditWorks.VehicleManagement.Domain.Exceptions;

namespace CreditWorks.VehicleManagement.Unit.Test.Application.Validators;

public sealed class RequestValidationTests
{
    private static IReadOnlyList<ValidationResult> Validate(object request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, true);
        return results;
    }

    private static VehicleCommand Vehicle() => new()
    {
        OwnerName = "Alex Smith", Manufacturer = "Mazda", Year = 2020,
        Vin = "ABC12345678901234", RegistrationNumber = "ABC123", WeightKg = 1850.75m
    };

    [Theory]
    [InlineData("ABC123", "ABC123")]
    [InlineData("ABC1234567890123!", "ABC123")]
    [InlineData("ABC12345678901234", "ABC-123")]
    [InlineData("ABC12345678901234", "ABCDEFG")]
    public void VehicleRequest_WhenIdentifiersInvalid_ThenRejectsRequest(string vin, string registration)
    {
        var request = Vehicle();
        request.Vin = vin;
        request.RegistrationNumber = registration;
        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void VehicleRequest_WhenWeightHasThreeDecimals_ThenRejectsRequest()
    {
        var request = Vehicle();
        request.WeightKg = 1850.751m;
        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(VehicleCommand.WeightKg)));
    }

    [Fact]
    public void VehicleRequest_WhenYearIsInFuture_ThenRejectsRequest()
    {
        var request = Vehicle();
        request.Year = DateTime.UtcNow.Year + 2;
        Assert.Contains(Validate(request), error => error.MemberNames.Contains(nameof(VehicleCommand.Year)));
    }

    [Theory]
    [InlineData(1999)]
    [InlineData(2051)]
    public void SearchRequest_WhenYearOutsideSupportedRange_ThenRejectsRequest(int year)
    {
        Assert.Contains(Validate(new VehicleSearchQuery { Year = year }),
            error => error.MemberNames.Contains(nameof(VehicleSearchQuery.Year)));
    }

    [Fact]
    public void SearchRequest_WhenFiltersValid_ThenAcceptsRequest()
    {
        Assert.Empty(Validate(new VehicleSearchQuery
        {
            ManufacturerName = "Mazda", Year = 2020, MinWeightKg = 500m, MaxWeightKg = 2500m
        }));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SearchRequest_WhenPageNumberIsNotPositive_ThenRejectsRequest(int pageNumber)
    {
        Assert.Contains(Validate(new VehicleSearchQuery { PageNumber = pageNumber }),
            error => error.MemberNames.Contains(nameof(VehicleSearchQuery.PageNumber)));
    }

    [Fact]
    public void SearchRequest_WhenWeightRangeIsReversed_ThenRejectsRequest()
    {
        Assert.Contains(Validate(new VehicleSearchQuery { MinWeightKg = 2500m, MaxWeightKg = 500m }),
            error => error.MemberNames.Contains(nameof(VehicleSearchQuery.MaxWeightKg)));
    }

    [Theory]
    [InlineData(-0.01, null)]
    [InlineData(null, -0.01)]
    public void SearchRequest_WhenWeightBoundIsNegative_ThenRejectsRequest(double? min, double? max)
    {
        Assert.NotEmpty(Validate(new VehicleSearchQuery
        {
            MinWeightKg = min is null ? null : (decimal)min.Value,
            MaxWeightKg = max is null ? null : (decimal)max.Value
        }));
    }

    [Theory]
    [InlineData("DoesNotExist")]
    [InlineData("Manufacturer.Name")]
    [InlineData("")]
    public void SearchRequest_WhenSortFieldUnsupported_ThenRejectsRequest(string field)
    {
        Assert.Contains(Validate(new VehicleSearchQuery
        {
            SortFieldList = [new FieldSortOrder { FieldName = field }]
        }), error => error.MemberNames.Contains(nameof(VehicleSearchQuery.SortFieldList)));
    }

    [Theory]
    [InlineData("ownerName")]
    [InlineData("manufacturer")]
    [InlineData("year")]
    [InlineData("weight")]
    public void SearchRequest_WhenSortFieldSupported_ThenAcceptsRequest(string field)
    {
        Assert.Empty(Validate(new VehicleSearchQuery
        {
            SortFieldList = [new FieldSortOrder { FieldName = field }]
        }));
    }

    [Fact]
    public void CategoryRequest_WhenNoCategoriesProvided_ThenRejectsCompleteConfiguration()
    {
        Assert.Throws<BusinessConflictException>(() => VehicleCategoryValidator.Validate([]));
    }

    [Fact]
    public void CategoryRequest_WhenDuplicateNameOnlyDiffersByCaseAndSpace_ThenRejectsConfiguration()
    {
        var categories = new List<CategoryDto>
        {
            new() { Name = "Light", IconKey = "car", MinWeightKg = 0, MaxWeightKg = 500 },
            new() { Name = " light ", IconKey = "truck", MinWeightKg = 500, MaxWeightKg = null }
        };
        Assert.Throws<BusinessConflictException>(() => VehicleCategoryValidator.Validate(categories));
    }
}
