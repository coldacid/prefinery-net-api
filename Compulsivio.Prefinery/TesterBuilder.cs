using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;

namespace Compulsivio.Prefinery
{
    internal static class TesterBuilder
    {
        /// <summary>
        /// Current state of the TesterBuilder state machine.
        /// </summary>
        private enum GetTesterState
        {
            /// <summary>Not currently within a &lt;tester&gt; element.</summary>
            None,

            /// <summary>Currently within a &lt;tester&gt; element, but not &lt;profile&gt;.</summary>
            InTester,

            /// <summary>Currently within a &lt;profile&gt; element</summary>
            InProfile
        }

        /// <summary>
        /// Creates a <see cref="T:Compulsivio.Prefinery.Tester"/> from a <see cref="T:System.Xml.XmlTextReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlTextReader"/> to read.</param>
        /// <returns>A <see cref="T:Compulsivio.Prefinery.Tester"/> constructed from values in <paramref name="reader"/>.</returns>
        /// <exception cref="T:Compulsivio.Prefinery.PrefineryException">Unexpected XML from <paramref name="reader"/>.</exception>
        public static Tester Build(IBeta beta, XmlTextReader reader)
        {
            var state = GetTesterState.InTester;
            var tester = new Tester() { Beta = beta };

            while (reader.Read())
            {
                switch (state)
                {
                case GetTesterState.InTester:
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name.ToLower() == "tester")
                    {
                        return tester;
                    }

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
                                tester.Email = value;
                                break;
                            case "id":
                                int id;
                                int.TryParse(value, out id);
                                tester.Id = id;
                                break;
                            case "invitation-code":
                                tester.InviteCode = value;
                                break;
                            case "status":
                                switch (value.ToLower())
                                {
                                case "imported":
                                    tester.Status = TesterStatus.Imported;
                                    break;
                                case "applied":
                                    tester.Status = TesterStatus.Applied;
                                    break;
                                case "invited":
                                    tester.Status = TesterStatus.Invited;
                                    break;
                                case "active":
                                    tester.Status = TesterStatus.Active;
                                    break;
                                default:
                                    tester.Status = TesterStatus.Unknown;
                                    break;
                                }

                                break;
                            case "created-at":
                            case "updated-at":
                                DateTimeOffset date;
                                DateTimeOffset.TryParse(value, out date);
                                if (element == "created-at")
                                {
                                    tester.Created = date;
                                }
                                else
                                {
                                    tester.Updated = date;
                                }

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
                        {
                            tester.Profile[field] = reader.Value.ToString();
                        }
                    }

                    break;
                }
            }

            throw new PrefineryException("should never get here");
        }

        /// <summary>
        /// Execute a given <see cref="T:System.Net.HttpWebRequest"/> and produce a list of <see cref="T:Compulsivio.Prefinery.Tester"/>s
        /// from the response.
        /// </summary>
        /// <param name="request">The <see cref="T:System.Net.HttpWebRequest"/> to execute.</param>
        /// <returns>Enumerable list of <see cref="T:Compulsivio.Prefinery.Tester"/>s returned by Prefinery.</returns>
        /// <exception cref="T:System.Net.WebException">Something went wrong during the HTTP request.</exception>
        /// <exception cref="T:Compulsivio.Prefinery.PrefineryException">The Prefinery API returned an error.</exception>
        public static IEnumerable<Tester> ProcessTesterRequest(IBeta beta, HttpWebRequest request)
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
                {
                    response = e.Response as HttpWebResponse;
                }
                else
                {
                    throw e;
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
                            {
                                yield return Build(beta, reader);
                            }
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
                            {
                                throw new PrefineryException(reader.Value.ToString());
                            }
                        }
                    }
                }
            }

            throw new PrefineryException("Unspecified error");
        }

        /// <summary>
        /// Replaces values in one <see cref="T:Compulsivio.Prefinery.Tester"/> with the values from another.
        /// </summary>
        /// <param name="tester">The <see cref="T:Compulsivio.Prefinery.Tester"/> to be overwritten.</param>
        /// <param name="temp">The <see cref="T:Compulsivio.Prefinery.Tester"/> whose values will be used to overwrite <paramref name="tester"/>.</param>
        public static void ReplaceTesterWithTemp(Tester tester, Tester temp)
        {
            // use a temporary and copy details into tester, yay for classes being by ref
            tester.Id = temp.Id;
            tester.InviteCode = temp.InviteCode;
            tester.Status = temp.Status;
            tester.Email = temp.Email;

            foreach (var pair in temp.Profile)
            {
                tester.Profile[pair.Key] = pair.Value;
            }
        }
    }
}
