using System.Web.Http.Controllers;
using ygBlog.Tools;

namespace ygBlog.Managment
{
    public class SessionManager
    {
        static string? ticket = null;
        public static bool TicketAvailable(string ticket) {
            return  SessionManager.ticket != null && SessionManager.ticket == ticket;
        }
        public static string RenewTicket() {
            ticket = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
            return ticket;
        }
        public static void ClearTicket()
        {
            ticket = null;
        }
    }
}
