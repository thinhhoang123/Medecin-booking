using System;
using System.Collections.Generic;
using System.Text;

namespace Doctor.Domain.ValueObjects
{
    public class ContactInfo
    {
        public string Email { get; }
        public string PhoneNumber { get; }
        public string? MobileNumber { get; }
        public string? Address { get; }
        public ContactInfo(
           string email,
           string phoneNumber,
           string? mobileNumber = null,
           string? address = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is required");

            if (!IsValidEmail(email))
                throw new DomainException("Invalid email format");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Phone number is required");

            Email = email;
            PhoneNumber = phoneNumber;
            MobileNumber = mobileNumber;
            Address = address;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool Equals(ContactInfo? other)
        {
            if (other is null) return false;
            return Email == other.Email && PhoneNumber == other.PhoneNumber;
        }

        public override bool Equals(object? obj)
        {
            return obj is ContactInfo info && Equals(info);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Email, PhoneNumber);
        }
    }
}
