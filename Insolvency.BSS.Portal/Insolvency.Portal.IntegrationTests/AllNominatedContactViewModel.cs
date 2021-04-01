using System.Net.Mail;
using AutoFixture;
using Insolvency.Common.Enums;
using Insolvency.Portal.Models.ViewModels;

namespace Insolvency.Portal.IntegrationTests
{
    public class AllNominatedContactViewModel : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var emailAddress = fixture.Create<MailAddress>();

            fixture.Customize<DebtorNominatedContactViewModel>(x =>
                x.With(p => p.EmailAddress, emailAddress.Address)
                .With(p => p.ConfirmEmailAddress, emailAddress.Address)
                .With(p => p.TelephoneNumber, "+44879879846548")
                .With(p => p.PointContactRole, $"{(int)PointContactRoleType.CareCoordinator}")
                .With(p => p.ContactConfirmationMethod, $"{(int)PointContactConfirmationMethod.Email}")
                .Without(x => x.MoratoriumId));
        }
    }
}
