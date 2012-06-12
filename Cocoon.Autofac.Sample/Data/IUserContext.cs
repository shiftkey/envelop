using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocoon.Autofac.Sample.Data
{
    public interface IUserContext
    {
        string GetHomePage();
    }

    public class UserContext : IUserContext
    {
        public string GetHomePage()
        {
            return "InterestingPhotos";
        }
    }
}
