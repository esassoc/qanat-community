using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class WaterAccountContacts
{
    public static async Task<WaterAccountContactDto> Create(QanatDbContext dbContext, int geographyID, WaterAccountContactUpsertDto waterAccountContactUpsertDto)
    {
        var waterAccountContact = new WaterAccountContact()
        {
            GeographyID = geographyID,
            ContactName = waterAccountContactUpsertDto.ContactName,
            ContactEmail = waterAccountContactUpsertDto.ContactEmail,
            ContactPhoneNumber = waterAccountContactUpsertDto.ContactPhoneNumber,
            Address = waterAccountContactUpsertDto.Address,
            SecondaryAddress = waterAccountContactUpsertDto.SecondaryAddress,
            City = waterAccountContactUpsertDto.City,
            State = waterAccountContactUpsertDto.State,
            ZipCode = waterAccountContactUpsertDto.ZipCode,
            PrefersPhysicalCommunication = waterAccountContactUpsertDto.PrefersPhysicalCommunication ?? true,
            AddressValidated = waterAccountContactUpsertDto.AddressValidated,
            AddressValidationJson = waterAccountContactUpsertDto.AddressValidationJson,
        };

        await dbContext.WaterAccountContacts.AddAsync(waterAccountContact);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(waterAccountContact).ReloadAsync();

        return await GetByIDAsDto(dbContext, waterAccountContact.WaterAccountContactID);
    }

    public static async Task<List<WaterAccountContactDto>> ListByGeographyIDAsDto(QanatDbContext dbContext, int geographyID)
    {
        var waterAccountContactDtos = await dbContext.WaterAccountContacts.AsNoTracking()
            .Include(x => x.WaterAccounts)
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsDto()).ToListAsync();

        return waterAccountContactDtos;
    }

    public static async Task<WaterAccountContactDto> GetByIDAsDto(QanatDbContext dbContext, int waterAccountContactID)
    {
        var waterAccountContact = await dbContext.WaterAccountContacts.AsNoTracking()
            .Include(x => x.WaterAccounts)
            .SingleOrDefaultAsync(x => x.WaterAccountContactID == waterAccountContactID);

        var waterAccountContactDto = waterAccountContact.AsDto();

        return waterAccountContactDto;
    }

    public static async Task<WaterAccountContactDto> GetByWaterAccountIDAsDto(QanatDbContext dbContext, int waterAccountID)
    {
        var waterAccount = await dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountContact)
            .SingleAsync(x => x.WaterAccountID == waterAccountID);

        var waterAccountContactDto = waterAccount.WaterAccountContact?.AsDto();

        return waterAccountContactDto;
    }

    public static WaterAccountContactSearchSummaryDto GetBySearchString(QanatDbContext dbContext, int geographyID, string searchString, UserDto user)
    {
        const int searchResultLimit = 10;

        var waterAccountContacts = dbContext.WaterAccountContacts.AsNoTracking()
            .Where(x => x.GeographyID == geographyID);
        
        var matchedWaterAccountContacts = waterAccountContacts
                .Where(x =>
                    (x.ContactName.ToUpper().Contains(searchString.ToUpper())) ||
                    x.ContactEmail.ToUpper().Contains(searchString) ||
                    x.ContactPhoneNumber.Contains(searchString) ||
                    x.FullAddress.ToUpper().Contains(searchString.ToUpper()))
                .ToList()
                .Select(x => x.AsSearchResultWithMatchedFieldsDto(searchString))
                .ToList();

        return new WaterAccountContactSearchSummaryDto()
        {
            TotalResults = matchedWaterAccountContacts.Count(),
            SearchResults = matchedWaterAccountContacts.Take(searchResultLimit).ToList()
        };
    }

    public static async Task<WaterAccountContactDto> Update(QanatDbContext dbContext, int waterAccountContactID, WaterAccountContactUpsertDto waterAccountContactUpsertDto)
    {
        var waterAccountContact = await dbContext.WaterAccountContacts.SingleAsync(x => x.WaterAccountContactID == waterAccountContactID);

        waterAccountContact.ContactName = waterAccountContactUpsertDto.ContactName;
        waterAccountContact.ContactEmail = waterAccountContactUpsertDto.ContactEmail;
        waterAccountContact.ContactPhoneNumber = waterAccountContactUpsertDto.ContactPhoneNumber;
        waterAccountContact.Address = waterAccountContactUpsertDto.Address;
        waterAccountContact.SecondaryAddress = waterAccountContactUpsertDto.SecondaryAddress;
        waterAccountContact.City = waterAccountContactUpsertDto.City;
        waterAccountContact.State = waterAccountContactUpsertDto.State;
        waterAccountContact.ZipCode = waterAccountContactUpsertDto.ZipCode;
        waterAccountContact.PrefersPhysicalCommunication = waterAccountContactUpsertDto.PrefersPhysicalCommunication ?? true;
        waterAccountContact.AddressValidated = waterAccountContactUpsertDto.AddressValidated;
        waterAccountContact.AddressValidationJson = waterAccountContactUpsertDto.AddressValidationJson;

        await dbContext.SaveChangesAsync();
        await dbContext.Entry(waterAccountContact).ReloadAsync();

        return await GetByIDAsDto(dbContext, waterAccountContactID);
    }

    public static async Task<WaterAccountContactDto> BulkUpdateAddresses(QanatDbContext dbContext, int waterAccountContactID, WaterAccountContactUpsertDto waterAccountContactUpsertDto)
    {
        var waterAccountContact = await dbContext.WaterAccountContacts.SingleAsync(x => x.WaterAccountContactID == waterAccountContactID);

        waterAccountContact.Address = waterAccountContactUpsertDto.Address;
        waterAccountContact.SecondaryAddress = waterAccountContactUpsertDto.SecondaryAddress;
        waterAccountContact.City = waterAccountContactUpsertDto.City;
        waterAccountContact.State = waterAccountContactUpsertDto.State;
        waterAccountContact.ZipCode = waterAccountContactUpsertDto.ZipCode;
        waterAccountContact.AddressValidated = waterAccountContactUpsertDto.AddressValidated;
        waterAccountContact.AddressValidationJson = waterAccountContactUpsertDto.AddressValidationJson;

        await dbContext.SaveChangesAsync();
        await dbContext.Entry(waterAccountContact).ReloadAsync();

        return await GetByIDAsDto(dbContext, waterAccountContactID);
    }

    public static List<ErrorMessage> ValidateDelete(QanatDbContext dbContext, int waterAccountContactID)
    {
        var errors = new List<ErrorMessage>();

        var waterAccountContact = dbContext.WaterAccountContacts.AsNoTracking()
            .Include(x => x.WaterAccounts)
            .Single(x => x.WaterAccountContactID == waterAccountContactID);
        
        if (waterAccountContact.WaterAccounts.Any())
        {
            var waterAccountCount = waterAccountContact.WaterAccounts.Count;
            
            errors.Add(new ErrorMessage() { Type="Water Account Contact", Message = $"This contact cannot be deleted because it is currently associated with {waterAccountCount} water account{(waterAccountCount > 1 ? "s" : "")}."});
        }

        return errors;
    }

    public static async Task Delete(QanatDbContext dbContext, int waterAccountContactID)
    {
        var waterAccountContact = await dbContext.WaterAccountContacts.SingleAsync(x => x.WaterAccountContactID == waterAccountContactID);
        dbContext.WaterAccountContacts.Remove(waterAccountContact);
        await dbContext.SaveChangesAsync();
    }
}