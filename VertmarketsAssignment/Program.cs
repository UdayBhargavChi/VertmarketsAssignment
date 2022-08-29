using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertmarketsAssignment
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            string tokenId = string.Empty;
            SharedClass shared = new SharedClass();
            HelperClass hc = new HelperClass(shared);
            tokenId = await hc.GetAuthorizeToken();
            Categories catogries = await hc.GetCategories(tokenId);
            Dictionary<string,List<int>> dataDic= await hc.GetMagazinesData(tokenId,catogries);
            Subscribers sub = await hc.GetSubscribersData(tokenId, dataDic, catogries);
            string response = await hc.PostAnswer(sub.data.Select(i => i.id).ToList(), tokenId);
            Console.WriteLine(response);
            Console.ReadLine();
        }
    }
}
