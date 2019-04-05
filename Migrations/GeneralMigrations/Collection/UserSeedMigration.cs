using Infrastructure.Enums;
using Microsoft.Extensions.DependencyInjection;
using MRMigrationMaster.Infrastructure.Attr;
using MRMigrationMaster.Infrastructure.Component;
using MRMigrationMaster.Infrastructure.Interface;
using MRMongoTools.Extensions.Identity.Component;
using MRMongoTools.Extensions.Identity.Interface;
using MRMongoTools.Extensions.Identity.Manager;
using MRMongoTools.Extensions.Identity.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralMigrations.Collection
{
    [MigrationAttr("User / Roles Seed", "04.04.2019")]
    class UserSeedMigration : Migration, IMigration
    {
        List<UserToSeed> UsersToSeed = new List<UserToSeed>
        {
            new UserToSeed
            {
                User = new MRUser
                {
                    Email = "oleg.timofeev20@gmail.com",
                    NormalizedEmail = "OLEGT.TIMOFEEV20@GMAIL.COM",
                    UserName = "oleg.timofeev20@gmail.com",
                    NormalizedUserName = "OLEGT.TIMOFEEV20@GMAIL.COM",
                    FirstName = "Oleh",
                    LastName = "Tymofieiev",
                    Sex = MRMongoTools.Extensions.Identity.Enum.UserSex.MALE,
                    IsEmailConfirmed = true
                },
                Password = "Tf27324_()_",
                Roles = new List<UserRoles>()
                {
                    UserRoles.ADMIN,
                    UserRoles.USER
                }
            }
        };

        public override async Task Action()
        {
            var provider = _services.BuildServiceProvider();

            var userRepository = provider.GetRequiredService<IMRUserStore>();
            var userManager = provider.GetRequiredService<MRUserManager>();
            var roleManager = provider.GetRequiredService<MRRoleManager>();

            Log("Seed roles", MRMigrationMaster.Infrastructure.Enum.LogType.INFO);
            List<MRRole> systemRoles = new List<MRRole>();

            foreach (var role in Enum.GetNames(typeof(UserRoles)))
            {
                MRRole exists;

                if (await roleManager.RoleExistsAsync(role))
                {
                    exists = await roleManager.FindByNameAsync(role);
                }
                else
                {
                    exists = new MRRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpperInvariant(),
                    };

                    await roleManager.CreateAsync(exists);
                }

                systemRoles.Add(exists);
            }

            Log($"Start seeding users. Total to seed: {UsersToSeed.Count}.", MRMigrationMaster.Infrastructure.Enum.LogType.NONE);

            int seeded = 0;
            int skiped = 0;

            foreach (var userObj in UsersToSeed)
            {
                var toSeedUser = userObj.User;
                var toSeedPassword = userObj.Password;
                var toSeedRoles = userObj.Roles;

                MRUser exists;

                if (await userRepository.Any(x => x.NormalizedEmail == toSeedUser.NormalizedEmail))
                {
                    exists = await userManager.FindByEmailAsync(toSeedUser.Email);
                    skiped++;
                }
                else
                {
                    exists = toSeedUser;
                    exists.SecurityStamp = "SS";
                    await userManager.CreateAsync(exists, toSeedPassword);
                    LogU(exists);
                    seeded++;
                }

                var roles = await userManager.GetRolesAsync(exists);

                var toDelete = roles.Where(x => !toSeedRoles.Any(z => z.ToString() == x));
                var toAdd = toSeedRoles.Where(x => !roles.Any(z => z == x.ToString()))?.Select(x => x.ToString());

                if (toDelete != null && toDelete.Any())
                {
                    await userManager.RemoveFromRolesAsync(exists, toDelete);
                }
                if (toAdd != null && toAdd.Any())
                {
                    await userManager.AddToRolesAsync(exists, toAdd);
                }
            }

            Log($"Seeded {seeded} users. (Skiped {skiped})", MRMigrationMaster.Infrastructure.Enum.LogType.INFO);
        }

        protected void LogU(MRUser user)
        {
            Log($"-- {user.Email}", MRMigrationMaster.Infrastructure.Enum.LogType.INFO);
        }


    }

    public class UserToSeed
    {
        public MRUser User { get; set; }
        public string Password { get; set; }
        public List<UserRoles> Roles { get; set; }
    }
}
