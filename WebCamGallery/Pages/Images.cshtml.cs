using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

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

		public IActionResult OnGet(string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) return NotFound();

			if (Directory.Exists(_imageDirectory))
			{
				var path = Path.Combine(_imageDirectory, fileName);
				if (System.IO.File.Exists(path))
				{
					if (Path.GetDirectoryName(path) == _imageDirectory)
					{
						//var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
						return base.PhysicalFile(path, "image/jpg"/*, fileName*/);
					}
				}
			}
			return NotFound();
		}
	}
}
