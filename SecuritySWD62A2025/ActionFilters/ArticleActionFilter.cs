﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SecuritySWD62A2025.Repositories;

namespace SecuritySWD62A2025.ActionFilters
{
    public class ArticleActionFilter: ActionFilterAttribute
    {
   
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string loggedInUser = context.HttpContext.User.Identity.Name; //email of logged in user
            string articleId = context.HttpContext.Request.Query["id"].ToString();

            //using this type of injection so as not to modify the method and class signature BECAUSE we are
            //inheriting

            ArticlesRepository articlesRepository = context.HttpContext.RequestServices.GetService<ArticlesRepository>();

            bool check = articlesRepository.IsUserAllowedToViewArticle(new Guid(articleId), loggedInUser);

            if (check == false)
            {
                context.Result = new ForbidResult();
            }

            base.OnActionExecuting(context);
        }

    }
}
