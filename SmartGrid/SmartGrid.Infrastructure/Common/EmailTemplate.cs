namespace SmartGrid.Infrastructure.Common
{
    public static class EmailTemplate
    {
        public static string Activation(string activationLink) => $@"
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset='UTF-8' />
        <title>Activate your SmartGrid account</title>
        </head>
        <body style='margin:0;padding:0;background-color:#f4f6f8;font-family:Arial,Helvetica,sans-serif;'>

        <table width='100%' cellpadding='0' cellspacing='0' style='padding:40px 0;background-color:#f4f6f8;'>
        <tr>
        <td align='center'>

        <table width='600' cellpadding='0' cellspacing='0' style='background:#ffffff;border-radius:8px;padding:40px;'>

        <tr>
        <td align='center' style='padding-bottom:30px;'>
            <h1 style='margin:0;color:#1f2937;'>Welcome to SmartGrid ⚡</h1>
        </td>
        </tr>

        <tr>
        <td style='color:#374151;font-size:16px;line-height:24px;padding-bottom:25px;'>
            Thank you for creating an account.<br/><br/>
            To complete your registration, please confirm your email address by clicking the button below.
        </td>
        </tr>

        <tr>
        <td align='center' style='padding:30px 0;'>
            <a href='{activationLink}'
                style='background-color:#2563eb;
                        color:#ffffff;
                        text-decoration:none;
                        padding:14px 28px;
                        border-radius:6px;
                        display:inline-block;
                        font-weight:bold;
                        font-size:16px;'>
                Activate Account
            </a>
        </td>
        </tr>

        <tr>
        <td style='color:#6b7280;font-size:14px;line-height:22px;padding-top:20px;'>
            This activation link is valid for <b>30 minutes</b>.<br/><br/>
            If you did not create this account, you can safely ignore this email.
        </td>
        </tr>

        <tr>
        <td style='padding-top:40px;color:#9ca3af;font-size:12px;text-align:center;'>
            © {DateTime.UtcNow.Year} SmartGrid Team (mozemo dodati broj tima :) ). All rights reserved.
        </td>
        </tr>

        </table>

        </td>
        </tr>
        </table>

        </body>
        </html>";

    public static string SimpleNotification(string title, string message) => $@"
        <!DOCTYPE html>
        <html><head><meta charset='UTF-8'/></head>
        <body style='font-family:Arial,sans-serif;background:#f4f6f8;padding:24px;'>
        <div style='max-width:560px;margin:0 auto;background:#fff;padding:24px;border-radius:8px;'>
        <h2 style='color:#1f2937;margin-top:0;'>{title}</h2>
        <p style='color:#374151;line-height:1.5;'>{message}</p>
        </div></body></html>";
    }
}
