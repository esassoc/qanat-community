using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public class CapturePostData
    {
        public List<string> cssStrings { get; set; }
        public List<string> cssUrls { get; set; }
        public string url { get; set; }
        public string cssSelector { get; set; }
        public string html { get; set; }
    }
}