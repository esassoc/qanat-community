//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OpenETDataType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class OpenETDataType : IHavePrimaryKey
    {
        public static readonly OpenETDataTypeEvapotranspiration Evapotranspiration = OpenETDataTypeEvapotranspiration.Instance;
        public static readonly OpenETDataTypePrecipitation Precipitation = OpenETDataTypePrecipitation.Instance;

        public static readonly List<OpenETDataType> All;
        public static readonly List<OpenETDataTypeSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, OpenETDataType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, OpenETDataTypeSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static OpenETDataType()
        {
            All = new List<OpenETDataType> { Evapotranspiration, Precipitation };
            AllAsSimpleDto = new List<OpenETDataTypeSimpleDto> { Evapotranspiration.AsSimpleDto(), Precipitation.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, OpenETDataType>(All.ToDictionary(x => x.OpenETDataTypeID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, OpenETDataTypeSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.OpenETDataTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected OpenETDataType(int openETDataTypeID, string openETDataTypeName, string openETDataTypeDisplayName, string openETDataTypeVariableName)
        {
            OpenETDataTypeID = openETDataTypeID;
            OpenETDataTypeName = openETDataTypeName;
            OpenETDataTypeDisplayName = openETDataTypeDisplayName;
            OpenETDataTypeVariableName = openETDataTypeVariableName;
        }

        [Key]
        public int OpenETDataTypeID { get; private set; }
        public string OpenETDataTypeName { get; private set; }
        public string OpenETDataTypeDisplayName { get; private set; }
        public string OpenETDataTypeVariableName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return OpenETDataTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(OpenETDataType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.OpenETDataTypeID == OpenETDataTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as OpenETDataType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return OpenETDataTypeID;
        }

        public static bool operator ==(OpenETDataType left, OpenETDataType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OpenETDataType left, OpenETDataType right)
        {
            return !Equals(left, right);
        }

        public OpenETDataTypeEnum ToEnum => (OpenETDataTypeEnum)GetHashCode();

        public static OpenETDataType ToType(int enumValue)
        {
            return ToType((OpenETDataTypeEnum)enumValue);
        }

        public static OpenETDataType ToType(OpenETDataTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case OpenETDataTypeEnum.Evapotranspiration:
                    return Evapotranspiration;
                case OpenETDataTypeEnum.Precipitation:
                    return Precipitation;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum OpenETDataTypeEnum
    {
        Evapotranspiration = 1,
        Precipitation = 2
    }

    public partial class OpenETDataTypeEvapotranspiration : OpenETDataType
    {
        private OpenETDataTypeEvapotranspiration(int openETDataTypeID, string openETDataTypeName, string openETDataTypeDisplayName, string openETDataTypeVariableName) : base(openETDataTypeID, openETDataTypeName, openETDataTypeDisplayName, openETDataTypeVariableName) {}
        public static readonly OpenETDataTypeEvapotranspiration Instance = new OpenETDataTypeEvapotranspiration(1, @"Evapotranspiration", @"Evapotranspiration", @"et");
    }

    public partial class OpenETDataTypePrecipitation : OpenETDataType
    {
        private OpenETDataTypePrecipitation(int openETDataTypeID, string openETDataTypeName, string openETDataTypeDisplayName, string openETDataTypeVariableName) : base(openETDataTypeID, openETDataTypeName, openETDataTypeDisplayName, openETDataTypeVariableName) {}
        public static readonly OpenETDataTypePrecipitation Instance = new OpenETDataTypePrecipitation(2, @"Precipitation", @"Precipitation", @"pr");
    }
}