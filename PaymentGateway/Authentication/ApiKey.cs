using System;

namespace PaymentGateway.Authentication
{
    public class ApiKey
    {
        public ApiKey(int id, string owner, bool isActive)
        {
            Id = id;
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            IsActive = isActive;
        }

        public int Id { get; }
        
        public string Owner { get; }

        public bool IsActive { get; }
    }
}
