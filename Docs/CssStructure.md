# CSS Structure Utilized by Kava Docs Themes

## Customizing CSS

Kava Docs Themes provide default CSS settings that are apply to the site's appearance. However, it is possible to create new CSS rules (and override existing ones) by specifying a custom CSS file that gets applied **after** the standard CSS settings get applied. Therefore, the custom CSS settings "win out" over the ones provided out of the box.

For more information on how to specify a custom CSS file, see [Table of Contents File Structure](TOC File Structure)

## Default CSS File

The current CSS file used by the Kava Docs Default Theme contains the following styles:

```css
* {
    padding: 0;
    margin: 0;
}

body::-webkit-scrollbar, ::-webkit-scrollbar {
    height: 6px;
    width: 6px;
}
-webkit-scrollbar-thumb, body::-webkit-scrollbar-thumb, ::-webkit-scrollbar-thumb {
    background-color: #6e6e6e;
    outline: 1px solid #333;
}

/* Overall structure of the page */

/* Table of Contents (complete tree) */

.toc {
    position: fixed;
    width: 275px;
    top: 110px;
    bottom: 70px;
    overflow-y: auto;
    box-shadow: 1px 0 20px rgba(0, 0, 0, .2);
    padding-right: 10px;
    padding-bottom: 20px;
    -ms-transition: all .3s;
    -moz-transition: all .3s; /* Firefox 4 */
    -webkit-transition: all .3s; /* Safari and Chrome */
    -o-transition: all .3s; /* Opera */
    transition: all .3s;
}
    .toc.scrolled {
        top: 50px;
    }


/* Main content area */

.content-container {
    margin-left: 300px;
    padding-top: 120px;
    padding-left: 30px;
    padding-right: 330px;
    padding-bottom: 100px;
    overflow-x: hidden;
    -ms-transition: all .3s;
    -moz-transition: all .3s; /* Firefox 4 */
    -webkit-transition: all .3s; /* Safari and Chrome */
    -o-transition: all .3s; /* Opera */
}

    .content-container.showMobileMenu {
        margin-left: 300px;
        margin-right: -300px;
    }

    .content-container pre {
        margin-top: 25px;
        margin-bottom: 25px;
    }

    .content-container pre code.hljs {
        overflow-x: auto;
        overflow-y: hidden;
        padding-left: 8px;
        padding-right: 8px;
    }

    .content-container h1 {
        color: #1e88e5;
        font-family: Roboto,"Helvetica Neue Light","Helvetica Neue",Helvetica,Arial,"Lucida Grande",sans-serif;
        font-weight: 300;
        font-size: 2.4em;
        margin-top: 40px;
        margin-bottom: 30px;
    }

    .content-container h2 {
        color: #1e88e5;
        font-family: Roboto,"Helvetica Neue Light","Helvetica Neue",Helvetica,Arial,"Lucida Grande",sans-serif;
        font-weight: 300;
        font-size: 1.8em;
        margin-top: 50px;
        margin-bottom: 30px;
    }

    .content-container h3 {
        color: #525252;
        font-family: Roboto,"Helvetica Neue Light","Helvetica Neue",Helvetica,Arial,"Lucida Grande",sans-serif;
        font-weight: 300;
        font-size: 1.6em;
        margin-top: 40px;
        margin-bottom: 20px;
    }

    .content-container p {
        line-height: 1.7;
        text-align: justify;
    }

    .content-container li {
        line-height: 1.7;
    }

    .content-container img {
        max-width: 100%;
        margin-top: 20px;
        margin-top: 20px;
    }

.navigation-footer {
    clear: both;
    margin-top: 50px;
}


/* The header contains the menu, logo, and similar elements */

.header {
    position: fixed;
    height: 110px;
    left: 0;
    right: 0;
    background-color: #1e88e5;
    background-color: #1e88e5de;
    -ms-transition: all .3s;
    -moz-transition: all .3s; /* Firefox 4 */
    -webkit-transition: all .3s; /* Safari and Chrome */
    -o-transition: all .3s; /* Opera */
    transition: all .3s;
}

    .header.showMobileMenu {
        left: 300px;
        right: -300px;
    }

    .header.scrolled {
        height: 50px;
    }

    .header .logo {
        float: left;
        margin: 10px 0 0 10px;
        max-width: 280px;
        max-height: 85px;
        -ms-transition: all .3s;
        -moz-transition: all .3s; /* Firefox 4 */
        -webkit-transition: all .3s; /* Safari and Chrome */
        -o-transition: all .3s; /* Opera */
        transition: all .3s;
        overflow: hidden;
    }

        .header.scrolled .logo {
            margin: 5px 0 0 10px;
            max-height: 40px;
        }

    .header .menu {
        display: block;
        max-height: 110px;
        margin-left: 280px;
        margin-right: 20px;
        margin-top: 35px;
        -ms-transition: all .3s;
        -moz-transition: all .3s; /* Firefox 4 */
        -webkit-transition: all .3s; /* Safari and Chrome */
        -o-transition: all .3s; /* Opera */
        transition: all .3s;
    }
    .header.scrolled .menu {
        display: block;
        max-height: 50px;
        margin-left: 150px;
        margin-right: 20px;
        margin-top: 15px;
    }

        .header .menu ul {
            font-size: 1.1em;
            float: right;
            font-weight: 200;
        }

            .header .menu ul li {
                float: left;
                list-style-type: none;
                margin: 0 4px 0 0;
            }

                .header .menu ul li a {
                    padding: 5px 10px;
                    color: white;
                }

                .header .menu ul li a:hover {
                    text-decoration: none;
                }

                    .header .menu ul li .subMenu ul li:hover {
                        background-color: #03142e;
                        background-color: rgba(8, 57, 129, 0.5);
                    }

    .header .subMenu {
        display: none;
        clear: both;
        padding: 0;
        margin: 15px 0 0 -10px;
        position: fixed;
        background-color: rgb(64, 64, 64);
        -webkit-box-shadow: 0 0 2px 2px #c5c5c5;
        -webkit-box-shadow: 0 0 2px 2px rgba(197, 197, 197, 0.3);
        -ms-box-shadow: 0 0 2px 2px #c5c5c5;
        -ms-box-shadow: 0 0 2px 2px rgba(197, 197, 197, 0.3);
        box-shadow: 0 0 2px 2px #c5c5c5;
        box-shadow: 0 0 2px 2px rgba(197, 197, 197, 0.3);
        -ms-border-radius: 10px;
        border-radius: 6px;
        font-size: 9pt;
    }

    .header .visibleSubMenu {
        display: block;
    }

    .header .subMenu ul {
        margin-top: 6px;
        margin-bottom: 6px;
    }

        .header .subMenu ul li {
            float: none !important;
            margin: 0 !important;
            padding: 8px 10px 8px 5px !important;
        }

            .header .subMenu ul li:hover {
                background-color: #585858;
                background-color: rgba(255,255,255,.15);
            }

            .header .subMenu ul li a {
                border-bottom: 0 !important;
                color: white !important;
            }

    .header .mobileMenuIcon {
        display: none;
        float: left;
        cursor: pointer;
        margin: 16px 10px;
        color: white;
        font-size: 1.2em;
    }
        
    .mobileMenu {
        display: none;
        position: fixed;
        top: 0;
        left: 0;
        width: 290px;
        bottom: 0;
        background-color: rgb(39, 39, 39);
        color: white;
        overflow-x: hidden;
        overflow-y: auto;
        -webkit-user-select: none;
        -moz-user-select: none;
        -ms-user-select: none;
        -o-user-select: none;
        user-select: none;
        z-index: -1;
        padding-right: 10px;
    }

    .mobile-menu-items {
        border-bottom: 1px solid white;
    }

        .mobileMenu.showMobileMenu {
            display: block;
        }

        .mobileMenu ul li .subMenu {
            display: block;
            width: 285px;
            padding: 5px 0 5px 0;
        }
    
            .mobileMenu ul li .subMenu ul {
                list-style: none;
                padding-left: 0;
                padding-top: 5px;
                margin: 0;
                width: 200px;
            }
    
                .mobileMenu ul li .subMenu ul li {
                    border-bottom: 0;
                    padding: 7px 0 7px 20px;
                    margin-left: 0;
                }
    
        .mobileMenu ul {
            list-style: none;
            padding-left: 0;
            padding-top: 5px;
        }
    
            .mobileMenu ul li {
                list-style: none;
                text-indent: 0;
                margin-left: 0;
                margin-top: 0;
                padding: 9px 0 9px 15px;
                font-weight: 300;
                font-size: 11pt;
                min-height: 25px;
                font-family: Roboto,"Helvetica Neue Light","Helvetica Neue",Helvetica,Arial,"Lucida Grande",sans-serif;
                cursor: pointer;
            }
    
                .mobileMenu ul li a, .mobileMenu ul li a:visited, .mobileMenu ul li a:hover {
                    color: white;
                    text-decoration: none;
                }
    
                .mobileMenu ul li.neutralMenuSelected {
                    border-left: 4px solid rgb(8, 57, 129);
                    background-color: #03142e;
                    background-color: rgba(8, 57, 129, 0.2);
                }
    
                .mobileMenu ul li.neutralMenu:hover {
                    border-left: 4px solid rgb(8, 57, 129);
                    padding: 9px 20px 9px 11px;
                }

/* Markdown checked lists                 */
.task-list-item {
    list-style-type: none;
}
.task-list-item-checkbox {
    margin: 0 0.2em 0.25em -1.6em;
    vertical-align: middle;
}
.task-list-item input[type="checkbox"] {
    margin: 0 0.2em 0.25em -1.6em;
    vertical-align: middle;
}


/* Right sidebar */

.sidebar {
    position: fixed;
    right: 5px;
    width: 275px;
    top: 130px;
    bottom: 50px;
    overflow-y: auto;
    -ms-transition: all .3s;
    -moz-transition: all .3s; /* Firefox 4 */
    -webkit-transition: all .3s; /* Safari and Chrome */
    -o-transition: all .3s; /* Opera */
    transition: all .3s;
}

    .sidebar.scrolled {
        top: 70px;
    }

    .sidebar ul {
        list-style-image: none;
        list-style-position: outside;
        list-style-type: none;
    }

        .sidebar li {
            font-size: 9pt;
            line-height: 18px;
            padding-top: 5px;
            padding-left: 10px;
            padding-bottom: 5px;
            margin-left: 5px;
            margin-top: 0px;
            margin-bottom: 0px;
            border-left: 1px solid #dddddd;
        }

            .sidebar li a:link {
                cursor: pointer;
                text-decoration: none;
                color: black;
            }

            .sidebar li a:visited {
                cursor: pointer;
                text-decoration: none;
                color: black;
            }

            .sidebar li a:hover {
                cursor: pointer;
                text-decoration: none;
                color: rgb(8, 57, 129);
            }

.sidebar li.outlineLevel1 {
    font-weight: bold;
}
.sidebar li.outlineLevel2 {
    padding-left: 20px;
}
.sidebar li.outlineLevel3 {
    padding-left: 30px;
}

/* Page Footer */
.footer {
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;
    height: 50px;
    background-color: rgba(0, 0, 0, 0.9);
    text-align: center;
    color: rgb(201, 201, 201);
    padding-top: 10px;
    padding-bottom: 10px;
    -ms-transition: all .3s;
    -moz-transition: all .3s; /* Firefox 4 */
    -webkit-transition: all .3s; /* Safari and Chrome */
    -o-transition: all .3s; /* Opera */
}

    .footer.showMobileMenu {
        left: 300px;
        right: -300px;
    }


/* Narrow desktop display */
@media only screen and (max-width : 1200px) {
    .sidebar {
        display: none;
    }

    .content-container {
        padding-right: 15px;
        padding-left: 15px;
    }
}

/* Tablets Portrait or Landscape */
@media only screen and (max-width: 1024px) {
    .header .menu {
        display: none !important;
    }

    .header {
        height: 50px;
    }

    .header .logo {
        margin: 5px 0 0 10px;
        max-height: 40px;
    }

    .header .mobileMenuIcon {
        display: block !important;
    }

    .toc {
        display: none;
    }

    .content-container {
        margin-left: 0px;
        background-color: white;
    }

    .footer.scrolled {
        display: none;
    }

    blockquote {
        padding: 16px 20px 8px 20px !important;
        margin: 24px 0 !important;
    }
}

/* Styles associated with content within elements */
/* .topicList {
} */

/* .topicListLevel0 {
} */

.topicLink {
    list-style-image: none;
    list-style-position: outside;
    list-style-type: none;
}

    .topicLink a:link {
        cursor: pointer;
        text-decoration: none;
        color: black;
    }

    .topicLink a:visited {
        cursor: pointer;
        text-decoration: none;
        color: black;
    }

    .topicLink a:hover {
        cursor: pointer;
        text-decoration: none;
        color: rgb(8, 57, 129);
    }

    .mobileMenu .topicLink a:link {
        cursor: pointer;
        text-decoration: none;
        color: white;
    }

    .mobileMenu .topicLink a:visited {
        cursor: pointer;
        text-decoration: none;
        color: white;
    }

    .mobileMenu .topicLink a:hover {
        cursor: pointer;
        text-decoration: none;
        color: white;
    }

.topicLevel0 {
    font-size: 11pt;
    line-height: 20px;
    padding-top: 5px;
    padding-bottom: 5px;
}

.topicLevel1, .topicLevel2, .topicLevel3, .topicLevel4, .topicLevel5 {
    font-size: 9pt;
    line-height: 18px;
    padding-top: 5px;
    padding-left: 10px;
    padding-bottom: 5px;
    margin-left: 5px;
}

.topicExpanded {
    transition: visibility .5s,opacity .5s,max-height .5s;
    opacity: 1;
    display: block;
}

.topicCollapsed {
    display: none;
    transition: visibility 275ms,opacity 275ms,max-height .28s;
    opacity: 0;
}

.selectedTopic>a {
    font-weight: bold;
    color: #1e88e5;
}
    .selectedTopic>a:visited {
        font-weight: bold;
        color: #1e88e5;
    }
    .selectedTopic>a:link {
        font-weight: bold;
        color: #1e88e5;
    }

.caret {
    transform-origin: 50% 50% 0px;
    transition-delay: 0s, 0s;
    transition-duration: .15s, .15s;
    transition-property: transform, -webkit-transform;
    transition-timing-function: ease-in-out;
    overflow: hidden;
    float: right;
    height: 24px;
    width: 24px;
}

    .mobileMenu .caret {
        padding: 0 10px;
    }

.caretExpanded {
    transform: rotate(90deg);
}

.caretCollapsed {
    transform: rotate(0deg);
}

a.areaLink {
    color: rgb(8, 57, 129);
}

    a.areaLink:visited {
        color: rgb(8, 57, 129);
    }

figure {
    color: rgb(8, 57, 129);
}

.navigatePrevious {
    float: left;
    padding: 5px 20px 5px 20px;
    background-color: #edf0f2;
    border-color: rgb(217, 220, 220);
    border-style: solid;
    border-width: 1px 1px 3px 1px;
}

    .navigatePrevious a:link {
        cursor: pointer;
        text-decoration: none;
        color: black;
    }

    .navigatePrevious a:visited {
        cursor: pointer;
        text-decoration: none;
        color: black;
    }

    .navigatePrevious a:hover {
        cursor: pointer;
        text-decoration: none;
        color: rgb(8, 57, 129);
    }

.navigateNext {
    float: right;
    padding: 5px 20px 5px 20px;
    background-color: #edf0f2;
    border-color: rgb(217, 220, 220);
    border-style: solid;
    border-width: 1px 1px 3px 1px;
}

    .navigateNext a:link {
        cursor: pointer;
        text-decoration: none;
        color: black;
    }

    .navigateNext a:visited {
        cursor: pointer;
        text-decoration: none;
        color: black;
    }

    .navigateNext a:hover {
        cursor: pointer;
        text-decoration: none;
        color: rgb(8, 57, 129);
    }

.root {
    display: flex;
    flex-direction: column;
    justify-content: space-around;
    flex-wrap: nowrap;
    align-items: stretch;
    width: 100%;
    height: 100%;
    overflow: hidden;
}

.contentContainer {
    display: flex;
    flex-direction: row;
    justify-content: space-around;
    flex-wrap: nowrap;
    align-items: stretch;
    width: 100%;
    height: 100%;
    overflow-y: hidden;
}


.mainTopicContent {
    flex: 1 1 auto;
    overflow-y: auto;
    overflow-x: hidden;
    height: 100%;
}

.currentOutline {
    flex: none;
    width: 240px;
    overflow: auto;
    padding-right: 10px;
    padding-left: 10px;
    padding-bottom: 20px;
}

a:link {
    color: #034af3;
    text-decoration: none;
}

a:visited {
    color: #034af3;
}

a:hover {
    color: #1d60ff;
    text-decoration: underline;
}

a:active {
    color: #12eb87;
}

body {
    background-color: white;
    font-size: 11pt;
    font-family: Roboto,"Helvetica Neue Light","Helvetica Neue",Helvetica,Arial,"Lucida Grande",sans-serif;
    margin: 0;
    padding: 0;
    color: Black;
    overflow-x: hidden;
}


p {
    margin-bottom: 10px;
    line-height: 1.5;
}

ul, ol {
    margin-bottom: 20px;
    line-height: 1.3em;
}

li {
    margin-top: .5em;
    margin-left: 25px;
    text-align: left;
}

img {
    border: 0;
}

table {
    width: 100%;
    overflow: auto;
    display: block;
    border-spacing: 0;
    border-collapse: collapse;
    margin: 15px 0;
    border-color: gray;
}


td, th {
    border: 1px solid #ddd;
    padding: 6px 13px;
    display: table-cell;
    vertical-align: top;
}

th {
    font-weight: bold;
    color: white;
    background: #555;
}

tbody>tr:nth-child(even) {
    background: #eee;
}

blockquote {
    color: #333;
    background-color: rgba(25,118,210,.05);
    border-left: 8px solid #1e88e5;
    padding: 16px 32px 8px 24px;
    margin: 24px 16px;
}

#tree-filter {
    width: calc(100% - 40px);
    padding: 8px 10px;
    color: #555;
    background-color: #fff;
    background-image: none;
    border: 1px solid #ccc;
    border-radius: 4px;
    margin: 10px 0 20px 15px;
}
#tree-filter-mobile {
    width: calc(100% - 40px);
    padding: 8px 10px;
    color: white;
    background-color: #555;
    background-image: none;
    border: 1px solid #ccc;
    border-radius: 4px;
    margin: 20px 0 10px 15px;
}
#tree-filter-mobile::placeholder {
    color: rgb(213, 213, 213);
}
#tree-filter-mobile::-webkit-input-placeholder {
    color: rgb(213, 213, 213);
}
#tree-filter-mobile:-moz-placeholder {
    color: rgb(213, 213, 213);
}
#tree-filter-mobile::-moz-placeholder {
    color: rgb(213, 213, 213);
}
#tree-filter-mobile::-ms-inputplaceholder {
    color: rgb(213, 213, 213);
}
#tree-filter-mobile:-ms-inputplaceholder {
    color: rgb(213, 213, 213);
}
```