using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace VertmarketsAssignment
{
    public class SharedClass
    {
        private string uRL = ConfigurationManager.AppSettings["baseURL"] ;

        public string Url
        {
            get { return uRL; }
            set { uRL = value; }
        }

    }
}
