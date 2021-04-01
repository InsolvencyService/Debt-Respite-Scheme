using System;
using System.Linq;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Common.Exceptions;
using Insolvency.Integration.Gateways.Entities;
using Insolvency.Integration.Gateways.MoratoriumEntities;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Gateways.Mapper
{
    public class MapperService : IMapperService
    {
        public DebtorDetailsResponse MapDebtorDetailPublicData(BreathingSpaceResponse response)
        {
            return new DebtorDetailsResponse
            {
                FirstName = response.DebtorDetails.FirstName,
                MiddleName = response.DebtorDetails.MiddleName,
                LastName = response.DebtorDetails.LastName,
                DateOfBirth = response.DebtorDetails.DateOfBirth.Value.Date,
                AddressHidden = response.DebtorDetails.AddressHidden,
                MoratoriumStatus = response.DebtorDetails.MoratoriumStatus,
                IsInMentalHealthMoratorium = response.DebtorDetails.IsInMentalHealthMoratorium,
                ReferenceNumber = response.DebtorDetails.ReferenceNumber,
                StartsOn = response.DebtorDetails.StartsOn,
                EndsOn = response.DebtorDetails.EndsOn,
                CancellationDate = response.DebtorDetails.CancellationDate,
                CancellationReason = response.DebtorDetails.CancellationReason
            };
        }

        public DebtorDetailsResponse MapDebtorNames(Ntt_breathingspacedebtor response)
        {
            return new DebtorDetailsResponse
            {
                FirstName = response.ntt_firstname,
                MiddleName = response.ntt_middlename,
                LastName = response.ntt_lastname,
                PreviousNames = response.ntt_breathingspacedebtor_ntt_previousname_DebtorId
                ?.OrderBy(x => x.createdon)
                ?.Select(x => new ClientPreviousNameResponse
                {
                    FirstName = x.ntt_firstname,
                    MiddleName = x.ntt_middlename,
                    LastName = x.ntt_lastname,
                    CreatedOn = x.createdon,
                    Id = x.GetId()
                })
                ?? Enumerable.Empty<ClientPreviousNameResponse>()
            };
        }

        public void SetDebtorDetailCurrentAddress(BreathingSpaceResponse response)
        {
            if (response.CurrentAddress == null)
                return;

            if (!response.DebtorDetails.AddressHidden)
            {
                response.CurrentAddress = MapCurrentAddressResponse(response.CurrentAddress);
                return;
            }
            else
            {
                response.CurrentAddress = null;
                return;
            }
        }

        public void SetDebtorDetailSensitiveData(BreathingSpaceResponse response, DynamicsGatewayOptions options)
        {
            response.PreviousAddresses = response.PreviousAddresses
                            .Where(add => add.DateTo.HasValue)
                            .Select(add => MapPreviousAddressResponse(add, true))
                            .ToList();
            response.DebtorDetails.IsInMentalHealthMoratorium = response.DebtorDetails.IsInMentalHealthMoratorium;
            response.DebtorDetails.NotificationEmailAddress = response.DebtorDetails.NotificationEmailAddress;
            response.DebtorDetails.ContactPreference = response.DebtorDetails.ContactPreference;
            response.DebtorDetails.StartsOn = response.DebtorDetails.StartsOn;
            response.DebtorDetails.EndsOn = response.DebtorDetails.EndsOn;
            response.DebtorDetails.CreatedOn = response.DebtorDetails.CreatedOn;
            response.DebtorDetails.ModifiedOn = response.DebtorDetails.ModifiedOn;
            response.DebtorDetails.MoratoriumStatus = response.DebtorDetails.MoratoriumStatus;
            response.DebtorBusinessDetails = response.DebtorBusinessDetails
                            .Select(b => new BusinessAddressResponse
                            {
                                BusinessName = b.BusinessName,
                                Address = MapCurrentAddressResponse(b.Address)
                            })
                            .ToList();
        }

        public void SetAccountSearchResult(AccountSearchResponse accountSearchResult, Ntt_breathingspacedebtor debtor, Ntt_breathingspacemoratorium moratorium)
        {
            var address = debtor.ntt_breathingspacedebtor_inss_inssaddress?.FirstOrDefault();

            accountSearchResult.BreathingSpaceId = moratorium.ntt_breathingspacemoratoriumid.Value;
            accountSearchResult.FirstName = debtor.ntt_firstname;
            accountSearchResult.MiddleName = debtor.ntt_middlename;
            accountSearchResult.Surname = debtor.ntt_lastname;
            accountSearchResult.DateOfBirth = debtor.ntt_dateofbirth.HasValue ? debtor.ntt_dateofbirth.Value.Date : default;
            accountSearchResult.BreathingSpaceReference = moratorium.ntt_referencenumber;
            accountSearchResult.Address = address != null ? new AddressResponse
            {
                AddressId = address.GetId(),
                AddressLine1 = address.inss_addressline1,
                AddressLine2 = address.inss_addressline2,
                TownCity = address.inss_addressline3,
                County = address.inss_addressline4,
                Country = address.inss_addressline5,
                Postcode = address.inss_postcode,
            } : null;
            accountSearchResult.StartDate = moratorium.ntt_commencementdate.HasValue 
                ? DateTime.SpecifyKind(moratorium.ntt_commencementdate.Value.Date, DateTimeKind.Utc) 
                : (DateTime?)null;
            accountSearchResult.EndDate = moratorium.GetMoratoriumEndDate().HasValue
                ? moratorium.GetMoratoriumEndDate()?.DateTime.FromUtcToSpecifiedTimeZone(
                                                    Constants.WindowsUKSystemTimeZone, 
                                                    Constants.LinuxUKSystemTimeZone)
                : null;
            accountSearchResult.MoratoriumStatus = moratorium.GetMoratoriumStatus();
            accountSearchResult.OrganisationName = moratorium.ntt_ManagingMoneyAdviserOrganisationId?.inss_name;
            accountSearchResult.MoratoriumType = moratorium.ntt_breathingspacetypeid.ntt_name;
            accountSearchResult.CreatedOn = moratorium.createdon.HasValue 
                ? DateTime.SpecifyKind(moratorium.createdon.Value.DateTime, DateTimeKind.Utc) 
                : default;
        }

        public AddressResponse MapCurrentAddressResponse(AddressResponse response) =>
            new AddressResponse
            {
                AddressId = response.AddressId,
                AddressLine1 = response.AddressLine1,
                AddressLine2 = response.AddressLine2,
                TownCity = response.TownCity,
                County = response.County,
                Postcode = response.Postcode,
                Country = response.Country
            };

        public PreviousAddressResponse MapPreviousAddressResponse(PreviousAddressResponse response, bool includeToFrom = false)
        {
            var customAddress = new PreviousAddressResponse
            {
                AddressId = response.AddressId,
                AddressLine1 = response.AddressLine1,
                AddressLine2 = response.AddressLine2,
                TownCity = response.TownCity,
                County = response.County,
                Postcode = response.Postcode,
                Country = response.County
            };

            if (includeToFrom && response.DateTo.HasValue)
            {
                customAddress.DateFrom = response.DateFrom.Value;
                customAddress.DateTo = response.DateTo.Value;
            }
            return customAddress;
        }

        public BreathingSpaceResponse BuildMoratorium(MoratoriumDetail response, DynamicsGatewayOptions options)
        {
            return new BreathingSpaceResponse
            {
                OrganisationId = response.MoneyAdviserOrganisation.Id,
                MoneyAdviserOrganization = response.MoneyAdviserOrganisation.ToMoneyAdviserOrganizationResponse(),
                DebtorDetails = new DebtorDetailsResponse
                {
                    ReferenceNumber = response.ReferenceNumber,
                    FirstName = response.Debtor.FirstName,
                    MiddleName = response.Debtor.MiddleName,
                    LastName = response.Debtor.LastName,
                    PreviousNames = response.Debtor.PreviousNames
                        ?.OrderBy(x => x.CreatedOn)
                        ?.Select(x => x.ToPreviousName())
                        ?? Enumerable.Empty<ClientPreviousNameResponse>(),
                    DateOfBirth = response.Debtor.DateOfBirth.ToDateTimeOffset().DateTime,
                    AddressHidden = response.IsAddressWithheld,
                    NotificationEmailAddress = response.Debtor.NotificationEmailAddress,
                    MoratoriumStatus = MoratoriumIdStatusMap.GetStatusFromId(response.StatusId),
                    IsInMentalHealthMoratorium = response.IsMentalHealth,
                    StartsOn = response.CommencementDate?.ToDateTimeOffset(),
                    EndsOn = response.ExpiryDate?.ToDateTimeOffset(),
                    CancellationDate = response.ClosureDate?.ToDateTimeOffset(),
                    CreatedOn = response.CreatedOn?.ToDateTimeOffset(),
                    ModifiedOn = response.ModifiedOn?.ToDateTimeOffset(),
                    ContactPreference = Enum.Parse<ContactPreferenceType>(response.Debtor.ContactPreferenceId.ToString()),
                    CancellationReason = response.ParentCancellationReason
                },
                CurrentAddress = response.Debtor.CurrentAddress.ToAddressResponse(),
                PreviousAddresses = response.Debtor.PreviousAddresses
                    ?.OrderBy(pd => pd.CreatedOn)
                    ?.Select(pd => pd.ToPreviousAddress())
                    .ToList() ?? Enumerable.Empty<PreviousAddressResponse>(),
                DebtorBusinessDetails = response.Debtor.Businesses
                    ?.OrderBy(b => b.CreatedOn)
                    ?.Select(b => b.ToBusinessAddress())
                    .ToList() ?? Enumerable.Empty<BusinessAddressResponse>(),
                DebtDetails = response.Debts
                    ?.Select(d => d.ToDebtDetail(options))
                    .ToList() ?? Enumerable.Empty<DebtDetailResponse>(),
                DebtorEligibilityReviews = response.DebtorEligibilityReview
                    ?.Select(e => e.ToDebtorEligibilityReview(options))
                    .ToList() ?? Enumerable.Empty<DebtorEligibilityReviewResponse>(),
                DebtorNominatedContactResponse = response.ExternalContacts
                    ?.Select(e => e.ToDebtorNominatedContactResponse(options))
                    .FirstOrDefault(),
                DebtorTransfer = response.MoratoriumTransfer?.ToDebtorTransfer(),
            };
        }

        public void SetBusinessAddress(BusinessAddressResponse businessAddress, AddressResponse currentAddress, bool addressHidden)
        {
            if (!addressHidden)
            {
                return;
            }
            if (businessAddress.Address.Equals(currentAddress))
            {
                businessAddress.Address = null;
            }
        }        

        public void FilterMoratoriumByCreditor(BreathingSpaceResponse response, Guid creditorId)
        {
            response.DebtDetails = response.DebtDetails.Where(d => d.CreditorId == creditorId);
            response.DebtorEligibilityReviews = response.DebtorEligibilityReviews.Where(d => d.CreditorId == creditorId);

            response.DebtorDetails.IsInMentalHealthMoratorium = null;

            foreach (var businessAddress in response.DebtorBusinessDetails)
            {
                SetBusinessAddress(businessAddress, response.CurrentAddress, response.DebtorDetails.AddressHidden);
            }

            SetDebtorDetailCurrentAddress(response);

            FilterMoratoriumTransferRequestByCreditor(response);
            FilterInactiveMoratoriumByCreditor(response);
            FilterDraftMoratoriumByCreditor(response);
        }

        private void FilterInactiveMoratoriumByCreditor(BreathingSpaceResponse response)
        {
            var bsStatus = response.DebtorDetails.MoratoriumStatus;
            if (bsStatus != MoratoriumStatus.Active && bsStatus != MoratoriumStatus.Draft)
            {
                response.DebtorDetails = new DebtorDetailsResponse()
                {
                    EndsOn = response.DebtorDetails.EndsOn,
                    ReferenceNumber = response.DebtorDetails.ReferenceNumber,
                    ModifiedOn = response.DebtorDetails.ModifiedOn,
                };

                response.CurrentAddress = null;
                response.DebtorBusinessDetails = null;
                response.MoneyAdviserOrganization = null;
                response.PreviousAddresses = null;
            }
        }

        private void FilterDraftMoratoriumByCreditor(BreathingSpaceResponse response)
        {
            var bsStatus = response.DebtorDetails.MoratoriumStatus;
            if (bsStatus == MoratoriumStatus.Draft)
            {
                throw new UnauthorizedHttpResponseException();
            }
        }

        private void FilterMoratoriumTransferRequestByCreditor(BreathingSpaceResponse response)
        {
            response.DebtorTransfer = null;
        }
    }
}
