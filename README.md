# PoolIt
## Create initial administrator
1. Make an account in the application.
2. Connect to the SQL database using a tool such as SSMS or DataGrip.
3. From the *AspNetRoles* table, note the ID of the Administrator role.
4. From the *AspNetUsers* table, note the ID of your newly created user account.
5. Create a new row in the *AspNetUserRoles* table and insert the user and role ID.
6. *(optional)* To enable more functionality such as managing user roles in the administration panel, repeat the same steps to add the SeniorAdministrator role to your user account.

## Set up GitHub login functionality
1. Create a [GitHub OAuth application](https://github.com/settings/developers).
2. In the *PoolIt.Web/appsettings.json* configuration file insert the GitHub Client ID and Client Secret.

Example:
```
"GitHub": {
  "ClientId": "2f6a7**********c3a9b",
  "ClientSecret": "b3ca86ff9d*************************8e772"
}
```

## Set up Google reCAPTCHA
1. Register a [reCAPTCHA v2 Checkbox site](https://www.google.com/recaptcha/admin).
2. In the *PoolIt.Web/appsettings.json* configuration file insert the Site key and Secret key.

Example:
```
"ReCaptcha": {
  "SiteKey": "7dG4L******************************7D0j3",
  "SecretKey": "7dG4L******************************9kLE4"
}
```
