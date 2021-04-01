using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Portal.Models;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.Portal.IntegrationTests
{
    [TestClass]
    public class IntegrationGatewayTests : IntegrationGatewayBase
    {
        [ClassInitialize]
        public static async Task ClassInitAsync(TestContext context) => await InitialiseDebtorAsync();

        [TestInitialize]
        public override void TestInit() => base.TestInit();

       
       

        [TestMethod]
        public void CreateClientAsync()
        { }

        [TestMethod]
        public async Task CreateDebtorAddressAsync()
        {
            // Arrange
            var model = Main.Fixture.Build<Address>()
                .WithAutoProperties()
                .Without(x => x.Postcode)
                .Without(x => x.Country)
                .Create();
            model.Postcode = "EC3V 3DG";
            model.Country = Constants.UkCountryValue;

            Main.SetMoneyAdviserCreditorApiCient();

            // Act
            var result = await Sut.CreateDebtorAddressAsync(model, MoratoriumId);

            // Assert
            Assert.AreNotEqual(Guid.Empty, result);
        }

        [TestMethod]
        public async Task Debtor_Address_Should_Be_Hidden()
        {
            // Arrange
            var model = new DebtorRadioYesNoViewModel
            {
                SubmitOption = Option.Yes.ToString(),
                MoratoriumId = MoratoriumId
            };
            Main.SetMoneyAdviserCreditorApiCient();

            // Act
            await Sut.DebtorHideAddressAsync(model);

            // Assert
            var result = await Sut.GetDebtorConfirmDetails(MoratoriumId);
            Assert.IsTrue(result.AddressDetail.AddressHidden);
        }

        [TestMethod]
        public async Task Debtor_Address_Should_Be_Unhidden()
        {
            // Arrange
            var model = new DebtorRadioYesNoViewModel
            {
                SubmitOption = Option.No.ToString(),
                MoratoriumId = MoratoriumId
            };
            Main.SetMoneyAdviserCreditorApiCient();

            // Act
            await Sut.DebtorHideAddressAsync(model);

            // Assert
            var result = await Sut.GetDebtorConfirmDetails(MoratoriumId);
            Assert.IsFalse(result.AddressDetail.AddressHidden);
        }

        [TestMethod]
        public async Task CreateAdHocCreditor_Should_Return_New_Id()
        {
            // Arrange
            var model = Main.Fixture.Create<string>();
            Main.SetMoneyAdviserCreditorApiCient();

            // Act
            var result = await Sut.CreateAdHocCreditor(model);

            // Assert
            Assert.AreNotEqual(default, result);
        }

        [TestMethod]
        public async Task SetContactPreference_For_Debtor_Should_Return_Debtor_Id()
        {
            // Arrange
            var model = Main.Fixture.Build<DebtorContactPreferenceViewModel>()
                .Without(x => x.EmailAddress)
                .Without(x => x.ConfirmEmailAddress)
                .Create();

            model.SubmitOption = ContactPreferenceType.None;

            Main.SetMoneyAdviserCreditorApiCient();

            await Sut.SubmitDebtorAccount(new DebtorRadioYesNoViewModel()
            {
                 MoratoriumId = MoratoriumId,
                 IsYesChecked = true
            });

            // Act
            await Sut.DebtorSetContactPreference(MoratoriumId, model);

            var clientDetails = await Sut.GetAccountSummary(MoratoriumId);

            // Assert
            Assert.AreEqual(ContactPreferenceType.None, clientDetails.DebtorDetail.NotificationDetail.PreferenceType);
        }

        [TestMethod]
        public async Task GetNominatedContactSummaryAsync()
        {
            // Arrange
            var inputModel = Main.Fixture.Create<DebtorNominatedContactViewModel>();
            inputModel.MoratoriumId = MoratoriumId;
            inputModel.CommunicationAddress.Postcode = "NW10 4BA";

            Main.SetMoneyAdviserCreditorApiCient();

            // Act
            await Sut.CreateNominatedContactAsync(inputModel);

            // Assert
            var result = await Sut.GetDebtorNominatedContactSummary(inputModel.MoratoriumId);
            Assert.IsNotNull(result);
            Assert.AreEqual(inputModel.FullName, result.FullName);
            Assert.AreEqual(inputModel.TelephoneNumber, result.TelephoneNumber);
            Assert.AreEqual(inputModel.EmailAddress, result.EmailAddress);
            Assert.AreEqual(inputModel.PointContactRole, $"{(int)result.PointContactRole}");
            Assert.AreEqual(inputModel.ContactConfirmationMethod, $"{(int)result.NotificationMethod}");
            Assert.AreEqual(inputModel.CommunicationAddress.Postcode, result.CommunicationAddress.Postcode);
        }

        [TestMethod]
        public async Task GetDebtorAccountSummary_Should_Return_Business_Detail_Async()
        {
            // Arrange
            var model = Main.Fixture.Create<DebtorAddBusinessViewModel>();
            await InitialiseDebtorAsync();
            model.MoratoriumId = MoratoriumId;
            model.DebtorCurrentAddress.Postcode = "NW10 4BA";           

            Main.SetMoneyAdviserCreditorApiCient();

            // Act
          
            await Sut.DebtorAddBusinessAsync(model);

            // Assert
            var result = await Sut.GetAccountSummary(MoratoriumId);
            var business = result.DebtorDetail.BusinessDetails.FirstOrDefault();
            Assert.AreEqual(business.BusinessName, model.BusinessName);
            Assert.AreEqual(business.BusinessAddress.AddressLine1, model.DebtorCurrentAddress.AddressLine1);
            Assert.AreEqual(business.BusinessAddress.AddressLine2, model.DebtorCurrentAddress.AddressLine2);
            Assert.AreEqual(business.BusinessAddress.TownCity, model.DebtorCurrentAddress.TownCity);
            Assert.AreEqual(business.BusinessAddress.County, model.DebtorCurrentAddress.County);
            Assert.AreEqual(business.BusinessAddress.Country, model.DebtorCurrentAddress.Country);
            Assert.AreEqual(business.BusinessAddress.Postcode, model.DebtorCurrentAddress.Postcode);
        }

        [TestMethod]
        public async Task GetDebtorAccountSummary_Business_Address_Should_Be_Hidden_Async()
        {
            // Arrange
            var model = Main.Fixture.Create<DebtorAddBusinessViewModel>();
            model.MoratoriumId = MoratoriumId;
            model.DebtorCurrentAddress.Postcode = "NW10 4BA";
            var hideAddressModel = new DebtorRadioYesNoViewModel
            {
                SubmitOption = Option.Yes.ToString(),
                MoratoriumId = MoratoriumId
            };
            var createAddressModel = new Address
            {
                AddressLine1 = model.DebtorCurrentAddress.AddressLine1,
                AddressLine2 = model.DebtorCurrentAddress.AddressLine2,
                TownCity = model.DebtorCurrentAddress.TownCity,
                County = model.DebtorCurrentAddress.County,
                Country = model.DebtorCurrentAddress.Country,
                Postcode = model.DebtorCurrentAddress.Postcode,
            };
           

            Main.SetMoneyAdviserCreditorApiCient();

            // Act
            await Sut.CreateDebtorAddressAsync(createAddressModel, MoratoriumId);
            await Sut.DebtorHideAddressAsync(hideAddressModel);
            await Sut.DebtorAddBusinessAsync(model);
          

            // Assert
            var result = await Sut.GetAccountSummary(MoratoriumId);
            var business = result.DebtorDetail.BusinessDetails.FirstOrDefault(b => b.BusinessName.Equals(model.BusinessName));

            Assert.IsNotNull(business);
            Assert.IsTrue(business.HideBusinessAddress);
        }

        [TestMethod]
        public async Task GetDebtorAccountSummary_Should_Return_Multiple_Business_Detail_Async()
        {
            // Arrange
            var models = Main.Fixture.CreateMany<DebtorAddBusinessViewModel>();
            await InitialiseDebtorAsync();
            Main.SetMoneyAdviserCreditorApiCient();
           

            // Act           

            foreach (var model in models)
            {
                model.MoratoriumId = MoratoriumId;
                model.DebtorCurrentAddress.Postcode = "NW10 4BA";

                await Sut.DebtorAddBusinessAsync(model);
            }

            // Assert
            var summary = await Sut.GetAccountSummary(MoratoriumId);

            foreach (var business in summary.DebtorDetail.BusinessDetails)
            {
                var model = models.Single(m => m.DebtorCurrentAddress.AddressLine1.Equals(business.BusinessAddress.AddressLine1));
                Assert.AreEqual(business.BusinessName, model.BusinessName);
                Assert.AreEqual(business.BusinessAddress.AddressLine1, model.DebtorCurrentAddress.AddressLine1);
                Assert.AreEqual(business.BusinessAddress.AddressLine2, model.DebtorCurrentAddress.AddressLine2);
                Assert.AreEqual(business.BusinessAddress.TownCity, model.DebtorCurrentAddress.TownCity);
                Assert.AreEqual(business.BusinessAddress.County, model.DebtorCurrentAddress.County);
                Assert.AreEqual(business.BusinessAddress.Country, model.DebtorCurrentAddress.Country);
                Assert.AreEqual(business.BusinessAddress.Postcode, model.DebtorCurrentAddress.Postcode);
            }
        }
    }
}
