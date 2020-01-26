﻿using System.Linq;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace PaymentGateway.Tests
{
    internal static class SqlExceptionFactory
    {
        internal static SqlException Create(int number = 1)
        {
            var collection = Construct<SqlErrorCollection>();
            var error = Construct<SqlError>(number, (byte)2, (byte)3, "server name", "error message", "proc", 100, null);

            typeof(SqlErrorCollection)
                .GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(collection, new object[] { error });
            
            return typeof(SqlException)
                .GetMethod("CreateException", BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    CallingConventions.ExplicitThis,
                    new[] { typeof(SqlErrorCollection), typeof(string) },
                    new ParameterModifier[] { })
                .Invoke(null, new object[] { collection, "7.0.0" }) as SqlException;
        }

        private static T Construct<T>(params object[] p)
        {
            var ctors = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)ctors.First(ctor => ctor.GetParameters().Length == p.Length).Invoke(p);
        }
    }
}
