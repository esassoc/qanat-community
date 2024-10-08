//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ContentType]
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Qanat.Models.DataTransferObjects;


namespace Qanat.EFModels.Entities
{
    public abstract partial class ContentType : IHavePrimaryKey
    {
        public static readonly ContentTypePageInstructions PageInstructions = ContentTypePageInstructions.Instance;
        public static readonly ContentTypeFormInstructions FormInstructions = ContentTypeFormInstructions.Instance;
        public static readonly ContentTypeFieldDefinition FieldDefinition = ContentTypeFieldDefinition.Instance;

        public static readonly List<ContentType> All;
        public static readonly List<ContentTypeSimpleDto> AllAsSimpleDto;
        public static readonly ReadOnlyDictionary<int, ContentType> AllLookupDictionary;
        public static readonly ReadOnlyDictionary<int, ContentTypeSimpleDto> AllAsSimpleDtoLookupDictionary;

        /// <summary>
        /// Static type constructor to coordinate static initialization order
        /// </summary>
        static ContentType()
        {
            All = new List<ContentType> { PageInstructions, FormInstructions, FieldDefinition };
            AllAsSimpleDto = new List<ContentTypeSimpleDto> { PageInstructions.AsSimpleDto(), FormInstructions.AsSimpleDto(), FieldDefinition.AsSimpleDto() };
            AllLookupDictionary = new ReadOnlyDictionary<int, ContentType>(All.ToDictionary(x => x.ContentTypeID));
            AllAsSimpleDtoLookupDictionary = new ReadOnlyDictionary<int, ContentTypeSimpleDto>(AllAsSimpleDto.ToDictionary(x => x.ContentTypeID));
        }

        /// <summary>
        /// Protected constructor only for use in instantiating the set of static lookup values that match database
        /// </summary>
        protected ContentType(int contentTypeID, string contentTypeName, string contentTypeDisplayName)
        {
            ContentTypeID = contentTypeID;
            ContentTypeName = contentTypeName;
            ContentTypeDisplayName = contentTypeDisplayName;
        }
        public List<CustomRichTextType> CustomRichTextTypes => CustomRichTextType.All.Where(x => x.ContentTypeID == ContentTypeID).ToList();
        [Key]
        public int ContentTypeID { get; private set; }
        public string ContentTypeName { get; private set; }
        public string ContentTypeDisplayName { get; private set; }
        [NotMapped]
        public int PrimaryKey { get { return ContentTypeID; } }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public bool Equals(ContentType other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ContentTypeID == ContentTypeID;
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ContentType);
        }

        /// <summary>
        /// Enum types are equal by primary key
        /// </summary>
        public override int GetHashCode()
        {
            return ContentTypeID;
        }

        public static bool operator ==(ContentType left, ContentType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ContentType left, ContentType right)
        {
            return !Equals(left, right);
        }

        public ContentTypeEnum ToEnum => (ContentTypeEnum)GetHashCode();

        public static ContentType ToType(int enumValue)
        {
            return ToType((ContentTypeEnum)enumValue);
        }

        public static ContentType ToType(ContentTypeEnum enumValue)
        {
            switch (enumValue)
            {
                case ContentTypeEnum.FieldDefinition:
                    return FieldDefinition;
                case ContentTypeEnum.FormInstructions:
                    return FormInstructions;
                case ContentTypeEnum.PageInstructions:
                    return PageInstructions;
                default:
                    throw new ArgumentException("Unable to map Enum: {enumValue}");
            }
        }
    }

    public enum ContentTypeEnum
    {
        PageInstructions = 1,
        FormInstructions = 2,
        FieldDefinition = 3
    }

    public partial class ContentTypePageInstructions : ContentType
    {
        private ContentTypePageInstructions(int contentTypeID, string contentTypeName, string contentTypeDisplayName) : base(contentTypeID, contentTypeName, contentTypeDisplayName) {}
        public static readonly ContentTypePageInstructions Instance = new ContentTypePageInstructions(1, @"PageInstructions", @"Page Instructions");
    }

    public partial class ContentTypeFormInstructions : ContentType
    {
        private ContentTypeFormInstructions(int contentTypeID, string contentTypeName, string contentTypeDisplayName) : base(contentTypeID, contentTypeName, contentTypeDisplayName) {}
        public static readonly ContentTypeFormInstructions Instance = new ContentTypeFormInstructions(2, @"FormInstructions", @"Form Instructions");
    }

    public partial class ContentTypeFieldDefinition : ContentType
    {
        private ContentTypeFieldDefinition(int contentTypeID, string contentTypeName, string contentTypeDisplayName) : base(contentTypeID, contentTypeName, contentTypeDisplayName) {}
        public static readonly ContentTypeFieldDefinition Instance = new ContentTypeFieldDefinition(3, @"FieldDefinition", @"Field Definition");
    }
}