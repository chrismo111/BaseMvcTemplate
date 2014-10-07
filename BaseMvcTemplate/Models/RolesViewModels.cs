using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BaseMvcTemplate.Models {

    public class RoleViewModel {
        [Display( Name = "Role Name" )]
        public string RoleName { get; set; }
        public string RoleID { get; set; }
    }

    public class UserRoleViewModel {
        [Display( Name = "Role Name" )]
        public string RoleName { get; set; }
        public string RoleID { get; set; }
        [Display( Name = "Users" )]
        public MultiSelectList Users { get; set; }
        public string[] SelectedUsers { get; set; }
    }

}