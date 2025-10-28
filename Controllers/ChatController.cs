using System.Security.Claims;
using BTL_WebNC.Models.Chat;
using BTL_WebNC.Models.RoomChat;
using BTL_WebNC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BTL_WebNC.Controllers
{
    public class ChatController : Controller
    {
        private readonly IRoomChatRepository _roomRepo;
        private readonly IChatRepository _chatRepo;
        private readonly IAccountRepository _accountRepo;

        public ChatController(
            IRoomChatRepository roomRepo,
            IChatRepository chatRepo,
            IAccountRepository accountRepo
        )
        {
            _roomRepo = roomRepo;
            _chatRepo = chatRepo;
            _accountRepo = accountRepo;
        }

        public async Task<IActionResult> Index(int? recipientId)
        {
            var currentUserId = AccountController.GetCurrentUserId(HttpContext);
            if (currentUserId == null)
                return RedirectToAction("Login", "Account");

            // If recipientId is provided, open direct chat (1-1) with that account
            if (recipientId.HasValue)
            {
                return RedirectToAction("Room", new { otherAccountId = recipientId.Value });
            }

            // show user's rooms
            var role = AccountController.GetCurrentUserRole(HttpContext);
            IEnumerable<RoomChatModel> rooms = Enumerable.Empty<RoomChatModel>();
            if (role == "Admin")
            {
                rooms = await _roomRepo.GetAllAsync();
            }
            else if (role == "Student")
            {
                rooms = await _roomRepo.GetByStudentIdAsync(currentUserId.Value);
            }
            else
            {
                rooms = await _roomRepo.GetByTeacherIdAsync(currentUserId.Value);
            }

            // prepare last-message preview for each room so the UI can show recent conversations
            var roomsDetailed = new List<object>();
            foreach (var r in rooms)
            {
                ChatModel last = null;
                try
                {
                    // conversation is stored by student/teacher ids
                    var conv = await _chatRepo.GetConversationAsync(r.StudentId, r.TeacherId);
                    last = conv?.OrderBy(m => m.CreatedAt).LastOrDefault();
                }
                catch
                {
                    last = null;
                }

                roomsDetailed.Add(new { Room = r, LastMessage = last });
            }

            ViewBag.Rooms = rooms;
            ViewBag.RoomsDetailed = roomsDetailed;
            return View("~/Views/Chat/Index.cshtml");
        }

        public async Task<IActionResult> Room(int otherAccountId)
        {
            var currentUserId = AccountController.GetCurrentUserId(HttpContext);
            if (currentUserId == null)
                return RedirectToAction("Login", "Account");

            var role = AccountController.GetCurrentUserRole(HttpContext);
            int studentId,
                teacherId;
            if (role == "Student")
            {
                studentId = currentUserId.Value;
                teacherId = otherAccountId;
            }
            else
            {
                // assume teacher viewing student's chat
                studentId = otherAccountId;
                teacherId = currentUserId.Value;
            }

            var room = await _roomRepo.GetByParticipantsAsync(studentId, teacherId);
            if (room == null)
            {
                room = new RoomChatModel { StudentId = studentId, TeacherId = teacherId };
                room = await _roomRepo.CreateAsync(room);
            }

            var messages = await _chatRepo.GetConversationAsync(studentId, teacherId);
            var otherAccount = await _accountRepo.GetByIdAsync(otherAccountId);

            ViewBag.Room = room;
            ViewBag.Messages = messages;
            ViewBag.Other = otherAccount;
            return View("~/Views/Chat/Room.cshtml");
        }
    }
}
