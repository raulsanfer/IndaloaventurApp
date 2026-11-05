namespace IndaloaventurApp.Frontend.Tests.Features.Inventory;

using System.ComponentModel.DataAnnotations;
using IndaloaventurApp.SharedUI.Models.Inventory;

public sealed class MemberMaterialReservationFormModelTests
{
    [Fact]
    public void Validate_RejectsInvalidReturnDateAndControlCharacters()
    {
        var form = new MemberMaterialReservationFormModel
        {
            InventoryMaterialId = 42,
            RequestedQuantity = 1,
            ScheduledPickupDate = new DateOnly(2026, 11, 12),
            EstimatedReturnDate = new DateOnly(2026, 11, 11),
            MemberNotes = "Texto\u0001 no permitido"
        };

        var results = Validate(form);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(MemberMaterialReservationFormModel.EstimatedReturnDate)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(MemberMaterialReservationFormModel.MemberNotes)));
    }

    [Fact]
    public void Validate_RejectsNonPositiveQuantity()
    {
        var form = new MemberMaterialReservationFormModel
        {
            InventoryMaterialId = 42,
            RequestedQuantity = 0,
            ScheduledPickupDate = new DateOnly(2026, 11, 12),
            EstimatedReturnDate = new DateOnly(2026, 11, 15)
        };

        var results = Validate(form);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(MemberMaterialReservationFormModel.RequestedQuantity)));
    }

    [Fact]
    public void ToRequest_NormalizesOptionalNotesAfterValidInput()
    {
        var form = new MemberMaterialReservationFormModel
        {
            InventoryMaterialId = 42,
            RequestedQuantity = 2,
            ScheduledPickupDate = new DateOnly(2026, 11, 12),
            EstimatedReturnDate = new DateOnly(2026, 11, 15),
            MemberNotes = "  Recogeré el material por la tarde.  "
        };

        var request = form.ToRequest();

        Assert.Equal(42, request.InventoryMaterialId);
        Assert.Equal(2, request.RequestedQuantity);
        Assert.Equal(new DateOnly(2026, 11, 12), request.ScheduledPickupDate);
        Assert.Equal(new DateOnly(2026, 11, 15), request.EstimatedReturnDate);
        Assert.Equal("Recogeré el material por la tarde.", request.MemberNotes);
    }

    private static IReadOnlyList<ValidationResult> Validate(MemberMaterialReservationFormModel form)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(form, new ValidationContext(form), results, validateAllProperties: true);
        return results;
    }
}
