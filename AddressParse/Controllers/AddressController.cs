using AddressParse.Lib;
using Microsoft.AspNetCore.Mvc;

namespace AddressParse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        IParser<AddressInfo> addressParser;

        public AddressController(IParser<AddressInfo> parser)
        {
            addressParser = parser;
        }

        [HttpPost]
        public AddressInfo Post([FromBody] SourceContent value)
        {
            return addressParser.Parse(value.Content);
        }
    }
}
