//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ExternalMapLayer]
namespace Qanat.EFModels.Entities
{
    public partial class ExternalMapLayer
    {
        public int PrimaryKey => ExternalMapLayerID;
        public ExternalMapLayerType ExternalMapLayerType => ExternalMapLayerType.AllLookupDictionary[ExternalMapLayerTypeID];

        public static class FieldLengths
        {
            public const int ExternalMapLayerDisplayName = 100;
            public const int ExternalMapLayerURL = 500;
            public const int ExternalMapLayerDescription = 200;
            public const int PopUpField = 100;
        }
    }
}