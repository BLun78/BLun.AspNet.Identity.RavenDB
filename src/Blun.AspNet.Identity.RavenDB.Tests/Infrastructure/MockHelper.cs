using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NSubstitute;
using NUnit.Framework.Constraints;
using RavenDB.AspNet.Identity;

namespace Blun.AspNet.Identity.RavenDB.Tests.Infrastructure
{
    public static class MockHelper
    {
        #region nsubstitut

        public static IIdentityValidator<TUser> CreateIdentityValidator<TUser, TKey>()
            where TUser : IdentityUser<TKey>
            where TKey : IConvertible, IComparable, IEquatable<TKey>
        {
            var substitute = Substitute.For<IIdentityValidator<IUser<TKey>>>();

            substitute.ValidateAsync(null).ReturnsForAnyArgs(Task.FromResult(IdentityResult.Success));

            return substitute;
        }

        public static IUserTokenProvider<TUser, TKey> CreateUserTokenProvider<TUser, TKey>()
            where TUser : IdentityUser<TKey>
            where TKey : IConvertible, IComparable, IEquatable<TKey>
        {
            var substitute = Substitute.For<IUserTokenProvider<TUser, TKey>>();

            substitute.IsValidProviderForUserAsync(null, null).ReturnsForAnyArgs(Task.FromResult(true));
            substitute.NotifyAsync("", null, null).ReturnsForAnyArgs(Task.FromResult(0));
            substitute.GenerateAsync("", null, null).ReturnsForAnyArgs(Task.FromResult("GeneratedToken"));
            substitute.ValidateAsync("", "", null, null).ReturnsForAnyArgs(Task.FromResult(true));

            return substitute;
        }

        #endregion
    }
}
