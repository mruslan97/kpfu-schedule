using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Integration
{
    [TestFixture]
    public class ScheduleSourceShould
    {
        private readonly string _url = "https://kpfu.ru";

        [Test]
        public void ScheduleSourceShouldBeAvailable()
        {
            var request = (HttpWebRequest)WebRequest.Create(_url);
            request.AllowAutoRedirect = false;
            request.Method = "HEAD";
            var response = (HttpWebResponse)request.GetResponse();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
