namespace IndaloaventurApp.Frontend.Tests.Features.Inventory;

using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Inventory;
using IndaloaventurApp.SharedUI.Components.Settings;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Inventory;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

public sealed class InventoryManagementViewTests : BunitContext
{
    [Fact]
    public void InventoryManagementView_RendersAccessibleTabsAndMaterialCatalog()
    {
        RegisterInventoryService(CreateCatalogService());

        var cut = Render<InventoryManagementView>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("role=\"tablist\"", cut.Markup);
            Assert.Contains("inventory-materials-tab", cut.Markup);
            Assert.Contains("inventory-categories-tab", cut.Markup);
            Assert.Contains("inventory-statuses-tab", cut.Markup);
            Assert.Equal("true", cut.Find("#inventory-materials-tab").GetAttribute("aria-selected"));
            Assert.Contains("Casco de escalada", cut.Markup);
            Assert.Contains("Material técnico", cut.Markup);
            Assert.Contains("Cascos", cut.Markup);
        });

        cut.Find("#inventory-categories-tab").Click();
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("inventory-categories-panel", cut.Markup);
            Assert.Contains("Material técnico", cut.Markup);
            Assert.Contains("Cascos", cut.Markup);
        });

        cut.Find("#inventory-statuses-tab").Click();
        cut.WaitForAssertion(() => Assert.Contains("Disponible", cut.Markup));
    }

    [Fact]
    public void InventoryManagementView_UsesApplicationDaisyUiComponents()
    {
        RegisterInventoryService(CreateCatalogService());

        var cut = Render<InventoryManagementView>();

        cut.WaitForAssertion(() =>
        {
            Assert.NotEmpty(cut.FindAll(".breadcrumbs"));
            Assert.NotEmpty(cut.FindAll(".inventory-management__hero.card.shadow-sm"));
            Assert.Equal(3, cut.FindAll(".tabs.tabs-box .tab").Count);
            Assert.NotEmpty(cut.FindAll(".inventory-management__card.card.shadow-sm"));
            Assert.NotEmpty(cut.FindAll(".table.table-zebra"));
            Assert.NotEmpty(cut.FindAll(".btn.btn-error.btn-outline"));
        });
    }

    [Fact]
    public void CategoryEditor_CreatesRootAndSubcategoryUsingOnlyRootParentOptions()
    {
        var categories = new List<InventoryCategory>();
        var requests = new List<CreateInventoryCategoryRequest>();
        var inventoryService = new RecordingInventoryService
        {
            GetCategoriesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<InventoryCategory>>.Success(categories.ToArray())),
            CreateCategoryHandler = (request, _) =>
            {
                requests.Add(request);
                var id = requests.Count + 9;
                if (request.ParentCategoryId.HasValue)
                {
                    var root = categories.Single(category => category.Id == request.ParentCategoryId.Value);
                    categories[categories.IndexOf(root)] = root with
                    {
                        Subcategories = root.Subcategories.Concat([new InventoryCategory(id, request.Name, root.Id, Array.Empty<InventoryCategory>())]).ToArray()
                    };
                }
                else
                {
                    categories.Add(new InventoryCategory(id, request.Name, null, Array.Empty<InventoryCategory>()));
                }

                return Task.FromResult(ServiceResult<int>.Success(id));
            }
        };
        RegisterInventoryService(inventoryService);

        var cut = Render<InventoryManagementView>();
        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll("#inventory-categories-tab")));
        cut.Find("#inventory-categories-tab").Click();

        cut.Find("#inventory-category-name").Change("Material técnico");
        cut.Find("form").Submit();
        cut.WaitForAssertion(() => Assert.Collection(requests, request =>
        {
            Assert.Equal("Material técnico", request.Name);
            Assert.Null(request.ParentCategoryId);
        }));

        cut.Find("#inventory-category-name").Change("Cascos");
        cut.Find("#inventory-category-parent").Change("10");
        cut.Find("form").Submit();
        cut.WaitForAssertion(() =>
        {
            Assert.Equal(2, requests.Count);
            Assert.Equal(10, requests[1].ParentCategoryId);
            Assert.Contains("Cascos", cut.Markup);
        });
    }

    [Fact]
    public void MaterialEditor_BlocksInvalidQuantityAndSerialNumberBeforeCallingService()
    {
        var createCalls = 0;
        var inventoryService = CreateCatalogService();
        inventoryService = new RecordingInventoryService
        {
            GetCategoriesHandler = inventoryService.GetCategoriesHandler,
            GetStatusesHandler = inventoryService.GetStatusesHandler,
            GetMaterialsHandler = inventoryService.GetMaterialsHandler,
            CreateMaterialHandler = (_, _) =>
            {
                createCalls++;
                return Task.FromResult(ServiceResult<int>.Success(42));
            }
        };
        RegisterInventoryService(inventoryService);

        var cut = Render<InventoryManagementView>();
        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll("#inventory-material-description")));

        cut.Find("#inventory-material-description").Change("Casco");
        cut.Find("#inventory-material-quantity").Change("2");
        cut.Find("#inventory-material-category").Change("11");
        cut.Find("#inventory-material-status").Change("3");
        cut.Find("#inventory-material-acquisition-or-donation").Change("Adquirido");
        cut.Find("#inventory-material-serial-number").Change("SERIE-1");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(0, createCalls);
            Assert.Contains("inventory_serial_number_quantity_invalid", cut.Markup);
        });
    }

    [Fact]
    public void CategoryDeletionConflict_PreservesRecordAndDisplaysApiDetail()
    {
        var inventoryService = CreateCatalogService();
        inventoryService = new RecordingInventoryService
        {
            GetCategoriesHandler = inventoryService.GetCategoriesHandler,
            GetStatusesHandler = inventoryService.GetStatusesHandler,
            GetMaterialsHandler = inventoryService.GetMaterialsHandler,
            DeleteCategoryHandler = (_, _) => Task.FromResult(ServiceResult<bool>.Failure(
                new ServiceError("inventory.conflict", "La categoría tiene material asignado.")))
        };
        RegisterInventoryService(inventoryService);

        var cut = Render<InventoryManagementView>();
        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll("#inventory-categories-tab")));
        cut.Find("#inventory-categories-tab").Click();
        cut.WaitForAssertion(() => Assert.Contains("Material técnico", cut.Markup));

        cut.FindAll("button")
            .First(button => button.TextContent.Contains("inventory_delete", StringComparison.Ordinal))
            .Click();

        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll("dialog")));
        cut.Find("dialog").QuerySelectorAll("button")
            .Single(button => button.TextContent.Contains("inventory_delete", StringComparison.Ordinal))
            .Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("La categoría tiene material asignado.", cut.Markup);
            Assert.Contains("Material técnico", cut.Markup);
        });
    }

    private void RegisterInventoryService(IInventoryService inventoryService)
    {
        Services.AddSingleton(inventoryService);
        Services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();
    }

    private static RecordingInventoryService CreateCatalogService()
    {
        var categories = new[]
        {
            new InventoryCategory(10, "Material técnico", null,
            [new InventoryCategory(11, "Cascos", 10, Array.Empty<InventoryCategory>())])
        };
        var statuses = new[] { new InventoryStatus(3, "Disponible") };
        var materials = new[]
        {
            new InventoryMaterial(42, "Casco de escalada", "Petzl Boreo", 1, 11, "Cascos", 10, "Material técnico", 3, "Disponible", new DateOnly(2026, 1, 18), "Adquirido", null, "CASCO-001")
        };

        return new RecordingInventoryService
        {
            GetCategoriesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<InventoryCategory>>.Success(categories)),
            GetStatusesHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<InventoryStatus>>.Success(statuses)),
            GetMaterialsHandler = _ => Task.FromResult(ServiceResult<IReadOnlyList<InventoryMaterial>>.Success(materials))
        };
    }
}
