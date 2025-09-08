//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FaqDisplayLocationType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class FaqDisplayLocationType : IHavePrimaryKey
    {
        public static readonly FaqDisplayLocationTypeGrowersGuide GrowersGuide = FaqDisplayLocationTypeGrowersGuide.Instance;
        public static readonly FaqDisplayLocationTypeWaterManagerGuide WaterManagerGuide = FaqDisplayLocationTypeWaterManagerGuide.Instance;
        public static readonly FaqDisplayLocationTypeRequestSupport RequestSupport = FaqDisplayLocationTypeRequestSupport.Instance;

        public static readonly List<FaqDisplayLocationType> All;
        public static readonly ReadOnlyDictionary<int, FaqDisplayLocationType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static FaqDisplayLocationType()
        {
            All = new List<FaqDisplayLocationType> { GrowersGuide, WaterManagerGuide, RequestSupport };
            AllLookupDictionary = new ReadOnlyDictionary<int, FaqDisplayLocationType>(All.ToDictionary(x => x.FaqDisplayLocationTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected FaqDisplayLocationType(int faqDisplayLocationTypeID, string faqDisplayLocationTypeName, string faqDisplayLocationTypeDisplayName)
        {
            FaqDisplayLocationTypeID = faqDisplayLocationTypeID;
            FaqDisplayLocationTypeName = faqDisplayLocationTypeName;
            FaqDisplayLocationTypeDisplayName = faqDisplayLocationTypeDisplayName;
        }

        [Key]
        public int FaqDisplayLocationTypeID { get; private set; }
        public string FaqDisplayLocationTypeName { get; private set; }
        public string FaqDisplayLocationTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return FaqDisplayLocationTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(FaqDisplayLocationType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.FaqDisplayLocationTypeID == FaqDisplayLocationTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as FaqDisplayLocationType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return FaqDisplayLocationTypeID;
        }

        public static bool operator ==(FaqDisplayLocationType left, FaqDisplayLocationType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FaqDisplayLocationType left, FaqDisplayLocationType right)
        {
            return !Equals(left, right);
        }

        public FaqDisplayLocationTypeEnum ToEnum => (FaqDisplayLocationTypeEnum)GetHashCode();

        public static FaqDisplayLocationType ToType(int enumValue)
        {
            return ToType((FaqDisplayLocationTypeEnum)enumValue);
        }

        public static FaqDisplayLocationType ToType(FaqDisplayLocationTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case FaqDisplayLocationTypeEnum.GrowersGuide:
                    return GrowersGuide;
                case FaqDisplayLocationTypeEnum.RequestSupport:
                    return RequestSupport;
                case FaqDisplayLocationTypeEnum.WaterManagerGuide:
                    return WaterManagerGuide;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum FaqDisplayLocationTypeEnum
    {
        GrowersGuide = 1,
        WaterManagerGuide = 2,
        RequestSupport = 3
    }

    public partial class FaqDisplayLocationTypeGrowersGuide : FaqDisplayLocationType
    {
        private FaqDisplayLocationTypeGrowersGuide(int faqDisplayLocationTypeID, string faqDisplayLocationTypeName, string faqDisplayLocationTypeDisplayName) : base(faqDisplayLocationTypeID, faqDisplayLocationTypeName, faqDisplayLocationTypeDisplayName) {}
        public static readonly FaqDisplayLocationTypeGrowersGuide Instance = new FaqDisplayLocationTypeGrowersGuide(1, @"GrowersGuide", @"Growers Guide");
    }

    public partial class FaqDisplayLocationTypeWaterManagerGuide : FaqDisplayLocationType
    {
        private FaqDisplayLocationTypeWaterManagerGuide(int faqDisplayLocationTypeID, string faqDisplayLocationTypeName, string faqDisplayLocationTypeDisplayName) : base(faqDisplayLocationTypeID, faqDisplayLocationTypeName, faqDisplayLocationTypeDisplayName) {}
        public static readonly FaqDisplayLocationTypeWaterManagerGuide Instance = new FaqDisplayLocationTypeWaterManagerGuide(2, @"WaterManagerGuide", @"Water Manager Guide");
    }

    public partial class FaqDisplayLocationTypeRequestSupport : FaqDisplayLocationType
    {
        private FaqDisplayLocationTypeRequestSupport(int faqDisplayLocationTypeID, string faqDisplayLocationTypeName, string faqDisplayLocationTypeDisplayName) : base(faqDisplayLocationTypeID, faqDisplayLocationTypeName, faqDisplayLocationTypeDisplayName) {}
        public static readonly FaqDisplayLocationTypeRequestSupport Instance = new FaqDisplayLocationTypeRequestSupport(3, @"RequestSupport", @"Request Support");
    }
}