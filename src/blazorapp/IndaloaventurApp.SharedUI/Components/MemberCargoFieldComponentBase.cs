namespace IndaloaventurApp.SharedUI.Components;

using IndaloaventurApp.SharedUI.Abstractions.Cargos;
using IndaloaventurApp.SharedUI.Models.Cargos;
using IndaloaventurApp.SharedUI.Models.Member;

public static class MemberCargoFieldComponentBase
{
    public static async Task<MemberCargoFieldState> BuildAsync(
        ICargoAdminService cargoAdminService,
        MemberSelfProfile profile,
        bool canEdit,
        CancellationToken cancellationToken = default)
    {
        var result = await cargoAdminService.GetCargosAsync(cancellationToken);
        var options = result.IsSuccess && result.Value is not null
            ? result.Value
                .Select(static cargo => new CargoSelectionOption(cargo.Id.ToString(), cargo.Description))
                .OrderBy(static option => option.Label, StringComparer.OrdinalIgnoreCase)
                .ToArray()
            : Array.Empty<CargoSelectionOption>();

        return new MemberCargoFieldState(
            ResolveCargoDisplayName(profile, options),
            options,
            canEdit,
            options.Length > 0);
    }

    public static string ResolveCargoDisplayName(MemberSelfProfile profile, IReadOnlyList<CargoSelectionOption> options)
    {
        if (!string.IsNullOrWhiteSpace(profile.CargoLabel))
        {
            return profile.CargoLabel!;
        }

        if (profile.CargoId.HasValue)
        {
            var selected = options.FirstOrDefault(option => string.Equals(option.Value, profile.CargoId.Value.ToString(), StringComparison.Ordinal));
            if (selected is not null)
            {
                return selected.Label;
            }

            return $"Cargo #{profile.CargoId.Value}";
        }

        return "Sin cargo asignado";
    }
}
