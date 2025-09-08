//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ModelBoundary]
namespace Qanat.EFModels.Entities
{
    public partial class ModelBoundary
    {
        public int PrimaryKey => ModelBoundaryID;
        public Model Model => Model.AllLookupDictionary[ModelID];

        public static class FieldLengths
        {

        }
    }
}