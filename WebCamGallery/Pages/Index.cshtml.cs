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
	public class IndexModel : PageModel
	{
		private readonly string _imageDirectory;

		public IEnumerable<string> Jpgs { get; private set; }

		public IndexModel(IConfiguration configuration)
		{
			var imageDirectory = configuration["ImageDirectory"];
			_imageDirectory = imageDirectory;
		}

		public void OnGet()
		{
			if (_imageDirectory != null && Directory.Exists(_imageDirectory))
			{
				var di = new DirectoryInfo(_imageDirectory);
				var files = di.EnumerateFiles("*.jpg", SearchOption.TopDirectoryOnly);

				Jpgs = files.OrderByDescending(f => f.LastWriteTime).Select(x => x.Name);
			}
		}
	}
}

