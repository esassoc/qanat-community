//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ExternalMapLayerType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class ExternalMapLayerType : IHavePrimaryKey
    {
        public static readonly ExternalMapLayerTypeESRIFeatureServer ESRIFeatureServer = ExternalMapLayerTypeESRIFeatureServer.Instance;
        public static readonly ExternalMapLayerTypeESRIMapServer ESRIMapServer = ExternalMapLayerTypeESRIMapServer.Instance;

        public static readonly List<ExternalMapLayerType> All;
        public static readonly ReadOnlyDictionary<int, ExternalMapLayerType> AllLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ExternalMapLayerType()
        {
            All = new List<ExternalMapLayerType> { ESRIFeatureServer, ESRIMapServer };
            AllLookupDictionary = new ReadOnlyDictionary<int, ExternalMapLayerType>(All.ToDictionary(x => x.ExternalMapLayerTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ExternalMapLayerType(int externalMapLayerTypeID, string externalMapLayerTypeName, string externalMapLayerTypeDisplayName)
        {
            ExternalMapLayerTypeID = externalMapLayerTypeID;
            ExternalMapLayerTypeName = externalMapLayerTypeName;
            ExternalMapLayerTypeDisplayName = externalMapLayerTypeDisplayName;
        }

        [Key]
        public int ExternalMapLayerTypeID { get; private set; }
        public string ExternalMapLayerTypeName { get; private set; }
        public string ExternalMapLayerTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ExternalMapLayerTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ExternalMapLayerType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ExternalMapLayerTypeID == ExternalMapLayerTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ExternalMapLayerType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ExternalMapLayerTypeID;
        }

        public static bool operator ==(ExternalMapLayerType left, ExternalMapLayerType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ExternalMapLayerType left, ExternalMapLayerType right)
        {
            return !Equals(left, right);
        }

        public ExternalMapLayerTypeEnum ToEnum => (ExternalMapLayerTypeEnum)GetHashCode();

        public static ExternalMapLayerType ToType(int enumValue)
        {
            return ToType((ExternalMapLayerTypeEnum)enumValue);
        }

        public static ExternalMapLayerType ToType(ExternalMapLayerTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case ExternalMapLayerTypeEnum.ESRIFeatureServer:
                    return ESRIFeatureServer;
                case ExternalMapLayerTypeEnum.ESRIMapServer:
                    return ESRIMapServer;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum ExternalMapLayerTypeEnum
    {
        ESRIFeatureServer = 1,
        ESRIMapServer = 2
    }

    public partial class ExternalMapLayerTypeESRIFeatureServer : ExternalMapLayerType
    {
        private ExternalMapLayerTypeESRIFeatureServer(int externalMapLayerTypeID, string externalMapLayerTypeName, string externalMapLayerTypeDisplayName) : base(externalMapLayerTypeID, externalMapLayerTypeName, externalMapLayerTypeDisplayName) {}
        public static readonly ExternalMapLayerTypeESRIFeatureServer Instance = new ExternalMapLayerTypeESRIFeatureServer(1, @"ESRIFeatureServer", @"ESRI Feature Server (WFS/vector)");
    }

    public partial class ExternalMapLayerTypeESRIMapServer : ExternalMapLayerType
    {
        private ExternalMapLayerTypeESRIMapServer(int externalMapLayerTypeID, string externalMapLayerTypeName, string externalMapLayerTypeDisplayName) : base(externalMapLayerTypeID, externalMapLayerTypeName, externalMapLayerTypeDisplayName) {}
        public static readonly ExternalMapLayerTypeESRIMapServer Instance = new ExternalMapLayerTypeESRIMapServer(2, @"ESRIMapServer", @"ESRI Map Server (WMS/raster)");
    }
}