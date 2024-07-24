using LearnAPI.Helper;
using LearnAPI.Repos;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LearnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly LearndataContext _context;
        public ProductController(IWebHostEnvironment environment, LearndataContext context)
        {
            _environment = environment;
            _context = context;
        }

        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string productcode)
        {
            APIResponse response = new APIResponse();
            try
            {
                string Filepath = GetFilepath(productcode);
                if (!Directory.Exists(Filepath))
                {
                    Directory.CreateDirectory(Filepath);
                }

                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
                using (FileStream stream = System.IO.File.Create(imagepath))
                {
                    await formFile.CopyToAsync(stream);
                    response.ResponseCode = 200;
                    response.Result = "pass";
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("MultiUploadImage")]
        public async Task<IActionResult> MultiUploadImage(IFormFileCollection fileCollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0;
            int errorcount = 0;
            try
            {
                string Filepath = GetFilepath(productcode);
                if (!Directory.Exists(Filepath))
                {
                    Directory.CreateDirectory(Filepath);
                }
                foreach (var file in fileCollection)
                {
                    string imagepath = Filepath + "\\" + file.FileName;
                    if (!System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }
                    using (FileStream stream = System.IO.File.Create(imagepath))
                    {
                        await file.CopyToAsync(stream);
                        passcount++;

                    }
                }

            }
            catch (Exception ex)
            {
                errorcount++;
                response.ErrorMessage = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = $"{passcount} Files uploaded & {errorcount} files failed";
            return Ok(response);
        }

        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string productcode)
        {
            string Imageurl = string.Empty;
            string hosturl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            try
            {
                string Filepath = GetFilepath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    Imageurl = hosturl + "/Upload/product/" + productcode + "/" + productcode + ".png";
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Ok(Imageurl);
        }

        [HttpGet("GetCollectionImage")]
        public async Task<IActionResult> GetCollectionImage(string productcode)
        {
            List<string> Imageurls = new List<string>();
            string hosturl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            try
            {
                string Filepath = GetFilepath(productcode);

                if (Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string filename = fileInfo.Name;
                        string imagepath = Filepath + "\\" + filename;
                        if (System.IO.File.Exists(imagepath))
                        {
                            string _Imageurl = hosturl + "/Upload/product/" + productcode + "/" + filename;
                            Imageurls.Add(_Imageurl);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
            }
            return Ok(Imageurls);
        }

        [HttpGet("download")]
        public async Task<IActionResult> download(string productcode)
        {
            //string Imageurl = string.Empty;
            //string hosturl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            try
            {
                string Filepath = GetFilepath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    MemoryStream stream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(imagepath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/png", productcode + ".png");

                    //Imageurl = hosturl + "/Upload/product/" + productcode + "/" + productcode + ".png";
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpDelete("remove")]
        public async Task<IActionResult> remove(string productcode)
        {
            //string Imageurl = string.Empty;
            //string hosturl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            try
            {
                string Filepath = GetFilepath(productcode);
                string imagepath = Filepath + "\\" + productcode + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("multiRemove")]
        public async Task<IActionResult> multiRemove(string productcode)
        {
            //string Imageurl = string.Empty;
            //string hosturl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            try
            {
                string Filepath = GetFilepath(productcode);
                if (Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        fileInfo.Delete();
                    }

                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("DBMultiUploadImage")]
        public async Task<IActionResult> DBMultiUploadImage(IFormFileCollection fileCollection, string productcode)
        {
            APIResponse response = new APIResponse();
            int passcount = 0;
            int errorcount = 0;
            try
            {
                foreach (var file in fileCollection)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        _context.TblProductimages.Add(new Repos.Models.TblProductimage()
                        {
                            Productcode = productcode,
                            Productimage = stream.ToArray()
                        });

                        await _context.SaveChangesAsync();
                        passcount++;
                    }

                }
            }
            catch (Exception ex)
            {
                errorcount++;
                response.ErrorMessage = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = $"{passcount} Files uploaded & {errorcount} files failed";
            return Ok(response);
        }

        [HttpGet("GetDBCollectionImage")]
        public async Task<IActionResult> GetDBCollectionImage(string productcode)
        {
            List<string> Imageurls = new List<string>();            
            try
            {
                var _productImage = _context.TblProductimages.Where(item => item.Productcode == productcode).ToList();
                if (_productImage != null && _productImage.Count > 0)
                {
                    _productImage.ForEach(item =>
                    {
                        Imageurls.Add(Convert.ToBase64String(item.Productimage));
                    });
                }
                else
                {
                    return NotFound();
                }            


            }
            catch (Exception ex)
            {
            }
            return Ok(Imageurls);
        }

        [HttpGet("DBdownload")]
        public async Task<IActionResult> DBdownload(string productcode)
        {
            try
            {

                var _productImage = await _context.TblProductimages.FirstOrDefaultAsync(item => item.Productcode == productcode);
                if (_productImage != null)
                {
                    return File(_productImage.Productimage, "image/png", productcode + ".png");
                }                
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [NonAction]
        private string GetFilepath(string productcode)
        {
            return _environment.WebRootPath + "\\Upload\\product\\" + productcode;
        }

    }
}
