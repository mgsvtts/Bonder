using Application.AttachTinkoffToken;
using Application.GetPortfolios;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Presentation.Controllers.Dto.AttachToken;
using Presentation.Filters;
using System.IO;

namespace Presentation.Controllers;

[ExceptionFilter]
[Route("api/portfolio")]
public sealed class PortfolioController : ControllerBase
{
    private readonly ISender _sender;

    public PortfolioController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("attach-token")]
    public async Task<IActionResult> AttachToken([FromBody] AttachTokenRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(request.Adapt<AttachTinkoffTokenCommand>(), cancellationToken);

        return NoContent();
    }

    [HttpGet]
    public async Task<IEnumerable<Portfolio>> GetPortfolios(CancellationToken cancellationToken)
    {
        return await _sender.Send(new GetPortfoliosQuery(HttpContext.Request.Headers.Authorization), cancellationToken);
    }

    [HttpPost("export")]
    public async Task ExportAsync([FromForm]IFormFile file)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage(file.OpenReadStream());
        var currentSheet = package.Workbook.Worksheets;
        var workSheet = currentSheet.First();
        workSheet.Cells[1, 1].Value = "Text in first row and first column"; // EPPlus index of columns and rows are from 1 not 0

        using var memStream = new MemoryStream();
        package.SaveAs(memStream);

        using var fileStream = System.IO.File.Create("path");
        byte[] buffer = new byte[1024];
        int bytesRead;
        do
        {
            bytesRead = memStream.Read(buffer, 0, buffer.Length);
            fileStream.Write(buffer, 0, bytesRead);
        } while (bytesRead > 0);
    }
}