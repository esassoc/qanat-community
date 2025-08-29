using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qanat.Models.DataTransferObjects;
using Qanat.PDFGenerator.Pdfs;
using Qanat.PDFGenerator.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Qanat.EFModels.Entities;

namespace Qanat.API.Services;

public class StatementService
{
    private readonly ILogger<FileService> _logger;
    private readonly QanatDbContext _dbContext;
    private readonly FileService _fileService;
    private readonly UsageStatementTemplatePdf _usageStatementTemplatePdf;

    public StatementService(ILogger<FileService> logger, QanatDbContext dbContext, FileService fileService, UsageStatementTemplatePdf usageStatementTemplatePdf)
    {
        _logger = logger;
        _dbContext = dbContext;
        _fileService = fileService;
        _usageStatementTemplatePdf = usageStatementTemplatePdf;
    }

    public async Task<byte[]> GetStatementTemplatePreview(UsageStatementWaterAccountDto dtoToSend)
    {
        //dtoToSend.VegaSpec = VegaSpecUtilities.GetMonthlyUsageChartVegaSpec(dtoToSend.UsageChartDataDtos, dtoToSend.ReportingPeriodEndDate.Year);

        var pdf = await _usageStatementTemplatePdf.BuildPdf(dtoToSend);
        return pdf;
    }

    public async Task GenerateStatementBatchPdfsByStatementBatchID(int statementBatchID, int currentUserID)
    {
        var dtosToSend = UsageStatementWaterAccounts.ListByStatementBatchID(_dbContext, statementBatchID);
        await GenerateStatementBatchPdfs(dtosToSend, statementBatchID, currentUserID);
    }

    public async Task GenerateStatementBatchPdfs(List<UsageStatementWaterAccountDto> dtosToSend, int statementBatchID, int currentUserID)
    {
        var statementBatch = await _dbContext.StatementBatches
            .Include(x => x.StatementBatchWaterAccounts)
            .SingleOrDefaultAsync(x => x.StatementBatchID == statementBatchID);

        var dtosToSendByWaterAccountID = dtosToSend.ToDictionary(x => x.WaterAccountID);

        foreach (var statementBatchWaterAccount in statementBatch.StatementBatchWaterAccounts)
        {
            var dtoToSend = dtosToSendByWaterAccountID.ContainsKey(statementBatchWaterAccount.WaterAccountID)
                ? dtosToSendByWaterAccountID[statementBatchWaterAccount.WaterAccountID]
            : null;
            if (dtoToSend == null) { continue; }

            //dtoToSend.VegaSpec = VegaSpecUtilities.GetMonthlyUsageChartVegaSpec(dtoToSend.UsageChartDataDtos, dtoToSend.ReportingPeriodEndDate.Year);

            var pdf = await _usageStatementTemplatePdf.BuildPdf(dtoToSend);

            var shortenedContactName = dtoToSend.ContactName.Length > 21 ? dtoToSend.ContactName.Substring(0, 21) : dtoToSend.ContactName;
            var fileName = $"{shortenedContactName}_{dtoToSend.WaterAccountNumber}_{statementBatch.StatementBatchName}.pdf";
            var prettyFileName = _fileService.MakePrettyFileName(fileName);

            var stream = new MemoryStream(pdf);
            var file = new FormFile(stream, 0, pdf.Length, prettyFileName, prettyFileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            var fileResource = await _fileService.CreateFileResource(_dbContext, file, currentUserID);

            statementBatchWaterAccount.FileResourceID = fileResource.FileResourceID;
        }

        statementBatch.StatementsGenerated = true;
        statementBatch.LastUpdated = DateTime.UtcNow;
        statementBatch.UpdateUserID = currentUserID;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<FileStream> GetStatementBatchFileResourcesAsZipFile( int statementBatchID)
    {
        var fileResources = await _dbContext.StatementBatchWaterAccounts.AsNoTracking()
            .Include(x => x.FileResource)
            .Where(x => x.StatementBatchID == statementBatchID && x.FileResourceID.HasValue)
            .Select(x => x.FileResource).ToListAsync();

        var fileStream = await _fileService.CreateZipFileFromFileResources(fileResources);
        return fileStream;
    }

    public async Task DeleteStatementBatchWaterAccountFileResources(int statementBatchID)
    {
        var statementBatchWaterAccounts = await _dbContext.StatementBatchWaterAccounts
            .Include(x => x.FileResource)
            .Where(x => x.StatementBatchID == statementBatchID).ToListAsync();

        var fileResourcesToDelete = statementBatchWaterAccounts.Where(x => x.FileResourceID.HasValue)
            .Select(x => x.FileResource).ToList();

        fileResourcesToDelete.ForEach(x => _fileService.DeleteFileStreamFromBlobStorage(x.FileResourceCanonicalName));

        statementBatchWaterAccounts.ForEach(x => x.FileResourceID = null);
        _dbContext.FileResources.RemoveRange(fileResourcesToDelete);

        await _dbContext.SaveChangesAsync();
    }
}