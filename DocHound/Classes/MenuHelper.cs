using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DocHound.Classes
{
    public static class MenuHelper
    {
        public static string GetMenu(IHtmlHelper helper, IEnumerable<MainMenuItem> menu, MenuMode mode = MenuMode.Top)
        {
            var mainMenuItems = menu as MainMenuItem[] ?? menu.ToArray();
            if (!mainMenuItems.Any()) return string.Empty;

            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var item in mainMenuItems)
            {
                sb.Append("<li>");
                sb.Append("<a class=\"area-link\" href=\"" + item.Link + "\">" + item.Title + "</a>");
                sb.Append("</li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
    }

    public enum MenuMode
    {
        Top,
        Mobile
    }
}
