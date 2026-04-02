using FluentValidation;
using PMTool.Application.DTOs.SubProject;

namespace PMTool.Application.Validators.SubProject;

public class CreateSubProjectRequestValidator : AbstractValidator<CreateSubProjectRequest>
{
    public CreateSubProjectRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Sub-project name is required.")
            .MaximumLength(200).WithMessage("Sub-project name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.ModuleOwnerId)
            .NotEmpty().WithMessage("Module owner is required.");

        RuleFor(x => x.StartDate)
            .Must((request, startDate) => 
            {
                if (!startDate.HasValue) return true;
                if (!request.DueDate.HasValue) return true;
                return startDate < request.DueDate;
            })
            .WithMessage("Start date must be before due date.");

        RuleFor(x => x.DueDate)
            .Must(dueDate => 
            {
                if (!dueDate.HasValue) return true;
                return dueDate > DateTime.UtcNow;
            })
            .WithMessage("Due date must be in the future.");

        RuleFor(x => x.TeamIds)
            .Must(teamIds => teamIds == null || teamIds.Count > 0)
            .WithMessage("At least one team should be assigned to the sub-project.");
    }
}
