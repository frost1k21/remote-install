using System;
using System.Threading.Tasks;
using RemoteInstall.ConfigFromFile.Models;

namespace RemoteInstall.WpfUI.Services
{
    public class FakeInstallOrEditService : IInstallOrEditService
    {
        public async Task<string> Install(string computerName, InstallSettings installSettings)
        {
            var statuses = new string[]
            {
                "0",
                "13",
                "87",
                "120",
                "1259",
                "1601",
                "1602",
                "1603",
                "1604",
                "1605",
                "1606",
                "1607",
                "1608",
                "1609",
                "1610",
                "1611",
                "1612",
                "1613",
                "1614",
                "1615",
                "1616",
                "1618",
                "1619",
                "1620",
                "1621",
                "1622",
                "1623",
                "1624",
                "1625",
                "1626",
                "1627",
                "1628",
                "1629",
                "1630",
                "1631",
                "1632",
                "1633",
                "1634",
                "1635",
                "1636",
                "1637",
                "1638",
                "1639",
                "1640",
                "1641",
                "1642",
                "1643",
                "1644",
                "1645",
                "1646",
                "1647",
                "1648",
                "1649",
                "1650",
                "1651",
                "3010",
                "2147549445"
            };
            var rnd = new Random();
            var rndNumber = rnd.Next(0, statuses.Length);
            await Task.Delay(1000);
            return statuses[rndNumber];
        }

        public Task<string> EditBase(string computerName, OneCBases oneCBases)
        {
            throw new System.NotImplementedException();
        }
    }
}
