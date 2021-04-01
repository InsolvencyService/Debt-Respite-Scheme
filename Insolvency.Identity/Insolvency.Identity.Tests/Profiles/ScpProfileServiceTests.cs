using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using Insolvency.Identity.Models;
using Insolvency.Identity.Models.Claims;
using Insolvency.Identity.Models.Claims.Types;
using Insolvency.Identity.Profiles.ProfileServices;
using Insolvency.Interfaces.IdentityManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Insolvency.Identity.Tests.Profiles
{
    [TestClass]
    public class ScpProfileServiceTests
    {
        private IFixture _fixture;
        private Mock<IIdentityManagementRepository> _mockIdentityRepo;
        private const string ScpGroupId = "1234567";
        private const string Email = "admin@contoso.com";

        private List<Organisation> NoOrganisations;
        private List<Organisation> OneOrganisation;
        private List<Organisation> MultipleOrganisations;

        private List<RoleUser> NoRoles;
        private List<RoleUser> RoleMatchingOneOrganisation;
        private List<RoleUser> MultipleRolesMatchingMultipleOrganisations;
        private List<RoleUser> MultipleRolesMatchingOneOrganisation;

        private List<RoleUser> MultipleRolesDifferentEmail;
        private List<RoleUser> MultipleRolesNotMatchingAnyOrganisation;
        private List<RoleUser> DuplicateRolesMatchingOneOrganisation;
        private List<RoleUser> MultipleRolesMatchingSomeOrganisations;


        [TestInitialize]
        public void SetUp()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            NoOrganisations = new List<Organisation>();
            OneOrganisation = new List<Organisation>()
        {
            new Organisation() { ScpGroupId = ScpGroupId, ExternalId = "EXT0", Id = Guid.NewGuid(), Type = "TYPE0", Name = "N1"  }
        };
            MultipleOrganisations = new List<Organisation>()
        {
            OneOrganisation[0],
            new Organisation() { ScpGroupId = ScpGroupId, ExternalId = "EXT1", Id = Guid.NewGuid(), Type = "TYPE1", Name = "N2"  },
            new Organisation() { ScpGroupId = ScpGroupId, ExternalId = "EXT2", Id = Guid.NewGuid(), Type = "TYPE2", Name = "N3"  }
        };

            NoRoles = new List<RoleUser>();
            RoleMatchingOneOrganisation = new List<RoleUser>()
        {
            new RoleUser() { Email = Email, OrganisationId = OneOrganisation[0].Id, Role = RoleType.Administrator }
        };

            MultipleRolesMatchingMultipleOrganisations = new List<RoleUser>()
        {
            new RoleUser() { Email = Email, OrganisationId = MultipleOrganisations[0].Id, Role = RoleType.Administrator },
            new RoleUser() { Email = Email, OrganisationId = MultipleOrganisations[1].Id, Role = RoleType.Administrator },
            new RoleUser() { Email = Email, OrganisationId = MultipleOrganisations[2].Id, Role = RoleType.Administrator },
            new RoleUser() { Email = Email, OrganisationId = MultipleOrganisations[2].Id, Role = RoleType.Administrator },
            new RoleUser() { Email = Email, OrganisationId = MultipleOrganisations[2].Id, Role = RoleType.Developer },
            new RoleUser() { Email = Email, OrganisationId = Guid.NewGuid(), Role = RoleType.Administrator },
            new RoleUser() { Email = Guid.NewGuid().ToString(), OrganisationId = Guid.NewGuid(), Role = RoleType.Administrator },
            new RoleUser() { Email = Guid.NewGuid().ToString(), OrganisationId = MultipleOrganisations[2].Id, Role = RoleType.Administrator }
        };

            MultipleRolesMatchingOneOrganisation = new List<RoleUser>()
        {
            new RoleUser() { Email = Email, OrganisationId = OneOrganisation[0].Id, Role = RoleType.Administrator },
            new RoleUser() { Email = Email, OrganisationId = OneOrganisation[0].Id, Role = RoleType.Developer }
        };

            MultipleRolesDifferentEmail = new List<RoleUser>()
        {
            new RoleUser() { Email = Guid.NewGuid().ToString(), OrganisationId = OneOrganisation[0].Id, Role = RoleType.Administrator },
            new RoleUser() { Email = Guid.NewGuid().ToString(), OrganisationId = OneOrganisation[0].Id, Role = RoleType.Developer }
        };

            MultipleRolesNotMatchingAnyOrganisation = new List<RoleUser>()
        {
            new RoleUser() { Email = Email, OrganisationId = Guid.NewGuid(), Role = RoleType.Administrator },
            new RoleUser() { Email = Email, OrganisationId = Guid.NewGuid(), Role = RoleType.Developer }
        };

            DuplicateRolesMatchingOneOrganisation = new List<RoleUser>()
        {
            new RoleUser() { Email = Email, OrganisationId = OneOrganisation[0].Id, Role = RoleType.Administrator },
            new RoleUser() { Email = Email, OrganisationId = OneOrganisation[0].Id, Role = RoleType.Administrator },
            new RoleUser() { Email = Email, OrganisationId = OneOrganisation[0].Id, Role = RoleType.Developer }
        };

            MultipleRolesMatchingSomeOrganisations = new List<RoleUser>()
        {
            new RoleUser() { Email = Email, OrganisationId = MultipleOrganisations[0].Id, Role = RoleType.Administrator },
            new RoleUser() { Email = Email, OrganisationId = MultipleOrganisations[2].Id, Role = RoleType.Administrator }
        };
        }


        public void ConfigureRepositoryWith(List<Organisation> organisations, List<RoleUser> roles)
        {
            _mockIdentityRepo = _fixture.Freeze<Mock<IIdentityManagementRepository>>();
            _mockIdentityRepo.Setup(p => p.GetOrganisationByScpGroupIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string groupId) =>
                {
                    organisations.ForEach(o => o.RoleUsers = roles.Where(ru => ru.OrganisationId == o.Id).ToList());
                    return organisations.Where(o => o.ScpGroupId == groupId).ToList();
                });
        }

        public ClaimsPrincipal GetSubject()
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                        new List<Claim>()
                        {
                            new Claim(JwtClaimTypes.Subject, "1234567"),
                            new Claim(JwtClaimTypes.Name, "NAME"),
                            new Claim(JwtClaimTypes.Email, Email),
                            new Claim(ScpClaimTypes.GroupId, ScpGroupId)
                        }
                    )
                );
        }

        public ScpProfileService CreateSut()
        {
            return _fixture.Create<ScpProfileService>();
        }

        [TestMethod]
        public async Task GetProfileDataAsync_WithNoOrganisationsAndNoRoles_ReturnsNoClaims()
        {
            // Arrange
            ConfigureRepositoryWith(NoOrganisations, NoRoles);
            var context = new ProfileDataRequestContext();
            context.RequestedClaimTypes = new List<string>();
            context.Subject = GetSubject();


            // Act
            var sut = CreateSut();

            // Assert
            await sut.GetProfileDataAsync(context);
            context.IssuedClaims.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task GetProfileDataAsync_WithOneOrganisationAndRole_ReturnsOneClaim()
        {
            // Arrange
            ConfigureRepositoryWith(OneOrganisation, RoleMatchingOneOrganisation);
            var context = new ProfileDataRequestContext();
            context.RequestedClaimTypes = new List<string>() { InssClaimTypes.Organisation };
            context.Subject = GetSubject();


            // Act
            var sut = CreateSut();

            // Assert
            await sut.GetProfileDataAsync(context);
            context.IssuedClaims.Count.Should().Be(1);
            var orgClaim = context.IssuedClaims.Select(c => OrganisationClaimType.FromClaim(c)).ToList()[0];
            orgClaim.Id.Should().Be(OneOrganisation[0].ExternalId);
            orgClaim.Name.Should().Be(OneOrganisation[0].Name);
            orgClaim.OrganisationTypeName.Should().Be(OneOrganisation[0].Type);
            orgClaim.CurrentUserRoles.Should().BeEquivalentTo(RoleMatchingOneOrganisation.Select(r => r.Role));
        }

        [TestMethod]
        public async Task GetProfileDataAsync_WithOneOrganisationWithoutRoles_ReturnsOneClaim()
        {
            // Arrange
            ConfigureRepositoryWith(OneOrganisation, NoRoles);
            var context = new ProfileDataRequestContext();
            context.RequestedClaimTypes = new List<string>() { InssClaimTypes.Organisation };
            context.Subject = GetSubject();


            // Act
            var sut = CreateSut();

            // Assert
            await sut.GetProfileDataAsync(context);
            context.IssuedClaims.Count.Should().Be(1);
            var orgClaim = context.IssuedClaims.Select(c => OrganisationClaimType.FromClaim(c)).ToList()[0];
            orgClaim.Id.Should().Be(OneOrganisation[0].ExternalId);
            orgClaim.Name.Should().Be(OneOrganisation[0].Name);
            orgClaim.OrganisationTypeName.Should().Be(OneOrganisation[0].Type);
            orgClaim.CurrentUserRoles.Should().BeEquivalentTo(NoRoles.Select(r => r.Role));
        }

        [TestMethod]
        public async Task GetProfileDataAsync_WithOneOrganisationMultipleRoles_ReturnsOneClaim()
        {
            // Arrange
            ConfigureRepositoryWith(OneOrganisation, MultipleRolesMatchingOneOrganisation);
            var context = new ProfileDataRequestContext();
            context.RequestedClaimTypes = new List<string>() { InssClaimTypes.Organisation };
            context.Subject = GetSubject();


            // Act
            var sut = CreateSut();

            // Assert
            await sut.GetProfileDataAsync(context);
            context.IssuedClaims.Count.Should().Be(1);
            var orgClaim = context.IssuedClaims.Select(c => OrganisationClaimType.FromClaim(c)).ToList()[0];
            orgClaim.Id.Should().Be(OneOrganisation[0].ExternalId);
            orgClaim.Name.Should().Be(OneOrganisation[0].Name);
            orgClaim.OrganisationTypeName.Should().Be(OneOrganisation[0].Type);
            orgClaim.CurrentUserRoles.Should().BeEquivalentTo(MultipleRolesMatchingOneOrganisation.Select(r => r.Role));
        }

        [TestMethod]
        public async Task GetProfileDataAsync_WithOneOrganisationDuplicateRoles_ReturnsOneClaim()
        {
            // Arrange
            ConfigureRepositoryWith(OneOrganisation, DuplicateRolesMatchingOneOrganisation);
            var context = new ProfileDataRequestContext();
            context.RequestedClaimTypes = new List<string>() { InssClaimTypes.Organisation };
            context.Subject = GetSubject();


            // Act
            var sut = CreateSut();

            // Assert
            await sut.GetProfileDataAsync(context);
            context.IssuedClaims.Count.Should().Be(1);
            var orgClaim = context.IssuedClaims.Select(c => OrganisationClaimType.FromClaim(c)).ToList()[0];
            orgClaim.Id.Should().Be(OneOrganisation[0].ExternalId);
            orgClaim.Name.Should().Be(OneOrganisation[0].Name);
            orgClaim.OrganisationTypeName.Should().Be(OneOrganisation[0].Type);
            orgClaim.CurrentUserRoles.Should().BeEquivalentTo(DuplicateRolesMatchingOneOrganisation.Select(r => r.Role).Distinct());
        }

        [TestMethod]
        public async Task GetProfileDataAsync_WithMultipleOrganisationNoRoles_ReturnsClaims()
        {
            // Arrange
            ConfigureRepositoryWith(MultipleOrganisations, NoRoles);
            var context = new ProfileDataRequestContext();
            context.RequestedClaimTypes = new List<string>() { InssClaimTypes.Organisation };
            context.Subject = GetSubject();


            // Act
            var sut = CreateSut();

            // Assert
            await sut.GetProfileDataAsync(context);
            context.IssuedClaims.Count.Should().Be(MultipleOrganisations.Count);
            foreach (var claim in context.IssuedClaims)
            {
                var orgClaim = OrganisationClaimType.FromClaim(claim);
                var organisation = MultipleOrganisations.Where(o => o.ExternalId == orgClaim.Id).SingleOrDefault();
                organisation.Should().NotBeNull();
                orgClaim.Id.Should().Be(organisation.ExternalId);
                orgClaim.Name.Should().Be(organisation.Name);
                orgClaim.OrganisationTypeName.Should().Be(organisation.Type);
                orgClaim.CurrentUserRoles.Should().BeEquivalentTo(NoRoles.Select(r => r.Role));
            }
        }

        [TestMethod]
        public async Task GetProfileDataAsync_WithMultipleOrganisationOneMatchingRole_ReturnsClaims()
        {
            // Arrange
            ConfigureRepositoryWith(MultipleOrganisations, RoleMatchingOneOrganisation);
            var context = new ProfileDataRequestContext();
            context.RequestedClaimTypes = new List<string>() { InssClaimTypes.Organisation };
            context.Subject = GetSubject();


            // Act
            var sut = CreateSut();

            // Assert
            await sut.GetProfileDataAsync(context);
            context.IssuedClaims.Count.Should().Be(MultipleOrganisations.Count);
            foreach (var claim in context.IssuedClaims)
            {
                var orgClaim = OrganisationClaimType.FromClaim(claim);
                var organisation = MultipleOrganisations.Where(o => o.ExternalId == orgClaim.Id).SingleOrDefault();
                organisation.Should().NotBeNull();

                var roles = RoleMatchingOneOrganisation.Where(r => r.OrganisationId == organisation.Id).Select(r => r.Role).Distinct();

                orgClaim.Id.Should().Be(organisation.ExternalId);
                orgClaim.Name.Should().Be(organisation.Name);
                orgClaim.OrganisationTypeName.Should().Be(organisation.Type);
                orgClaim.CurrentUserRoles.Should().BeEquivalentTo(roles);
            }
        }

        [TestMethod]
        public async Task GetProfileDataAsync_WithMultipleOrganisationMultipleMatchingRoles_ReturnsClaims()
        {
            // Arrange
            ConfigureRepositoryWith(MultipleOrganisations, MultipleRolesMatchingMultipleOrganisations);
            var context = new ProfileDataRequestContext();
            context.RequestedClaimTypes = new List<string>() { InssClaimTypes.Organisation };
            context.Subject = GetSubject();


            // Act
            var sut = CreateSut();

            // Assert
            await sut.GetProfileDataAsync(context);
            context.IssuedClaims.Count.Should().Be(MultipleOrganisations.Count);
            foreach (var claim in context.IssuedClaims)
            {
                var orgClaim = OrganisationClaimType.FromClaim(claim);
                var organisation = MultipleOrganisations.Where(o => o.ExternalId == orgClaim.Id).SingleOrDefault();
                organisation.Should().NotBeNull();

                var roles = MultipleRolesMatchingMultipleOrganisations
                    .Where(r => r.Email == Email)
                    .Where(r => r.OrganisationId == organisation.Id)
                    .Select(r => r.Role)
                    .Distinct();

                orgClaim.Id.Should().Be(organisation.ExternalId);
                orgClaim.Name.Should().Be(organisation.Name);
                orgClaim.OrganisationTypeName.Should().Be(organisation.Type);
                orgClaim.CurrentUserRoles.Should().BeEquivalentTo(roles);
            }
        }

        [TestMethod]
        public async Task GetProfileDataAsync_WithMultipleOrganisationSomeMatchingRoles_ReturnsClaims()
        {
            // Arrange
            ConfigureRepositoryWith(MultipleOrganisations, MultipleRolesMatchingSomeOrganisations);
            var context = new ProfileDataRequestContext();
            context.RequestedClaimTypes = new List<string>() { InssClaimTypes.Organisation };
            context.Subject = GetSubject();


            // Act
            var sut = CreateSut();

            // Assert
            await sut.GetProfileDataAsync(context);
            context.IssuedClaims.Count.Should().Be(MultipleOrganisations.Count);
            foreach (var claim in context.IssuedClaims)
            {
                var orgClaim = OrganisationClaimType.FromClaim(claim);
                var organisation = MultipleOrganisations.Where(o => o.ExternalId == orgClaim.Id).SingleOrDefault();
                organisation.Should().NotBeNull();

                var roles = MultipleRolesMatchingSomeOrganisations.Where(r => r.OrganisationId == organisation.Id).Select(r => r.Role).Distinct(); ;

                orgClaim.Id.Should().Be(organisation.ExternalId);
                orgClaim.Name.Should().Be(organisation.Name);
                orgClaim.OrganisationTypeName.Should().Be(organisation.Type);
                orgClaim.CurrentUserRoles.Should().BeEquivalentTo(roles);
            }
        }

        [TestMethod]
        public async Task GetProfileDataAsync_WithMultipleOrganisationRolesNotMatching_ReturnsClaims()
        {
            // Arrange
            ConfigureRepositoryWith(MultipleOrganisations, MultipleRolesNotMatchingAnyOrganisation);
            var context = new ProfileDataRequestContext();
            context.RequestedClaimTypes = new List<string>() { InssClaimTypes.Organisation };
            context.Subject = GetSubject();


            // Act
            var sut = CreateSut();

            // Assert
            await sut.GetProfileDataAsync(context);
            context.IssuedClaims.Count.Should().Be(MultipleOrganisations.Count);
            foreach (var claim in context.IssuedClaims)
            {
                var orgClaim = OrganisationClaimType.FromClaim(claim);
                var organisation = MultipleOrganisations.Where(o => o.ExternalId == orgClaim.Id).SingleOrDefault();
                organisation.Should().NotBeNull();

                var roles = MultipleRolesNotMatchingAnyOrganisation.Where(r => r.OrganisationId == organisation.Id).Select(r => r.Role).Distinct(); ;

                orgClaim.Id.Should().Be(organisation.ExternalId);
                orgClaim.Name.Should().Be(organisation.Name);
                orgClaim.OrganisationTypeName.Should().Be(organisation.Type);
                orgClaim.CurrentUserRoles.Should().BeEquivalentTo(roles);
            }
        }

        [TestMethod]
        public async Task GetProfileDataAsync_WithMultipleOrganisationRolesForDifferentUrl_ReturnsClaims()
        {
            // Arrange
            ConfigureRepositoryWith(MultipleOrganisations, MultipleRolesDifferentEmail);
            var context = new ProfileDataRequestContext();
            context.RequestedClaimTypes = new List<string>() { InssClaimTypes.Organisation };
            context.Subject = GetSubject();


            // Act
            var sut = CreateSut();

            // Assert
            await sut.GetProfileDataAsync(context);
            context.IssuedClaims.Count.Should().Be(MultipleOrganisations.Count);
            foreach (var claim in context.IssuedClaims)
            {
                var orgClaim = OrganisationClaimType.FromClaim(claim);
                var organisation = MultipleOrganisations.Where(o => o.ExternalId == orgClaim.Id).SingleOrDefault();
                organisation.Should().NotBeNull();

                var roles = MultipleRolesDifferentEmail
                            .Where(r => r.OrganisationId == organisation.Id)
                            .Where(r => r.Email == Email)
                            .Select(r => r.Role)
                            .Distinct();

                orgClaim.Id.Should().Be(organisation.ExternalId);
                orgClaim.Name.Should().Be(organisation.Name);
                orgClaim.OrganisationTypeName.Should().Be(organisation.Type);
                orgClaim.CurrentUserRoles.Should().BeEquivalentTo(roles);
            }
        }
    }
}
