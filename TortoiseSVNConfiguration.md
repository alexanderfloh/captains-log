# How to configure Tortoise SVN for publishing artifacts #

Tortoise SVN -> Settings -> General -> Edit

```
enable-auto-props = yes
[auto-props]
*.application = svn:mime-type=application/x-ms-application
*.htm = svn:mime-type=text/html
*.manifest = svn:mime-type=application/x-ms-manifest
*.deploy = svn:mime-type=application/octet-stream
```