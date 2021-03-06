﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace createsend_dotnet
{
    public class Client
    {
        public string ApiKey { get; set; }

        private CreateSendCredentials AuthCredentials
        {
            get { return new CreateSendCredentials(ApiKey != null ? ApiKey : CreateSendOptions.ApiKey, "x"); }
        }

        public string ClientID { get; set; }

        public Client(string clientID)
        {
            ClientID = clientID;
        }

        public static string Create(string apiKey, string companyName, string country, string timezone)
        {
            return HttpHelper.Post<ClientDetail, string>(
                new CreateSendCredentials(apiKey, "x"),
                "/clients.json",
                null,
                new ClientDetail()
                {
                    CompanyName = companyName,
                    Country = country,
                    TimeZone = timezone
                });
        }

        public ClientWithSettings Details()
        {
            return HttpHelper.Get<ClientWithSettings>(AuthCredentials, string.Format("/clients/{0}.json", ClientID), null);
        }

        public IEnumerable<CampaignDetail> Campaigns()
        {
            return HttpHelper.Get<CampaignDetail[]>(AuthCredentials, string.Format("/clients/{0}/campaigns.json", ClientID), null);
        }

        public IEnumerable<ScheduledCampaignDetail> Scheduled()
        {
            return HttpHelper.Get<ScheduledCampaignDetail[]>(AuthCredentials, string.Format("/clients/{0}/scheduled.json", ClientID), null);
        }

        public IEnumerable<DraftDetail> Drafts()
        {
            return HttpHelper.Get<DraftDetail[]>(AuthCredentials, string.Format("/clients/{0}/drafts.json", ClientID), null);
        }

        public IEnumerable<BasicList> Lists()
        {
            return HttpHelper.Get<BasicList[]>(AuthCredentials, string.Format("/clients/{0}/lists.json", ClientID), null);
        }

        public IEnumerable<ListForEmail> ListsForEmail(string email)
        {
            NameValueCollection args = new NameValueCollection();
            args.Add("email", email);
            return HttpHelper.Get<ListForEmail[]>(AuthCredentials, string.Format("/clients/{0}/listsforemail.json", ClientID), args);
        }

        public IEnumerable<BasicSegment> Segments()
        {
            return HttpHelper.Get<BasicSegment[]>(AuthCredentials, string.Format("/clients/{0}/segments.json", ClientID), null);
        }

        public PagedCollection<SuppressedSubscriber> SuppressionList(int page, int pageSize, string orderField, string orderDirection)
        {
            NameValueCollection queryArguments = new NameValueCollection();
            queryArguments.Add("page", page.ToString());
            queryArguments.Add("pagesize", pageSize.ToString());
            queryArguments.Add("orderfield", orderField);
            queryArguments.Add("orderdirection", orderDirection);

            return HttpHelper.Get<PagedCollection<SuppressedSubscriber>>(AuthCredentials, string.Format("/clients/{0}/suppressionlist.json", ClientID), queryArguments);
        }

        public void Suppress(string[] emails)
        {
            HttpHelper.Post<SuppressionDetails, string>(
                AuthCredentials,
                string.Format("/clients/{0}/suppress.json", ClientID), null,
                new SuppressionDetails { EmailAddresses = emails });
        }

        public void Unsuppress(string email)
        {
            HttpHelper.Put<string, PersonResult>(
                AuthCredentials,
                string.Format("/clients/{0}/unsuppress.json", ClientID),
                new NameValueCollection { { "email", email } }, null);
        }

        public IEnumerable<BasicTemplate> Templates()
        {
            return HttpHelper.Get<BasicTemplate[]>(AuthCredentials, string.Format("/clients/{0}/templates.json", ClientID), null);
        }

        public void SetBasics(string companyName, string country, string timezone)
        {
            HttpHelper.Put<ClientDetail, string>(
                AuthCredentials,
                string.Format("/clients/{0}/setbasics.json", ClientID), null,
                new ClientDetail()
                {
                    CompanyName = companyName,
                    Country = country,
                    TimeZone = timezone
                });
        }

        public void SetPAYGBilling(string currency, bool clientPays, bool canPurchaseCredits, int markupPercentage, decimal markupOnDelivery, decimal markupPerRecipient, decimal markupOnDesignSpamTest)
        {
            HttpHelper.Put<BillingOptions, string>(
                AuthCredentials,
                string.Format("/clients/{0}/setpaygbilling.json", ClientID), null,
                new BillingOptions()
                {
                    Currency = currency,
                    ClientPays = clientPays,
                    CanPurchaseCredits = canPurchaseCredits,
                    MarkupPercentage = markupPercentage,
                    MarkupOnDelivery = markupOnDelivery,
                    MarkupPerRecipient = markupPerRecipient,
                    MarkupOnDesignSpamTest = markupOnDesignSpamTest
                });
        }

        public void SetMonthlyBilling(string currency, bool clientPays, bool canPurchaseCredits, int markupPercentage)
        {
            SetMonthlyBilling(currency, clientPays, canPurchaseCredits, markupPercentage, null);
        }

        public void SetMonthlyBilling(string currency, bool clientPays, bool canPurchaseCredits, int markupPercentage, MonthlyScheme scheme)
        {
            SetMonthlyBilling(currency, clientPays, canPurchaseCredits, markupPercentage, (MonthlyScheme?)scheme);
        }

        private void SetMonthlyBilling(string currency, bool clientPays, bool canPurchaseCredits, int markupPercentage, MonthlyScheme? scheme)
        {
            HttpHelper.Put<BillingOptions, string>(
                AuthCredentials,
                string.Format("/clients/{0}/setmonthlybilling.json", ClientID), null,
                new BillingOptions()
                {
                    Currency = currency,
                    ClientPays = clientPays,
                    CanPurchaseCredits = canPurchaseCredits,
                    MarkupPercentage = markupPercentage,
                    MonthlyScheme = scheme
                });
        }

        /// <summary>
        /// Transfer credits to or from this client.
        /// </summary>
        /// <param name="credits">The number of credits to transfer. This
        /// value may be either positive if you want to allocate credits
        /// from your account to the client, or negative if you want to
        /// deduct credits from the client back into your account.</param>
        /// <param name="canUseMyCreditsWhenTheyRunOut">If set to true, will
        /// allow the client to continue sending using your credits or payment
        /// details once they run out of credits, and if set to false, will
        /// prevent the client from using your credits to continue sending
        /// until you allocate more credits to them.</param>
        /// <returns>The details of the credits transfer, including the credits
        /// in your account now, as well as the credits belonging to the client
        /// now.</returns>
        public CreditsTransferResult TransferCredits(
            int credits, bool canUseMyCreditsWhenTheyRunOut)
        {
            return HttpHelper.Post<CreditsTransferDetails, CreditsTransferResult>(
                AuthCredentials,
                string.Format("/clients/{0}/credits.json", ClientID), null,
                new CreditsTransferDetails
                {
                    Credits = credits,
                    CanUseMyCreditsWhenTheyRunOut =
                        canUseMyCreditsWhenTheyRunOut
                });
        }

        public void Delete()
        {
            HttpHelper.Delete(AuthCredentials, string.Format("/clients/{0}.json", ClientID), null);
        }

        public string GetPrimaryContact()
        {
            return HttpHelper.Get<PersonResult>(AuthCredentials, string.Format("/clients/{0}/primarycontact.json", ClientID), null).EmailAddress;
        }

        public string SetPrimaryContact(string emailAddress)
        {
            return HttpHelper.Put<string, PersonResult>(AuthCredentials, string.Format("/clients/{0}/primarycontact.json", ClientID), new NameValueCollection { { "email", emailAddress } }, null).EmailAddress;
        }

        public IEnumerable<PersonDetails> People()
        {
            return HttpHelper.Get<IEnumerable<PersonDetails>>(AuthCredentials, string.Format("/clients/{0}/people.json", ClientID), null);
        }
    }
}
