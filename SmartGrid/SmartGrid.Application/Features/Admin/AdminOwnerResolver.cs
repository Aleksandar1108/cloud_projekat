using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.Admin
{
    internal static class AdminOwnerResolver
    {
        public static string ResolveName(User? user)
        {
            if (user is null)
            {
                return "Nepoznat korisnik";
            }

            var email = user.Email.Value;
            var atIndex = email.IndexOf('@');
            if (atIndex <= 0)
            {
                return email;
            }

            return email[..atIndex].Replace('.', ' ');
        }
    }
}
