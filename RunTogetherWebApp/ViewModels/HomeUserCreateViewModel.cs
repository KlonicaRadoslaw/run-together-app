﻿using System.ComponentModel.DataAnnotations;

namespace RunTogetherWebApp.ViewModels
{
    public class HomeUserCreateViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Location { get; set; }
    }
}
