using System.IO;

namespace PublisherPlus.Patch
{
  internal static class Extensions
  {
    public static string Italic(this string self) => "<i>" + self + "</i>";
    public static string Bold(this string self) => "<b>" + self + "</b>";

    public static bool ExistsNow(this FileSystemInfo self)
    {
      self.Refresh();
      return self.Exists;
    }
  }
}
