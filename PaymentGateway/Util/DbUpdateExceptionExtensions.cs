using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace PaymentGateway.Util
{
    public static class DbUpdateExceptionExtensions
    {
        public static bool IsViolationOfUniqueIndex(this DbUpdateException exception) => (exception?.InnerException as SqlException) switch
        {
            { Number: 2601 } => true,
            _ => false
        };

        public static bool IsViolationOfUniqueConstraint(this DbUpdateException exception) => (exception?.InnerException as SqlException) switch
        {
            { Number: 2627 } => true,
            _ => false
        };
    }
}
