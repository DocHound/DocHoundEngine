using System.Text;
using DocHound.Interfaces;
using Newtonsoft.Json.Linq;

namespace DocHound.TopicRenderers.VisualStudioTeamSystem
{
    public class WorkItemTopicRenderer : ITopicRenderer
    {
        public string RenderToHtml(TopicInformation topic, string imageRootUrl = "", TocSettings settings = null)
        {
            if (string.IsNullOrEmpty(topic.OriginalContent)) return string.Empty;

            var sb = new StringBuilder();

            dynamic workItems = JObject.Parse(topic.OriginalContent);
            if (workItems.value != null)
                foreach (var workItem in workItems.value)
                    RenderWorkItem(sb, workItem, imageRootUrl, settings);

            return sb.ToString();
        }

        private void RenderWorkItem(StringBuilder sb, dynamic workItem, string imageRootUrl, TocSettings settings)
        {
            // TODO: This should really be based on some kind of template instead
            // TODO: The way the heading IDs are created here will not work if there is more than one item
            // TODO: All data fields (so NOT the description and acceptance criteria) need to be properly encoded

            if (workItem.fields == null) return;

            var fields = workItem.fields as JObject;
            if (fields == null) return;

            if (fields["System.Title"] != null)
            {
                var title = fields["System.Title"].ToString().Trim();
                if (fields["System.WorkItemType"] != null)
                    title = fields["System.WorkItemType"].ToString().Trim() + ": " + title;
                sb.Append("<h1 id=\"vsts-workitem-title\" class=\"vsts-workitem-title\">" + title + "</h1>");
            }


            sb.Append("<div class=\"vsts-workitem-statearea\">");
            sb.Append("<table>");
            if (fields["System.AssignedTo"] != null)
                sb.Append("<tr class=\"vsts-workitem-assignedto\"><td class=\"vsts-workitem-label vsts-workitem-assignedto-label\">Assigned To:</td><td class=\"vsts-workitem-data vsts-workitem-assignedto-data\">" + fields["System.AssignedTo"].ToString().Trim() + "</td></tr>");
            if (fields["System.WorkItemType"] != null)
                sb.Append("<tr class=\"vsts-workitem-workitemtype\"><td class=\"vsts-workitem-label vsts-workitem-workitemtype-label\">Work Item Type:</td><td class=\"vsts-workitem-data vsts-workitem-workitemtype-data\">" + fields["System.WorkItemType"].ToString().Trim() + "</td></tr>");
            if (fields["System.TeamProject"] != null)
                sb.Append("<tr class=\"vsts-workitem-teamproject\"><td class=\"vsts-workitem-label vsts-workitem-teamproject-label\">Team Project:</td><td class=\"vsts-workitem-data vsts-workitem-teamproject-data\">" + fields["System.TeamProject"].ToString().Trim() + "</td></tr>");
            if (fields["System.AreaPath"] != null)
                sb.Append("<tr class=\"vsts-workitem-areapath\"><td class=\"vsts-workitem-label vsts-workitem-areapath-label\">Area Path:</td><td class=\"vsts-workitem-data vsts-workitem-areapath-data\">" + fields["System.AreaPath"].ToString().Trim() + "</td></tr>");
            if (fields["System.IterationPath"] != null)
                sb.Append("<tr class=\"vsts-workitem-iterationpath\"><td class=\"vsts-workitem-label vsts-workitem-iterationpath-label\">Iteration Path:</td><td class=\"vsts-workitem-data vsts-workitem-iterationpath-data\">" + fields["System.IterationPath"].ToString().Trim() + "</td></tr>");
            if (fields["System.State"] != null)
                sb.Append("<tr class=\"vsts-workitem-state\"><td class=\"vsts-workitem-label vsts-workitem-state-label\">State:</td><td class=\"vsts-workitem-data vsts-workitem-state-data\">" + fields["System.State"].ToString().Trim() + "</td></tr>");
            if (fields["System.Reason"] != null)
                sb.Append("<tr class=\"vsts-workitem-reason\"><td class=\"vsts-workitem-label vsts-workitem-reason-label\">Reason:</td><td class=\"vsts-workitem-data vsts-workitem-reason-data\">" + fields["System.Reason"].ToString().Trim() + "</td></tr>");
            sb.Append("</table>");
            sb.Append("</div>");

            if (fields["System.Description"] != null)
            {
                sb.Append("<h2 id=\"vsts-workitem-description\" >Description</h2>");
                sb.Append("<div class=\"vsts-workitem-description\">" + fields["System.Description"] + "</div>");
            }

            if (fields["Microsoft.VSTS.Common.AcceptanceCriteria"] != null)
            {
                sb.Append("<h2 id=\"vsts-workitem-acceptancecriteria\" >Acceptance Criteria</h2>");
                sb.Append("<div class=\"vsts-workitem-acceptancecriteria\">" + fields["Microsoft.VSTS.Common.AcceptanceCriteria"] + "</div>");
            }
        }
    }
}
