using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DocHound.Classes
{
    public static class CodeHelper
    {
        public static string GetMetaKeywords(dynamic viewBag)
        {
            string keywords = viewBag.Keywords;
            if (string.IsNullOrEmpty(keywords)) keywords = ".NET, iOS, Android, Consulting, Custom Software, Training, CODE Magazine, CODE Consulting, CODE Consulting, CODE Training, CODE Framework, VFP Conversion";
            keywords = keywords.Replace("CoDe", "CODE");
            return keywords.Trim();
        }

        public static string GetMetaAuthors(dynamic viewBag)
        {
            string authors = viewBag.Authors;
            if (string.IsNullOrEmpty(authors)) authors = "CODE Magazine, EPS Software Corp.";
            else
            {
                if (authors.ToLower().IndexOf("eps software corp.", StringComparison.Ordinal) < 0)
                    authors = "EPS Software Corp., " + authors;
                if (authors.ToLower().IndexOf("code magazine", StringComparison.Ordinal) < 0)
                    authors = "CODE Magazine, " + authors;
            }
            authors = authors.Replace("CoDe", "CODE");
            return authors.Trim();
        }

        public static string GetTitle(dynamic viewBag)
        {
            string title = viewBag.Title;

            if (string.IsNullOrEmpty(title)) title = "CODE Online | CODE Consulting | CODE Staffing | CODE Magazine | CODE Training | CODE Framework | VFP Conversion";
            title = title.Replace("CoDe", "CODE");

            return title.Trim();
        }

        public static string GetMetaDescription(dynamic viewBag)
        {
            string description = viewBag.Description;

            if (string.IsNullOrEmpty(description)) description = viewBag.Title;
            if (string.IsNullOrEmpty(description)) description = "CODE Online | CODE Consulting | CODE Staffing | CODE Magazine | CODE Training | CODE Framework | VFP Conversion";

            if (description.Length > 155) description = description.Substring(0, 152) + "...";
            description = description.Replace("CoDe", "CODE");

            return description.Trim();
        }

        public static string GetOgMetaType(dynamic viewBag)
        {
            string type = viewBag.OgType;

            if (string.IsNullOrEmpty(type)) type = "website";

            return type.Trim();
        }

        public static string GetMetaImageTag(dynamic viewBag, MetaNamespace ns = MetaNamespace.OpenGraph)
        {
            var image = viewBag.OgImage;

            if (string.IsNullOrEmpty(image)) return string.Empty;
            if (ns == MetaNamespace.OpenGraph) return "<meta name=\"og:image\" value=\"http://www.codemag.com/ " + image + "\" />";
            if (ns == MetaNamespace.Twitter) return "<meta name=\"twitter:image\" value=\"http://www.codemag.com/ " + image + "\" />";

            return string.Empty;
        }

        /// <summary>
        /// Returns the HTML for the header image
        /// </summary>
        /// <param name="logo">The logo.</param>
        /// <returns></returns>
        public static string HeaderImage(Logos logo)
        {
            var imageName = string.Empty;

            switch (logo)
            {
                case Logos.CodeConsulting:
                    imageName = "CODEConsulting_Small.png";
                    break;
                case Logos.CodeFocusMagazine:
                    imageName = "CODEFocus_Small.png";
                    break;
                case Logos.CodeMagazine:
                    imageName = "CODEMagazine_Small.png";
                    break;
                case Logos.CodeProductFocusMagazine:
                    imageName = "CODEProductFocus_Small.png";
                    break;
                case Logos.CodeStaffing:
                    imageName = "CODEStaffing_Small.png";
                    break;
                case Logos.CodeTraining:
                    imageName = "CODETraining_Small.png";
                    break;
                case Logos.CodeGroup:
                case Logos.CodeOnline:
                    imageName = "CODEGroup_Small.png";
                    break;
                case Logos.StateOfDotNet:
                    imageName = "StateOfDotNet_Small.png";
                    break;
                case Logos.RoadToWindows8:
                    imageName = "CODEConsulting_Small.png";
                    break;
                case Logos.CodeFramework:
                    imageName = "CodeFramework_Small.png";
                    break;
                case Logos.VfpConversion:
                    imageName = "VfpConversion_Small.gif";
                    break;
            }

            return "<img src=\"/Images/Logos/" + imageName + "\" />";
        }

        public static string GetCurrentHeaderImage(string logoOverride = "")
        {
            if (!string.IsNullOrEmpty(logoOverride)) return "<img src=\"" + logoOverride + "\" />";
            return HeaderImage(Logos.CodeFramework);

            //var currentController = GetCurrentController();
            //switch (currentController)
            //{
            //    case "signup":
            //    case "training":
            //        return HeaderImage(Logos.CodeTraining);
            //    case "lunch":
            //        return HeaderImage(Logos.CodeTraining);
            //    case "eventslist":
            //        return HeaderImage(Logos.CodeTraining);
            //    case "consulting":
            //    case "techhelp":
            //    case "energy":
            //    case "cloud":
            //        return HeaderImage(Logos.CodeConsulting);
            //    case "subscribe":
            //    case "renew":
            //    case "write":
            //    case "advertise":
            //    case "magazine":
            //    case "article":
            //        return HeaderImage(Logos.CodeMagazine);
            //    case "focus":
            //        return HeaderImage(Logos.CodeFocusMagazine);
            //    case "staffing":
            //        return HeaderImage(Logos.CodeStaffing);
            //    case "stateofdotnet":
            //        return HeaderImage(Logos.StateOfDotNet);
            //    case "roadtowin8":
            //        return HeaderImage(Logos.RoadToWindows8);
            //    case "framework":
            //        return HeaderImage(Logos.CodeFramework);
            //    case "vfpconversion":
            //        return HeaderImage(Logos.VfpConversion);
            //    default:
            //        return HeaderImage(Logos.CodeOnline);
            //}
        }

        public static string GetMenuClass(string controller)
        {
            controller = controller.ToLower();
            var currentController = GetCurrentController();
            return currentController == controller ? "selectedMenu" : string.Empty;
        }

        public static bool IsMenuItemSelected(string controller, string action = "")
        {
            controller = controller.ToLower();
            var currentController = GetCurrentController();
            var currentAction = GetCurrentControllerAction();

            // Special cases
            if (currentController == "techhelp" && controller == "consulting") return true;
            if (currentController == "energy" && controller == "consulting") return true;
            if (currentController == "cloud" && controller == "consulting") return true;
            if (currentController == "stateofdotnet" && controller == "training") return true;
            if (currentController == "codecamps" && controller == "training") return true;
            if (currentController == "lunch" && controller == "training") return true;
            if (currentController == "signup" && controller == "training") return true;
            if (currentController == "write" && controller == "magazine") return true;
            if (currentController == "advertise" && controller == "magazine") return true;
            if (currentController == "anniversaryAdvertisers" && controller == "magazine") return true;
            if (currentController == "article" && controller == "magazine") return true;
            if (currentController == "press" && controller == "about") return true;
            if (currentController == "people" && controller == "about") return true;
            if (currentController == "jobs" && controller == "about") return true;
            
            if (!string.IsNullOrEmpty(action) && action.ToLower() != "index")
                return currentController.ToLower() == controller.ToLower() && action.ToLower() == currentAction.ToLower();
            return currentController.ToLower() == controller.ToLower();
        }

        public static string GetCurrentBreadCrumbTrail()
        {
            var sb = new StringBuilder();
            var currentController = GetCurrentController();

            sb.Append("<ul>");
            sb.Append("<li class=\"breadcrumb\"><a href=\"/\">Codemag.com Home</a></li>");
            switch (currentController)
            {
                case "training":
                case "eventslist":
                    sb.Append("<li class=\"breadcrumb\">&raquo</li>");
                    sb.Append("<li class=\"breadcrumb\"><a href=\"/training\">CODE Training</a></li>");
                    break;
                case "lunch":
                    sb.Append("<li class=\"breadcrumb\">&raquo</li>");
                    sb.Append("<li class=\"breadcrumb\"><a href=\"/lunch\">Lunch with CODE</a></li>");
                    break;
                case "techhelp":
                case "energy":
                case "cloud":
                case "consulting":
                    sb.Append("<li class=\"breadcrumb\">&raquo</li>");
                    sb.Append("<li class=\"breadcrumb\"><a href=\"/consulting\">CODE Consulting</a></li>");
                    break;
                case "subscribe":
                case "write":
                case "advertise":
                case "magazine":
                    sb.Append("<li class=\"breadcrumb\">&raquo</li>");
                    sb.Append("<li class=\"breadcrumb\"><a href=\"/magazine\">CODE Magazine</a></li>");
                    break;
                case "focus":
                    sb.Append("<li class=\"breadcrumb\">&raquo</li>");
                    sb.Append("<li class=\"breadcrumb\"><a href=\"/focus\">CODE Magazine</a></li>");
                    break;
                case "staffing":
                    sb.Append("<li class=\"breadcrumb\">&raquo</li>");
                    sb.Append("<li class=\"breadcrumb\"><a href=\"/staffing\">CODE Staffing</a></li>");
                    break;
                case "roadtowin8":
                case "stateofdotnet":
                    sb.Append("<li class=\"breadcrumb\">&raquo</li>");
                    sb.Append("<li class=\"breadcrumb\"><a href=\"/magazine\">State of CODE</a></li>");
                    break;
                case "framework":
                    sb.Append("<li class=\"breadcrumb\">&raquo</li>");
                    sb.Append("<li class=\"breadcrumb\"><a href=\"/framework\">CODE Framework</a></li>");
                    break;
                case "vfpconversion":
                    sb.Append("<li class=\"breadcrumb\">&raquo</li>");
                    sb.Append("<li class=\"breadcrumb\"><a href=\"/vfpconversion\">VFP Conversion</a></li>");
                    break;
                default:
                    Debugger.Break();
                    break;
            }
            sb.Append("</ul>");

            return sb.ToString();
        }

        public static string GetCurrentAreaStyle()
        {
            var currentController = GetCurrentController();

            switch (currentController)
            {
                case "stateofdotnet":
                case "codecamps":
                case "events":
                case "training":
                case "signup":
                case "lunch":
                    return GetAreaStyles("rgb(28, 134, 185)");
                case "article":
                    // TODO: Must vary with the type of article we have
                    return GetAreaStyles("rgb(66, 182, 74)");
                case "advertise":
                case "ads":
                case "blog":
                case "book":
                case "ebook":
                case "codecast":
                case "coder":
                case "subscribe":
                case "magazine":
                    return GetAreaStyles("rgb(239, 192, 13)");  //yellow - yuck
                    //return GetAreaStyles("rgb(66, 182, 74)"); Green - cool if it fits in with everything else
                    //return GetAreaStyles("rgb(28, 134, 185)");
                case "consulting":
                case "techhelp":
                case "energy":
                case "cloud":
                    return GetAreaStyles("rgb(178, 12, 29)");
                case "framework":
                    return GetAreaStyles("rgb(237, 126, 1)");
                case "vfpconversion":
                case "accountmate":
                    return GetAreaStyles("rgb(254, 0, 0)");
                case "staffing":
                    return GetAreaStyles("rgb(66, 182, 74)");
                default:
                    return GetAreaStyles("rgb(8, 57, 129)");
            }
        }

        private static string GetAreaStyles(string color)
        {
            return ".areaForeground { color: " + color + "; } .areaBackground { background: " + color + "; } a.areaLink { color: " + color + "; } a.areaLink:hover { color: " + color + "; } a.areaLink:active { color: " + color + "; } a.areaLink:visited { color: " + color + "; }";
        }

        public static string GetCurrentBreadCrumbTrailStyle()
        {
            var sb = new StringBuilder();
            var currentController = GetCurrentController();

            sb.Append("#menu ul li.breadcrumb {");
            switch (currentController)
            {
                case "roadtowin8":
                case "stateofdotnet":
                case "lunch":
                case "codecamps":
                case "training":
                case "eventslist":
                    sb.Append("background: #1c88ba;");
                    break;
                case "vfpconversion":
                case "accountmate":
                case "consulting":
                case "techhelp":
                case "energy":
                case "cloud":
                    sb.Append("background: red;");
                    break;
                case "subscribe":
                case "write":
                case "advertise":
                case "focus":
                case "magazine":
                    sb.Append("background: yellow;");
                    break;
                case "staffing":
                    sb.Append("background: green;");
                    break;
                case "framework":
                    sb.Append("background: orange;");
                    break;
                default:
                    sb.Append("background: black;");
                    break;
            }
            sb.Append("}");

            return sb.ToString();
        }

        public static string GetMenu(IHtmlHelper helper, IEnumerable<MainMenuItem> menu, MenuMode mode = MenuMode.Top)
        {
            if (menu == null || !menu.Any()) return string.Empty;

            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var item in menu)
                AddMenuItem(item.Title, item.Link, sb);
            sb.Append("</ul>");
            return sb.ToString();
        }

        private static void AddMenuItem(string text, string link, StringBuilder sb)
        {
            sb.Append("<li>");
            sb.Append("<a class=\"areaLink\" href=\"" + link + "\">" + text + "</a>");
            sb.Append("</li>");
        }

        private static void AddSubMenuItem(string text, string controller, string action, StringBuilder sb, IHtmlHelper helper)
        {
            sb.Append("<li>");
            sb.Append("<a class=\"areaLink\" href=\"/" + controller + "/" + action + "/\">" + text + "</a>");
            //sb.Append(helper.ActionLink(text, action, controller, null, new {@class = "areaLink"}));
            sb.Append("</li>");
        }

        private static void AddMenuItem(string text, string controller, string action, string currentController, string currentAction, StringBuilder sb, IHtmlHelper helper, MenuMode mode, string imageTag = "")
        {
            currentController = currentController.ToLower();

            sb.Append("<li");

            string cssClass;

            var isItemSelected = IsMenuItemSelected(controller, action);

            if (isItemSelected)
                switch (controller.ToLower())
                {
                    case "stateofdotnet":
                    case "codecamps":
                    case "events":
                    case "lunch":
                    case "training":
                        cssClass = "trainingMenuSelected";
                        break;
                    case "advertise":
                    case "ads":
                    case "article":
                    case "blog":
                    case "book":
                    case "ebook":
                    case "codecast":
                    case "coder":
                    case "subscribe":
                    case "magazine":
                        cssClass = "magazineMenuSelected";
                        break;
                    case "consulting":
                    case "techhelp":
                    case "energy":
                    case "cloud":
                        cssClass = "consultingMenuSelected";
                        break;
                    case "framework":
                        cssClass = "frameworkMenuSelected";
                        break;
                    case "vfpconversion":
                    case "accountmate":
                        cssClass = "vfpconversionMenuSelected";
                        break;
                    case "staffing":
                        cssClass = "staffingMenuSelected";
                        break;
                    default:
                        cssClass = "neutralMenuSelected";
                        break;
                }
            else
                switch (controller.ToLower())
                {
                    case "stateofdotnet":
                    case "codecamps":
                    case "events":
                    case "lunch":
                    case "training":
                        cssClass = "trainingMenu";
                        break;
                    case "advertise":
                    case "ads":
                    case "article":
                    case "blog":
                    case "book":
                    case "ebook":
                    case "codecast":
                    case "coder":
                    case "subscribe":
                    case "magazine":
                        cssClass = "magazineMenu";
                        break;
                    case "consulting":
                    case "techhelp":
                    case "energy":
                    case "cloud":
                        cssClass = "consultingMenu";
                        break;
                    case "framework":
                        cssClass = "frameworkMenu";
                        break;
                    case "vfpconversion":
                    case "accountmate":
                        cssClass = "vfpconversionMenu";
                        break;
                    case "staffing":
                        cssClass = "staffingMenu";
                        break;
                    default:
                        cssClass = "neutralMenu";
                        break;
                }

            if (!string.IsNullOrEmpty(cssClass)) sb.Append(" class=\"" + cssClass + "\"");
            sb.Append(">");

            if (!string.IsNullOrEmpty(imageTag))
                sb.Append(imageTag);

            sb.Append("<a href=\"/" + controller + "/" + action + "\">" + text + "</a>");
            //sb.Append(helper.ActionLink(text, action, controller).ToHtmlString().ToString(CultureInfo.InvariantCulture));

            if (cssClass.Contains("Selected"))
            {
                if (currentController == "lunch" || currentController == "stateofdotnet" || currentController == "codecamps" || currentController == "training")
                {
                    sb.Append("<div class=\"subMenu\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("Training Home", "Training", "Index", sb, helper);
                    AddSubMenuItem("Classes", "Training", "Index", sb, helper);
                    AddSubMenuItem("Mentoring", "Training", "Mentoring", sb, helper);
                    AddSubMenuItem("State of .NET", "StateOfDotNet", "Index", sb, helper);
                    AddSubMenuItem("Lunch with CODE", "Lunch", "Index", sb, helper);
                    AddSubMenuItem("Code Camps", "CodeCamps", "Index", sb, helper);
                    // AddSubMenuItem("Corporate Discounts", "Training", "Corporate", sb, helper);
                    AddSubMenuItem("ASP.NET MVC", "Training", "TopicSpecificTraining", sb, helper);
                    AddSubMenuItem("DevNet Training", "DevNet", "DevNetTraining", sb, helper);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
                else if (currentController == "vfpconversion")
                {
                    sb.Append("<div class=\"subMenu\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("VFP Conversion Home", "VFPConversion", "Index", sb, helper);
                    AddSubMenuItem("Services", "VFPConversion", "VFPServices", sb, helper);
                    AddSubMenuItem("Tools", "VFPConversion", "Tools", sb, helper);
                    AddSubMenuItem("Articles", "VFPConversion", "Articles", sb, helper);
                    //AddSubMenuItem("Resources & Articles", "VFPConversion", "Resources", sb, helper);
                    //AddSubMenuItem("PJX Analyzer", "VFPConversion", "PJXAnalyzer", sb, helper);
                    //AddSubMenuItem("Conversion Services", "Consulting", "ServiceDetail/VFPConversion", sb, helper);
                    //AddSubMenuItem("VFP Development Services", "Consulting", "ServiceDetail/VFP", sb, helper);
                    AddSubMenuItem("Fox End of Life", "VFPConversion", "History", sb, helper);
                    AddSubMenuItem("Contact Us", "Consulting", "Inquiry", sb, helper);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
                else if (currentController == "staffing")
                {
                    sb.Append("<div class=\"subMenu\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("Staffing Home", "Staffing", "Index", sb, helper);
                    AddSubMenuItem("Looking for Staff?", "Staffing", "Index", sb, helper);
                    AddSubMenuItem("Looking for Work?", "Staffing", "Apply", sb, helper);
                    //AddSubMenuItem("Services & Technologies", "Staffing", "StaffingServices", sb, helper);
                    //AddSubMenuItem("Developers", "Staffing", "StaffingDevelopers", sb, helper);
                    //AddSubMenuItem("Clients", "Staffing", "StaffingClients", sb, helper);
                    AddSubMenuItem("Contact Us", "Consulting", "Inquiry", sb, helper);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
                else if (currentController == "consulting" || currentController == "energy" || currentController == "cloud")
                {
                    sb.Append("<div class=\"subMenu\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("Consulting Home", "Consulting", "Index", sb, helper);
                    AddSubMenuItem("Services & Technologies", "Consulting", "Services", sb, helper);
                    AddSubMenuItem("VFP Conversion", "VFPConversion", "Index", sb, helper);
                    AddSubMenuItem("Energy Software", "Energy", "Index", sb, helper);
                    AddSubMenuItem("Azure & Other Clouds", "Cloud", "Index", sb, helper);
                    //AddSubMenuItem("Current Specials", "Consulting", "Specials", sb, helper);
                    AddSubMenuItem("Contact Us", "Consulting", "Inquiry", sb, helper);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
                else if (currentController == "article" || currentController == "magazine" || currentController == "write" || currentController == "advertise")
                {
                    sb.Append("<div class=\"subMenu\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("Magazine Home", "Magazine", "Index", sb, helper);
                    AddSubMenuItem("All Issues", "Magazine", "AllIssues", sb, helper);
                    AddSubMenuItem("Subscribe", "Magazine", "Subscribe", sb, helper);
                    AddSubMenuItem("My (Digital) Magazines", "My", "Magazines", sb, helper);
                    AddSubMenuItem("Where is my Magazine?", "My", "Fulfillment", sb, helper);
                    AddSubMenuItem("My Subscriber Account", "My", "Index", sb, helper);
                    AddSubMenuItem("Advertise", "Advertise", "Index", sb, helper);
                    AddSubMenuItem("Write", "Write", "Index", sb, helper);
                    //AddSubMenuItem("Rates", "Advertise", "Rates", sb, helper, false);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
                else if (currentController == "framework")
                {
                    sb.Append("<div class=\"subMenu\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("Framework Home", "Framework", "Index", sb, helper);
                    AddSubMenuItem("Get Started & Documentation", "Framework", "GetStarted", sb, helper);
                    AddSubMenuItem("Download", "Framework", "Download", sb, helper);
                    //AddSubMenuItem("Training", "Framework", "Training", sb, helper);
                    AddSubMenuItem("Support & Services", "Framework", "Support", sb, helper);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
                else if (currentController != "mycode")
                {
                    sb.Append("<div class=\"subMenu "+currentController+"\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("Home", "Home", "Index", sb, helper);
                    AddSubMenuItem("About Us", "About", "Index", sb, helper);
                    AddSubMenuItem("Videos", "Video", "Index", sb, helper);
                    AddSubMenuItem("Press Releases", "Press", "Index", sb, helper);
                    AddSubMenuItem("People", "People", "Index", sb, helper);
                    AddSubMenuItem("Careers", "Jobs", "Index", sb, helper);
                    AddSubMenuItem("Privacy Policy", "Home", "Privacy", sb, helper);
                    AddSubMenuItem("Contact Us", "Consulting", "Inquiry", sb, helper);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
            }
            else if (mode == MenuMode.Top)
            {
                controller = controller.ToLower();
                if (controller == "lunch" || controller == "stateofdotnet" || controller == "codecamps" || controller == "training")
                {
                    sb.Append("<div class=\"subMenu\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("Training Home", "Training", "Index", sb, helper);
                    AddSubMenuItem("Classes", "Training", "Index", sb, helper);
                    AddSubMenuItem("Mentoring", "Training", "Mentoring", sb, helper);
                    AddSubMenuItem("State of .NET", "StateOfDotNet", "Index", sb, helper);
                    AddSubMenuItem("Lunch with CODE", "Lunch", "Index", sb, helper);
                    AddSubMenuItem("Code Camps", "CodeCamps", "Index", sb, helper);
                    // AddSubMenuItem("Corporate Discounts", "Training", "Corporate", sb, helper);
                    AddSubMenuItem("ASP.NET MVC", "Training", "TopicSpecificTraining", sb, helper);
                    AddSubMenuItem("DevNet Training", "DevNet", "DevNetTraining", sb, helper);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
                else if (controller == "vfpconversion")
                {
                    sb.Append("<div class=\"subMenu\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("VFP Conversion Home", "VFPConversion", "Index", sb, helper);
                    AddSubMenuItem("Services", "VFPConversion", "VFPServices", sb, helper);
                    AddSubMenuItem("Tools", "VFPConversion", "Tools", sb, helper);
                    AddSubMenuItem("Articles", "VFPConversion", "Articles", sb, helper);
                    //AddSubMenuItem("Resources & Articles", "VFPConversion", "Resources", sb, helper);
                    //AddSubMenuItem("PJX Analyzer", "VFPConversion", "PJXAnalyzer", sb, helper);
                    //AddSubMenuItem("Conversion Services", "Consulting", "ServiceDetail/VFPConversion", sb, helper);
                    //AddSubMenuItem("VFP Development Services", "Consulting", "ServiceDetail/VFP", sb, helper);
                    AddSubMenuItem("Fox End of Life", "VFPConversion", "History", sb, helper);
                    AddSubMenuItem("Contact Us", "Consulting", "Inquiry", sb, helper);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
                else if (controller == "staffing")
                {
                    sb.Append("<div class=\"subMenu\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("Staffing Home", "Staffing", "Index", sb, helper);
                    AddSubMenuItem("Looking for Staff?", "Staffing", "Index", sb, helper);
                    AddSubMenuItem("Looking for Work?", "Staffing", "Apply", sb, helper);
                    //AddSubMenuItem("Services & Technologies", "Staffing", "StaffingServices", sb, helper);
                    //AddSubMenuItem("Developers", "Staffing", "StaffingDevelopers", sb, helper);
                    //AddSubMenuItem("Clients", "Staffing", "StaffingClients", sb, helper);
                    AddSubMenuItem("Contact Us", "Consulting", "Inquiry", sb, helper);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
                else if (controller == "consulting" || controller == "energy" || controller == "cloud")
                {
                    sb.Append("<div class=\"subMenu\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("Consulting Home", "Consulting", "Index", sb, helper);
                    AddSubMenuItem("Services & Technologies", "Consulting", "Services", sb, helper);
                    AddSubMenuItem("VFP Conversion", "VFPConversion", "Index", sb, helper);
                    AddSubMenuItem("Azure & Other Clouds", "Cloud", "Index", sb, helper);
                    AddSubMenuItem("Energy Software", "Energy", "Index", sb, helper);
                    //AddSubMenuItem("Current Specials", "Consulting", "Specials", sb, helper);
                    AddSubMenuItem("Contact Us", "Consulting", "Inquiry", sb, helper);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
                else if (controller == "article" || controller == "magazine" || controller == "write" || controller == "advertise")
                {
                    sb.Append("<div class=\"subMenu\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("Magazine Home", "Magazine", "Index", sb, helper);
                    AddSubMenuItem("All Issues", "Magazine", "AllIssues", sb, helper);
                    AddSubMenuItem("Subscribe", "Magazine", "Subscribe", sb, helper);
                    AddSubMenuItem("My (Digital) Magazines", "My", "Magazines", sb, helper);
                    AddSubMenuItem("Where is my Magazine?", "My", "Fulfillment", sb, helper);
                    AddSubMenuItem("My Subscriber Account", "My", "Index", sb, helper);
                    AddSubMenuItem("Advertise", "Advertise", "Index", sb, helper);
                    AddSubMenuItem("Write", "Write", "Index", sb, helper);
                    //AddSubMenuItem("Rates", "Advertise", "Rates", sb, helper, false);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
                else if (controller == "framework")
                {
                    sb.Append("<div class=\"subMenu\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("Framework Home", "Framework", "Index", sb, helper);
                    AddSubMenuItem("Get Started & Documentation", "Framework", "GetStarted", sb, helper);
                    AddSubMenuItem("Download", "Framework", "Download", sb, helper);
                    //AddSubMenuItem("Training", "Framework", "Training", sb, helper);
                    AddSubMenuItem("Support & Services", "Framework", "Support", sb, helper);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
                else if (controller != "mycode" && action.ToLower() != "logon")
                {
                    sb.Append("<div class=\"subMenu " + currentController + "\">");
                    sb.Append("<ul>");
                    AddSubMenuItem("CODE Home", "Home", "Index", sb, helper);
                    AddSubMenuItem("About Us", "About", "Index", sb, helper);
                    AddSubMenuItem("Videos", "Video", "Index", sb, helper);
                    AddSubMenuItem("Press Releases", "Press", "Index", sb, helper);
                    AddSubMenuItem("People", "People", "Index", sb, helper);
                    AddSubMenuItem("Careers", "Jobs", "Index", sb, helper);
                    AddSubMenuItem("Privacy Policy", "Home", "Privacy", sb, helper);
                    AddSubMenuItem("Contact Us", "Consulting", "Inquiry", sb, helper);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                }
            }

            if (controller.ToLower() == "mycode")
            {
                sb.Append("<div class=\"subMenu\">");
                sb.Append("<ul>");
                AddSubMenuItem("My CODE", "MyCode", "Index", sb, helper);
                AddSubMenuItem("Log Out", "Account", "Logoff", sb, helper);
                sb.Append("</ul>");
                sb.Append("</div>");
                
            }

            sb.Append("</li>");
        }

        /// <summary>
        /// Returns the current controller
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentController()
        {
            // TODO: 
            return "Docs";

            //var url = HttpContext.Current.Request.Url.AbsoluteUri;
            //var at = StringHelper.At("/", url, 3);
            //url = url.Substring(at - 1);

            //if (url.Length == 1) return "home";

            //url = url.Substring(1);
            //var urls = url.Split('/');

            //var controllerName = urls[0].ToLower();
            //if (controllerName == "my") controllerName = "MyCode";
            //return controllerName;
        }

        /// <summary>
        /// Returns the current controller
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentControllerAction()
        {
            // TODO: 
            return "Index";

            //var url = HttpContext.Current.Request.Url.AbsoluteUri;
            //var at = StringHelper.At("/", url, 3);
            //url = url.Substring(at - 1);

            //if (url.Length == 1) return "index";

            //url = url.Substring(1);
            //var urls = url.Split('/');

            //return urls.Length == 2 ? urls[1].ToLower() : "index";
        }

        public static string GetMenuClass(string controller, string action)
        {
            // TODO:
            //controller = controller.ToLower();
            //action = action.ToLower();

            //var url = HttpContext.Current.Request.Url.AbsoluteUri;
            //var at = StringHelper.At("/", url, 3);
            //url = url.Substring(at);
            //var urls = url.Split('/');
            //if (urls.Length == 2)
            //    if (urls[0].ToLower() == controller && urls[1].ToLower() == action)
            //        return "selectedMenu";

            return string.Empty;
        }
    }

    public enum MenuMode
    {
        Top,
        Mobile
    }

    public enum Logos
    {
        CodeGroup,
        CodeOnline,
        CodeTraining,
        CodeConsulting,
        CodeStaffing,
        CodeMagazine,
        CodeFocusMagazine,
        CodeProductFocusMagazine,
        CodeCast,
        StateOfDotNet,
        RoadToWindows8,
        CodeFramework,
        VfpConversion
    }

    public enum MetaNamespace
    {
        OpenGraph,
        Twitter
    }
}
