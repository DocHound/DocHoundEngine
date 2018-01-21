# Supported VSTS Features

Kava Docs supports a number of features related to VSTS ("Visual Studio Team System", also known as "TFS" or "Team Foundation Server"). This includes the ability to render individual work items as well as displaying work item queries in a variety of ways.

## Displaying Individual Work Items as Nodes in the TOC

The simplest form of integrating a TFS work item is to display a single work item as an entry in the TOC (Table of Contents). To do this, the topic in the TOC has to be configured similar to this:

```json
{ 
  "title": "Work Item 32",
  "link": "32",
  "type": "vsts-workitem"
}
```

This assumes that a VSTS server is already configured as the root repository for this documentation setup. Otherwise, the server has to be specified as well:

```json
{ 
  "title": "Work Item 32",
  "link": "32",
  "type": "vsts-workitem",
  "vsts-instance": "myinstance.visualstudio.com",
  "vsts-pat": "[Persona Access Token for authentication]"
}
```

## Displaying the Results of a Work Item Query as a Topic

It is also possible to display the results of a work item query as the contents of a topic (a list of resulting items is shown as the topic content). To do this, configure the node like so:

```json
{ 
  "title": "My Query",
  "link": "Query Name",
  "type": "vsts-workitem-query",
  "vsts-instance": "myinstance.visualstudio.com",
  "vsts-project": "ProjectName",
  "vsts-pat": "[Persona Access Token for authentication]"
}
```
