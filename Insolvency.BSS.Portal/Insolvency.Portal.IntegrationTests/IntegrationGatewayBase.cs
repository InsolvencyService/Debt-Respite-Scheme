using System;
using System.Threading.Tasks;
using AutoFixture;
using Insolvency.Portal.Gateways;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.Portal.IntegrationTests
{
    public class IntegrationGatewayBase
    {
        public IntegrationGateway Sut { get; set; }
        public static Guid MoratoriumId { get; set; }

        public virtual void TestInit()
        {
            CheckIfDebtorWasCreated();
            SetSut();
        }



        public static async Task InitialiseDebtorAsync()
        {
            // Arrange
            Main.SetMoneyAdviserCreditorApiCient();
            var sut = new IntegrationGateway(Main.ApiClient);
            var model = Main.Fixture.Create<ClientDetailsCreateViewModel>();
            model.BirthYear = 1980;

            var validDate = false;
            while (!validDate)
            {
                try
                {
                    _ = new DateTime(model.BirthYear.Value, model.BirthMonth.Value, model.BirthDay.Value);
                    validDate = true;
                }
                catch
                {
                    model = Main.Fixture.Create<ClientDetailsCreateViewModel>();
                    model.BirthYear = 1980;
                }
            }

            // Act & Asssert

            var result = await sut.CreateClientAsync(model);
            MoratoriumId = result;
        }
        public void CheckIfDebtorWasCreated()
        {
            if (MoratoriumId == default(Guid))
            {
                Assert.Fail("MoratoriumId is not present, InitialiseDebtor failed");
            }
        }

        public void SetSut() => Sut = new IntegrationGateway(Main.ApiClient);
    }
}
