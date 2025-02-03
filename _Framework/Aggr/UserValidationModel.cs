namespace _Framework.Aggr
{
    public class UserValidationModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public List<UserAddressValidation>? Address { get; set; }
        public int Age { get; set; }
    }
}
