namespace IndaloaventurApp.Frontend.Tests.Features.Licenses;

using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Licenses;
using IndaloaventurApp.Web.Client.Infrastructure.Licenses;

public sealed class FederativeLicenseContractTests
{
    private static RecordingSessionService Session(bool isMember = true, string role = "Member")
    {
        var session = new RecordingSessionService();
        session.SetSession(new AuthSession("test-token", "Bearer", 3600, isMember, [role]));
        return session;
    }
    private static HttpClient Client(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        => new(new StubHttpMessageHandler(handler)) { BaseAddress = new("https://localhost") };
    private static HttpResponseMessage Json(string json, HttpStatusCode status = HttpStatusCode.OK)
        => new(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

    /// <summary>Verifies types use authenticated catalog and preserve server order and names.</summary>
    [Fact]
    public async Task Types_UseAuthenticatedCatalog_AndPreserveServerOrderAndNames()
    {
        using var http = Client(request => {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("/api/licencias-federativas/tipos", request.RequestUri!.PathAndQuery);
            Assert.Equal("Bearer", request.Headers.Authorization!.Scheme);
            Assert.Equal("test-token", request.Headers.Authorization.Parameter);
            return Task.FromResult(Json("""[{"id":1,"nombre":"Completa"},{"id":2,"nombre":"Media 01/07 - 31/12"},{"id":3,"nombre":"Día"}]"""));
        });
        var result = await new FederativeLicenseApiClient(http, Session()).GetLicenseTypesAsync();
        Assert.True(result.IsSuccess);
        Assert.Equal(new[] { 1,2,3 }, result.Value!.Select(x => x.Id));
        Assert.Equal(new[] { "Completa", "Media 01/07 - 31/12", "Día" }, result.Value!.Select(x => x.Nombre));
    }

    /// <summary>Verifies creation sends only allowed fields and materializes date only coverage.</summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Creation_SendsOnlyAllowedFields_AndMaterializesDateOnlyCoverage(bool daily)
    {
        var id=Guid.NewGuid();
        DateOnly? day = daily ? new DateOnly(2026,11,20) : null;
        using var http=Client(async request => {
            Assert.Equal("/api/licencias-federativas/me/solicitudes",request.RequestUri!.PathAndQuery);
            Assert.Equal("test-token",request.Headers.Authorization!.Parameter);
            using var body=JsonDocument.Parse(await request.Content!.ReadAsStringAsync());
            Assert.Equal(new[] { "fechaFin", "fechaInicio", "tarifaLicenciaFederativaId", "temporada" },body.RootElement.EnumerateObject().Select(x=>x.Name).Order());
            Assert.Equal(2026,body.RootElement.GetProperty("temporada").GetInt32());
            Assert.Equal(123,body.RootElement.GetProperty("tarifaLicenciaFederativaId").GetInt32());
            Assert.Equal(daily ? "2026-11-20" : null,body.RootElement.GetProperty("fechaInicio").GetString());
            Assert.Equal(daily ? "2026-11-20" : null,body.RootElement.GetProperty("fechaFin").GetString());
            return new(HttpStatusCode.Created) { Content=JsonContent.Create(new {
                id, temporada=2026, estado="Pendiente", fechaCreacionUtc=DateTime.UtcNow, tarifaLicenciaFederativaId=123,
                licencia="A", categoria="Adulto", territorio="Nacional", tipoLicenciaId=daily ? 3 : 1,
                tipoLicenciaNombre=daily ? "Día" : "Completa", precioClub=12.5, precioIndependiente=(double?)null,
                fechaInicio=day, fechaFin=day }) };
        });
        var result=await new FederativeLicenseApiClient(http,Session()).CreateFederativeLicenseRequestAsync(new(2026,123,day,day));
        Assert.True(result.IsSuccess);
        Assert.Equal(id,result.Value!.Id);
        Assert.Equal(daily ? 3 : 1,result.Value.TipoLicenciaId);
        Assert.Equal(daily ? "Día" : "Completa",result.Value.TipoLicenciaNombre);
        Assert.Equal(day,result.Value.FechaInicio);
        Assert.Equal(day,result.Value.FechaFin);
    }

    /// <summary>Verifies list preserves multiple requests per season and migrated null dates.</summary>
    [Fact]
    public async Task List_PreservesMultipleRequestsPerSeason_AndMigratedNullDates()
    {
        var first=new FederativeLicenseRequest(Guid.NewGuid(),2026,3,"Día","A","Adulto","Nacional","Pendiente",new(2026,11,20),new(2026,11,20));
        var second=first with { Id=Guid.NewGuid() };
        var third=first with { Id=Guid.NewGuid(), TipoLicenciaId=1,TipoLicenciaNombre="Completa", FechaInicio=null, FechaFin=null };
        using var http=Client(request => {
            Assert.Equal("/api/licencias-federativas/me/solicitudes",request.RequestUri!.PathAndQuery);
            Assert.Equal("test-token", request.Headers.Authorization!.Parameter);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) {Content=JsonContent.Create(new[] {first,second,third})});
        });
        var result=await new FederativeLicenseApiClient(http,Session()).GetMyFederativeLicensesAsync();
        Assert.True(result.IsSuccess);
        Assert.Equal(new[]{first.Id,second.Id,third.Id},result.Value!.Select(x=>x.Id));
        Assert.Equal(first.FechaInicio,result.Value![0].FechaInicio);
        Assert.Null(result.Value![2].FechaInicio);
    }

    /// <summary>Verifies creation fails safely for http errors.</summary>
    [Theory]
    [InlineData(400,"licenses.request_invalid")]
    [InlineData(409,"licenses.request_conflict")]
    [InlineData(401,"auth.session_invalid")]
    [InlineData(403,"licenses.not_member")]
    [InlineData(404,"licenses.request_rate_not_found")]
    [InlineData(500,"licenses.request_failed")]
    public async Task Creation_FailsSafely_ForHttpErrors(int status,string code)
    {
        using var http=Client(_ => Task.FromResult(Json("{}",(HttpStatusCode)status)));
        var result=await new FederativeLicenseApiClient(http,Session()).CreateFederativeLicenseRequestAsync(new(2026,123));
        Assert.False(result.IsSuccess);
        Assert.Equal(code,result.Error!.Code);
        if(status==409) { Assert.Contains("Completa o Media",result.Error.Message); Assert.Contains("Día",result.Error.Message); }
    }

    /// <summary>Verifies queries handle expired and forbidden sessions.</summary>
    [Theory]
    [InlineData("types",401,"auth.session_invalid")]
    [InlineData("types",403,"licenses.not_member")]
    [InlineData("rates",401,"auth.session_invalid")]
    [InlineData("rates",403,"licenses.not_member")]
    [InlineData("list",401,"auth.session_invalid")]
    [InlineData("list",403,"licenses.not_member")]
    public async Task Queries_HandleExpiredAndForbiddenSessions(string operation,int status,string code)
    {
        using var http=Client(_=>Task.FromResult(Json("{}",(HttpStatusCode)status)));
        var sut=new FederativeLicenseApiClient(http,Session());
        var error=operation switch {
            "types" => (await sut.GetLicenseTypesAsync()).Error,
            "rates" => (await sut.GetAvailableRatesAsync(2026,3)).Error,
            _ => (await sut.GetMyFederativeLicensesAsync()).Error };
        Assert.Equal(code,error!.Code);
    }

    /// <summary>Verifies member operations deny wrong roles and inactive members.</summary>
    [Theory]
    [InlineData(false,"Member")]
    [InlineData(false,"Admin")]
    [InlineData(true,"Admin")]
    [InlineData(true,"")]
    public async Task MemberOperations_DenyWrongRolesAndInactiveMembers(bool isMember,string role)
    {
        using var http=Client(_=>throw new InvalidOperationException("No HTTP allowed"));
        var sut=new FederativeLicenseApiClient(http,Session(isMember,role));
        Assert.Equal("licenses.not_member",(await sut.GetLicenseTypesAsync()).Error!.Code);
        Assert.Equal("licenses.not_member",(await sut.GetAvailableRatesAsync(2026,3)).Error!.Code);
        Assert.Equal("licenses.not_member",(await sut.GetMyFederativeLicensesAsync()).Error!.Code);
        Assert.Equal("licenses.not_member",(await sut.CreateFederativeLicenseRequestAsync(new(2026,123))).Error!.Code);
    }

    /// <summary>Verifies member operations deny missing session.</summary>
    [Fact]
    public async Task MemberOperations_DenyMissingSession()
    {
        using var http=Client(_=>throw new InvalidOperationException("No HTTP allowed"));
        var sut=new FederativeLicenseApiClient(http,new RecordingSessionService());
        Assert.Equal("auth.missing_session",(await sut.GetLicenseTypesAsync()).Error!.Code);
        Assert.Equal("auth.missing_session",(await sut.GetAvailableRatesAsync(2026,3)).Error!.Code);
        Assert.Equal("auth.missing_session",(await sut.GetMyFederativeLicensesAsync()).Error!.Code);
        Assert.Equal("auth.missing_session",(await sut.CreateFederativeLicenseRequestAsync(new(2026,123))).Error!.Code);
    }

    /// <summary>Verifies failures return safe errors.</summary>
    [Theory]
    [InlineData("types", "json")]
    [InlineData("types", "network")]
    [InlineData("types", "timeout")]
    [InlineData("rates", "json")]
    [InlineData("rates", "network")]
    [InlineData("rates", "timeout")]
    [InlineData("list", "json")]
    [InlineData("create", "json")]
    public async Task Failures_ReturnSafeErrors(string operation,string failure)
    {
        using var http=Client(_ => failure switch {
            "network" => throw new HttpRequestException("sensitive internal details"),
            "timeout" => throw new TaskCanceledException("sensitive internal details"),
            _ => Task.FromResult(Json("invalid sensitive internal details")) });
        var sut=new FederativeLicenseApiClient(http,Session());
        var error=operation switch {
            "types" => (await sut.GetLicenseTypesAsync()).Error,
            "rates" => (await sut.GetAvailableRatesAsync(2026,3)).Error,
            "list" => (await sut.GetMyFederativeLicensesAsync()).Error,
            _ => (await sut.CreateFederativeLicenseRequestAsync(new(2026,123))).Error };
        Assert.NotNull(error);
        Assert.DoesNotContain("sensitive",error.Message);
    }

    /// <summary>Verifies admin operations preserve type and coverage and only send state.</summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task AdminOperations_PreserveTypeAndCoverage_AndOnlySendState(bool isMember)
    {
        var user=Guid.NewGuid(); var id=Guid.NewGuid(); var creation=new DateTime(2026,11,16,10,0,0,DateTimeKind.Utc);
        var item=new AdminFederativeLicenseRequest(id,user,"member@example.test",2026,"Pendiente",creation,123,"A","Adulto","Nacional",20m,null,3,"Día",new(2026,11,20),new(2026,11,21));
        object Payload(AdminFederativeLicenseRequest value) => new {
            value.Id, value.UserId, value.UserEmail, value.Temporada, value.Estado, value.FechaCreacionUtc,
            value.TarifaLicenciaFederativaId, value.Licencia, value.Categoria, territorio=value.AmbitoTerritorial,
            value.PrecioClub, value.PrecioIndependiente, value.TipoLicenciaId, value.TipoLicenciaNombre, value.FechaInicio, value.FechaFin };
        using var http=Client(async request => {
            Assert.Equal("test-token",request.Headers.Authorization!.Parameter);
            if(request.Method==HttpMethod.Put) {
                Assert.Equal($"/api/licencias-federativas/admin/users/{user:D}/solicitudes/{id:D}/estado",request.RequestUri!.PathAndQuery);
                Assert.Equal("{\"estado\":\"Confirmada\"}",await request.Content!.ReadAsStringAsync());
                return new(HttpStatusCode.OK) { Content=JsonContent.Create(Payload(item with { Estado="Confirmada" })) };
            }
            return new(HttpStatusCode.OK) { Content=JsonContent.Create(new[]{Payload(item),Payload(item with { Id=Guid.NewGuid(),FechaInicio=null,FechaFin=null })}) };
        });
        var sut=new AdminFederativeLicenseApiClient(http,Session(isMember,"Admin"));
        var list=await sut.GetFederativeLicensesAsync(new(user));
        Assert.True(list.IsSuccess);
        Assert.Equal(2,list.Value!.Count);
        Assert.Equal(item,list.Value[0]);
        Assert.Null(list.Value[1].FechaInicio);
        var changed=await sut.UpdateFederativeLicenseStatusAsync(new(user,id,"Confirmada"));
        Assert.True(changed.IsSuccess);
        Assert.Equal(item with { Estado="Confirmada" },changed.Value);
    }

    /// <summary>Verifies admin operations handle expired and forbidden sessions.</summary>
    [Theory]
    [InlineData(401,"auth.session_invalid")]
    [InlineData(403,"licenses.admin_forbidden")]
    public async Task AdminOperations_HandleExpiredAndForbiddenSessions(int status,string code)
    {
        using var http=Client(_=>Task.FromResult(Json("{}",(HttpStatusCode)status)));
        var sut=new AdminFederativeLicenseApiClient(http,Session(false,"Admin"));
        Assert.Equal(code,(await sut.GetFederativeLicensesAsync(new())).Error!.Code);
        Assert.Equal(code,(await sut.UpdateFederativeLicenseStatusAsync(new(Guid.NewGuid(),Guid.NewGuid(),"Confirmada"))).Error!.Code);
    }
}
