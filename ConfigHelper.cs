using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPublic
{
    public static class ConfigHelper
    {
        public static double GetDoubleConfig(string key)
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrEmpty(value))
                {
                    double dvalue;
                    if (double.TryParse(value, out dvalue))
                    {
                        return dvalue;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        public static string GetStrConfig(string key)
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings[key];
            }
            catch (Exception ex)
            {
            }
            return "";
        }

        public static double GetConfig(string v)
        {
            throw new NotImplementedException();
        }
    }
}
