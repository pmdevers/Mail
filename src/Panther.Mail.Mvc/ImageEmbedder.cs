namespace Panther.Mail.Mvc
{
    //public class ImageEmbedder
    //{
    //    internal static string ViewDataKey = "Panther.ImageEmbedder";

    //    private readonly Func<string, LinkedResource> _createLinkedResource;
    //    private readonly Dictionary<string, LinkedResource> _images = new Dictionary<string, LinkedResource>();

    //    public ImageEmbedder()
    //    {
    //        _createLinkedResource = CreateLinkedResource;
    //    }

    //    public bool HasImages
    //    {
    //        get { return _images.Count > 0; }
    //    }

    //    public static LinkedResource CreateLinkedResource(string imagePathOrUrl)
    //    {
    //        if (Uri.IsWellFormedUriString(imagePathOrUrl, UriKind.Absolute))
    //        {
    //            var client = new WebClient();
    //            var bytes = client.DownloadData(imagePathOrUrl);
    //            return new LinkedResource(new MemoryStream(bytes));
    //        }
    //        return new LinkedResource(File.OpenRead(imagePathOrUrl));
    //    }

    //    public LinkedResource ReferenceImage(string imagePathOrUrl, string contentType = null)
    //    {
    //        LinkedResource resource;
    //        if (_images.TryGetValue(imagePathOrUrl, out resource))
    //        {
    //            return resource;
    //        }

    //        resource = _createLinkedResource(imagePathOrUrl);

    //        contentType = contentType ?? DetermineContentType(imagePathOrUrl);
    //        if (contentType != null)
    //        {
    //            resource.ContentType = new ContentType(contentType);
    //        }

    //        _images[imagePathOrUrl] = resource;
    //        return resource;
    //    }

    //    string DetermineContentType(string pathOrUrl)
    //    {
    //        if (pathOrUrl == null)
    //        {
    //            throw new ArgumentNullException(nameof(pathOrUrl));
    //        }

    //        var extension = Path.GetExtension(pathOrUrl).ToLowerInvariant();
    //        switch (extension)
    //        {
    //            case ".png":
    //                return "image/png";
    //            case ".jpeg":
    //            case ".jpg":
    //                return "image/jpeg";
    //            case ".gif":
    //                return "image/gif";
    //            default:
    //                return null;
    //        }
    //    }

    //    /// <summary>
    //    ///     Adds recorded <see cref="LinkedResource" /> image references to the given <see cref="AlternateView" />.
    //    /// </summary>
    //    public void AddImagesToView(AlternateView view)
    //    {
    //        foreach (var image in _images)
    //        {
    //            view.LinkedResources.Add(image.Value);
    //        }
    //    }
    //}
}