using TwentyFourDays.Persistence.Services;
using Umbraco.Cms.Web.Common.Controllers;

namespace TwentyFourDays;

public class TestController : UmbracoApiController
{
    private readonly MoviesImportService _moviesImportService;

    public TestController(MoviesImportService moviesImportService)
    {
        _moviesImportService = moviesImportService;
    }

    public async Task TestImport()
    {
        await _moviesImportService.Import();
    }
    
}