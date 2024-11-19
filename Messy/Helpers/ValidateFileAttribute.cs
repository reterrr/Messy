using System.ComponentModel.DataAnnotations;
using File = Messy.Models.File;

namespace Messy.Helpers;

[AttributeUsage(AttributeTargets.Property)]
public class ValidateFileAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not IEnumerable<File> files)
            throw new ValidationException("Files must be of type IEnumerable<File>");

        if (files.Any(file => file.Length > FileSettings.MaxFileSize))
            throw new ValidationException("Files must be less than " + FileSettings.MaxFileSize);
        
        var totalSize = files.Sum(file => file.Length);
        if (totalSize > FileSettings.MaxAssignedFilesSize)
            throw new ValidationException("Total files size must be less than " + FileSettings.MaxAssignedFilesSize);
        
        return true;
    }   
}