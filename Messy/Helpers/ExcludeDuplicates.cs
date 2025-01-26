using System.ComponentModel.DataAnnotations;

namespace Messy.Helpers;

[AttributeUsage(AttributeTargets.Property)]
public class ExcludeDuplicates : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var list = (IList<long>)value;

        var uniqueList = list.Distinct().ToList();

        var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);

        if (property != null && property.CanWrite)
        {
            property.SetValue(validationContext.ObjectInstance, uniqueList);
        }

        return ValidationResult.Success;
    }
}