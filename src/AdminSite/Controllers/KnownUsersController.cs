﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.AdminSite.Controllers;

/// <summary>
/// KnownUsers Controller.
/// </summary>
[ServiceFilter(typeof(KnownUserAttribute))]
public class KnownUsersController : BaseController
{
    private readonly IKnownUsersRepository knownUsersRepository;
    private readonly IRoleRepository roleRepository;
    private readonly SaaSClientLogger<KnownUsersController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="KnownUsersController" /> class.
    /// </summary>
    /// <param name = "knownUsersRepository" > The known users repository.</param>
    /// <param name="logger">The logger.</param>

    public KnownUsersController(
        IKnownUsersRepository knownUsersRepository,
        IRoleRepository roleRepository,
        IAppVersionService appVersionService,
        SaaSClientLogger<KnownUsersController> logger, 
        IApplicationConfigRepository applicationConfigRepository):base(applicationConfigRepository, appVersionService)
    {
        this.knownUsersRepository = knownUsersRepository;
        this.roleRepository = roleRepository;
        this.logger = logger;
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <returns>All known users.</returns>
    public IActionResult Index()
    {
        this.logger.Info("KnownUsers Controller / Index");
        try
        {

            var getAllRoles = this.roleRepository.GetAllRoles().ToList<Roles>();
            var getAllKnownUsers = this.knownUsersRepository.GetAllKnownUsers();

            foreach (var user in getAllKnownUsers)
            {
                foreach (var role in getAllRoles)
                {
                    if (user.RoleId == role.Id)
                        user.Role = role;
                }
            }
            ViewBag.Roles = new SelectList(getAllRoles, "Id", "Name"); 

            return this.View(getAllKnownUsers);
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Save known users.
    /// </summary>
    /// <param name="knownUsers">The list of known users</param>
    /// <returns> Json.</returns>
    public JsonResult SaveKnownUsers([FromBody] IEnumerable<KnownUsers> knownUsers)
    {

        this.logger.Info("KnownUsers Controller / SaveKnownUsers");
        try
        {
            return Json(this.knownUsersRepository.SaveAllKnownUsers(knownUsers));
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return Json(string.Empty);
        }
    }


}