using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;
using Insolvency.Integration.Models.Shared.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.OData.Client;

namespace Insolvency.Integration.IntegrationTests
{
    [TestClass]
    public class DynamicsGatewayTests : DynamicsGatewayBase
    {
        [ClassInitialize]
        public static async Task ClassInitAsync(TestContext context) => await InitialiseDebtorAsync();

        [TestInitialize]
        public override void TestInit() => base.TestInit();

        [TestMethod]
        public void CreateClientWithDetailsAsync()
        { }

        [TestMethod]
        public async Task CreateDebtorAddressAsync()
        {
            // Arrange
            var model = Main.Fixture.Create<CustomAddress>();
            model.OwnerId = MoratoriumId;

            // Act & Asssert
            var result = await Sut_MoneyAdvSvc.CreateDebtorAddressAsync(model);

            // Assert
            Assert.AreNotEqual(default, result);
        }

        [TestMethod]
        public async Task Should_Add_DebtorNominatedContactAsync()
        {
            // Arrange
            var model = Main.Fixture.Create<NominatedContactCreateRequest>();
            model.MoratoriumId = MoratoriumId;

            // Act
            var result = await Sut_MoneyAdvSvc.CreateNominatedContactAsync(model);

            // Asssert
            Assert.IsNotNull(result.GetType().GetProperty("ContactId"));
            Assert.IsNotNull(result.GetType().GetProperty("RoleId"));
        }

        [TestMethod]
        public async Task Should_Add_DebtorBusinessAddressAsync()
        {
            // Arrange
            var model = Main.Fixture.Create<BusinessAddressRequest>();
            model.Address.OwnerId = MoratoriumId;

            // Act
            var result = await Sut_MoneyAdvSvc.AddDebtorBusinessAddressAsync(model);

            // Asssert
            Assert.IsNotNull(result.GetType().GetProperty("BusinessId"));
            Assert.IsNotNull(result.GetType().GetProperty("AddressId"));
        }

        [TestMethod]
        public async Task CreateAdHocCreditor_Should_Return_New_Id()
        {
            // Arrange
            var model = Main.Fixture.Create<string>();

            // Act
            var result = await Sut_SharedSvc.CreateAdHocCreditor(model);

            // Assert
            Assert.AreNotEqual(default, result);
        }

        //[TestMethod]
        public async Task Should_Order_Debtor_Previous_Address_By_Asc_GetDebtorDetailsAsync()
        {
            // Arrange
            await InitialiseDebtorAsync();

            var addressModel = Main.Fixture.Build<CustomAddress>()
                .Do(x => x.OwnerId = MoratoriumId)
                .Without(x => x.Postcode)
                .Without(x => x.OwnerId)
                .CreateMany()
                .ToList();

            addressModel[0].Postcode = "NW10 2AB";
            addressModel[1].Postcode = "NW10 2BD";
            addressModel[2].Postcode = "NW10 2JU";

            // Act
            foreach (var item in addressModel)
            {
                await Sut_MoneyAdvSvc.CreateDebtorAddressAsync(item);
            }

            // Assert
            var debtorDetail = await Sut_SharedSvc.GetMaBreathingSpaceAsync(MoratoriumId, Main.ManagingMoneyOrganisationId);
            var prevAddresses = debtorDetail.PreviousAddresses.ToList();
            var postcodesResult = prevAddresses.Select(x => x.Postcode).ToList();

            foreach (var item in addressModel)
            {
                Assert.IsTrue(postcodesResult.Contains(item.Postcode));
            }

            Assert.AreEqual(addressModel.First().Postcode, prevAddresses.First().Postcode);
            Assert.AreEqual(addressModel.Last().Postcode, prevAddresses.Last().Postcode);
        }

        [TestMethod]
        public async Task SetContactPreference_Should_Return_Debtor_Id()
        {
            // Arrange
            await AddAdHocCreditorAndDebt();

            var model = Main.Fixture.Create<DebtorContactPreferenceRequest>();
            model.MoratoriumId = MoratoriumId;
            model.Preference = ContactPreferenceType.None;

            await Sut_MoneyAdvSvc.SaveDebtorAsync(new DebtorAccountSaveRequest()
            {
                MoratoriumId = model.MoratoriumId
            });

            // Act
            await Sut_MoneyAdvSvc.DebtorSetContactPreference(model);
            var clientDetails = await Sut_SharedSvc.GetMaBreathingSpaceAsync(MoratoriumId, Main.ManagingMoneyOrganisationId);

            // Assert
            Assert.AreEqual(model.Preference, clientDetails.DebtorDetails.ContactPreference);
        }

        [TestMethod]
        public async Task SetContactPreference_To_Letter_Should_Fail_With_No_Address()
        {
            // Arrange
            await AddAdHocCreditorAndDebt();

            var model = Main.Fixture.Create<DebtorContactPreferenceRequest>();
            model.MoratoriumId = MoratoriumId;
            model.Preference = ContactPreferenceType.Post;

            async Task result()
            {
                await Sut_MoneyAdvSvc.SaveDebtorAsync(new DebtorAccountSaveRequest()
                {
                    MoratoriumId = model.MoratoriumId
                });

                await Sut_MoneyAdvSvc.DebtorSetContactPreference(model);
            }

            // Act + Assert
            await Assert.ThrowsExceptionAsync<WebRequestException>(result);
        }

        [TestMethod]
        public async Task SetContactPreference_To_Letter_Should_Not_Fail_With_Address()
        {
            // Arrange
            await AddAdHocCreditorAndDebt();

            await SetDebtorAddress();
            var model = Main.Fixture.Create<DebtorContactPreferenceRequest>();
            model.MoratoriumId = MoratoriumId;
            model.Preference = ContactPreferenceType.Post;

            await Sut_MoneyAdvSvc.SaveDebtorAsync(new DebtorAccountSaveRequest()
            {
                MoratoriumId = model.MoratoriumId
            });

            // Act
            await Sut_MoneyAdvSvc.DebtorSetContactPreference(model);
            var clientDetails = await Sut_SharedSvc.GetMaBreathingSpaceAsync(MoratoriumId, Main.ManagingMoneyOrganisationId);

            // Assert
            Assert.AreEqual(model.Preference, clientDetails.DebtorDetails.ContactPreference);
        }

        [TestMethod]
        public async Task GetNominatedContactSummaryAsync()
        {
            // Arrange
            var inputModel = Main.Fixture.Create<NominatedContactCreateRequest>();
            await InitialiseDebtorAsync();
            inputModel.MoratoriumId = MoratoriumId;
            inputModel.Address.Postcode = "NW10 4BA";

            // Act
            await Sut_MoneyAdvSvc.CreateNominatedContactAsync(inputModel);

            // Assert
            var result = await Sut_MoneyAdvSvc.GetNominatedContactAsync(inputModel.MoratoriumId);
            Assert.IsNotNull(result);
            Assert.AreEqual(inputModel.FullName, result.FullName);
            Assert.AreEqual(inputModel.TelephoneNumber, result.TelephoneNumber);
            Assert.AreEqual(inputModel.EmailAddress, result.EmailAddress);
            Assert.AreEqual(inputModel.PointContactRole, result.PointContactRole);
            Assert.AreEqual(inputModel.ContactConfirmationMethod, result.NotificationMethod);
            Assert.AreEqual(inputModel.Address.Postcode, result.CommunicationAddress.Postcode);
        }

        [TestMethod]
        public async Task GetNominatedContactSummary_Should_Return_Latest_Record_Async()
        {
            // Arrange
            var inputModels = Main.Fixture.CreateMany<NominatedContactCreateRequest>().ToList();
            await InitialiseDebtorAsync();

            // Act
            for (var i = 0; i < inputModels.Count; i++)
            {
                var inputModel = inputModels[i];
                inputModel.MoratoriumId = MoratoriumId;
                inputModel.Address.Postcode = "NW10 4BA";

                // override the last item
                if (i == inputModels.Count - 1)
                {
                    inputModel.TelephoneNumber = null;
                    inputModel.ContactConfirmationMethod = (int)PointContactConfirmationMethod.Email;
                    inputModel.Address = null;
                }
                await Sut_MoneyAdvSvc.CreateNominatedContactAsync(inputModel);
            }

            // Assert
            var result = await Sut_MoneyAdvSvc.GetNominatedContactAsync(MoratoriumId);
            var lastInputModelItem = inputModels[inputModels.Count - 1];
            Assert.IsNotNull(result);
            Assert.AreEqual(lastInputModelItem.FullName, result.FullName);
            Assert.AreEqual(lastInputModelItem.TelephoneNumber, result.TelephoneNumber);
            Assert.AreEqual(lastInputModelItem.EmailAddress, result.EmailAddress);
            Assert.AreEqual(lastInputModelItem.PointContactRole, result.PointContactRole);
            Assert.AreEqual(lastInputModelItem.ContactConfirmationMethod, result.NotificationMethod);
            Assert.AreEqual(lastInputModelItem.Address?.Postcode, result.CommunicationAddress?.Postcode);
        }

        [TestMethod]
        public async Task GetDebtorAccountSummary_Should_Return_Business_Detail_Async()
        {
            // Arrange
            var model = Main.Fixture.Create<BusinessAddressRequest>();
            model.Address.OwnerId = MoratoriumId;
            model.Address.Postcode = "NW10 4BA";

            // Act
            await Sut_MoneyAdvSvc.AddDebtorBusinessAddressAsync(model);

            // Assert
            var result = await Sut_SharedSvc.GetMaBreathingSpaceAsync(MoratoriumId, Main.ManagingMoneyOrganisationId);
            var business = result.DebtorBusinessDetails.FirstOrDefault();
            Assert.AreEqual(business.BusinessName, model.BusinessName);
            Assert.AreEqual(business.Address.AddressLine1, model.Address.AddressLine1);
            Assert.AreEqual(business.Address.AddressLine2, model.Address.AddressLine2);
            Assert.AreEqual(business.Address.TownCity, model.Address.TownCity);
            Assert.AreEqual(business.Address.County, model.Address.County);
            Assert.AreEqual(business.Address.Country, model.Address.Country);
            Assert.AreEqual(business.Address.Postcode, model.Address.Postcode);
        }

        [TestMethod]
        public async Task GetDebtorAccountSummary_Should_Return_Multiple_Business_Detail_Async()
        {
            // Arrange
            var models = Main.Fixture.CreateMany<BusinessAddressRequest>();
            await InitialiseDebtorAsync();

            // Act
            foreach (var model in models)
            {
                model.Address.OwnerId = MoratoriumId;
                model.Address.Postcode = "NW10 4BA";

                await Sut_MoneyAdvSvc.AddDebtorBusinessAddressAsync(model);
            }
            var expectedNames = models.Select(x => x.BusinessName).OrderBy(x => x).ToList();

            // Assert
            var summary = await Sut_SharedSvc.GetMaBreathingSpaceAsync(MoratoriumId, Main.ManagingMoneyOrganisationId);

            var names = summary.DebtorBusinessDetails.Select(x => x.BusinessName).OrderBy(x => x).ToList();

            Assert.IsTrue(expectedNames.SequenceEqual(names));
        }

        [TestMethod]
        public async Task SubmitDebtEligibilityReview_Should_Be_Removed_Debt_Eligibility_Review_Async()
        {
            // Arrange
            await AddAdHocCreditorAndDebt();

            await Sut_MoneyAdvSvc.SaveDebtorAsync(new DebtorAccountSaveRequest()
            {
                MoratoriumId = MoratoriumId
            });

            var model = Main.Fixture.Build<CreateAdHocDebtRequest>()
                .WithAutoProperties()
                .Without(x => x.MoratoriumId)
                .Without(x => x.NINO)
                .Without(x => x.Postcode)
                .Create();

            model.Postcode = "SW19 1BD";
            model.MoratoriumId = MoratoriumId;

            await Sut_MoneyAdvSvc.CreateDebtAndAdHocCreditor(model);

            var breathingSpace = await Sut_SharedSvc.GetMaBreathingSpaceAsync(model.MoratoriumId, Main.ManagingMoneyOrganisationId);

            var debtToBeReviewed = breathingSpace.DebtDetails.First();

            var debtReviewRequest = new DebtEligibilityReviewRequest
            {
                DebtId = debtToBeReviewed.Id,
                ReviewType = DebtEligibilityReviewReasons.NotEligible,
                CreditorNotes = "Test note",
                MoneyAdviserId = Main.ManagingMoneyOrganisationId
            };
            await Sut_SharedSvc.CreateDebtEligibilityReviewRequest(debtReviewRequest);
            var debtDetail = await Sut_MoneyAdvSvc.GetDebtDetail(debtReviewRequest.DebtId);

            // Act
            var debtEligibilityModel = new DebtEligibilityReviewResponseRequest
            {
                DebtId = debtDetail.Id,
                MoneyAdviserNotes = "Test Notes",
                RemoveAfterReview = true
            };

            await Sut_MoneyAdvSvc.SubmitDebtEligibilityReview(debtEligibilityModel);

            // Assert
            var debtDetailAssert = await Sut_MoneyAdvSvc.GetDebtDetail(debtReviewRequest.DebtId);

            Assert.AreEqual(debtReviewRequest.DebtId, debtDetailAssert.Id);
            Assert.AreEqual(DebtEligibilityReviewStatus.NotEligibleAfterAdviserReview, debtDetailAssert.DebtEligibilityReview.Status);
            Assert.AreEqual(debtReviewRequest.CreditorNotes, debtDetailAssert.DebtEligibilityReview.CreditorNotes);
            Assert.AreEqual(DateTime.Today.Date, debtDetailAssert.ModifiedOn.Value.Date);
            Assert.AreEqual(debtToBeReviewed.Amount, debtDetailAssert.Amount);
            Assert.AreEqual(debtEligibilityModel.MoneyAdviserNotes, debtDetailAssert.DebtEligibilityReview.MoneyAdviserNotes);
        }

        [TestMethod]
        public async Task SubmitDebtEligibilityReview_Should_Not_Be_Removed_Debt_Eligibility_Review_Async()
        {
            // Arrange
            await AddAdHocCreditorAndDebt();

            await Sut_MoneyAdvSvc.SaveDebtorAsync(new DebtorAccountSaveRequest()
            {
                MoratoriumId = MoratoriumId
            });

            var model = Main.Fixture.Build<CreateAdHocDebtRequest>()
                .WithAutoProperties()
                .Without(x => x.MoratoriumId)
                .Without(x => x.NINO)
                .Without(x => x.Postcode)
                .Create();

            model.Postcode = "SW19 1BD";
            model.MoratoriumId = MoratoriumId;

            await Sut_MoneyAdvSvc.CreateDebtAndAdHocCreditor(model);

            var breathingSpace = await Sut_SharedSvc.GetMaBreathingSpaceAsync(model.MoratoriumId, Main.ManagingMoneyOrganisationId);

            var debtToBeReviewed = breathingSpace.DebtDetails.First();

            var debtReviewRequest = new DebtEligibilityReviewRequest
            {
                DebtId = debtToBeReviewed.Id,
                ReviewType = DebtEligibilityReviewReasons.NotEligible,
                CreditorNotes = "Test note",
                MoneyAdviserId = Main.ManagingMoneyOrganisationId
            };
            await Sut_SharedSvc.CreateDebtEligibilityReviewRequest(debtReviewRequest);
            var debtDetail = await Sut_MoneyAdvSvc.GetDebtDetail(debtReviewRequest.DebtId);

            // Act
            var debtEligibilityModel = new DebtEligibilityReviewResponseRequest
            {
                DebtId = debtDetail.Id,
                MoneyAdviserNotes = "Test note",
                RemoveAfterReview = false
            };

            await Sut_MoneyAdvSvc.SubmitDebtEligibilityReview(debtEligibilityModel);

            // Assert
            var debtDetailAssert = await Sut_MoneyAdvSvc.GetDebtDetail(debtReviewRequest.DebtId);

            Assert.AreEqual(debtReviewRequest.DebtId, debtDetailAssert.Id);
            Assert.AreEqual(DebtEligibilityReviewStatus.EligibleAfterAdviserReview, debtDetailAssert.DebtEligibilityReview.Status);
            Assert.AreEqual(debtReviewRequest.CreditorNotes, debtDetailAssert.DebtEligibilityReview.CreditorNotes);
            Assert.AreEqual(DateTime.Today.Date, debtDetailAssert.ModifiedOn.Value.Date);
            Assert.AreEqual(debtToBeReviewed.Amount, debtDetailAssert.Amount);
            Assert.AreEqual(debtEligibilityModel.MoneyAdviserNotes, debtDetailAssert.DebtEligibilityReview.MoneyAdviserNotes);
        }
    }
}
