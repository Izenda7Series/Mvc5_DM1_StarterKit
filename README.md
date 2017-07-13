# Izenda Mvc5Starterkit (Back-end Standalone)

 :warning: This kit has been recently migrated and we are still in the process of updating the documentation. If you require assistance, please contact Izenda Support.
 
 
## Overview
This Starterkit showcases how to embed the front-end of Izenda into a MVC5 application that uses a stand-alone API back-end.

 :warning: The MVC Kit is designed for demonstration purposes and should not be used as an “as-is” fully-integrated solution. You can use the kit for reference or a baseline but ensure that security and customization meet the standards of your company.
 
 :warning: This deployment works with version 1.25.0. If you wish to update to a later version, ensure that you run the update scripts consecutively.

### Deploying the standalone API and Izenda Configuration Database

- Deploy the <a href="https://downloads.izenda.com/v1.25.0/API.zip">Izenda API</a> to IIS.

- Run DBScripts/mvc5_izenda.sql to create a database named 'MVC5_Izenda' (This is the database for the Izenda configuration. It contains report definitions, dashboards,etc.). You may use any name of your choosing, just be sure to modify the script to use the new database name.

- Download a copy of the <a href="https://github.com/Izenda7Series/Mvc5StarterKit/blob/master/Mvc5StarterKit/izendadb.config">izendadb.config</a> file and copy it to the root of your API deployment. Then modify the file with a valid connection string to this new database. If the connection string contains a ‘/’, ensure that you escape it ‘//’

- In IzendaSystemSettings update AuthValidateAccessTokenUrl to be fully qualified with the Starterkit's base address. e.g. api/Account/validateIzendaAuthToken --> http://localhost:14809/api/Account/validateIzendaAuthToken

- In IzendaSystemSettings update AuthGetAccessTokenUrl to be fully qualified with the Starterkit's base address. e.g. 

e.g. api/getAccessToken --> http://localhost:14809/api/getAccessToken

### Deploying the MVC Starter Kit Database

- Run DBScripts/mvc5.sql to create a database named 'mvc5'. This is the database for the .NET application. It contains the users, roles, tenants used to login. You may use any name of your choosing, just be sure to modify the script to use the new database name.

### Deploying the Retail Database (optional)

Create the Retail database with the <a  href="https://github.com/Izenda7Series/Angular2Starterkit/blob/master/DbScripts/RetailDbScript.sql">RetailDbScript.sql</a> file.

 

### Deploying the MVC Kit

izenda.integrate.js

- Modify the hostApi to point to the port of your Izenda API

Web.Config

-- Update the connection string to mvc5 database.

-- Update IzendaApiUrl to Izenda Web Api above.

- Copy Izenda Embedded UI v1.25.0 and store in Scripts/izenda folder.

-- Open javascript file under folder Scripts/izenda.integrate.js to update the hostApi to point to Izenda Web Api. 

### Update RSA Keys

- Use Izenda's RSA Key Generator Utility Located at http://downloads.izenda.com/Utilities/Izenda.Synergy.RSATool.zip

  1. AuthRSAPublicKey value in the IzendaSystemSettings table of the Izenda database (note: only use keysize < 1024 to generate because max-length for this field in database is 256) . This value is your public key and should be in XML format.
  2. And RSAPrivateKey value in Web.config file of the MVC Kit. This value is your private key and should be in PEM format.

 

### Initial Log in

- Log in as the System User First. Navigate to the Settings page to update your license key and add database connections. Log out of Izenda to allow these changes to take effect.

Initial User for logging in.

System User: <br />
- Tenant:
- Username: IzendaAdmin@system.com <br />
- Password: Izenda@123 <br />

**DELDG**: <br />
Employee Role: <br />
- Tenant: DELDG
- Username: employee@deldg.com
- Password: Izenda@123

Manager Role:<br />
- Tenant: DELDG
- Username: manager@deldg.com
- Password: Izenda@123

VP Role:<br />
- Tenant: DELDG
- Username: vp@deldg.com
- Password: Izenda@123

**NATWR** <br />
Employee Role:<br />
- Tenant: NATWR
- Username: employee@natwr.com
- Password: Izenda@123

Manager Role:<br />
- Tenant: NATWR
- Username: manager@natwr.com
- Password: Izenda@123

VP Role:<br />
- Tenant: NATWR
- Username: vp@natwr.com
- Password: Izenda@123

**RETCL** <br />
Employee Role:<br />
- Tenant: RETCL
- Username: employee@retcl.com
- Password: Izenda@123

Manager Role:<br />
- Tenant: RETCL
- Username: manager@retcl.com
- Password: Izenda@123

VP Role:<br />
- Tenant: RETCL
- Username: vp@retcl.com
- Password: Izenda@123


## Post Installation

 :warning: In order to ensure smooth operation of this kit, the items below should be reviewed.
 
 
### Exporting

Update the WebUrl value in the IzendaSystemSetting table with the URL for your front-end. You can use the script below to accomplish this. As general best practice, we recommend backing up your database before making any manual updates.

```sql

UPDATE [IzendaSystemSetting]
SET [Value] = '<your url here including the trailing slash>'
WHERE [Name] = 'WebUrl'

``` 

If you do not update this setting, charts and other visualizations may not render correctly when emailed or exported. This will also be evident in the log files as shown below:

`[ERROR][ExportingLogic ] Convert to image:
System.Exception: HTML load error. The remote content was not found at the server - HTTP error 404`
