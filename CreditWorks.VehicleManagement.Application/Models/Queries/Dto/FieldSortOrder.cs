using System.ComponentModel.DataAnnotations;

namespace CreditWorks.VehicleManagement.Application.Models.Queries.Dto
{
    public record FieldSortOrder
    {
        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Field name must be in alphanumeric")]
        public string FieldName { get; set; }
        public bool IsAscending { get; set; } = true;
    }
}
