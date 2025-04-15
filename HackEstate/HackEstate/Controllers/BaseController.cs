using HackEstate.Managers;
using HackEstate.Models;
using HackEstate.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HackEstate.Controllers
{
    public class BaseController : Controller
    {
        public DbAb7a0dHackestatedbContext _db;
        public BaseRepository<User> _userRepo;
        public BaseRepository<Property> _propertyRepo;
        public BaseRepository<AgentProperty> _agentPropertyRepo;
        public BaseRepository<Event> _eventRepo;
        public BaseRepository<ChatMessage> _chatMessageRepo;
        public BaseRepository<PropertyImage> _propertyImageRepo;
        public BaseRepository<UserQuizAnswer> _userQuizAnswerRepo;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected readonly MailManager _mailManager;
        protected ISession _session => _httpContextAccessor.HttpContext.Session;
        public BaseController()
        {
            _db = new DbAb7a0dHackestatedbContext();
            _userRepo = new BaseRepository<User>();
            _propertyRepo = new BaseRepository<Property>();
            _agentPropertyRepo = new BaseRepository<AgentProperty>();
            _eventRepo = new BaseRepository<Event>();
            _chatMessageRepo = new BaseRepository<ChatMessage>();
            _propertyImageRepo = new BaseRepository<PropertyImage>();
            _userQuizAnswerRepo = new BaseRepository<UserQuizAnswer>();
        }
    }
}