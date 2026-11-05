using FluentValidation;
using System.Linq.Expressions;
using IndaloAventurApi.Domain.Inventario;
namespace IndaloAventurApi.Application.Features.Inventario.InventarioMateriales;
internal static class InventarioMaterialCommandRules
{
    public static void Apply<T>(AbstractValidator<T> v, Expression<Func<T,string>> d, Expression<Func<T,string?>> mm, Expression<Func<T,int>> q, Expression<Func<T,int>> c, Expression<Func<T,int>> e, Expression<Func<T,DateOnly?>> f, Expression<Func<T,string>> a, Expression<Func<T,string?>> o, Expression<Func<T,string?>> s)
    { Text(v.RuleFor(d), "La descripcion", 250, false, true); Text(v.RuleFor(mm), "La marca/modelo", 200, false, false); v.RuleFor(q).GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero."); v.RuleFor(c).GreaterThan(0).WithMessage("La categoria es obligatoria."); v.RuleFor(e).GreaterThan(0).WithMessage("El estado es obligatorio."); v.RuleFor(f).Must(x => x is null || x <= DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("La fecha de adquisicion no puede ser futura."); Text(v.RuleFor(a), "El campo adquirido/donado", 250, false, true); Text(v.RuleFor(o), "Las observaciones", 2000, true, false); Text(v.RuleFor(s), "El numero de serie", 120, false, false); var quantity=q.Compile(); v.RuleFor(s).Must((x, value) => string.IsNullOrWhiteSpace(value) || quantity(x) == 1).WithMessage("Un material con numero de serie debe tener cantidad uno."); }
    private static void Text<T>(IRuleBuilderInitial<T,string?> r, string n, int m, bool lines, bool required) { if (required) r.NotEmpty().WithMessage($"{n} es obligatorio."); r.MaximumLength(m).WithMessage($"{n} no puede superar {m} caracteres.").Must(x => x is null || x.All(c => !char.IsControl(c) || (lines && c == '\n'))).WithMessage($"{n} contiene caracteres no permitidos."); }
}
