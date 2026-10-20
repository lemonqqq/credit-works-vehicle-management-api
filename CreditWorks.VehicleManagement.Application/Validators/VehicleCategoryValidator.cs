using CreditWorks.VehicleManagement.Application.Models.Commands;
using CreditWorks.VehicleManagement.Domain.Exceptions;

namespace CreditWorks.VehicleManagement.Application.Validators
{
    /// <summary>
    ///     Validates a complete set of lower-inclusive, upper-exclusive ranges.
    /// </summary>
    internal static class VehicleCategoryValidator
    {
        /// <summary>
        ///     Checks the supplied ranges for gaps, overlaps, and invalid bounds.
        /// </summary>
        /// <param name="list">The proposed categories.</param>
        /// <exception cref="BusinessConflictException">The range configuration is invalid.</exception>
        public static void Validate(List<CategoryDto> list)
        {
            if (list is null || list.Count == 0)
                throw new BusinessConflictException("At least one category is required for complete coverage.");

            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var sorted = list.OrderBy(x => x.MinWeightKg).ToList();
            decimal expectedLowerBound = 0;

            for (var index = 0; index < sorted.Count; index++)
            {
                var category = sorted[index];
                if (string.IsNullOrWhiteSpace(category.Name))
                    throw new BusinessConflictException("Category name is required.");
                if (!names.Add(category.Name.Trim()))
                    throw new BusinessConflictException($"Duplicate category name: {category.Name}.");
                if (string.IsNullOrWhiteSpace(category.IconKey))
                    throw new BusinessConflictException($"Category {category.Name} requires an icon.");
                if (decimal.Round(category.MinWeightKg, 2) != category.MinWeightKg
                    || category.MaxWeightKg is { } upper && decimal.Round(upper, 2) != upper)
                    throw new BusinessConflictException($"Category {category.Name} boundary precision exceeds two decimal places.");
                if (category.MinWeightKg < 0)
                    throw new BusinessConflictException($"Category {category.Name} has a negative lower bound.");
                if (category.MinWeightKg > expectedLowerBound)
                    throw new BusinessConflictException($"Missing weight range before {category.Name}.");
                if (category.MinWeightKg < expectedLowerBound)
                    throw new BusinessConflictException($"Overlap before category {category.Name}.");

                if (category.MaxWeightKg is null)
                {
                    if (index != sorted.Count - 1)
                        throw new BusinessConflictException("Only the final category may have an open upper bound.");
                    return;
                }
                if (category.MaxWeightKg <= category.MinWeightKg)
                    throw new BusinessConflictException($"Category {category.Name} upper bound must be greater than its lower bound.");

                expectedLowerBound = category.MaxWeightKg.Value;
            }

            throw new BusinessConflictException("The final category must have an open upper bound.");
        }
    }

}
