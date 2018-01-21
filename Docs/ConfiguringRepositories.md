# Configuring Documentation Repositories

All Kava Docs documentation sites are driven by a root repository that contains documentation files and (usually) a TOC (Table of Contents) file (typically called _toc.json). Kava Docs supports a number of different repositories (and we are always adding more supported options). 

> Note: In addition to the files in the root repository, the TOC, as well as individual documents, can refer to secondary repositories or individual documents that live outside the main repository. However, there is always a single root repository that ties everything together.

The root repository has to be configured in the settings. For instance, to point to a Git Repository managed my Microsoft Team Foundation Server, the configuration can look like so:

```json
"RepositoryType": "VSTSGit",
"VSTSInstance": "https://myrepo.visualstudio.com",
"VSTSProjectName": "MyDocs",
"VSTSPAT": "access key"
```

Or, to access a repository managed by GitHub, it could be configured like so:

```json
"RepositoryType": "GitHubRaw",
"GitHubProject": "MarkusEggerInc/CodeFrameworkDocs"
```

Note that different settings are used for each repository type. The only setting that is always present is the **RepositoryType** setting, which specifies the fundamental type of repository that is being used.

## RepositoryType: GitHubRaw

This type of repository access provides raw (unauthenticated) access to GibHub projects. The advantage of this is that this simply uses publicly available URLs to access the repository, which means that no special authorization is required. However, this only works with public repositories. Also, this is subject to the GitHub caching policy, which means that it may take up to several minutes for changes to show up in the Kava Docs documentation site.

The following settings are available for raw GitHub access:

| Setting Name | Values | Required? |
|--------------|--------|-----------|
| RepositoryType | GitHubRaw | Yes |
| GitHubProject | Name of the organization and public repository in GitHub. Example: MarkusEggerInc/CodeFrameworkDocs | No |
| MasterUrl | Instead of specifying the GitHubProject setting, one can specify a full raw GitHub URL. This is useful when the documentation starts as a sub-folder of the repository, or when a branch other than the master branch is desired. Example: https://raw.githubusercontent.com/DocHound/DocHoundEngine/master/Docs/ | No |

## RepositoryType: VSTSGit

This type of repository access is used to access a Git repository in a Microsoft Team Foundation Server instance. (Typically, not not always, hosted on VisualStudio.com). Note that the repository must use the Git standard. Older TFS source control repositories are not supported.

| Setting Name | Values | Required? |
|--------------|--------|-----------|
| RepositoryType | VSTSGit | Yes |
| VSTSInstance | Name of the TFS instance. Example: https://myrepo.visualstudio.com | Yes |
| VSTSProjectName | Name of the project within that TFS instance. Example: MyDocs | Yes |
| VSTSPAT | Personal Access Token used to authenticate against TFS. This token can be created in the TFS interface. | Yes |

## Additional Repository Types under Consideration or Development

The following repository types are being considered, or actively developed:

* [x] GitHub (for non-private GitHub repositories that require authentication)
* [ ] Git (general Git repository support, for Git repositories *not* hosted by GitHub or TFS)
* [ ] One Drive
* [ ] DropBox
* [ ] Azure Blob Storage
