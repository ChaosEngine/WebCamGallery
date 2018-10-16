using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Globalization;
using System.IO;

namespace WebCamGallery.Pages
{
	public class ImagesModel : PageModel
	{
		private readonly string _imageDirectory;

		public ImagesModel(IConfiguration configuration)
		{
			var imageDirectory = configuration["ImageDirectory"];
			_imageDirectory = imageDirectory;
		}

		[ResponseCache(VaryByHeader = "User-Agent", Duration = 30, Location = ResponseCacheLocation.Any)]
		public IActionResult OnGet(string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) return NotFound();

			var path = Path.Combine(_imageDirectory, fileName);
			if (System.IO.File.Exists(path))
			{
				if (Path.GetDirectoryName(path) == _imageDirectory)
				{
					var fi = new FileInfo(path);
					var length = fi.Length;
					DateTimeOffset last = fi.LastWriteTime;
					// Truncate to the second.
					var lastModified = new DateTimeOffset(last.Year, last.Month, last.Day, last.Hour, last.Minute, last.Second, last.Offset).ToUniversalTime();

					long etagHash = lastModified.ToFileTime() ^ length;
					var etag_str = '\"' + Convert.ToString(etagHash, 16) + '\"';
					
					string incomingEtag = (Request.Headers as FrameRequestHeaders).HeaderIfNoneMatch;

					if (String.Compare(incomingEtag, etag_str) == 0)
					{
						Response.Clear();
						Response.StatusCode = (int)System.Net.HttpStatusCode.NotModified;
						return new StatusCodeResult((int)System.Net.HttpStatusCode.NotModified);
					}
					var etag = new EntityTagHeaderValue(etag_str);

					//var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
					PhysicalFileResult pfr = base.PhysicalFile(path, "image/jpg");
					pfr.EntityTag = etag;

					return pfr;

					//var image = new FileInfo(path);
					//if (!String.IsNullOrEmpty(Request.Headers["If-Modified-Since"]))
					//{
					//	CultureInfo provider = CultureInfo.InvariantCulture;
					//	var lastMod = DateTime.ParseExact(Request.Headers["If-Modified-Since"], "r", provider).ToLocalTime();
					//	if (lastMod == image.LastWriteTime.AddMilliseconds(-image.LastWriteTime.Millisecond))
					//	{
					//		Response.StatusCode = 304;
					//		//Response.StatusDescription = "Not Modified";
					//		return Content(String.Empty);
					//	}
					//}
					//var stream = new MemoryStream(image.GetImage());
					//Response.Cache.SetCacheability(HttpCacheability.Public);
					//Response.Cache.SetLastModified(image.TimeStamp);
					//return File(stream, image.MimeType);

				}
			}
			return NotFound();
		}
	}
}
