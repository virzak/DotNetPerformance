# DotNetPerformance

Tests various functionality of .NET

- Read files in a loop sync vs async

## Setup

Create `usersettings.json` file with a path to a large file:

```json
{
  "Data": {
    "DefaultDocument": "C:\\pathtofile.mp4"
  }
}
```

And set it to "Copy Always"
