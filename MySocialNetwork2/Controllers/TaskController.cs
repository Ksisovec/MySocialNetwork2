using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using MySocialNetwork2.Models;
using MySocialNetwork2.Repository.Interfaces;
using MySocialNetwork2.ViewModels;

namespace MySocialNetwork2.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskRepository taskRepository;
        private readonly ICategoryRepository categoryRepositoryitory;
        private readonly IAnswersRepository answersRepository;
        private ApplicationUserManager userManager;


        public TaskController()
        {

        }

        public TaskController(ITaskRepository repository, 
            ICategoryRepository categoryRepositoryitory,
            IAnswersRepository answersRepository)
        {
            this.taskRepository = repository;
            this.categoryRepositoryitory = categoryRepositoryitory;
            this.answersRepository = answersRepository;
        }


        public ApplicationUserManager UserManager
        {
            get { return userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { userManager = value; }
        }


        [HttpGet]
        public ActionResult CreateTaskResult()
        {
            //List<Category> categoryList = (from x in categoryRepositoryitory.Get().ToList()
            //                                       select new Category()
            //                                  {
            //                                      ID = x.ID,
            //                                      CategoryName = x.CategoryName
            //                                  }).OrderBy(w => w.CategoryName).ToList();

            //ViewBag.CategoryList = new SelectList(categoryList, "ID", "CategoryName");

            //ViewBag.categories = categoryRepositoryitory.Get().Select(category => category.CategoryName);
            return View(new CreateTaskViewModel());
        }




        [HttpPost]
        public ActionResult CreateTaskResult(CreateTaskViewModel model)
        {
            if (model.Name == null || model.Name == "" || model.Content == null || model.Content == "")
                RedirectToAction("Index", "Home");
            //model.Author = User.Identity.Name;

            var taskmodel = new Task();
            //{
            taskmodel.NameOfTask = model.Name;
            taskmodel.ContentOfTask = model.Content;
            var category = categoryRepositoryitory.FindByID(model.SelectCategoryId);//.GetEnumerator().Current;
            //var CategoryN = categoryRepositoryitory.Get(category => category.ID == model.SelectCategoryId).ToString();
            //taskmodel.Category = category;
            taskmodel.CategoryId = category.ID;
            taskmodel.CategoryName = category.CategoryName;
            taskmodel.UserCreated = User.Identity.Name;
            taskmodel.Likes = 0;
            taskmodel.Dislikes = 0;
            taskmodel.RatingOfTask = 0;
            taskmodel.Locked = false;
            taskmodel.UserCreated = User.Identity.Name;
            if (model.Answers != null)
            {
                ICollection<Answers> answers = new Collection<Answers>();
                List<String> modelAnswers = System.Web.Helpers.Json.Decode<List<String>>(model.Answers);
                foreach (String ans in modelAnswers)
                {
                    Answers answer = new Answers();
                    answer.ContentOfAnswer = ans;
                    answer.TaskID = taskmodel.ID;
                    answers.Add(answer);
                    answersRepository.Insert(answer);
                }
                taskmodel.Answers = answers;
            }
            taskRepository.Insert(taskmodel);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}