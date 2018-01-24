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

This assumes that a VSTS server is already configured as the root repository for this documentation setup. Otherwise, VSTS information has to be specified either at the root of the TOC file like so:

```json
{ // This is the root
  "title": "My Docs",
  // More here...
  "settings": {
    "vstsInstance": "https://[your-server].visualstudio.com", // Use your server name here
    "vstsProjectName": "[your-project]", // Use your project name here
    "vstsPat": "[personal-access-token]" // Use your actual PAT here
  }
}
```

This configures settings for the entire documentation tree, thus when using anything from VSTS, these settings will be applied. However, it is also possible to configure these settings on each topic node. (Note: If both global and topic settings are present, the topic settings will override the global ones).

```json
"topics": [
  { 
    "title": "Work Item 32",
    "link": "32",
    "type": "vsts-workitem",
    "settings": {
      "vstsInstance": "https://[your-server].visualstudio.com", // Use your server name here
      "vstsProjectName": "[your-project]", // Use your project name here
      "vstsPat": "[personal-access-token]" // Use your actual PAT here
    }
  }
]
```

## Displaying the Results of a Work Item Query as a Topic

It is also possible to display the results of a work item query as the contents of a topic (a list of resulting items is shown as the topic content). To do this, configure the node like so:

```json
{ 
  "title": "My Query",
  "link": "Query Name",
  "type": "vsts-workitem-query",
}
```

## Displaying the Results of a Work Item Query as Sub-Nodes

It is possible to display TFS work item queries in a way where each item within the query result becomes a sub-node of the current TOC node. The sub-nodes are then displayed as if they were individually configured work item nodes (see above).

```json
{ 
  "title": "My Query",
  "link": "Query Name",
  "type": "vsts-workitem-query:toc",
}
```

> Note: Displaying work item query results in the table of contents can put considerable strain on the documentation engine and diminish performance.

## Displaying a List of Queries as a Topic

It is possible to create a topic that shows a list of queries, so the user can then choose a query to execute and see the results as as described above.

```json
{ 
  "title": "My Query",
  "link": "Optional Query Filter",
  "type": "vsts-workitem-queries",
  "vsts-instance": "myinstance.visualstudio.com",
  "vsts-project": "ProjectName",
  "vsts-pat": "[Personal Access Token for authentication]"
}
```

## Displaying a List of Queries as Sub-Nodes

It is possible to display TFS work item queries in a way where each query becomes a sub-node of the current TOC node. The sub-nodes are then displayed as additional nodes similar to manually creating a query node as described above.

```json
{ 
  "title": "Work Item Queries",
  "link": "Optional Query Filter",
  "type": "vsts-workitem-queries:toc",
  "vsts-instance": "myinstance.visualstudio.com",
  "vsts-project": "ProjectName",
  "vsts-pat": "[Personal Access Token for authentication]"
}
```

> Note: Displaying work item queries in the table of contents can put considerable strain on the documentation engine and diminish performance.
