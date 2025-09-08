using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class ModelExtensionMethods
    {
        public static ModelSimpleDto AsSimpleDto(this Model model)
        {
            var dto = new ModelSimpleDto()
            {
                ModelID = model.ModelID,
                ModelSubbasin = model.ModelSubbasin,
                ModelName = model.ModelName,
                ModelShortName = model.ModelShortName,
                ModelDescription = model.ModelDescription,
                ModelEngine = model.ModelEngine,
                GETModelID = model.GETModelID,
                ModelImage = model.ModelImage
            };
            return dto;
        }
    }
}