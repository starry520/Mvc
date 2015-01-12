using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using System.Collections.Generic;

namespace ModelBindingWebSite.Controllers
{
    public class FileUploadController : Controller
    {
        public IActionResult UploadSingle(IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest();
            }
            return Created("", file);
        }

        public IActionResult UploadMultiple(IEnumerable<IFormFile> files)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest();
            }
            return Created("", files);
        }
    }
}
