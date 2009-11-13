using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace Compulsivio.Prefinery
{
    public class PrefineryCore
    {
        public int BetaId { get; private set; }
        public string BetaName { get; private set; }
        public string ApiKey { get; private set; }
        public string DecodeKey { get; private set; }

        public PrefineryCore(int betaId, string betaName, string apiKey, string decodeKey)
        {
            BetaId = betaId;
            BetaName = betaName;
            ApiKey = apiKey;
            DecodeKey = decodeKey;
        }

        private string EncodeEmail(string email)
        {
            var encoder = new ASCIIEncoding();
            var sha1 = System.Security.Cryptography.SHA1.Create();

            var bytes = encoder.GetBytes(DecodeKey + email.ToLower());

            var rightCode = new StringBuilder(42);
            foreach (var b in sha1.ComputeHash(bytes))
                rightCode.Append(b.ToString("X2"));

            return rightCode.ToString();
        }

        public bool IsValidInviteCode(string email, string invite)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("email");
            if (string.IsNullOrEmpty(invite))
                throw new ArgumentNullException("invite");

            return EncodeEmail(email).Equals(invite);
        }

        private enum GetTesterState { None, InTester, InProfile };
        private Tester BuildTester(XmlTextReader reader)
        {
            var state = GetTesterState.InTester;
            var tester = new Tester() { Repository = this };

            while (reader.Read())
            {
                switch (state)
                {
                case GetTesterState.InTester:
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower() == "tester")
                        return tester;
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.ToLower() == "profile")
                        {
                            state = GetTesterState.InProfile;
                            break;
                        }
                        var element = reader.Name.ToLower();
                        if (reader.Read())
                        {
                            var value = reader.Value.ToString();
                            switch (element)
                            {
                            case "email":
                                tester.Email = value; break;
                            case "id":
                                int id;
                                int.TryParse(value, out id);
                                tester.Id = id;
                                break;
                            case "invitation-code":
                                tester.InviteCode = value; break;
                            case "status":
                                switch (value.ToLower())
                                {
                                case "imported":
                                    tester.Status = TesterStatus.Imported; break;
                                case "applied":
                                    tester.Status = TesterStatus.Applied; break;
                                case "invited":
                                    tester.Status = TesterStatus.Invited; break;
                                case "active":
                                    tester.Status = TesterStatus.Active; break;
                                default:
                                    tester.Status = TesterStatus.Unknown; break;
                                }
                                break;
                            case "created-at":
                            case "updated-at":
                                DateTimeOffset date;
                                DateTimeOffset.TryParse(value, out date);
                                if (element == "created-at")
                                    tester.Created = date;
                                else
                                    tester.Updated = date;
                                break;
                            default:
                                break;
                            }
                        }
                    }
                    break;

                case GetTesterState.InProfile:
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower() == "profile")
                    {
                        state = GetTesterState.InTester;
                        break;
                    }
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        var field = reader.Name;
                        if (reader.Read())
                            tester.Profile[field] = reader.Value.ToString();
                    }
                    break;
                }
            }
            throw new PrefineryException("should never get here");
        }

        private IEnumerable<Tester> ProcessTesterRequest(HttpWebRequest request)
        {
            // get our response
            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                    response = e.Response as HttpWebResponse;
                else
                {
                    throw new PrefineryException(e);
                }
            }

            // HTTP 200 means we're good; anything else means errors
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new XmlTextReader(stream))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "tester")
                                yield return this.BuildTester(reader);
                        }
                        yield break;
                    }
                }
            }

            using (var stream = response.GetResponseStream())
            {
                using (var reader = new XmlTextReader(stream))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name.ToLower() == "error")
                        {
                            if (reader.Read())
                                throw new PrefineryException(reader.Value.ToString());
                        }
                    }
                }
            }

            throw new PrefineryException("Unspecified error");
        }

        public Tester GetTester(int id)
        {
            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers/{3}.xml?api_key={2}", BetaName, BetaId, ApiKey, id))
                as HttpWebRequest;
            request.Method = "GET";

            return this.ProcessTesterRequest(request).First();
        }

        public Tester GetTester(string email)
        {
            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers.xml?api_key={2}&email={3}", BetaName, BetaId, ApiKey, email))
                as HttpWebRequest;
            request.Method = "GET";

            return this.ProcessTesterRequest(request).First();
        }

        public IEnumerable<Tester> GetTesters()
        {
            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers.xml?api_key={2}", BetaName, BetaId, ApiKey))
                as HttpWebRequest;
            request.Method = "GET";

            return this.ProcessTesterRequest(request);
        }

        private void ReplaceTesterWithTemp(Tester tester, Tester temp)
        {
            // use a temporary and copy details into tester, yay for classes being by ref
            tester.Id = temp.Id;
            tester.InviteCode = temp.InviteCode;
            tester.Status = temp.Status;
            tester.Email = temp.Email;
            foreach (var pair in temp.Profile)
                tester.Profile[pair.Key] = pair.Value;
        }

        public void AddTester(Tester tester)
        {
            // make sure we're doing the right thing
            if (tester.Id.HasValue)
                throw new InvalidOperationException("Tester has Id and so has already been added");
            if (string.IsNullOrEmpty(tester.Email))
                throw new ArgumentException("Tester needs email address before it can be added");
            if (tester.Status == TesterStatus.Rejected)
                throw new ArgumentException("Tester must be added before it can be rejected");

            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers.xml?api_key={2}", BetaName, BetaId, ApiKey))
                as HttpWebRequest;
            request.Method = "POST";

            request.ContentType = "text/xml";
            using (var stream = request.GetRequestStream())
            {
                using (var writer = new System.IO.StreamWriter(stream, new UTF8Encoding()))
                {
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    writer.WriteLine("<tester>");
                    writer.WriteLine("<email>{0}</email>", tester.Email);

                    if (!string.IsNullOrEmpty(tester.InviteCode))
                        writer.WriteLine("<invitation-code>{0}</invitation-code>", tester.InviteCode);
                    if (tester.Status != TesterStatus.Unknown)
                        writer.WriteLine("<status>{0}</status>", tester.Status.ToString().ToLower());

                    if (tester.Profile.Count > 0)
                    {
                        writer.WriteLine("<profile>");
                        foreach (var pair in tester.Profile)
                            writer.WriteLine("<{0}>{1}</{0}>", pair.Key, pair.Value);
                        writer.WriteLine("</profile>");
                    }

                    writer.WriteLine("</tester>");
                }
            }

            ReplaceTesterWithTemp(tester, this.ProcessTesterRequest(request).First());
        }

        public void UpdateTester(Tester tester)
        {
            // make sure we're doing the right thing
            if (!tester.Id.HasValue)
                throw new InvalidOperationException("Add tester before updating it");

            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers/{3}.xml?api_key={2}", BetaName, BetaId, ApiKey, tester.Id))
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
                        writer.WriteLine("<email>{0}</email>", tester.Email);

                    if (tester.Status != TesterStatus.Unknown && tester.Status != TesterStatus.Applied && tester.Status != TesterStatus.Imported)
                        writer.WriteLine("<status>{0}</status>", tester.Status.ToString().ToLower());

                    if (tester.Profile.Count > 0)
                    {
                        writer.WriteLine("<profile>");
                        foreach (var pair in tester.Profile)
                            writer.WriteLine("<{0}>{1}</{0}>", pair.Key, pair.Value);
                        writer.WriteLine("</profile>");
                    }

                    writer.WriteLine("</tester>");
                }
            }

            ReplaceTesterWithTemp(tester, this.ProcessTesterRequest(request).First());
        }

        public void DeleteTester(Tester tester)
        {
            if (!tester.Id.HasValue)
                throw new ArgumentException("Tester needs to have been added before being deleted");

            DeleteTester(tester.Id.Value);
        }

        public void DeleteTester(int id)
        {
            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers/{3}.xml?api_key={2}", BetaName, BetaId, ApiKey, id))
                as HttpWebRequest;
            request.Method = "DELETE";
        }

        public bool ValidateCode(Tester tester, string inviteCode)
        {
            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers/{3}/verify.xml?api_key={2}&invite_code={4}", BetaName, BetaId, ApiKey, tester.Id, inviteCode))
                as HttpWebRequest;
            request.Method = "GET";

            // get our response
            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                    response = e.Response as HttpWebResponse;
                else
                    throw new PrefineryException(e);
            }

            // HTTP 200 means we're good; anything else means errors
            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            else if (response.StatusCode >= HttpStatusCode.InternalServerError)
                throw new PrefineryException(string.Format("Server error {0}", response.StatusCode));
            else
                return false;
        }

        public void CheckinTester(Tester tester)
        {
            if (tester.Id.HasValue)
                CheckinTester(tester.Id.Value);
            else if (!string.IsNullOrEmpty(tester.Email))
                CheckinTester(tester.Email);
            else
                throw new ArgumentException("Tester needs either an Id or email address to be checked in");
        }

        public void CheckinTester(int id)
        {
            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/testers/{3}/checkin.xml?api_key={2}", BetaName, BetaId, ApiKey, id))
                as HttpWebRequest;
            request.Method = "GET";

            // get our response
            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                    response = e.Response as HttpWebResponse;
                else
                    throw new PrefineryException(e);
            }
        }

        public void CheckinTester(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("email");

            // build our request
            var request = WebRequest.Create(
                string.Format("http://{0}.prefinery.com/api/v1/betas/{1}/checkin.xml?api_key={2}", BetaName, BetaId, ApiKey))
                as HttpWebRequest;
            request.Method = "GET";

            request.ContentType = "text/xml";
            using (var stream = request.GetRequestStream())
            {
                using (var writer = new System.IO.StreamWriter(stream, new UTF8Encoding()))
                {
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    writer.WriteLine("<checkin>");
                    writer.WriteLine("<email>{0}</email>", email);
                    writer.WriteLine("</checkin>");
                }
            }

            // get our response
            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                    response = e.Response as HttpWebResponse;
                else
                    throw new PrefineryException(e);
            }
        }
    }
}
