using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blun.AspNet.Identity.RavenDB.Tests.Infrastructure;
using Blun.AspNet.Identity.RavenDB.Tests.Models;
using Microsoft.AspNet.Identity;
using NSubstitute;
using NUnit.Framework;
using Raven.Client;
using RavenDB.AspNet.Identity;

// ReSharper disable once CheckNamespace
namespace Blun.AspNet.Identity.RavenDB.Tests
{
    [TestFixture]
    public class UserEmailStoreTest : BaseTest<SimpleUser>
    {
        public UserEmailStoreTest()
        {
            _cleanUpRavenDBAktion = this.ClaenUpRavenDb;
        }

        public async Task ClaenUpRavenDb()
        {
            await DeleteUsers();
        }
        private static async Task CreatUsers(IAsyncDocumentSession session, string prefix)
        {
            await session.StoreAsync(new SimpleUser() { UserName = prefix + "_" + "Admin" });
            await session.StoreAsync(new SimpleUser() { UserName = prefix + "_" + "Benutzer" });
            await session.StoreAsync(new SimpleUser() { UserName = prefix + "_" + "Reader" });
            await session.StoreAsync(new SimpleUser() { UserName = prefix + "_" + "Writer" });

            await session.SaveChangesAsync();

            await Task.Delay(_delay);
        }

        private async Task DeleteUsers(string prefix = "")
        {
            IList<SimpleUser> ids = new List<SimpleUser>();
            if (string.IsNullOrWhiteSpace(prefix))
            {
                ids = await GetSession().Query<SimpleUser>().ToListAsync();
            }
            else
            {
                ids = await GetSession().Query<SimpleUser>().Where(x => x.UserName.StartsWith(prefix) || x.UserName == prefix).ToListAsync();
            }

            foreach (var id in ids)
            {
                GetSession().Delete<SimpleUser>(id);
            }
            await GetSession().SaveChangesAsync();
        }

        #region IUserEmailStore

        [Test]
        public async Task IUserEmailStore_FindByEmailAsync_Email_User_True()
        {
            // Arrange
            string name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name,
                Email = name
            });
            SimpleUser userResult = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                userResult = await mgr.FindByEmailAsync(name);
            }

            // Assert
            Assert.IsInstanceOf<SimpleUser>(userResult);
            Assert.AreEqual(name, userResult.Email);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserEmailStore_FindByEmailAsync_Email_User_False()
        {
            // Arrange
            string name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name,
                Email = name
            });
            SimpleUser userResult = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                userResult = await mgr.FindByEmailAsync(name + "1");
            }

            // Assert
            Assert.IsNull(userResult);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserEmailStore_GetEmailAsync_User_Email_True()
        {
            // Arrange
            string name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name,
                Email = name
            });
            var email = string.Empty;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                email = await mgr.GetEmailAsync(user.Id);
            }

            // Assert
            Assert.AreEqual(user.Email, email);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserEmailStore_GetEmailAsync_User_Email_Exception()
        {
            // Arrange
            string name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name,
                Email = name
            });
            var email = string.Empty;
            Exception e = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                try
                {
                    email = await mgr.GetEmailAsync(user.Id + "1dewrvevwre65443fd436546f456543f643");
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }

            // Assert
            Assert.IsInstanceOf<InvalidOperationException>(e);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserEmailStore_GetEmailConfirmedAsync_User_Email_True()
        {
            // Arrange
            string name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name,
                Email = name,
                IsEmailConfirmed = true
            });
            var result = false;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                result = await mgr.IsEmailConfirmedAsync(user.Id);
            }

            // Assert
            Assert.True(result);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserEmailStore_GetEmailConfirmedAsync_User_Email_False()
        {
            // Arrange
            string name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name,
                Email = name,
                IsEmailConfirmed = false
            });
            var result = true;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                result = await mgr.IsEmailConfirmedAsync(user.Id);
            }

            // Assert
            Assert.IsFalse(result);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserEmailStore_GetEmailConfirmedAsync_User_Email_Exception()
        {
            // Arrange
            string name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name,
                Email = name,
                IsEmailConfirmed = true
            });
            var result = false;
            Exception e = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                try
                {
                    result = await mgr.IsEmailConfirmedAsync(user.Id + "1dewrvevwre65443fd436546f456543f643");
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }

            // Assert
            Assert.IsFalse(result);
            Assert.IsInstanceOf<InvalidOperationException>(e);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserEmailStore_SetEmailAsync_User_Email_True()
        {
            // Arrange
            string name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name,
                Email = name,
                IsEmailConfirmed = true
            });
            IdentityResult result;
            var newEmail = "new_Email@mail.com";

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                result = await mgr.SetEmailAsync(user.Id, newEmail);
            }

            // Assert
            var userAssert = await GetSession().LoadAsync<SimpleUser>(user.Id);
            Assert.AreEqual(newEmail, userAssert.Email);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserEmailStore_SetEmailAsync_User_Empty_True()
        {
            // Arrange
            string name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name,
                Email = name,
                IsEmailConfirmed = true
            });
            IdentityResult result;
            var newEmail = "";

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                result = await mgr.SetEmailAsync(user.Id, newEmail);
            }

            // Assert
            var userAssert = await GetSession().LoadAsync<SimpleUser>(user.Id);
            Assert.AreEqual(newEmail, userAssert.Email);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserEmailStore_SetEmailAsync_User_Null_True()
        {
            // Arrange
            string name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name,
                Email = name,
                IsEmailConfirmed = true
            });
            IdentityResult result;
            string newEmail = null;
            Exception e = null;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true }))
            {
                try
                {
                    result = await mgr.SetEmailAsync(user.Id, newEmail);
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }

            // Assert
            Assert.IsInstanceOf<ArgumentNullException>(e);

            // TearDown
            await base.Delete(GetSession(), user);
        }

        [Test]
        public async Task IUserEmailStore_SetEmailConfirmedAsync_User_Email_True()
        {
            // Arrange
            string name = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName;
            var user = await StoreAsync(GetSession(), new SimpleUser()
            {
                UserName = name,
                Email = name,
                IsEmailConfirmed = true
            });
            IdentityResult result;

            // Act
            using (var mgr = new UserManager<SimpleUser>(new UserStore<SimpleUser, SimpleRole>(GetSession()) { AutoSaveChanges = true })
            {
                UserValidator = MockHelper.CreateIdentityValidator<SimpleUser,string>(),
                UserTokenProvider = MockHelper.CreateUserTokenProvider<SimpleUser, string>()
            })
            {
                result = await mgr.ConfirmEmailAsync(user.Id, "wzdewfr3745ewf");
            }

            // Assert
            Assert.True(result.Succeeded);
            Assert.IsEmpty("TODO");

            // TearDown
            await base.Delete(GetSession(), user);
        }

        #endregion

       
    }
}
