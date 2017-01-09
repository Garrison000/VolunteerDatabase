﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VolunteerDatabase.Entity;
using VolunteerDatabase.Interface;

namespace VolunteerDatabase.Helper
{
    public class AuthorizeHelper<TData, TOut>
    {
        public delegate TOut AuthorizeFunction(TData data);

        private AuthorizeHelper() { }

        public static TOut Execute(IAuthorizeInput<TData, AppUser> input, AuthorizeFunction function)
        {
            var attribute = (AppAuthorizeAttribute)Attribute.GetCustomAttribute(function.GetMethodInfo(), typeof(AppAuthorizeAttribute));
            var roleEnum = attribute.Role;
            if (!Authorize(input, function))
            {
                throw new AppUserNotAuthorizedException(input.Claims, roleEnum);
            }
            var output = function(input.Data);
            return output;
        }

        public static bool Authorize(IAuthorizeInput<TData, AppUser> input, AuthorizeFunction function)
        {
            var attribute = (AppAuthorizeAttribute)Attribute.GetCustomAttribute(function.GetMethodInfo(), typeof(AppAuthorizeAttribute));
            AppRoleEnum roleEnum;
            if (attribute == null)
            {
                roleEnum = AppRoleEnum.OrgnizationAdministrator;
            }
            else
            {
                roleEnum = attribute.Role;
            }
            if (input == null || input.Claims == null)
            {
                return false;
            }
            if (!input.Claims.IsAuthenticated)
            {
                return false;
            }
            if (input.Claims.IsInRole(AppRoleEnum.Administrator))
            {
                return true;
            }
            if (input.Claims.IsInRole(roleEnum))
            {
                return true;
            }
            if (typeof(IProject).IsAssignableFrom(input.Data.GetType()))
            {
                var project = input.Data as IProject;
                if (project.Manager.Name == input.Claims.User.Name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
