//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CustomAttribute]
namespace Qanat.EFModels.Entities
{
    public partial class CustomAttribute
    {
        public int PrimaryKey => CustomAttributeID;
        public CustomAttributeType CustomAttributeType => CustomAttributeType.AllLookupDictionary[CustomAttributeTypeID];

        public static class FieldLengths
        {
            public const int CustomAttributeName = 60;
        }
    }
}