# NeruNook

NeruNook is a .NET 10 Blazor Web App. The public landing page preserves the original editorial HTML/CSS design. Interactive Server rendering is limited to the protected CMS pages.

## Stack

- .NET 10 Blazor Web App
- MudBlazor 9.10.0 for the CMS
- ASP.NET Core Identity with roles and secure cookies
- EF Core 10 with SQLite
- MailKit for SMTP contact notifications
- ASP.NET Core rate limiting

## Run locally

```powershell
dotnet restore
dotnet run
```

The application applies pending EF Core migrations during startup. The SQLite database is created as `nerunook.db` in the application content root.

## Required configuration

Never commit real credentials. Admin seed values should use .NET user-secrets, environment variables, or the hosting provider's secret store.

```powershell
dotnet user-secrets set "AdminSeed:Email" "admin@example.com"
dotnet user-secrets set "AdminSeed:Password" "replace-with-a-strong-password"
```

For local contact notifications, `appsettings.Development.json` contains the HostPapa configuration structure and is ignored by Git. Replace only `Smtp:Password` with the real mailbox password before running locally. HostPapa uses `mail.papamail.net` on port `465` with SSL-on-connect; it does not use STARTTLS on that port.

Equivalent environment variables use double underscores, for example:

- `AdminSeed__Email`
- `AdminSeed__Password`
- `ContactNotifications__OwnerEmail`
- `Smtp__Host`
- `Smtp__Port`
- `Smtp__UseSsl`
- `Smtp__Username`
- `Smtp__Password`
- `Smtp__FromEmail`
- `Smtp__FromName`
- optional `DataProtection__KeysPath` for a persistent host-mounted key directory

If the two AdminSeed values are absent, no default user is created. The `Admin` role is still created. Public registration is intentionally disabled.

## Contact workflow

The public form posts to `/contact/submit` with antiforgery protection, server validation, a hidden honeypot, and a fixed-window IP rate limit. A valid inquiry is saved before email delivery is attempted.

If SMTP is unavailable or unconfigured, the saved inquiry is retained and the failure is logged without exposing server details. Notification messages are sent from the configured NeruNook sender and set the customer's address as `Reply-To`.

The local HostPapa notification values are:

- `ContactNotifications:OwnerEmail`: `info@nerunook.com`
- `Smtp:Host`: `mail.papamail.net`
- `Smtp:Port`: `465`
- `Smtp:UseSsl`: `true` (`SecureSocketOptions.SslOnConnect`)
- `Smtp:Username`: `info@nerunook.com`
- `Smtp:Password`: the real mailbox password, never committed or logged
- `Smtp:FromEmail`: `info@nerunook.com`
- `Smtp:FromName`: `NeruNook`

## CMS

- Login: `/admin/login`
- Dashboard: `/admin`
- Contact inbox: `/admin/contact-inquiries`
- User management: `/admin/users`

All CMS routes require the `Admin` role. User management supports creating accounts, enabling/disabling accounts, resetting passwords, and assigning/removing the Admin role. The last active administrator cannot be disabled or stripped of the Admin role.

## Migration

The initial migration is `InitialIdentityAndContactInbox`. It contains both the ASP.NET Core Identity schema and the `ContactInquiries` table.
