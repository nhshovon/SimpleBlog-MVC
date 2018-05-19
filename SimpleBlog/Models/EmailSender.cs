using System;
using System.Net.Mail;
using System.Threading.Tasks;
using MailMessage = System.Net.Mail.MailMessage;

namespace SimpleBlog.Models
{
    internal sealed class EmailSender
    {
        public void Send(string hostName, int portNumber, string senderEmail, string senderEmailPass, string subject, string mailBody, string receiverEmail, string fromEmail, string FromEmailDisplayName, string receiverDisplayName, string subjectLineForTemplate)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient(hostName, portNumber);

                client.Credentials = new System.Net.NetworkCredential(senderEmail, senderEmailPass);
                client.EnableSsl = true;
                message.Subject = subject;
                message.Body = GetEmailBody(subjectLineForTemplate, mailBody);
                message.IsBodyHtml = true;

                message.From = new MailAddress(fromEmail, FromEmailDisplayName);

                message.To.Clear();
                message.To.Add(new MailAddress(receiverEmail, receiverDisplayName));

                client.Send(message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> SendEmail(string hostName, int portNumber, string senderEmail, string senderEmailPass, string subject, string mailBody, string receiverEmail, string fromEmail, string FromEmailDisplayName, string receiverDisplayName, string subjectLineForTemplate)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient(hostName, portNumber);
                client.Credentials = new System.Net.NetworkCredential(senderEmail, senderEmailPass);
                client.EnableSsl = true;
                message.Subject = subject;
                message.Body = GetEmailBody(subjectLineForTemplate, mailBody);
                message.IsBodyHtml = true;

                message.From = new MailAddress(fromEmail, FromEmailDisplayName);

                message.To.Clear();
                message.To.Add(new MailAddress(receiverEmail, receiverDisplayName));

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Send(string hostName, string senderEmail, string senderEmailPass, string subject, string mailBody, string receiverEmail, string fromEmail, string FromEmailDisplayName, string receiverDisplayName, string subjectLineForTemplate)
        {
            bool status = false;
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient(hostName, 587);
                client.Credentials = new System.Net.NetworkCredential(senderEmail, senderEmailPass);
                client.EnableSsl = true;
                message.Subject = subject;
                message.Body = GetEmailBody(subjectLineForTemplate, mailBody);
                message.IsBodyHtml = true;
                message.From = new MailAddress(fromEmail, FromEmailDisplayName);

                message.To.Clear();
                message.To.Add(new MailAddress(receiverEmail, receiverDisplayName));

                client.Send(message);
                status = true;
            }
            catch (Exception)
            {
                return status;
                throw;
            }
            return status;
        }

        protected string GetEmailBody(string heading, string body)
        {
            return string.Format(@"<html><head>
<meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
</head><body><table id=""m_4251586426606173851main"" height=""100%"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#F4F7FA"">
    <tbody>
      <tr>
        <td valign = ""top"">
          <table class=""m_4251586426606173851innermain"" style=""margin:0 auto;table-layout:fixed"" width=""580"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#F4F7FA"" align=""center"">
            <tbody>
              <tr>
                <td colspan=""4"">
                  <table class=""m_4251586426606173851logo"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                    <tbody>
                      <tr>
                        <td colspan=""2"" height =""30""></td>
                      </tr>
                      <tr>
                        <td valign=""top"" align=""center"">
                          <a href=""http://www.globedse.com"" style=""display:inline-block;text-align:center"" target=""_blank"">
                            <img src=""http://globedse.com/public_layout_v2/images/logo.png"" alt=""Globe Securities Ltd"" class=""CToWUd""  border=""0"">
                          </a>
                        </td>
                      </tr>
                      <tr>
                        <td colspan=""2"" height=""30""></td>
                      </tr>
                    </tbody>
                  </table>

                  <table style=""border-radius:4px"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"">
                    <tbody>
                      <tr>
                        <td height=""40""></td>
                      </tr>
                      <tr style=""font-family:-apple-system,BlinkMacSystemFont,'Segoe UI','Roboto','Oxygen','Ubuntu','Cantarell','Fira Sans','Droid Sans','Helvetica Neue',sans-serif;color:#4e5c6e;font-size:14px;line-height:20px;margin-top:20px"">
                        <td class=""m_4251586426606173851content"" colspan=""2"" style=""padding-left:90px;padding-right:90px"" valign=""top"" align=""center"">

                          <table width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" bgcolor=""#ffffff"">
  <tbody>
    <tr>
  <td colspan=""2"" cellpadding=""3"" valign=""bottom"" align=""center"">
   <img alt=""Globe Securities Ltd."" src=""http://globedse.com/public_layout_v2/images/email_icon.png"" class=""CToWUd"" width=""80"">
  </td>
</tr>

<tr><td height=""30""></td></tr>
<tr>
      <td align=""center"">
        <span style=""color:#48545d;font-size:22px;line-height:24px"">
        {0}
        </span>
      </td>
    </tr>

    <tr><td height=""24""></td></tr>
<tr>
  <td height=""1"" bgcolor=""#DAE1E9""></td>
</tr>

<tr><td height=""24""></td></tr>
{1}
<tr><td height=""20""></td></tr>
<tr>
<td align=""center"">
<p style=""color:#a2a2a2;font-size:12px;line-height:17px;font-style:italic"">
Best regards, <br>
Globe Securities Ltd. </p>
</td>
</tr>
  </tbody>
</table>

                        </td>
                      </tr>
                      <tr>
                        <td height=""60""></td>
                      </tr>
                    </tbody>
                  </table>
                    <table id=""m_4251586426606173851promo"" style=""margin-top:20px"" width= ""100%"" cellspacing=""0"" cellpadding= ""0"" border=""0"">
                      <tbody>
                        <tr>
                          <td colspan=""2"" height=""20""></td>
                        </tr>
                        <tr>
                          <td colspan=""2"" align=""center"">
                            <span style=""font-size:14px;font-weight:500;margin-bottom:10px;color:#7e8a98;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI','Roboto','Oxygen','Ubuntu','Cantarell','Fira Sans','Droid Sans','Helvetica Neue','sans-serif'""> Connect Anytime, Anywhere with Globe Securities Limited </span>
                          </td>
                        </tr>
                        <tr>
                          <td colspan=""1"" height=""20"" width=""50%"" align=""center"">
                            <div style=""font-family:-apple-system,BlinkMacSystemFont,'Segoe UI','Roboto','Oxygen','Ubuntu','Cantarell','Fira Sans','Droid Sans','Helvetica Neue',sans-serif;margin-top:15px;margin-bottom:20px;width:400px;display:inline-block;color:#9ba6b2;font-size:14px;line-height:24px""> Smart & Simple Tools for Active Traders. We are committed to help you achieve your goals. <div></div></div></td>
                        </tr>
                        <tr>
                          <td width=""100%"" valign=""top"" align= 'center'>
                            <a href=""http://www.globedse.com/"" style=""font-family:-apple-system,BlinkMacSystemFont,'Segoe UI','Roboto','Oxygen','Ubuntu','Cantarell','Fira Sans','Droid Sans','Helvetica Neue',sans-serif;display:inline-block;font-size:14px;padding:12px 15px;background-color:#47bb70;color:#ffffff;border:1px solid #47bb70;border-radius:4px;text-decoration:none;text-align:center;font-weight:500"" target=""_blank"">
                              Get Started
                            </a>
                          </td>
                        </tr>
                        <tr>
                          <td colspan=""2"" height=""0""></td>
                        </tr>
                      </tbody>
                    </table>
                  <table width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                    <tbody>
                      <tr>
                        <td height=""10""> &nbsp;</td>
                      </tr>
                      <tr>
                        <td valign=""top"" align=""center"">
                          <span style=""font-family:-apple-system,BlinkMacSystemFont,'Segoe UI','Roboto','Oxygen','Ubuntu','Cantarell','Fira Sans','Droid Sans','Helvetica Neue',sans-serif;color:#9eb0c9;font-size:10px"">©
                            <a href=""http://www.globedse.com/"" style=""color:#9eb0c9!important;text-decoration:none"" target= ""_blank""> Globe Securities Ltd.</a> {2}
                          </span>
                        </td>
                      </tr>
                      <tr>
                        <td height=""50""> &nbsp;</td>
                      </tr>
                    </tbody>
                  </table>
                </td>
              </tr>
            </tbody>
          </table>
        </td>
      </tr>
    </tbody>
  </table></body></html>", heading, body, DateTime.Today.Year);
        }
    }
}