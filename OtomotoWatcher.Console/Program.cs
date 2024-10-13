

using AngleSharp;
using AngleSharp.Dom;
using OtomotoWatcher.Console;
using OtomotoWatcher.Console.Utils;
using System.ComponentModel;
using System.Net.Http.Headers;


string otomotoUrl = "https://www.otomoto.pl/osobowe/bmw/seria-3/od-2010?search%5Bfilter_enum_fuel_type%5D=petrol&search%5Bfilter_enum_gearbox%5D=automatic&search%5Bfilter_enum_generation%5D=gen-f30-2012&search%5Bfilter_float_engine_capacity%3Afrom%5D=2500&search%5Bfilter_float_price%3Ato%5D=100000&search%5Badvanced_search_expanded%5D=true";
string otomotoUrl2 = "https://www.otomoto.pl/osobowe/bmw/seria-3/od-2010?search%5Bfilter_enum_fuel_type%5D=petrol&search%5Bfilter_enum_gearbox%5D=automatic&search%5Bfilter_enum_generation%5D=gen-f30-2012&search%5Bfilter_float_engine_capacity%3Afrom%5D=2500&search%5Bfilter_float_price%3Ato%5D=100000&page=2&search%5Badvanced_search_expanded%5D=true";
string otomotoUrl3 = "https://www.otomoto.pl/osobowe/bmw/seria-3/od-2010?search%5Bfilter_enum_gearbox%5D=automatic&search%5Bfilter_enum_generation%5D=gen-f30-2012&search%5Bfilter_float_engine_capacity%3Afrom%5D=2500&search%5Bfilter_float_price%3Ato%5D=100000&page=2&search%5Badvanced_search_expanded%5D=true";
string olxUrl = "https://www.olx.pl/motoryzacja/samochody/bmw/?page=1&search%5Bfilter_enum_model%5D%5B0%5D=3-as-sorozat&search%5Bfilter_enum_petrol%5D%5B0%5D=petrol&search%5Bfilter_float_enginesize%3Afrom%5D=2800&search%5Bfilter_float_price%3Ato%5D=92000&search%5Bfilter_float_year%3Afrom%5D=2013&search%5Bfilter_float_year%3Ato%5D=2018";
string olxUrl2 = "https://www.olx.pl/motoryzacja/samochody/bmw/?page=2&search%5Bfilter_enum_model%5D%5B0%5D=3-as-sorozat&search%5Bfilter_enum_petrol%5D%5B0%5D=petrol&search%5Bfilter_float_enginesize%3Afrom%5D=2800&search%5Bfilter_float_price%3Ato%5D=92000&search%5Bfilter_float_year%3Afrom%5D=2013&search%5Bfilter_float_year%3Ato%5D=2018";

/*
var userAgentManager = new UserAgentManager();
using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgentManager.GetOne());
var response = await httpClient.GetStringAsync(otomotoUrl);
*/

var browsingConfig = Configuration.Default.WithDefaultLoader();
var browsingContext = new BrowsingContext(browsingConfig);
var document = await browsingContext.OpenAsync(olxUrl2);

var pagingManager = new PagingManager(document);

var listingsManager = new ListingsManager(document);



int a = 1;

