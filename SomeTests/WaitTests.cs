﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill;
using System.Diagnostics;
using FluentAssertions;

namespace SomeTests
{
    [TestClass]
    public class WaitTests
    {
        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void ShouldThrowExceptionWhenConditionRemainsFalseAfterMaxWait()
        {
            var maxWait = TimeSpan.FromMilliseconds(300);
            
            Wait.NoMoreThan(maxWait).Until(() => false);
        }

        [TestMethod]
        public void ShouldWaitMaxIntervalWhenConditionRemainsFalseAfterMaxWait()
        {
            var maxWait = TimeSpan.FromMilliseconds(300);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                Wait.NoMoreThan(maxWait).Until(() => false);
            }
            catch
            {
            }
            stopwatch.Stop();
            Assert.IsTrue(stopwatch.Elapsed > maxWait);
        }

        [TestMethod]
        public void ShouldReturnAsSoonAsConditionBecomesTrueForMaxWait()
        {
            var maxWait = TimeSpan.FromMilliseconds(300);
            var timeUntilConditionIsTrue = TimeSpan.FromMilliseconds(100);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
                Wait.NoMoreThan(maxWait).Until(() => stopwatch.Elapsed >= timeUntilConditionIsTrue);
            
            stopwatch.Stop();
            stopwatch.Elapsed.Should().BeCloseTo(timeUntilConditionIsTrue, 50);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void ShouldThrowExceptionWhenConditionRemainsFalseAfterRetries()
        {
            var retryInterval = TimeSpan.FromMilliseconds(100);

            Wait.WithRetry(3, retryInterval).Until(() => false);
        }

        [TestMethod]
        public void ShouldWaitFullRetryTimeWhenConditionRemainsFalseAfterRetries()
        {
            var retryInterval = TimeSpan.FromMilliseconds(100);
            const int retryCount = 3;
            var stopwatch = new Stopwatch();
            
            try
            {
                stopwatch.Start();
                Wait.WithRetry(retryCount, retryInterval).Until(() => false);
            }
            catch
            {
                stopwatch.Stop();
            }
            
            Assert.IsTrue(stopwatch.Elapsed > TimeSpan.FromMilliseconds(retryInterval.Milliseconds * (retryCount + 1) /*initial call*/), string.Format("Elapsed was {0}", stopwatch.Elapsed));
        }

        [TestMethod]
        public void ShouldReturnAsSoonAsConditionBecomesTrueByRreachingEstablishedTimeLimitForRetryWait()
        {
            var retryInterval = TimeSpan.FromMilliseconds(100);
            var timeUntilConditionBecomesTrue = TimeSpan.FromMilliseconds(250);
            var retriesDone = 0;
            const int retryCount = 3;
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                Wait.WithRetry(retryCount, retryInterval).Doing(() => retriesDone++).Until(() => stopwatch.Elapsed > timeUntilConditionBecomesTrue);
                stopwatch.Stop();
            }
            catch
            {
                
            }

            stopwatch.Elapsed.Should().BeCloseTo(timeUntilConditionBecomesTrue, 50);
        }

        [TestMethod]
        public void ShouldReturnAsSoonAsConditionBecomesTrueByReachingEstablishedRetriesCountForRetryWait()
        {
            var retryInterval = TimeSpan.FromMilliseconds(100);
            var timeUntilConditionBecomesTrue = TimeSpan.FromMilliseconds(250);
            var retriesDone = 0;
            const int retryCount = 3;

            Wait.WithRetry(retryCount, retryInterval).Doing(() => retriesDone++).Until(() => retriesDone == 2);
            
            Assert.IsTrue(retriesDone == 2, "Retries Count is different than expected");
        }
    }
}
