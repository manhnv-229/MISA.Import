using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Interfaces
{
    public interface IValidatableObject
    {
        //
        // Summary:
        //     Determines whether the specified object is valid.
        //
        // Parameters:
        //   validationContext:
        //     The validation context.
        //
        // Returns:
        //     A collection that holds failed-validation information.
        IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }
}
