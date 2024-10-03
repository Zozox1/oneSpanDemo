using Microsoft.AspNetCore.Mvc;
using oneSpanDemo.Models;

namespace oneSpanDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Documents : ControllerBase
    {
        Iedocument oneSpanIntegrator;

        public Documents(Iedocument _oneSpanIntegrator) 
        {
            oneSpanIntegrator = _oneSpanIntegrator;
        }


        [HttpGet(Name = "GetDocuments")]
        public async Task<IActionResult> Get(string transaction)
        {
            var documentsZip = await oneSpanIntegrator.GetOneSpanDocument(transaction);

            if(documentsZip == null || documentsZip.status == DocumentsResultStatus.UnAbleToLoad || documentsZip.status == DocumentsResultStatus.NoFile)
                return NotFound();

            else if (documentsZip.status == DocumentsResultStatus.UnAuthorized)
                return Unauthorized();



             return File(documentsZip.stream, "application/zip");
        }
    }
}
