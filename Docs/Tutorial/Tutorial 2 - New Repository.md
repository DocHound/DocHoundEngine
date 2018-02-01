# Tutorial - Create new Docs in a new Github Repository

> This tutorial assumes you have an existing Kava Docs account as well as a Github account.

> Note: This tutorial shows how to manually create a new Github repository and manually edit Markdown files to create content. However, you may find the use of a more advanced editing tool, such as Markdown Monster, more agreeable. For that scenario, see tutorial # 3.

## Step 1: Create a new Gibhub Repository

1. Go to Github and choose to create a new repository. 
2. Choose to have Github create a ```readme.md``` markdown file. (This is not strictly required, but it is convenient for our purposes).
3. Click on ```readme.md``` to view the contents of that file right on Github.
4. Click the edit link to edit the content
5. Add new markdown content, such as this example text:
 
```md
# Hello World!

This is an *example* markdown file.

> This is a note.
```

6. Commit ("save") the file

> Note: Markdown editing is greatly simplified using one of the many powerful markdown editors. We recommend Markdown Monster.

## Step 2: Add more content if desired

1. In the new repository, add another markdown file, such as ```test.md```
2. Click on the file to see its contents.
3. Click to edit the file.
4. Add more content such as the following:

```md
# Test

The *quic*k brown fox jumps over the **lazy** dog.

> This is another note.
```

5. Commit the file

## Step 3: Create a Table of Contents

> It is not strictly required to have a table of contents in your repository, but it is recommended as it simplifies things and provides a great degree of control over your documentation setup.

1. Create a new file called ```_toc.json``` (note the underscore!) and open it for editing.
2. Put the following JSON into that file:

```json
{
  "title": "My First Docs",
  "owner": "Your Name",
  "language": "en-US",
  "version": "1.0",
  "topics": [
    {
      "title": "Welcome to My Docs",
      "topics": [
        {
          "title": "Readme",
          "link": "readme.md"
        },
        {
          "title": "Test",
          "link": "Test.md"
        }
      ]
    }
  ]
}
```

> Note: Markdown Monster also has special features to edit ```_toc.json``` files. We highly recommend using a tool to edit this file, rather than attempting to create the table of contents manually.

> Note: The ```_toc.json``` file structure is based on the JSON standard. You can find out more about all the options available in this file [here](/TocFileStructure).

## Step 4: Create the Kava Docs Repository

You now have a valid documentation repository. You can now follow the [First Tutorial](/Tutorial-1) to create the Kava Docs repository.
