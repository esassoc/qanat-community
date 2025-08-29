using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qanat.Models.Security
{
    public class Rights
    {
        public bool? CanRead { get; set; }
        public bool? CanUpdate { get; set; }
        public bool? CanCreate { get; set; }
        public bool? CanDelete { get; set; }

        public static implicit operator Rights(short rightMask)
        {
            return new Rights
            {
                CanCreate = ((RightsEnum)rightMask).HasFlag(RightsEnum.Create),
                CanRead = ((RightsEnum)rightMask).HasFlag(RightsEnum.Read),
                CanUpdate = ((RightsEnum)rightMask).HasFlag(RightsEnum.Update),
                CanDelete = ((RightsEnum)rightMask).HasFlag(RightsEnum.Delete)
            };
        }

        public static implicit operator Rights(long rightMask)
        {
            return new Rights
            {
                CanCreate = ((RightsEnum)rightMask).HasFlag(RightsEnum.Create),
                CanRead = ((RightsEnum)rightMask).HasFlag(RightsEnum.Read),
                CanUpdate = ((RightsEnum)rightMask).HasFlag(RightsEnum.Update),
                CanDelete = ((RightsEnum)rightMask).HasFlag(RightsEnum.Delete)
            };
        }


        public static implicit operator Rights(int rightMask)
        {
            return new Rights
            {
                CanCreate = ((RightsEnum)rightMask).HasFlag(RightsEnum.Create),
                CanRead = ((RightsEnum)rightMask).HasFlag(RightsEnum.Read),
                CanUpdate = ((RightsEnum)rightMask).HasFlag(RightsEnum.Update),
                CanDelete = ((RightsEnum)rightMask).HasFlag(RightsEnum.Delete)
            };
        }

        public static implicit operator short(Rights rights)
        {
            if (rights.CanCreate == null || rights.CanRead == null || rights.CanUpdate == null || rights.CanDelete == null)
            {
                throw new ArgumentException("Rights must have all permissions defined.");
            }

            var mask = 0;
            if (rights.CanCreate.Value)
                mask = mask | (int)RightsEnum.Create;
            if (rights.CanRead.Value)
                mask = mask | (int)RightsEnum.Read;
            if (rights.CanUpdate.Value)
                mask = mask | (int)RightsEnum.Update;
            if (rights.CanDelete.Value)
                mask = mask | (int)RightsEnum.Delete;

            return (short)mask;
        }
    }

    /**
     * Example of rights assignments:
     * CRUD = (C = 0b00100 = 4) + (R = 0b00001 = 1) + (U = 0b00010 = 2) + (D = 0b01000 = 8) = 0b00111 = 15
     * CRU  = (C = 0b00100 = 4) + (R = 0b00001 = 1) + (U = 0b00010 = 2) = 0b00111 = 7
     * CR   = (C = 0b00100 = 4) + (R = 0b00001 = 1) = 0b00101 = 5
     * R    = (R = 0b00001 = 1) = 1
     *
     * etc..
     */
    [Flags]
    public enum RightsEnum
    {
        None = 0b00000,   // 0
        Read = 0b00001,   // 1
        Update = 0b00010, // 2
        Create = 0b00100, // 4
        Delete = 0b01000  // 8
    }

    public class RightsConverter : JsonConverter<Rights>
    {
        public override Rights Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }
            else
            {
                return new Rights();
            }
        }

        public override void Write(Utf8JsonWriter writer, Rights value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }
            writer.WriteNumberValue(value);
        }
    }
}