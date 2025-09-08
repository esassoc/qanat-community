using System.ComponentModel.DataAnnotations.Schema;

namespace Qanat.EFModels.Entities
{
    public partial class User
    {
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public string Rights => Role.Rights;

        [NotMapped]
        public string Flags => Role.Flags;

        // MCS: This method is currently not used by application code, but remains because it is used by integration tests to create new users for testing
    }
}