//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETSync]
namespace Qanat.EFModels.Entities
{
    public partial class OpenETSync
    {
        public int PrimaryKey => OpenETSyncID;
        public OpenETDataType OpenETDataType => OpenETDataType.AllLookupDictionary[OpenETDataTypeID];

        public static class FieldLengths
        {

        }
    }
}