using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Claims;
//using static System.Net.Mime.MediaTypeNames;
using HackEstate.Controllers;

[Authorize]
public class FaceComparisonController : BaseController
{
    private readonly IWebHostEnvironment _env;

    private readonly string apiKey = "n-uANst-d1I8RrgncEeiMpgX4IaDOcxp";
    private readonly string apiSecret = "3vRRxZf6VZgCIr1cnrKRj3iSEOYEDicH";
    private readonly string apiUrl = "https://api-us.faceplusplus.com/facepp/v3/compare";
    private readonly string detectApiUrl = "https://api-us.faceplusplus.com/facepp/v3/detect";
    private readonly HttpClient _httpClient;

    public FaceComparisonController(IWebHostEnvironment env)
    {
        _env = env;
        _httpClient = new HttpClient();
    }

    [HttpPost]
    public async Task<JsonResult> CompareImages([FromBody] dynamic payload)
    {
        try
        {
            string capturedImageBase64 = payload.capturedImageBase64;
            int userId = int.Parse(User.FindFirstValue("UserId"));
            var user = _userRepo.Get(userId);
            if (user == null || string.IsNullOrEmpty(user.IdentificationCardUrl))
            {
                return Json(new { success = false, message = "Profile picture not found." });
            }

            string profilePicturePath = Path.Combine(_env.WebRootPath, user.IdentificationCardUrl.TrimStart('~', '/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (!System.IO.File.Exists(profilePicturePath))
            {
                return Json(new { success = false, message = "Profile picture file not found." });
            }

            // Compress images
            string compressedCapturedImageBase64 = CompressImageBase64(capturedImageBase64, 1024, 1024, 75);

            byte[] profilePictureBytes = await System.IO.File.ReadAllBytesAsync(profilePicturePath);
            string profilePictureBase64 = Convert.ToBase64String(profilePictureBytes);
            string compressedProfilePictureBase64 = CompressImageBase64(profilePictureBase64, 1024, 1024, 75);

            JObject result = await CompareFaces(compressedCapturedImageBase64, compressedProfilePictureBase64);
            int faces = await DetectNumberOfFaces(compressedCapturedImageBase64);

            double confidence = result["confidence"]?.ToObject<double>() ?? 0;

            return Json(new { success = true, data = new { confidence }, noOfFaces = faces });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private string CompressImageBase64(string base64Image, int maxWidth, int maxHeight, int quality)
    {
        byte[] imageBytes = Convert.FromBase64String(base64Image);
        using var inputStream = new MemoryStream(imageBytes);
        using var image = Image.FromStream(inputStream);

        int newWidth = image.Width;
        int newHeight = image.Height;

        if (image.Width > maxWidth || image.Height > maxHeight)
        {
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);
            newWidth = (int)(image.Width * ratio);
            newHeight = (int)(image.Height * ratio);
        }

        using var bitmap = new Bitmap(image, newWidth, newHeight);
        using var outputStream = new MemoryStream();

        var encoder = GetEncoder(ImageFormat.Jpeg);
        var encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

        bitmap.Save(outputStream, encoder, encoderParams);
        return Convert.ToBase64String(outputStream.ToArray());
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        return ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == format.Guid);
    }

    private async Task<JObject> CompareFaces(string imageBase64_1, string imageBase64_2)
    {
        var content = new MultipartFormDataContent
        {
            { new StringContent(apiKey), "\"api_key\"" },
            { new StringContent(apiSecret), "\"api_secret\"" },
            { new StringContent(imageBase64_1), "\"image_base64_1\"" },
            { new StringContent(imageBase64_2), "\"image_base64_2\"" }
        };

        var response = await _httpClient.PostAsync(apiUrl, content);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Compare API Error: {response.StatusCode} - {responseString}");
        }

        return JObject.Parse(responseString);
    }

    private async Task<int> DetectNumberOfFaces(string imageBase64)
    {
        var content = new MultipartFormDataContent
        {
            { new StringContent(apiKey), "\"api_key\"" },
            { new StringContent(apiSecret), "\"api_secret\"" },
            { new StringContent(imageBase64), "\"image_base64\"" }
        };

        var response = await _httpClient.PostAsync(detectApiUrl, content);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Detect API Error: {response.StatusCode} - {responseString}");
        }

        var json = JObject.Parse(responseString);
        var faces = json["faces"] as JArray;
        return faces?.Count ?? 0;
    }
}
