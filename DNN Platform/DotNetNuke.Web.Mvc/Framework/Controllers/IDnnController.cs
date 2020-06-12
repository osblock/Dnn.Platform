﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

using System.Web.Mvc;
using System.Web.UI;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.UI.Modules;
using DotNetNuke.Web.Mvc.Helpers;

namespace DotNetNuke.Web.Mvc.Framework.Controllers
{
    public interface IDnnController : IController
    {
        ControllerContext ControllerContext { get; }

        Page DnnPage { get; set; }

        string LocalResourceFile { get; set; }

        string LocalizeString(string key);

        ModuleActionCollection ModuleActions { get; set; }

        ModuleInstanceContext ModuleContext { get; set; }

        ActionResult ResultOfLastExecute { get; }

        bool ValidateRequest { get; set; }

        ViewEngineCollection ViewEngineCollectionEx { get; set; }

        DnnUrlHelper Url { get; set; }
    }
}
