using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using Compulsivio.Prefinery.Configuration;

namespace Compulsivio.Prefinery
{
    /// <summary>
    /// Repository and API manager for Prefinery betas.
    /// </summary>
    public class PrefineryCore : IBetaRepository
    {
        /// <summary>
        /// Initializes a new instance of the PrefineryCore class.
        /// </summary>
        /// <remarks>
        /// The default constructor requires that a Prefinery section is set up in your App.config or
        /// Web.config file. If you do not want to put Prefinery's configuration there, you will have to
        /// use the alternate constructor and add betas manually.
        /// </remarks>
        public PrefineryCore()
        {
            PrefineryConfigHandler config = (PrefineryConfigHandler)ConfigurationManager.GetSection("Prefinery");

            this.AccountName = config.Account.AccountName;
            this.ApiKey = config.Account.ApiKey;

            this.Betas = new List<Beta>();
            foreach (var b in config.Betas)
            {
                var beta = (BetaElement)b;
                this.Betas.Add(new Beta { Id = beta.Id, Name = beta.Name, DecodeKey = beta.DecodeKey, Repository = this });
            }
        }

        /// <summary>
        /// Initializes a new instance of the PrefineryCore class.
        /// </summary>
        /// <param name="accountName">The account/beta name as it appears in the Prefinery API calls.</param>
        /// <param name="apiKey">The API key for the Prefinery account.</param>
        public PrefineryCore(string accountName, string apiKey)
        {
            this.AccountName = accountName;
            this.ApiKey = apiKey;

            this.Betas = new List<Beta>();
        }

        /// <summary>
        /// Gets the account/beta name as it appears in the Prefinery API calls. 
        /// </summary>
        public string AccountName { get; private set; }

        /// <summary>
        /// Gets the API key for the Prefinery account.
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Gets or sets a list of betas associated with the Prefinery account.
        /// </summary>
        private List<Beta> Betas { get; set; }

        #region IBetaRepository Members

        /// <summary>
        /// Finds a beta with a specified identification number.
        /// </summary>
        /// <param name="id">The identification number of the beta to return.</param>
        /// <returns>A <see cref="T:Compulsivio.Prefinery.IBeta"/> representing the beta.</returns>
        public IBeta GetBeta(int id)
        {
            return this.Betas.FirstOrDefault(b => b.Id == id);
        }

        /// <summary>
        /// Finds a beta with a specified name.
        /// </summary>
        /// <param name="name">The name of the beta to return.</param>
        /// <returns>A <see cref="T:Compulsivio.Prefinery.IBeta"/> representing the beta.</returns>
        /// <remarks>
        /// This is the name given to the beta in the application configuration file, or set when
        /// creating a beta in code. This is <strong>not</strong> the name by which the beta is
        /// known to Prefinery.
        /// </remarks>
        public IBeta GetBeta(string name)
        {
            return this.Betas.FirstOrDefault(b => b.Name == name);
        }

        /// <summary>
        /// Gets all betas managed by the repository.
        /// </summary>
        /// <returns>An enumerable list of <see cref="T:Compulsivio.Prefinery.IBeta"/> objects.</returns>
        public IEnumerable<IBeta> GetBetas()
        {
            foreach (var b in this.Betas)
            {
                yield return b;
            }

            yield break;
        }

        /// <summary>
        /// Adds a beta to the repository for managing.
        /// </summary>
        /// <param name="beta">A <see cref="T:Compulsivio.Prefinery.IBeta"/> for the repository to manage.</param>
        public void AddBeta(IBeta beta)
        {
            var betaObj = beta as Beta;
            if (betaObj == null)
            {
                throw new ArgumentException("Incompatable IBeta implementation");
            }

            betaObj.Repository = this;
            this.Betas.Add(betaObj);
        }

        #endregion

        #region Internal tester repository members

        /// <summary>
        /// Return the <see cref="T:Compulsivio.Prefinery.Tester"/> with a given ID number.
        /// </summary>
        /// <param name="beta">The beta object associated with the returned testers.</param>
        /// <param name="id">The ID number of the tester to return.</param>
        /// <returns>A <see cref="T:Compulsivio.Prefinery.Tester"/> with the given ID number.</returns>
        internal Tester GetTester(IBeta beta, int id)
        {
            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers/{3}.xml?api_key={2}", this.AccountName, beta.Id, this.ApiKey, id))
                as HttpWebRequest;
            request.Method = "GET";

            // execute the request and return the first result
            return TesterBuilder.ProcessTesterRequest(beta, request).First();
        }

        /// <summary>
        /// Return the <see cref="T:Compulsivio.Prefinery.Tester"/> with a given e-mail address.
        /// </summary>
        /// <param name="beta">The beta object associated with the returned testers.</param>
        /// <param name="email">The e-mail address associated with the tester.</param>
        /// <returns>A <see cref="T:Compulsivio.Prefinery.Tester"/> with the given e-mail address.</returns>
        internal Tester GetTester(IBeta beta, string email)
        {
            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers.xml?api_key={2}&email={3}", this.AccountName, beta.Id, this.ApiKey, email))
                as HttpWebRequest;
            request.Method = "GET";

            // execute the request and return the first result
            return TesterBuilder.ProcessTesterRequest(beta, request).First();
        }

        /// <summary>
        /// Request a list of all testers associated with a beta.
        /// </summary>
        /// <param name="beta">The beta object associated with the returned testers.</param>
        /// <returns>An enumerable list of <see cref="T:Compulsivio.Prefinery.Tester"/>s.</returns>
        internal IEnumerable<Tester> GetTesters(IBeta beta)
        {
            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers.xml?api_key={2}", this.AccountName, beta.Id, this.ApiKey))
                as HttpWebRequest;
            request.Method = "GET";

            // execute the request and return the results
            return TesterBuilder.ProcessTesterRequest(beta, request);
        }

        /// <summary>
        /// Add a tester to Prefinery.
        /// </summary>
        /// <param name="beta">The beta object associated with the returned testers.</param>
        /// <param name="tester">The <see cref="T:Compulsivio.Prefinery.Tester"/> to add.</param>
        /// <exception cref="T:System.InvalidOperationException"><paramref name="tester"/> has already been added.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="tester"/> is missing an email address.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="tester"/> cannot be added with an initial status of Rejected.</exception>
        internal void AddTester(IBeta beta, ITester tester)
        {
            // make sure we're doing the right thing
            if (tester.Beta != null)
            {
                throw new InvalidOperationException("Tester has Id and so has already been added");
            }

            if (string.IsNullOrEmpty(tester.Email))
            {
                throw new ArgumentException("Tester needs email address before it can be added");
            }

            if (tester.Status == TesterStatus.Rejected)
            {
                throw new ArgumentException("Tester must be added before it can be rejected");
            }

            var testerObj = tester as Tester;
            if (testerObj == null)
            {
                throw new ArgumentException("Incompatable ITester implementation");
            }

            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers.xml?api_key={2}", this.AccountName, beta.Id, this.ApiKey))
                as HttpWebRequest;
            request.Method = "POST";

            request.ContentType = "text/xml";
            using (var stream = request.GetRequestStream())
            {
                using (var writer = new System.IO.StreamWriter(stream, new UTF8Encoding()))
                {
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    writer.WriteLine("<tester>");
                    writer.WriteLine("<email>{0}</email>", testerObj.Email);

                    if (!string.IsNullOrEmpty(testerObj.InviteCode))
                    {
                        writer.WriteLine("<invitation-code>{0}</invitation-code>", testerObj.InviteCode);
                    }

                    if (testerObj.Status != TesterStatus.Unknown)
                    {
                        writer.WriteLine("<status>{0}</status>", testerObj.Status.ToString().ToLower());
                    }

                    if (testerObj.Profile.Count > 0)
                    {
                        writer.WriteLine("<profile>");
                        foreach (var pair in testerObj.Profile)
                        {
                            writer.WriteLine("<{0}>{1}</{0}>", pair.Key, pair.Value);
                        }

                        writer.WriteLine("</profile>");
                    }

                    writer.WriteLine("</tester>");
                }
            }

            // execute the request and return the first result
            TesterBuilder.ReplaceTesterWithTemp(testerObj, TesterBuilder.ProcessTesterRequest(beta, request).First());
            testerObj.Beta = beta;
        }

        /// <summary>
        /// Send a <see cref="T:Compulsivio.Prefinery.Tester"/>'s changes to Prefinery.
        /// </summary>
        /// <param name="tester">The <see cref="T:Compulsivio.Prefinery.Tester"/> whose changes are to be sent.</param>
        /// <exception cref="T:System.InvalidOperationException"><paramref name="tester"/> is not associated with any beta.</exception>
        internal void UpdateTester(ITester tester)
        {
            // make sure we're doing the right thing
            if (!tester.Id.HasValue)
            {
                throw new InvalidOperationException("Add tester before updating it");
            }

            var testerObj = tester as Tester;
            if (testerObj == null)
            {
                throw new System.ArgumentException("Incompatable ITester implementation");
            }

            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers/{3}.xml?api_key={2}", this.AccountName, testerObj.Beta.Id, this.ApiKey, tester.Id))
                as HttpWebRequest;
            request.Method = "PUT";

            request.ContentType = "text/xml";
            using (var stream = request.GetRequestStream())
            {
                using (var writer = new System.IO.StreamWriter(stream, new UTF8Encoding()))
                {
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    writer.WriteLine("<tester>");

                    if (!string.IsNullOrEmpty(tester.Email))
                    {
                        writer.WriteLine("<email>{0}</email>", tester.Email);
                    }

                    if (tester.Status != TesterStatus.Unknown && testerObj.Status != TesterStatus.Applied && testerObj.Status != TesterStatus.Imported)
                    {
                        writer.WriteLine("<status>{0}</status>", testerObj.Status.ToString().ToLower());
                    }

                    if (testerObj.Profile.Count > 0)
                    {
                        writer.WriteLine("<profile>");
                        foreach (var pair in testerObj.Profile)
                        {
                            writer.WriteLine("<{0}>{1}</{0}>", pair.Key, pair.Value);
                        }

                        writer.WriteLine("</profile>");
                    }

                    writer.WriteLine("</tester>");
                }
            }

            // execute the request and return the first result
            TesterBuilder.ReplaceTesterWithTemp(testerObj, TesterBuilder.ProcessTesterRequest(testerObj.Beta, request).First());
        }

        /// <summary>
        /// Remove a tester from its associated beta.
        /// </summary>
        /// <param name="tester">A <see cref="T:Compulsivio.Prefinery.Tester"/> to remove.</param>
        /// <exception cref="T:System.ArgumentException">The tester has not be associated with a beta.</exception>
        internal void DeleteTester(ITester tester)
        {
            if (!tester.Id.HasValue)
            {
                throw new ArgumentException("Tester needs to have been added before being deleted");
            }

            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers/{3}.xml?api_key={2}", this.AccountName, tester.Beta.Id, this.ApiKey, tester.Id.Value))
                as HttpWebRequest;
            request.Method = "DELETE";

            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError && (e.Response as HttpWebResponse).StatusCode < HttpStatusCode.InternalServerError)
                {
                    response = e.Response as HttpWebResponse;
                }
                else
                {
                    throw e;
                }
            }

            // HTTP 200 means we're good; anything else means errors
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                throw new PrefineryException(response.StatusDescription);
            }
        }

        /// <summary>
        /// Test if an invite code provided is valid for a given <see cref="T:Compulsivio.Prefinery.Tester"/>.
        /// </summary>
        /// <param name="tester"><see cref="T:Compulsivio.Prefinery.Tester"/> who has provided an invite code.</param>
        /// <param name="inviteCode">The invite code provided by <paramref name="tester"/>.</param>
        /// <returns><value>true</value> if the invide code is valid; <value>false</value> otherwise.</returns>
        /// <exception cref="T:System.Net.WebException">Something went wrong while communicating with Prefinery.</exception>
        internal bool ValidateTesterCode(ITester tester, string inviteCode)
        {
            // build our request
            var request = WebRequest.Create(
                string.Format(
                    "http://{0}.prefinery.com/api/v1/betas/{1}/testers/{3}/verify.xml?api_key={2}&invite_code={4}",
                    new object[] { this.AccountName, tester.Beta.Id, this.ApiKey, tester.Id, inviteCode }
                )
            ) as HttpWebRequest;
            request.Method = "GET";

            // get our response
            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError && (e.Response as HttpWebResponse).StatusCode < HttpStatusCode.InternalServerError)
                {
                    response = e.Response as HttpWebResponse;
                }
                else
                {
                    throw e;
                }
            }

            // HTTP 200 means we're good; anything else means errors
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check in a tester with Prefinery.
        /// </summary>
        /// <param name="tester">A <see cref="T:Compulsivio.Prefinery.Tester"/> to check in.</param>
        /// <exception cref="T:System.ArgumentException">The provided tester is missing both Id and e-mail address.</exception>
        /// <exception cref="T:System.Net.WebException">Something went wrong while communicating with Prefinery.</exception>
        internal void CheckinTester(ITester tester)
        {
            HttpWebRequest request;
            if (tester.Id.HasValue)
            {
                request = WebRequest.Create(
                    string.Format(
                        "http://{0}.prefinery.com/api/v1/betas/{1}/testers/{3}/checkin.xml?api_key={2}",
                        new object[] { this.AccountName, tester.Beta.Id, this.ApiKey, tester.Id }
                    )
                ) as HttpWebRequest;
            }
            else if (!string.IsNullOrEmpty(tester.Email))
            {
                request = WebRequest.Create(
                    string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/checkin.xml?api_key={2}", this.AccountName, tester.Beta.Id, this.ApiKey))
                    as HttpWebRequest;
            }
            else
            {
                throw new ArgumentException("Tester needs either an Id or email address to be checked in");
            }

            request.Method = "POST";

            // get our response
            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    response = e.Response as HttpWebResponse;
                }
                else
                {
                    throw e;
                }
            }

            // HTTP 200 means we're good; anything else means errors
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                throw new PrefineryException(response.StatusDescription);
            }
        }

        #endregion
    }
}
