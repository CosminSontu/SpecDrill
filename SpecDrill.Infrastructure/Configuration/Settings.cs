﻿using SpecDrill.Configuration.WebDriver;
using SpecDrill.Configuration.Homepages;
using SpecDrill.Infrastructure.Configuration;
using SpecDrill.Infrastructure.Configuration.WebDriver.Browser.Window;

namespace SpecDrill.Configuration
{
    public class Settings
    {
        public WebDriverConfiguration WebDriver { get; set; }

        public HomepageConfiguration[] Homepages { get; set; }
    }
}
