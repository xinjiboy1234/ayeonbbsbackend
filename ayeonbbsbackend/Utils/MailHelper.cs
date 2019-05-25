using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Utils
{
    public class MailHelper
    {

        public static void SendEMail(string title, string content, string description)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("1214728803@qq.com"));
            message.To.Add(new MailboxAddress(description));
            message.Subject = title;

            var builder = new BodyBuilder();
            builder.HtmlBody = content;  //html
            //builder.TextBody = "";  //文本
            message.Body = builder.ToMessageBody();
            var client = new SmtpClient();

            client.Connect("smtp.qq.com", 587, false);     //连接服务
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate("1214728803@qq.com", ""); //验证账号密码
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
