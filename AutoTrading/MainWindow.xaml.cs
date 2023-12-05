using AutoTrading.Extensions;
using Serilog;
using SSI.FCTrading.Client.Models;
using SSI.FCTrading.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Principal;
using System.Diagnostics.Metrics;
using Serilog.Core;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace AutoTrading
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ILogger _logger = null;
        private string _url = @"https://fc-tradeapi.ssi.com.vn";
        private string _urlStream = @"https://fc-tradehub.ssi.com.vn/";
        private string _consumerId = "2398f6cfaefd4d94b213156d01560ca9";
        private string _consumerSecret = "8c8bee69763e4efc99855c3fdbbc9cde";
        private string _privateKey = "PFJTQUtleVZhbHVlPjxNb2R1bHVzPnVNOGROeFc2WFpIR1ZqR1ZrU2ExQi95K0dmRW8wRWZxdnYwaVQwOVBjNUdBZXNWSUo5aVJ6NlZjRlJKRXZtalQ3anVKU20rdE1VOFNpS2pRTjJHQ1JLMnl1dkt2MUt4dXFtd2tFYlo3azJwSkpCOG8xU2pNSDdWMTVHK1VmeUMzS1FOODNla2cycFdpWkJHUDJuQmNsSVVMc0FEcnlNdGMrUW5vbEJKUlU2RT08L01vZHVsdXM+PEV4cG9uZW50PkFRQUI8L0V4cG9uZW50PjxQPjV6Q3hjQTVVUmVLMmV6dHBpTWdNUzU3dDgyTlN3Y09XTjVEQjZXalRrUFMvYkludkZKa2xQWmFsYU9iK1JnMUQwSDBCSk5aa1V6U2hhcjRTVEY1YUtRPT08L1A+PFE+ektRNlJkdDN4UjZJekFRT0E3NE04V3h0TmlaTm5CRUVSUWdWVHo2T2pTZ3RockhNQXNycDBPVThmd2ZHbDJCTnJ1V0hqcUVpUDExZVBNY1N6QzVNdVE9PTwvUT48RFA+d1F5ckRod1pDT1pnWkpUZThpWENCcDltcVRkR0VxREUzZzlWclJjb20wR1VXd2p2Q0M4OXBxa1Y1SHdHMWU0YnM0dSttY2tncTA0bWYrREpuTldveVE9PTwvRFA+PERRPlhJQi90c1FWeDR5ZDJWcWIyeVUxUkl2MmNkdXVpVFZIOE14T2xadDVaR1VjN3gyL1VpUFd2UTVoNGlucG90TnRTZm1HNVBvQk9STkRYY1crd1h6TVFRPT08L0RRPjxJbnZlcnNlUT5keWd4b1RXaEJFMlJ5VGdLc2c0bnFNUnZ1ZEZOVEROeWdwNjBleWhPNmJpQnQ0b0I0V0EreEhsK2U4MXc1SHVmQ3B3aWc4eGhtbElxWGlvZFNvWEZ2QT09PC9JbnZlcnNlUT48RD5LMWJleWhFOVlENFVYaGhJcy81NHk4TzRyUXJDQUg0dERkYjlHYzVHbXBxUG43cnVIZklLMThBMnBQWmd5cXA0c05DamRLSlY0azloMEV5T2I3NWROdW16MjZxcjRsQkJBYlJvSzhNVlB5Nm00NTYrVkRvWTY5UVcrc2RidWNibmRwaDFQZjRqb1RNems2akJqdmFqT0hrMlh2SXFCbkdkSC9penlzMm1Fc0U9PC9EPjwvUlNBS2V5VmFsdWU+";
        private bool _isSave = false;
        private string _code = "L686868@3i";
        private string _token = string.Empty;

        private AuthenProvider _authenProvider = null;
        public MainWindow()
        {
            InitializeComponent();

            _logger = MyLogger.CreateLogger();

           
        }

        private void New_Order_Click(object sender, RoutedEventArgs e)
        {
            var client = new ApiClient(_url, _authenProvider, _logger);
            
            var request = new NewOrderRequest()
            {
                account = account_txt.Text,
                buySell = "B", //B or S
                channelID = "IW",  //TA ("instrumentID": "VN30F2306") , WT (“instrumentID":"VN30F2106") , IW (instrumentID: "SSI")
                instrumentID = instrument_id.Text, //instrumentID: "SSI"
                market = "VN",  //VN: stock market   VNFE: derivatives market
                orderType = cb_orderType.Text,  //LO , ATO , ATC , MP ,MTL ,MOK ,MAK ,PLO
                price = double.Parse(price_txt.Text),
                quantity = int.Parse(quantity_txt.Text),
                stopOrder = false,// True: For conditional order  , False: for normal order
                modifiable = true,
                code = _code,
                stopPrice = 0,//Default stopprice = 0  , If stoporder = True, stopprice > 0
                stopType = "", // If stoporder = True, stopType in value list (D: Down , U: Up, V: Trailling Up, E: Trailing Down, O: OCO , B: BullBear)
                stopStep = 0, //Default stopstep = 0  If stoporder = True, stopstep >=0
                lossStep = 0, // Default losstep = 0 If stoporder = True and stopstyle = B, lossstep >0
                profitStep = 0 // Default losstep = 0
            };

            
            request.requestID = new Random().Next(0, 9999999).ToString();
            request.deviceId = Extentions.GetMACAddress();

            tbl_status.Text = JsonConvert.SerializeObject(client.NewOrder(request));
           
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            _authenProvider = new AuthenProvider(_url, _consumerId, _consumerSecret, _code,
                                                   _privateKey, _isSave, TwoFactorType.PIN, _logger);

            _token = await _authenProvider.GetAccessToken(true);
            token_txt.Text = _token;
            //parse token
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(_token);

            string sub = token.Subject;

            account_txt.Text = sub + "1";
             

            order_stack.IsEnabled = true;
            token_stack.IsEnabled = false;
        }
    }
}
