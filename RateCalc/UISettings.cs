using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace RateCalc
{


    internal class UISettings : ConfigurationSection
    {
        [ConfigurationProperty("language", DefaultValue = "en")]
        public String language
        {
            get { return (String)this["language"]; }
            set { this["language"] = value; }
        }
        [ConfigurationProperty("decimalSeparator", IsRequired = false, DefaultValue = '.')]
        public char DecimalSeparator
        {
            get => (char)this["decimalSeparator"];
            set => this["decimalSeparator"] = value;
        }
        [ConfigurationProperty("thousandSeparator", IsRequired = false, DefaultValue = ',')]
        public char ThousandSeparator
        {
            get => (char)this["thousandSeparator"];
            set => this["thousandSeparator"] = value;
        }
    }
}
