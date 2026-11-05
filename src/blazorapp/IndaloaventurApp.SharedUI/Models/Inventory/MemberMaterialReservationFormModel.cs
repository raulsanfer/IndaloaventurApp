namespace IndaloaventurApp.SharedUI.Models.Inventory;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents member-entered values for creating a material reservation.
/// </summary>
public sealed class MemberMaterialReservationFormModel : IValidatableObject
{
    /// <summary>Gets or sets the selected inventory material identifier.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un material disponible.")]
    public int? InventoryMaterialId { get; set; }

    /// <summary>Gets or sets the number of material units requested.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser un número entero mayor que cero.")]
    public int? RequestedQuantity { get; set; }

    /// <summary>Gets or sets the planned collection date.</summary>
    [Required(ErrorMessage = "Indica la fecha prevista de recogida.")]
    public DateOnly? ScheduledPickupDate { get; set; }

    /// <summary>Gets or sets the estimated return date.</summary>
    [Required(ErrorMessage = "Indica la fecha estimada de devolución.")]
    public DateOnly? EstimatedReturnDate { get; set; }

    /// <summary>Gets or sets the optional notes for the club.</summary>
    [StringLength(2000, ErrorMessage = "Las observaciones no pueden superar 2.000 caracteres.")]
    public string? MemberNotes { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ScheduledPickupDate is DateOnly pickupDate &&
            EstimatedReturnDate is DateOnly returnDate &&
            returnDate < pickupDate)
        {
            yield return new ValidationResult(
                "La fecha estimada de devolución no puede ser anterior a la recogida prevista.",
                [nameof(EstimatedReturnDate)]);
        }

        if (!string.IsNullOrEmpty(MemberNotes) && MemberNotes.Any(static character => char.IsControl(character) && character is not '\r' and not '\n'))
        {
            yield return new ValidationResult(
                "Las observaciones contienen caracteres no permitidos.",
                [nameof(MemberNotes)]);
        }
    }

    /// <summary>Converts a valid form to the reservation API request.</summary>
    /// <exception cref="InvalidOperationException">Thrown when required form values are missing.</exception>
    public CreateMemberMaterialReservationRequest ToRequest()
    {
        if (InventoryMaterialId is not int materialId ||
            RequestedQuantity is not int quantity ||
            ScheduledPickupDate is not DateOnly pickupDate ||
            EstimatedReturnDate is not DateOnly returnDate)
        {
            throw new InvalidOperationException("El formulario de reserva no contiene los datos obligatorios.");
        }

        return new CreateMemberMaterialReservationRequest(
            materialId,
            quantity,
            pickupDate,
            returnDate,
            NormalizeNotes(MemberNotes));
    }

    private static string? NormalizeNotes(string? notes)
        => string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
}
