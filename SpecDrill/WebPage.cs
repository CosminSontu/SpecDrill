﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SpecDrill.AutomationScopes;
using SpecDrill.Infrastructure.Logging;
using SpecDrill.Infrastructure.Logging.Interfaces;
using SpecDrill.SecondaryPorts.AutomationFramework;
using SpecDrill.SecondaryPorts.AutomationFramework.Core;

namespace SpecDrill
{
    public class WebPage : ElementBase, IPage
    {
        protected ILogger Log = Infrastructure.Logging.Log.Get<WebPage>();
        private string titlePattern;
        public WebPage(string titlePattern) : base(null, ElementLocator.Create(By.TagName, "html"))
        {
            this.titlePattern = titlePattern;
        }

        public string Title
        {
            get
            {
                string retrievedTitle = null;
                try
                {
                    retrievedTitle = this.Browser.PageTitle;
                }
                catch (Exception e)
                {
                    Log.Error("Cannot read page Title!", e);
                }
                return retrievedTitle;
            }
        }
        

        //public IElement Element
        //{
        //    get { return rootElement; }
        //}

        #region IPage
        public virtual bool IsLoaded
        {
            get
            {
                object result =
                this.Browser.ExecuteJavascript(@"
                    if (document.readyState !== 'complete') {
                        return false;
                    }
                    if ((!document.jQuery) || (document.jQuery.active || (document.jQuery.ajax && document.jQuery.ajax.active))) {
                        return false;
                    } 
				    if (document.angular) {
                        if (!window.specDrill) {
                            window.specDrill = { silence : false };
                        }
                        var injector = window.angular.element('body').injector();
                        var $rootScope = injector.get('$rootScope');
                        var $http = injector.get('$http');
                        var $timeout = injector.get('$timeout');
                         
                        if ($rootScope.$$phase === '$apply' || $rootScope.$$phase === '$digest' || $http.pendingRequests.length != 0) {
                            window.specDrill.silence = false;
                            return false;
                        }

                        if (!window.specDrill.silence) {
                            $timeout(function () { window.specDrill.silence = true; }, 0);
                            return false;
                        }
                    }
                    return true;
                ");

                var isLoaded = this.Title != null &&
                               Regex.IsMatch(this.Title, this.titlePattern);

                Log.Info("LoadCompleted = {0}, retrievedTitle = {1}, patternToMatch = {2}", isLoaded, this.Title ?? "(null)",
                    this.titlePattern ?? "(null)");

                return isLoaded;

            }
        }

        public void RefreshPage()
        {
            browser.RefreshPage();
            Wait.Until(() => this.IsLoaded);
            this.WaitForSilence();
        }

        public virtual void WaitForSilence()
        {
        }

        // try and see if virtual IsPageLoaded can be used to sum up all kinds of wait (static, jQuery, Angular1, Angular2, etc)
        // goal is to have an immediately returning test so we can wait on it
        // currently there is no IsPageLoaded method on IPage so we can use in lambda
        #endregion
    }
}
