using System.Net.Mail;
using AutoFixture;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;

namespace Insolvency.Integration.IntegrationTests
{
    public class AllNominatedContactModel : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var emailAddress = fixture.Create<MailAddress>();
            fixture.Customize<NominatedContactCreateRequest>(x =>
                x.With(p => p.EmailAddress, emailAddress.Address)
                .With(p => p.ConfirmEmailAddress, emailAddress.Address)
                .With(p => p.TelephoneNumber, "+44879879846548")
                .With(p => p.PointContactRole, PointContactRoleType.CareCoordinator)
                .With(p => p.ContactConfirmationMethod, PointContactConfirmationMethod.Email)
                .With(x => x.Address, new NominatedContactAddress())
                .Without(x => x.MoratoriumId));

            fixture.Customize<NominatedContactUpdateRequest>(x =>
               x.With(p => p.EmailAddress, emailAddress.Address)
               .With(p => p.ConfirmEmailAddress, emailAddress.Address)
               .With(p => p.TelephoneNumber, "+44879879846548")
               .With(p => p.PointContactRole, PointContactRoleType.CareCoordinator)
               .With(p => p.ContactConfirmationMethod, PointContactConfirmationMethod.Email)
               .With(x => x.Address, new NominatedContactAddress())
               .Without(x => x.MoratoriumId));
        }
    }
}