# PoolIt
## Set up GitHub login functionality
1. Create a [GitHub OAuth application](https://github.com/settings/developers).
2. In the *PoolIt.Web/appsettings.json* configuration file insert the GitHub Client ID and Client Secret.

Example:
```
...
"GitHub": {
    "ClientId": "2f6a7**********c3a9b",
    "ClientSecret": "b3ca86ff9d*************************8e772"
  }
...
```
