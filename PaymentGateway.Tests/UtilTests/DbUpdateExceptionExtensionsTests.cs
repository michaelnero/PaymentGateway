using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentGateway.Util;

namespace PaymentGateway.Tests.UtilTests
{
    [TestClass]
    public class DbUpdateExceptionExtensionsTests
    {
        [TestClass]
        public class IsViolationOfUniqueIndexTests
        {
            [TestMethod]
            public void Null_Returns_False()
            {
                var result = DbUpdateExceptionExtensions.IsViolationOfUniqueIndex(null);

                Assert.IsFalse(result);
            }

            [TestMethod]
            public void InnerException_Not_SqlException_Returns_False()
            {
                var exception = new DbUpdateException("", new Exception());
                
                var result = exception.IsViolationOfUniqueIndex();
                
                Assert.IsFalse(result);
            }

            [TestMethod]
            public void InnerException_SqlException_Not_Code_2601_Returns_False()
            {
                var sqlException = SqlExceptionFactory.Create(1);
                var exception = new DbUpdateException("", sqlException);

                var result = exception.IsViolationOfUniqueIndex();

                Assert.IsFalse(result);
            }

            [TestMethod]
            public void InnerException_SqlException_Code_2601_Returns_False()
            {
                var sqlException = SqlExceptionFactory.Create(2601);
                var exception = new DbUpdateException("", sqlException);

                var result = exception.IsViolationOfUniqueIndex();

                Assert.IsTrue(result);
            }
        }

        [TestClass]
        public class IsViolationOfUniqueConstraintTests
        {
            [TestMethod]
            public void Null_Returns_False()
            {
                var result = DbUpdateExceptionExtensions.IsViolationOfUniqueConstraint(null);

                Assert.IsFalse(result);
            }

            [TestMethod]
            public void InnerException_Not_SqlException_Returns_False()
            {
                var exception = new DbUpdateException("", new Exception());

                var result = exception.IsViolationOfUniqueConstraint();

                Assert.IsFalse(result);
            }

            [TestMethod]
            public void InnerException_SqlException_Not_Code_2627_Returns_False()
            {
                var sqlException = SqlExceptionFactory.Create(1);
                var exception = new DbUpdateException("", sqlException);

                var result = exception.IsViolationOfUniqueConstraint();

                Assert.IsFalse(result);
            }

            [TestMethod]
            public void InnerException_SqlException_Code_2627_Returns_False()
            {
                var sqlException = SqlExceptionFactory.Create(2627);
                var exception = new DbUpdateException("", sqlException);

                var result = exception.IsViolationOfUniqueConstraint();

                Assert.IsTrue(result);
            }
        }
    }
}
